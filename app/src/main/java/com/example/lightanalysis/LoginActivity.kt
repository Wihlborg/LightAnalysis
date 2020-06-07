package com.example.lightanalysis

import android.content.Intent
import android.os.Bundle
import android.os.Handler
import android.widget.Toast
import androidx.appcompat.app.AppCompatActivity
import kotlinx.android.synthetic.main.activity_login.*


class LoginActivity : AppCompatActivity() {
    override fun onCreate(savedInstanceState: Bundle?) {
        super.onCreate(savedInstanceState)
        setContentView(R.layout.activity_login)

        login_button.setOnClickListener {
            onLogin() }

        register_button.setOnClickListener {
            onRegistration()
        }

    }

    private fun onRegistration() {
        startActivity(Intent(this, RegistrationActivity::class.java))
    }


    private fun onLogin(){
        val queueUtils = QueueUtils()

        val success = queueUtils.attemptLogin(email_text.toString(), pw_text.toString())

        if (success) {
            // Using a handler to delay loading the PhotoActivity
            Handler().postDelayed({

                // Start activity
                startActivity(Intent(this, CameraActivity::class.java))

                // Close this activity
                finish()

            }, 2000)
        } else{
            Toast.makeText(this, "Login Failed", Toast.LENGTH_LONG).show()
        }
    }
}

