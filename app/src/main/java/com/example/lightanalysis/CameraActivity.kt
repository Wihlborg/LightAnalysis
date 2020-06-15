package com.example.lightanalysis

import android.Manifest
import android.content.pm.PackageManager
import android.graphics.Matrix
import android.os.AsyncTask
import androidx.appcompat.app.AppCompatActivity
import android.os.Bundle
import android.util.Size
import android.view.Surface
import android.view.ViewGroup
import android.widget.ImageButton
import android.widget.Toast
import androidx.core.app.ActivityCompat
import androidx.core.content.ContextCompat
import androidx.camera.core.*
import kotlinx.android.synthetic.main.activity_camera.*
import java.util.concurrent.Executors

private const val REQUEST_CODE_PERMISSIONS = 10
private val REQUIRED_PERMISSIONS = arrayOf(Manifest.permission.CAMERA)
private val executor = Executors.newSingleThreadExecutor()
private val queueUtils = QueueUtils();

class CameraActivity : AppCompatActivity() {

    override fun onCreate(savedInstanceState: Bundle?) {
        super.onCreate(savedInstanceState)
        setContentView(R.layout.activity_camera)

        if (allPermissionsGranted()){
            view_finder.post { startCamera() }
        } else{
            ActivityCompat.requestPermissions(this, REQUIRED_PERMISSIONS, REQUEST_CODE_PERMISSIONS)
        }

        view_finder.addOnLayoutChangeListener { _, _, _, _, _, _, _, _, _ ->
            updateTransform()
        }

        queueUtils.initQueues()
    }

    private fun updateTransform(){
        val matrix = Matrix()

        val centerX = view_finder.width/ 2f
        val centerY = view_finder.height / 2f

        val rotationDegrees = when(view_finder.display.rotation){
            Surface.ROTATION_0 -> 0
            Surface.ROTATION_90 -> 90
            Surface.ROTATION_180 -> 180
            Surface.ROTATION_270 -> 270
            else -> return
        }

        matrix.postRotate(-rotationDegrees.toFloat(), centerX, centerY)

        view_finder.setTransform(matrix)
    }

    override fun onRequestPermissionsResult(
        requestCode: Int,
        permissions: Array<String>,
        grantResults: IntArray) {
        if (requestCode == REQUEST_CODE_PERMISSIONS){
            if (allPermissionsGranted()){
                view_finder.post{startCamera()}
            } else{
                Toast.makeText(this, "Permissions not granted by user", Toast.LENGTH_SHORT).show()
                finish()
            }
        }
    }

    private fun allPermissionsGranted() = REQUIRED_PERMISSIONS.all {
        ContextCompat.checkSelfPermission(baseContext, it) == PackageManager.PERMISSION_GRANTED
    }

    private fun startCamera(){

        // Create configuration object for the viewfinder use case
        val previewConfig = PreviewConfig.Builder().apply {
            setTargetResolution(Size(640, 480))
        }.build()


// Build the viewfinder use case
        val preview = Preview(previewConfig)

        preview.setOnPreviewOutputUpdateListener {
            val parent = view_finder.parent as ViewGroup
            parent.removeView(view_finder)
            parent.addView(view_finder, 0)

            view_finder.surfaceTexture = it.surfaceTexture

            updateTransform()
        }

        val imageCaptureConfig = ImageCaptureConfig.Builder().apply {
            setCaptureMode(ImageCapture.CaptureMode.MAX_QUALITY)
            setTargetRotation(windowManager.defaultDisplay.rotation)
        }.build()

        val imageCapture = ImageCapture(imageCaptureConfig)

        findViewById<ImageButton>(R.id.capture_button).setOnClickListener {
            imageCapture.takePicture(executor, object : ImageCapture.OnImageCapturedListener() {
                override fun onCaptureSuccess(image: ImageProxy?, rotationDegrees: Int) {
                    ImageUploadActivity(image!!).execute()
                }
            })
        }

        CameraX.bindToLifecycle(this, preview, imageCapture)
    }

    inner class ImageUploadActivity(proxy: ImageProxy): AsyncTask<Void, Void, Void>(){
        private val imageProxy = proxy;
        override fun doInBackground(vararg p0: Void?): Void? {
            queueUtils.uploadImageToStorage(imageProxy, System.currentTimeMillis().toString())
            imageProxy.close()
            return null
        }
    }
}
