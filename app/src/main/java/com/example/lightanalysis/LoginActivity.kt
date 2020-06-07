package com.example.lightanalysis

import android.content.Intent
import androidx.appcompat.app.AppCompatActivity
import android.os.Bundle
import android.os.Handler
import android.widget.Button
import com.example.lightanalysisapp.CameraActivity
import kotlinx.android.synthetic.main.activity_login.*

class LoginActivity : AppCompatActivity() {

    override fun onCreate(savedInstanceState: Bundle?) {
        super.onCreate(savedInstanceState)
        setContentView(R.layout.activity_login)

        login_button.setOnClickListener {
            //TODO Actual login check
            onLogin() }

        register_button.setOnClickListener {
            onRegistration()
        }

    }

    private fun onRegistration() {
        startActivity(Intent(this, RegistrationActivity::class.java))
    }


    private fun onLogin(){
        // Using a handler to delay loading the PhotoActivity
        Handler().postDelayed({

            // Start activity
            startActivity(Intent(this, CameraActivity::class.java))

            // Close this activity
            finish()

        }, 2000)
    }
}

