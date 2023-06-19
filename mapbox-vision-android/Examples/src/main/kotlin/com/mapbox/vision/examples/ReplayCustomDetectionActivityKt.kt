package com.mapbox.vision.examples

import android.R.string
import android.graphics.*
import android.os.Bundle
import android.os.Environment
import android.util.Log
import android.view.View
import androidx.appcompat.app.AlertDialog
import com.mapbox.vision.VisionManager
import com.mapbox.vision.VisionManager.startRecording
import com.mapbox.vision.VisionReplayManager
import com.mapbox.vision.mobile.core.interfaces.VisionEventsListener
import com.mapbox.vision.mobile.core.models.AuthorizationStatus
import com.mapbox.vision.mobile.core.models.Camera
import com.mapbox.vision.mobile.core.models.Country
import com.mapbox.vision.mobile.core.models.FrameSegmentation
import com.mapbox.vision.mobile.core.models.classification.FrameSignClassifications
import com.mapbox.vision.mobile.core.models.detection.Detection
import com.mapbox.vision.mobile.core.models.detection.DetectionClass
import com.mapbox.vision.mobile.core.models.detection.FrameDetections
import com.mapbox.vision.mobile.core.models.frame.Image
import com.mapbox.vision.mobile.core.models.position.VehicleState
import com.mapbox.vision.mobile.core.models.road.RoadDescription
import com.mapbox.vision.mobile.core.models.world.WorldDescription
import com.mapbox.vision.performance.ModelPerformance
import com.mapbox.vision.performance.ModelPerformanceMode
import com.mapbox.vision.performance.ModelPerformanceRate
import kotlinx.android.synthetic.main.activity_custom_detection.*
import java.io.File
import java.io.FileFilter
import java.nio.ByteBuffer
import kotlin.math.pow
import kotlin.math.sqrt


open class ReplayCustomDetectionActivityKt : CustomDetectionActivityKt() {

    override fun startVisionManager() {
        if (allPermissionsGranted() && !visionManagerWasInit) {
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
                            ModelPerformanceMode.FIXED, ModelPerformanceRate.HIGH
                        )
                    )
                    VisionReplayManager.visionEventsListener = visionEventsListener
                    VisionReplayManager.start()
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

    override fun stopVisionManager() {
        if (visionManagerWasInit) {
            VisionReplayManager.stop()
            VisionReplayManager.destroy()
            visionManagerWasInit = false
        }
    }

    override fun initViews() {
        setContentView(R.layout.activity_replay_custom_detection)
    }
}
