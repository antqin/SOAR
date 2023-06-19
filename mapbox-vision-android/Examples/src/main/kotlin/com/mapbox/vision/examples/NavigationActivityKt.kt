package com.mapbox.vision.examples

import android.location.Location
import android.os.Environment
import android.widget.Button
import android.widget.EditText
import android.widget.LinearLayout
import android.widget.Toast
import android.view.View
import android.util.Log
import androidx.appcompat.app.AlertDialog
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
import com.mapbox.vision.performance.ModelPerformance
import com.mapbox.vision.performance.ModelPerformanceMode
import com.mapbox.vision.performance.ModelPerformanceRate
import com.mapbox.vision.utils.VisionLogger
import kotlinx.android.synthetic.main.activity_ar_navigation.*
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
open class NavigationActivityKt : BaseActivity(), RouteListener, ProgressChangeListener, OffRouteListener {

    companion object {
        private var TAG = NavigationActivityKt::class.java.simpleName
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

    // Dummy points from Gates --> university ave; need to update in future
    private val ROUTE_ORIGIN = Point.fromLngLat(-122.17325, 37.42999)
    private val ROUTE_DESTINATION = Point.fromLngLat(-122.16227, 37.44485)
    // Global vars for origin, destination points
    private var originPoint: Point? = null
    private var destinationPoint: Point? = null

    protected open fun setArRenderOptions(visionArView: VisionArView) {
        // enable fence rendering
        visionArView.setFenceVisible(true)
    }

    override fun onPermissionsGranted() {
        startVisionManager()
        startNavigation()
    }

    override fun initViews() {
        setContentView(R.layout.activity_ar_navigation)

        originEditText = findViewById(R.id.originEditText)
        destinationEditText = findViewById(R.id.destinationEditText)
        submitCoordinatesButton = findViewById(R.id.submitCoordinatesButton)
        hamburgerButton = findViewById(R.id.hamburgerButton)
        inputLayout = findViewById(R.id.inputLayout)

        // HARD CODED NAVIGATION HERE
//        val initialOriginLatitude = 37.423239 // Replace with your desired origin latitude
//        val initialOriginLongitude = -122.168440 // Replace with your desired origin longitude
//        val initialDestinationLatitude = 37.423581 // Replace with your desired destination latitude
//        val initialDestinationLongitude = -122.170483 // Replace with your desired destination longitude
        val initialOriginLatitude = 37.42919950657073 // Replace with your desired origin latitude
        val initialOriginLongitude = -122.16907482104808 // Replace with your desired origin longitude
        //val initialOriginLatitude = 37.423239 // Replace with your desired origin latitude
        //val initialOriginLongitude = -122.168440 // Replace with your desired origin longitude
//        val initialDestinationLatitude = 37.423581 // Replace with your desired destination latitude
//        val initialDestinationLongitude = -122.170483 // Replace with your desired destination longitude
        val initialDestinationLatitude = 37.44441140958959 // Replace with your desired destination latitude
        val initialDestinationLongitude = -122.16101886284447 // Replace with your desired destination longitude


        // Set the text property of the EditText fields with the initial LatLng values
        originEditText.setText("${initialOriginLatitude},${initialOriginLongitude}")
        destinationEditText.setText("${initialDestinationLatitude},${initialDestinationLongitude}")

        // Set the hamburger button click listener
        hamburgerButton.setOnClickListener {
            toggleInputVisibility()
        }

        submitCoordinatesButton.setOnClickListener {
            val originText = originEditText.text.toString()
            val destinationText = destinationEditText.text.toString()
            if (isValidCoordinate(originText) && isValidCoordinate(destinationText)) {
                val originLatLng = originText.split(',')
                val destinationLatLng = destinationText.split(',')
                val origin = Point.fromLngLat(originLatLng[1].toDouble(), originLatLng[0].toDouble())
                val destination = Point.fromLngLat(destinationLatLng[1].toDouble(), destinationLatLng[0].toDouble())

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

                        VisionReplayManager.visionEventsListener = object : VisionEventsListener {}
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
            //.origin(ROUTE_ORIGIN)
            .origin(origin)
            //.destination(ROUTE_DESTINATION)
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

                    // routes = response.body()!!.routes()
                    directionsRoute = response.body()!!.routes()[0]

                    // if (showAlternativePaths) {
                    //     val routes = response.body()!!.routes()
                    //     // TODO: Give the user a choice among routes

                    //     // print out number of routes
                    //     Toast.makeText(this, "Received ${routes.size} alternative routes", Toast.LENGTH_SHORT).show()

                    //     // for now we will just take the first one (so this does nothing)
                    //     directionsRoute = routes[0]

                    // }
                    // else {
                    //     // Accept the first route
                    //     directionsRoute = routes[0]
                    // }

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
        Log.d(TAG, "onProgressChange called")
        lastRouteProgress = routeProgress
        updateETAAndDistance(routeProgress)
    }

    override fun userOffRoute(location: Location) {
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

    private fun updateETAAndDistance(routeProgress: RouteProgress) {
        Log.d(TAG, "updateETAAndDistance called")
        // extract eta and format as string
        val eta = routeProgress.durationRemaining().toLong()
        val hours = TimeUnit.SECONDS.toHours(eta)
        val minutes = TimeUnit.SECONDS.toMinutes(eta) % 60
        val seconds = eta % 60
        val etaString = if (hours > 0) {
            String.format("%02d:%02d:%02d", hours, minutes, seconds)
        } else {
            String.format("%02d:%02d", minutes, seconds)
        }

        // extract distance and format as string
        val distanceRemaining = routeProgress.distanceRemaining()
        val distanceString = if (distanceRemaining < 1000) {
            String.format("%.0f meters", distanceRemaining)
        } else {
            String.format("%.2f km", distanceRemaining / 1000)
        }

        Log.d(TAG, "ETA: $etaString, Distance: $distanceString")

        // Update UI on the main thread
        runOnUiThread {
            eta_text_view.text = "ETA: $etaString"
            distance_remaining_text_view.text = "Distance: $distanceString"
        }
    }


}