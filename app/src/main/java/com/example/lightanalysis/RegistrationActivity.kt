package com.example.lightanalysis

import android.content.Intent
import android.os.AsyncTask
import android.os.Bundle
import android.util.Log
import android.widget.Toast
import androidx.appcompat.app.AppCompatActivity
import com.microsoft.azure.storage.StorageException
import kotlinx.android.synthetic.main.activity_registration.*

class RegistrationActivity : AppCompatActivity() {

    var mEmail = ""
    var mPassword = ""
    lateinit var mAuthTask: AsyncTask<Void, Void, Boolean>

    override fun onCreate(savedInstanceState: Bundle?) {
        super.onCreate(savedInstanceState)
        setContentView(R.layout.activity_registration)

        register_button.setOnClickListener {
            mEmail = email_text.text.toString()
            mPassword = pw_text.text.toString()
            mAuthTask = UserRegistrationActivity()
            mAuthTask.execute()

        }
    }
    //AsyncTask for handling network tasks
    inner class UserRegistrationActivity: AsyncTask<Void, Void, Boolean>() {
        override fun doInBackground(vararg p0: Void?): Boolean {
            val queueUtils = QueueUtils()
            queueUtils.initQueues()
            var check = false

            try {
                check = queueUtils.attemptRegistration(mEmail, mPassword)
            } catch (e:StorageException){
                Log.e("Loginerror", "Azure queue error")
            }
            return check
        }

        override fun onPostExecute(result: Boolean?) {

            val success = result ?: false

            if (success){
                Toast.makeText(applicationContext,  "Registration successful", Toast.LENGTH_LONG).show()
            } else{
                Toast.makeText(applicationContext,  "Registration failed", Toast.LENGTH_LONG).show()
            }

            finish()

            startActivity(Intent(baseContext, LoginActivity::class.java))
        }

    }
}
