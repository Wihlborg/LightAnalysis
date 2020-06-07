package com.example.lightanalysis

import androidx.appcompat.app.AppCompatActivity
import android.os.Bundle
import android.widget.Toast
import kotlinx.android.synthetic.main.activity_registration.*

class RegistrationActivity : AppCompatActivity() {

    override fun onCreate(savedInstanceState: Bundle?) {
        super.onCreate(savedInstanceState)
        setContentView(R.layout.activity_registration)

        register_button.setOnClickListener {
            //TODO implement registration logic

            Toast.makeText(this, "Registration made", Toast.LENGTH_SHORT).show()
            super.onBackPressed()
        }
    }
}
