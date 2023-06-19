package com.mapbox.vision.examples

import com.mapbox.vision.ar.FenceVisualParams
import com.mapbox.vision.ar.LaneVisualParams
import com.mapbox.vision.ar.VisionArManager
import com.mapbox.vision.ar.core.models.Color
import com.mapbox.vision.ar.view.gl.VisionArView

class CustomNavigationActivityKt : NavigationActivityKt() {

   override fun setArRenderOptions(visionArView: VisionArView) {

       visionArView.setLaneVisible(true)

       // Set the desired lane length in meters.
       VisionArManager.setLaneLength(40f)

       val laneVisualParams = LaneVisualParams(
           // Set light blue lane color, slightly opaque
           color = Color(0.0f, 0.5f, 1.0f, 0.6f),
           // Set lane width in meters
           width = 3.0f,
           // Set the length of the chevrons in meters
           // the longer the chevron, the fewer chevrons there are ~ approximating a path
           arrowLength = 2.0f
       )
       visionArView.setLaneVisualParams(laneVisualParams)

       // Enable fence rendering
       visionArView.setFenceVisible(true)

       val fenceVisualParams = FenceVisualParams(
           // Set red fence color, slightly opaque
           color = Color(1.0f, 0.0f, 0.0f, 0.6f),
           // Set the height of the fence in meters
           height = 2.0f,
           // Set the number of arrows in the fence
           sectionsCount = 3
       )
       visionArView.setFenceVisualParams(fenceVisualParams)


       // Setting AR render overall quality a bit lower to gain performance
       visionArView.setArQuality(0.7f)
   }
}
