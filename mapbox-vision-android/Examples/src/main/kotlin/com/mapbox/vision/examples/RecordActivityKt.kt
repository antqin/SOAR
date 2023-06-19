package com.mapbox.vision.examples

import android.graphics.*
import android.icu.util.Calendar
import android.os.Environment
import android.os.Parcel
import android.os.Parcelable
import android.view.View
import android.widget.Toast
import com.google.android.material.snackbar.Snackbar
import com.mapbox.vision.VisionManager
import kotlinx.android.synthetic.main.activity_custom_detection.*
import java.io.File
import java.text.SimpleDateFormat
import java.util.*


open class RecordActivityKt() : CustomDetectionActivityKt(), Parcelable {
    var visionManagerIsRecording = false

    constructor(parcel: Parcel) : this() {
        visionManagerIsRecording = parcel.readByte() != 0.toByte()
    }

    override fun stopRecording() {
        VisionManager.stopRecording()
        val fab: View = findViewById(R.id.fab)
        Snackbar.make(fab, "Stopped Recording", Snackbar.LENGTH_LONG)
            .setAction("Action", null)
            .show()
    }

    fun createNewFolder(): File {
        val filename = getExternalFilesDir(Environment.DIRECTORY_DOCUMENTS)
        val sdf = SimpleDateFormat("yyyyMMdd_HHmmss", Locale.getDefault())
        val currentDateTime: String = sdf.format(Date())
        val file = File(filename.toString() + "/" + currentDateTime)
        return file
    }

    fun startRecording() {
        val fab: View = findViewById(R.id.fab)
        val path = createNewFolder()
        VisionManager.startRecording(path.toString())
        Snackbar.make(fab, "Started Recording " + path.toString(), Snackbar.LENGTH_LONG)
            .setAction("Action", null)
            .show()
    }



    override fun initViews() {
        setContentView(R.layout.activity_record)
        val fab: View = findViewById(R.id.fab)
        fab.setOnClickListener { view ->
            visionManagerIsRecording = !visionManagerIsRecording
            if (visionManagerIsRecording) {
                startRecording()
            } else {
                stopRecording()
            }
        }
    }

    override fun writeToParcel(parcel: Parcel, flags: Int) {
        parcel.writeByte(if (visionManagerIsRecording) 1 else 0)
    }

    override fun describeContents(): Int {
        return 0
    }

    companion object CREATOR : Parcelable.Creator<RecordActivityKt> {
        override fun createFromParcel(parcel: Parcel): RecordActivityKt {
            return RecordActivityKt(parcel)
        }

        override fun newArray(size: Int): Array<RecordActivityKt?> {
            return arrayOfNulls(size)
        }
    }
}
