package com.mapbox.vision.examples

import android.graphics.Bitmap
import android.graphics.BitmapFactory
import android.graphics.Canvas
import android.graphics.Color
import android.graphics.Paint
import android.graphics.PorterDuff
import android.graphics.Rect
import android.location.Location
import android.os.Environment
import android.widget.Button
import android.widget.EditText
import android.widget.LinearLayout
import android.widget.Toast
import android.view.View
import android.view.View.GONE
import android.view.MenuItem
import android.util.Log
import androidx.appcompat.app.AlertDialog
import com.mapbox.mapboxsdk.geometry.LatLng
import com.mapbox.android.core.location.LocationEngineCallback
import com.mapbox.android.core.location.LocationEngineProvider
import com.mapbox.android.core.location.LocationEngineRequest
import com.mapbox.android.core.location.LocationEngineResult
import com.mapbox.api.directions.v5.models.DirectionsResponse
import com.mapbox.api.directions.v5.models.DirectionsRoute
import com.mapbox.core.constants.Constants
import com.mapbox.geojson.Point
import com.mapbox.geojson.utils.PolylineUtils
import com.mapbox.services.android.navigation.v5.navigation.MapboxNavigation
import com.mapbox.services.android.navigation.v5.navigation.MapboxNavigationOptions
import com.mapbox.services.android.navigation.v5.navigation.NavigationRoute
import com.mapbox.services.android.navigation.v5.offroute.OffRouteListener
import com.mapbox.services.android.navigation.v5.route.RouteFetcher
import com.mapbox.services.android.navigation.v5.route.RouteListener
import com.mapbox.services.android.navigation.v5.routeprogress.ProgressChangeListener
import com.mapbox.services.android.navigation.v5.routeprogress.RouteProgress
import com.mapbox.vision.VisionManager
import com.mapbox.vision.VisionReplayManager
import com.mapbox.vision.ar.VisionArManager
import com.mapbox.vision.ar.core.models.ManeuverType
import com.mapbox.vision.ar.core.models.Route
import com.mapbox.vision.ar.core.models.RoutePoint
import com.mapbox.vision.ar.view.gl.VisionArView
import com.mapbox.vision.mobile.core.interfaces.VisionEventsListener
import com.mapbox.vision.mobile.core.models.position.GeoCoordinate
import com.mapbox.vision.mobile.core.models.AuthorizationStatus
import com.mapbox.vision.mobile.core.models.Camera
import com.mapbox.vision.mobile.core.models.Country
import com.mapbox.vision.mobile.core.models.FrameSegmentation
import com.mapbox.vision.mobile.core.models.classification.FrameSignClassifications
import com.mapbox.vision.mobile.core.models.detection.FrameDetections
import com.mapbox.vision.mobile.core.models.position.GeoLocation
import com.mapbox.vision.mobile.core.models.position.VehicleState
import com.mapbox.vision.mobile.core.models.road.RoadDescription
import com.mapbox.vision.mobile.core.models.world.WorldCoordinate
import com.mapbox.vision.mobile.core.models.world.WorldDescription
import com.mapbox.vision.performance.ModelPerformance
import com.mapbox.vision.performance.ModelPerformanceMode
import com.mapbox.vision.performance.ModelPerformanceRate
import com.mapbox.vision.utils.VisionLogger
import kotlin.math.min
import kotlinx.android.synthetic.main.activity_ar_all.*
import java.util.concurrent.TimeUnit
import retrofit2.Call
import retrofit2.Callback
import retrofit2.Response
import java.io.File
import java.io.FileFilter

/**

* Example shows how Vision and VisionAR SDKs are used to draw AR lane over the video stream from camera.
* Also, Mapbox navigation services are used to build route and  navigation session.
*/
open class AllActivityKt : BaseActivity(), RouteListener, ProgressChangeListener, OffRouteListener {

    companion object {
        private var TAG = AllActivityKt::class.java.simpleName

        // POI will start to appear at this distance, starting with transparent and appearing gradually
        private const val MIN_DISTANCE_METERS_FOR_DRAW_LABEL = 150

        // POI will start to appear from transparent to non-transparent during this first meters of showing distance
        private const val DISTANCE_FOR_ALPHA_APPEAR_METERS = 50
        private const val LABEL_SIZE_METERS = 12
        private const val LABEL_ABOVE_GROUND_METERS = 8
    }

    // Mapbox navigation instance
    private lateinit var mapboxNavigation: MapboxNavigation

    // Fetches route from points.
    private lateinit var routeFetcher: RouteFetcher
    private lateinit var lastRouteProgress: RouteProgress
    private lateinit var directionsRoute: DirectionsRoute

    private var visionManagerWasInit = false
    private var navigationWasStarted = false

    // Initialize origin, destination fields and submit button
    private lateinit var originEditText: EditText
    private lateinit var destinationEditText: EditText
    private lateinit var submitCoordinatesButton: Button
    private lateinit var hamburgerButton: Button
    private lateinit var inputLayout: LinearLayout

    // Global vars for origin, destination points
    private var originPoint: Point? = null
    private var destinationPoint: Point? = null

    // Global var for current state
    private var currentVehicleState: VehicleState? = null


    // Initialize POIs
    private val poiList: List<POI> by lazy { providePOIList() }
    private val selectedPOITypes = mutableSetOf<String>()

    private val visionEventsListener = object : VisionEventsListener {

        private var cameraCalibrated = false
        private val paint = Paint()
        private var bitmapCameraFrame = Bitmap.createBitmap(1, 1, Bitmap.Config.ARGB_8888)
        private var canvasCameraFrame = Canvas()

        override fun onAuthorizationStatusUpdated(authorizationStatus: AuthorizationStatus) {}

        override fun onFrameSegmentationUpdated(frameSegmentation: FrameSegmentation) {}

        override fun onFrameDetectionsUpdated(frameDetections: FrameDetections) {}

        override fun onFrameSignClassificationsUpdated(frameSignClassifications: FrameSignClassifications) {}

        override fun onRoadDescriptionUpdated(roadDescription: RoadDescription) {}

        override fun onWorldDescriptionUpdated(worldDescription: WorldDescription) {}

        override fun onCameraUpdated(camera: Camera) {
            if (camera.calibrationProgress == 1.0f && !cameraCalibrated) {
                cameraCalibrated = true
                bitmapCameraFrame = Bitmap.createBitmap(camera.frameWidth, camera.frameHeight, Bitmap.Config.ARGB_8888)
                canvasCameraFrame = Canvas(bitmapCameraFrame)
                runOnUiThread {
                    camera_calibration_text.visibility = GONE
                }
            } else {
                runOnUiThread {
                    camera_calibration_text.text = getString(R.string.camera_calibration_progress, (camera.calibrationProgress * 100).toInt())
                }
            }
        }

        override fun onVehicleStateUpdated(vehicleState: VehicleState) {
            currentVehicleState = vehicleState
            if (cameraCalibrated) {
                updatePOIStateAndDrawInternal(vehicleState.geoLocation)
            }
        }

        override fun onCountryUpdated(country: Country) {}

        override fun onUpdateCompleted() {}

        internal fun updatePOIStateAndDrawInternal(newVehicleGeoLocation: GeoLocation) {
            if (poiList.isEmpty()) {
                return
            }

            val poiStateList = calculatePOIStateListRegardingVehicle(newVehicleGeoLocation.geoCoordinate)
            val poiStateListToShow = filterPOIByDistance(poiStateList)
            if (poiStateListToShow.isEmpty()) {
                return
            }
            val poiDrawDataList = preparePOIDrawData(poiStateListToShow)
            updateBitmapByPOIList(bitmapCameraFrame, poiDrawDataList)
            runOnUiThread {
                poi_view.setImageBitmap(bitmapCameraFrame)
            }
        }

        // Calculate POI distance to vehicle and WorldCoordinates regarding the vehicle
        private fun calculatePOIStateListRegardingVehicle(currentVehicleGeoCoordinate: GeoCoordinate): List<POIState> {
            val currentVehicleLatLng = LatLng(currentVehicleGeoCoordinate.latitude, currentVehicleGeoCoordinate.longitude)
            return poiList.mapNotNull {
                val latLng = LatLng(it.latitude, it.longitude)
                val geoCoordinate = GeoCoordinate(latLng.latitude, latLng.longitude)
                val worldCoordinate = VisionReplayManager.geoToWorld(geoCoordinate) ?: return@mapNotNull null
                val distanceToVehicle = latLng.distanceTo(currentVehicleLatLng).toInt()
                POIState(it, distanceToVehicle, worldCoordinate)
            }
        }

        // Show only POI which is close enough and behind the car
        private fun filterPOIByDistance(poiStateList: List<POIState>) = poiStateList.filter {
            val x = it.worldCoordinate.x
            val typeMatches = selectedPOITypes.contains(it.poi.type)
            // Check if POI is behind vehicle and close enough to start appearing
            (x > 0) && (it.distanceToVehicle < MIN_DISTANCE_METERS_FOR_DRAW_LABEL) && typeMatches
        }

        private fun preparePOIDrawData(poiStateList: List<POIState>): List<POIDrawData> = poiStateList.map { poiState ->
            // Prepare bounding rect for POI in mobile screen coordinates
            val poiBitmapRect = calculatePOIScreenRect(poiState.worldCoordinate)
            val poiLabelAlpha = calculatePOILabelAlpha(poiState)
            POIDrawData(poiState, poiState.poi.bitmap, poiBitmapRect, poiLabelAlpha)
        }

        private fun calculatePOIScreenRect(poiWorldCoordinate: WorldCoordinate): Rect {
            // Calculate left top coordinate of POI in real world using POI world coordinate
            val worldLeftTop = poiWorldCoordinate.copy(
                    y = poiWorldCoordinate.y + LABEL_SIZE_METERS / 2,
                    z = poiWorldCoordinate.z + LABEL_ABOVE_GROUND_METERS + LABEL_SIZE_METERS
            )

            // Calculate right bottom coordinate of POI in real world using POI world coordinate
            val worldRightBottom = poiWorldCoordinate.copy(
                    y = poiWorldCoordinate.y - LABEL_SIZE_METERS / 2,
                    z = poiWorldCoordinate.z + LABEL_ABOVE_GROUND_METERS
            )
            val poiBitmapRect = Rect(0, 0, 0, 0)

            // Calculate POI left top position on camera frame from real word coordinates
            VisionReplayManager.worldToPixel(worldLeftTop)?.run {
                poiBitmapRect.left = x
                poiBitmapRect.top = y
            }

            // Calculate POI right bottom position on camera frame from real word coordinates
            VisionReplayManager.worldToPixel(worldRightBottom)?.run {
                poiBitmapRect.right = x
                poiBitmapRect.bottom = y
            }
            return poiBitmapRect
        }

        private fun calculatePOILabelAlpha(poiState: POIState): Int {
            val minDistance = min(MIN_DISTANCE_METERS_FOR_DRAW_LABEL - poiState.distanceToVehicle, DISTANCE_FOR_ALPHA_APPEAR_METERS)
            return ((minDistance / DISTANCE_FOR_ALPHA_APPEAR_METERS.toFloat()) * 255).toInt()
        }

        private fun updateBitmapByPOIList(bitmap: Bitmap, poiDrawDataList: List<POIDrawData>) {
            canvasCameraFrame.drawColor(Color.TRANSPARENT, PorterDuff.Mode.CLEAR)
            for (p in poiDrawDataList) {
                Log.d(TAG, "Name: " + p.poiState.poi.name)
                with(p) {
                    paint.alpha = poiBitmapAlpha
                    canvasCameraFrame.drawBitmap(poiBitmap, null, poiBitmapRect, paint)

                    // Draw POI name above the icon
                    val text = p.poiState.poi.name
                    paint.color = Color.WHITE
                    paint.textSize = 15f
                    val xPos = poiBitmapRect.centerX()
                    val yPos = poiBitmapRect.top
                    canvasCameraFrame.drawText(text, xPos.toFloat(), yPos.toFloat(), paint)
                }
            }
        }
    }

    fun updatePOIStateAndDraw(newVehicleGeoLocation: GeoLocation) {
        visionEventsListener.updatePOIStateAndDrawInternal(newVehicleGeoLocation)
    }

    // Initialize Mapbox location engine
    private val arLocationEngine by lazy {
        LocationEngineProvider.getBestLocationEngine(this)
    }

    private val arLocationEngineRequest by lazy {
        LocationEngineRequest.Builder(0)
            .setPriority(LocationEngineRequest.PRIORITY_HIGH_ACCURACY)
            .setFastestInterval(1000)
            .build()
    }

    private val locationCallback by lazy {
        object : LocationEngineCallback<LocationEngineResult> {
            override fun onSuccess(result: LocationEngineResult?) {}

            override fun onFailure(exception: Exception) {}
        }
    }

    protected open fun setArRenderOptions(visionArView: VisionArView) {
        // enable fence rendering
        visionArView.setFenceVisible(true)
    }

    override fun onPermissionsGranted() {
        startVisionManager()
        startNavigation()
    }

    override fun initViews() {
        /* NEED TO EDIT THIS WITH THE LAYOUT FOR THE NEW APP */
        setContentView(R.layout.activity_ar_all)

        originEditText = findViewById(R.id.originEditText)
        destinationEditText = findViewById(R.id.destinationEditText)
        submitCoordinatesButton = findViewById(R.id.submitCoordinatesButton)
        hamburgerButton = findViewById(R.id.hamburgerButton)
        inputLayout = findViewById(R.id.inputLayout)

        // HARD CODED NAVIGATION HERE
        // val initialOriginLatitude = 37.42919950657073 // Replace with your desired origin latitude
        val initialOriginLatitude = 37.429196 // Replace with your desired origin latitude
        // val initialOriginLongitude = -122.16907482104808 // Replace with your desired origin longitude
        val initialOriginLongitude = -122.169075 // Replace with your desired origin longitude
        //val initialOriginLatitude = 37.423239 // Replace with your desired origin latitude
        //val initialOriginLongitude = -122.168440 // Replace with your desired origin longitude
//        val initialDestinationLatitude = 37.423581 // Replace with your desired destination latitude
//        val initialDestinationLongitude = -122.170483 // Replace with your desired destination longitude
//        val initialDestinationLatitude = 37.44441140958959 // Replace with your desired destination latitude
//        val initialDestinationLongitude = -122.16101886284447 // Replace with your desired destination longitude
        val initialDestinationLatitude = 37.44441 // Replace with your desired destination latitude
        val initialDestinationLongitude = -122.16102 // Replace with your desired destination longitude

        // Set text color to white
        originEditText.setTextColor(Color.WHITE)
        destinationEditText.setTextColor(Color.WHITE)

        // Set the text property of the EditText fields with the initial LatLng values
        originEditText.setText("${initialOriginLatitude},${initialOriginLongitude}")
        destinationEditText.setText("${initialDestinationLatitude},${initialDestinationLongitude}")

        // Set the hamburger button click listener
        hamburgerButton.setOnClickListener {
            toggleInputVisibility()
        }

        // Set up pop up menu
        val poiTypes = arrayOf("Cafe", "Restaurant", "Landmark", "Other")
        selectedPOITypes.addAll(poiTypes)
        val checkedItems = BooleanArray(poiTypes.size) { true } // Initially, all types are selected

        findViewById<Button>(R.id.poi_type_selection_button).setOnClickListener {
            AlertDialog.Builder(this)
                .setTitle("Select POI types")
                .setMultiChoiceItems(poiTypes, checkedItems) { _, which, isChecked ->
                    val typeName = poiTypes[which]
                    if (isChecked) {
                        selectedPOITypes.add(typeName)
                    } else {
                        selectedPOITypes.remove(typeName)
                    }
                }
//                .setPositiveButton("OK", null)
                .setPositiveButton("OK") { _, _ ->
                    currentVehicleState?.let {
                        updatePOIStateAndDraw(it.geoLocation)
                    }
                }
                .show()
        }

        submitCoordinatesButton.setOnClickListener {
            val originText = originEditText.text.toString()
            val destinationText = destinationEditText.text.toString()
            if (isValidCoordinate(originText) && isValidCoordinate(destinationText)) {
                val originLatLng = originText.split(',')
                val destinationLatLng = destinationText.split(',')
                val origin = Point.fromLngLat(originLatLng[1].toDouble(), originLatLng[0].toDouble())
                val destination = Point.fromLngLat(destinationLatLng[1].toDouble(), destinationLatLng[0].toDouble())

                Log.d(TAG, "origin " + origin.toString())
                Log.d(TAG, "destination " + destination.toString())
                startNavigation(origin, destination)
            } else {
                Toast.makeText(this, "Invalid coordinates. Please use format 'lat,lng'", Toast.LENGTH_SHORT).show()
            }
        }
    }

    private fun isValidCoordinate(coordinate: String): Boolean {
        val coordinatePattern = "[-+]?[0-9]*\\.?[0-9]+,\\s*[-+]?[0-9]*\\.?[0-9]+"
        return coordinate.matches(coordinatePattern.toRegex())
    }

    private fun toggleInputVisibility() {
        if (inputLayout.visibility == View.VISIBLE) {
            inputLayout.visibility = View.GONE
        } else {
            inputLayout.visibility = View.VISIBLE
        }
    }

    override fun onStart() {
        super.onStart()
        startVisionManager()
        startNavigation()
    }

    override fun onStop() {
        super.onStop()
        stopVisionManager()
        stopNavigation()
    }

    private fun startVisionManager() {
        if (allPermissionsGranted() && !visionManagerWasInit) {
            // Create and start VisionManager.
            val file = File(getExternalFilesDir(Environment.DIRECTORY_DOCUMENTS).toString())
            val singleItems = file.listFiles(FileFilter { it.isDirectory}).map { it.absolutePath }.toTypedArray()
            var checkedItem = 1
            val recordings: View = findViewById(R.id.recordings)

            recordings.setOnClickListener { view ->
                stopVisionManager()
                AlertDialog.Builder(this)
                    .setTitle("Choose a Folder")
                    .setPositiveButton("Start") { dialog, which ->
                        VisionReplayManager.create(singleItems[checkedItem])
                        VisionReplayManager.setModelPerformance(
                            ModelPerformance.On(
                                ModelPerformanceMode.DYNAMIC,
                                ModelPerformanceRate.LOW
                            )
                        )

                        VisionReplayManager.visionEventsListener = visionEventsListener
                        VisionReplayManager.start()

                        // Create VisionArManager.
                        VisionArManager.create(VisionReplayManager)
                        mapbox_ar_view.setArManager(VisionArManager)
                        setArRenderOptions(mapbox_ar_view)

                        visionManagerWasInit = true
                    }
                    // Single-choice items (initialized with checked item)
                    .setSingleChoiceItems(singleItems, checkedItem) { dialog, which ->
                        checkedItem = which
                    }
                    .show()
            }
        }
    }

    private fun stopVisionManager() {
        if (visionManagerWasInit) {
            VisionArManager.destroy()
            VisionReplayManager.stop()
            VisionReplayManager.destroy()
            visionManagerWasInit = false
        }
    }

    private fun providePOIList(): List<POI> {

        val poiHooverTower = POI(
            name = "Hoover Tower",
            type = "Landmark",
            longitude = -122.166549,
            latitude = 37.427792,
            bitmap = getBitmapFromAssets("ic_hoover.png"))

        val poiGatesBuilding = POI(
            name = "Gates Computer Science",
            type = "Landmark",
            longitude = -122.173377,
            latitude = 37.430162,
            bitmap = getBitmapFromAssets("ic_building.png"))

        val poiOval = POI(
            name = "Stanford Oval",
            type = "Landmark",
            longitude = -122.169343,
            latitude = 37.430241,
            bitmap = getBitmapFromAssets("ic_lake.png"))

        val poiTresidder = POI(
            name = "Tresidder",
            type = "Cafe",
            longitude = -122.170754,
            latitude = 37.423939,
            bitmap = getBitmapFromAssets("ic_cafe.png"))

        val poiCoHo = POI(
            name = "CoHo",
            type = "Cafe",
            longitude = -122.170902,
            latitude = 37.424123,
            bitmap = getBitmapFromAssets("ic_cafe.png"))

        val poiLakeLag = POI(
            name = "Lake Lagunita",
            type = "Landmark",
            longitude = -122.173317,
            latitude = 37.423016,
            bitmap = getBitmapFromAssets("ic_lake.png"))

        val poiStanfordStadium = POI(
            name = "Stanford Football Stadium",
            type = "Other",
            longitude = -122.161313,
            latitude = 37.435268,
            bitmap = getBitmapFromAssets("ic_lake.png"))

        val poiNola = POI(
            name = "Nola",
            type = "Restaurant",
            longitude = -122.161443,
            latitude = 37.445241,
            bitmap = getBitmapFromAssets("ic_restaurant.png"))

        val poiVerve = POI(
            name = "Verve",
            type = "Cafe",
            longitude = -122.163046,
            latitude = 37.444194,
            bitmap = getBitmapFromAssets("ic_cafe.png"))

        val poiBlueBottle = POI(
            name = "Blue Bottle",
            type = "Cafe",
            longitude = -122.159628,
            latitude = 37.447663,
            bitmap = getBitmapFromAssets("ic_cafe.png"))

        val poiRamenNagi = POI(
            name = "Ramen Nagi",
            type = "Restaurant",
            longitude = -122.160683,
            latitude = 37.44564,
            bitmap = getBitmapFromAssets("ic_restaurant.png"))

        val poiOrens = POI(
            name = "Oren's Hummus",
            type = "Restaurant",
            longitude = -122.162148,
            latitude = 37.445711,
            bitmap = getBitmapFromAssets("ic_restaurant.png"))

        val poiMovieTheater = POI(
            name = "Landmark Movie Theater",
            type = "Other",
            longitude = -122.163623,
            latitude = 37.4450873,
            bitmap = getBitmapFromAssets("ic_building.png"))

        val poiCreamery = POI(
            name = "Creamery",
            type = "Restaurant",
            longitude = -122.161833,
            latitude = 37.443924,
            bitmap = getBitmapFromAssets("ic_restaurant.png")
        )

        val poiNobu = POI(
            name = "Nobu",
            type = "Restaurant",
            longitude = -122.161479,
            latitude = 37.443748,
            bitmap = getBitmapFromAssets("ic_restaurant.png")
        )

        return arrayListOf(
            poiHooverTower, poiGatesBuilding, poiOval, poiTresidder, poiCoHo, poiLakeLag,
            poiStanfordStadium, poiNola, poiVerve, poiBlueBottle, poiRamenNagi, poiOrens,
            poiMovieTheater, poiCreamery, poiNobu
        )
    }

    private fun getBitmapFromAssets(asset: String): Bitmap {
        val assetManager = this.assets
        val stream = assetManager.open(asset)
        return BitmapFactory.decodeStream(stream)
    }

    data class POI(val name: String, val type: String, val longitude: Double, val latitude: Double, val bitmap: Bitmap)

    data class POIDrawData(val poiState: POIState, val poiBitmap: Bitmap, val poiBitmapRect: Rect, val poiBitmapAlpha: Int)

    data class POIState(val poi: POI, val distanceToVehicle: Int, val worldCoordinate: WorldCoordinate)

    private fun startNavigation(origin: Point? = null, destination: Point? = null) {
        if (allPermissionsGranted() && !navigationWasStarted) {
            if (origin != null && destination != null) {
                originPoint = origin
                destinationPoint = destination
            }
            if (originPoint != null && destinationPoint != null) {
                // initialize navigation functionality
                mapboxNavigation = MapboxNavigation(
                    this,
                    getString(R.string.mapbox_access_token),
                    MapboxNavigationOptions.builder().build()
                )

                // initialize route fetcher functionality
                routeFetcher = RouteFetcher(this, getString(R.string.mapbox_access_token))
                routeFetcher.addRouteListener(this)

                try {
                    arLocationEngine.requestLocationUpdates(
                        arLocationEngineRequest,
                        locationCallback,
                        mainLooper
                    )
                } catch (se: SecurityException) {
                    VisionLogger.e(TAG, se.toString())
                }

                initDirectionsRoute(originPoint!!, destinationPoint!!)

                // Route need to be reestablished if off route happens.
                mapboxNavigation.addOffRouteListener(this)
                mapboxNavigation.addProgressChangeListener(this)

                navigationWasStarted = true
            }
        }
    }

    private fun stopNavigation() {
        if (navigationWasStarted) {
            arLocationEngine.removeLocationUpdates(locationCallback)

            mapboxNavigation.removeProgressChangeListener(this)
            mapboxNavigation.removeOffRouteListener(this)
            mapboxNavigation.stopNavigation()

            navigationWasStarted = false
        }
    }

    private fun initDirectionsRoute(origin: Point, destination: Point) {
    //private fun initDirectionsRoute(showAlternativePaths: Boolean = false) {
        // Build and request navigation directions from origin to destination
        NavigationRoute.builder(this)
            .accessToken(getString(R.string.mapbox_access_token))
            .origin(origin)
            .destination(destination)
            //.alternatives(showAlternativePaths)
            .build()
            .getRoute(object : Callback<DirectionsResponse> { // Request the route
                // Handle the route response
                override fun onResponse(
                    call: Call<DirectionsResponse>,
                    response: Response<DirectionsResponse>
                ) {
                    // Check if the response is valid and contains routes
                    if (response.body() == null || response.body()!!.routes().isEmpty()) {
                        return
                    }
                    Log.d(TAG, "Response body: " + response.body().toString())
                    // routes = response.body()!!.routes()
                    directionsRoute = response.body()!!.routes()[0]
                    Log.d(TAG, "Route: " + directionsRoute.toString())

                    // Start navigation with the retrieved route
                    mapboxNavigation.startNavigation(directionsRoute)

                    // Set route progress.
                    VisionArManager.setRoute(
                        Route(
                            // Get route points from the route
                            directionsRoute.getRoutePoints(),
                            // Get ETA, if null set to 0
                            directionsRoute.duration()?.toFloat() ?: 0f,
                            // TODO: add source and destination street names
                            "",
                            ""
                        )
                    )
                }

                // Catch any errors and print trace
                override fun onFailure(call: Call<DirectionsResponse>, t: Throwable) {
                    t.printStackTrace()
                }
            })
    }

    override fun onErrorReceived(throwable: Throwable?) {
        throwable?.printStackTrace()

        mapboxNavigation.stopNavigation()
        Toast.makeText(this, "Can not calculate the route requested", Toast.LENGTH_SHORT).show()
    }

    override fun onResponseReceived(response: DirectionsResponse, routeProgress: RouteProgress?) {
        mapboxNavigation.stopNavigation()
        if (response.routes().isEmpty()) {
            Toast.makeText(this, "Can not calculate the route requested", Toast.LENGTH_SHORT).show()
        } else {
            mapboxNavigation.startNavigation(response.routes()[0])

            val route = response.routes()[0]
            Log.d(TAG, "Route: " + route.toString())

            // Set route progress.
            VisionArManager.setRoute(
                Route(
                    // Get route points from the route
                    route.getRoutePoints(),
                    // Get ETA, if null set to 0
                    route.duration()?.toFloat() ?: 0f,
                    // TODO: add source and destination street names
                    "",
                    ""
                )
            )
        }
    }

    override fun onProgressChange(location: Location, routeProgress: RouteProgress) {
        Log.d(TAG, "onProgressChange " + routeProgress.currentState().toString())
        lastRouteProgress = routeProgress
        //updateETAAndDistance(routeProgress)
    }

    override fun userOffRoute(location: Location) {
        Log.d(TAG, "userOffRoute")
        Log.d(TAG, "userOffRoute: location " + location.toString())
        routeFetcher.findRouteFromRouteProgress(location, lastRouteProgress)
    }

    private fun DirectionsRoute.getRoutePoints(): Array<RoutePoint> {
        val routePoints = arrayListOf<RoutePoint>()
        // Iterate over each leg of the route
        legs()?.forEach { leg ->
            // Iterate over each step of the leg (e.g., "turn right", etc.)
            leg.steps()?.forEach { step ->
                val maneuverPoint = RoutePoint(
                    GeoCoordinate(
                        latitude = step.maneuver().location().latitude(),
                        longitude = step.maneuver().location().longitude()
                    ),
                    step.maneuver().type().mapToManeuverType()
                )
                routePoints.add(maneuverPoint)

                /*
                Decompose step into component points (if necessary)
                E.g.: maneuver point insufficient to represent a curved road; the geometry of the step
                provides additional coordinates along the path of the curved segment.
                */
                step.geometry()
                    ?.buildStepPointsFromGeometry()
                    ?.map { geometryStep ->
                        RoutePoint(
                            GeoCoordinate(
                                latitude = geometryStep.latitude(),
                                longitude = geometryStep.longitude()
                            )
                        )
                    }
                    ?.let { stepPoints ->
                        routePoints.addAll(stepPoints)
                    }
            }
        }

        return routePoints.toTypedArray()
    }

    private fun String.buildStepPointsFromGeometry(): List<Point> {
        return PolylineUtils.decode(this, Constants.PRECISION_6)
    }

    private fun String?.mapToManeuverType(): ManeuverType = when (this) {
        "turn" -> ManeuverType.Turn
        "depart" -> ManeuverType.Depart
        "arrive" -> ManeuverType.Arrive
        "merge" -> ManeuverType.Merge
        "on ramp" -> ManeuverType.OnRamp
        "off ramp" -> ManeuverType.OffRamp
        "fork" -> ManeuverType.Fork
        "roundabout" -> ManeuverType.Roundabout
        "exit roundabout" -> ManeuverType.RoundaboutExit
        "end of road" -> ManeuverType.EndOfRoad
        "new name" -> ManeuverType.NewName
        "continue" -> ManeuverType.Continue
        "rotary" -> ManeuverType.Rotary
        "roundabout turn" -> ManeuverType.RoundaboutTurn
        "notification" -> ManeuverType.Notification
        "exit rotary" -> ManeuverType.RoundaboutExit
        else -> ManeuverType.None
    }

//    private fun updateETAAndDistance(routeProgress: RouteProgress) {
//        Log.d(TAG, "updateETAAndDistance called")
//        Log.d(TAG, "routeProgress " + routeProgress.toString())
//        // extract eta and format as string
//        val eta = routeProgress.durationRemaining().toLong()
//        Log.d(TAG, "ETA : " + eta.toString())
//        val hours = TimeUnit.SECONDS.toHours(eta)
//        val minutes = TimeUnit.SECONDS.toMinutes(eta) % 60
//        val seconds = eta % 60
//        val etaString = if (hours > 0) {
//            String.format("%02d:%02d:%02d", hours, minutes, seconds)
//        } else {
//            String.format("%02d:%02d", minutes, seconds)
//        }
//
//        // extract distance and format as string
//        val distanceRemaining = routeProgress.distanceRemaining()
//        Log.d(TAG, "Distance : " + distanceRemaining.toString())
//        val distanceString = if (distanceRemaining < 1000) {
//            String.format("%.0f meters", distanceRemaining)
//        } else {
//            String.format("%.2f km", distanceRemaining / 1000)
//        }
//
//        Log.d(TAG, "ETA: $etaString, Distance: $distanceString")
//
//        // Update UI on the main thread
//        runOnUiThread {
//            eta_text_view.text = "ETA: $etaString"
//            distance_remaining_text_view.text = "Distance: $distanceString"
//        }
//    }


}