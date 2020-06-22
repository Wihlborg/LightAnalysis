package com.example.lightanalysis;

import android.content.Intent;
import android.os.AsyncTask;
import android.os.Bundle;
import android.os.Handler;
import android.util.Log;
import android.view.View;
import android.widget.TextView;
import android.widget.Toast;
import androidx.appcompat.app.AppCompatActivity;
import android.view.View;
import android.view.View.OnClickListener;

import com.example.lightanalysis.models.User;
import com.microsoft.azure.storage.StorageException;

public class LoginActivity  extends AppCompatActivity{
    //Enables Log comments
    private static final String TAG = "LogInActivity";

    /**
     * Keep track of the login task to ensure we can cancel it if requested.
     */
    protected UserLoginTask mAuthTask = null;

    // UI references.

    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_login);

        findViewById(R.id.login_button).setOnClickListener(new OnClickListener() {
            @Override
            public void onClick(View v) {
                attemptLogin();
            }
        });

        findViewById(R.id.register_button).setOnClickListener(new OnClickListener() {
            @Override
            public void onClick(View view) {
                Intent intent = new Intent(LoginActivity.this, RegistrationActivity.class);
                startActivity(intent);
            }
        });
    }
    //Starts the AsyncTask for login
    private void attemptLogin(){
        if (mAuthTask != null){
            return;
        }

        TextView emailView = findViewById(R.id.email_text);
        TextView pwView = findViewById(R.id.pw_text);
        String email = emailView.getText().toString();
        String pw = pwView.getText().toString();

        mAuthTask = new UserLoginTask(email, pw);
        mAuthTask.execute((Void) null);


    }
    //Asynctask for handling network tasks
    class UserLoginTask extends AsyncTask<Void, Void, Boolean> {

        private final String mUsername;
        private final String mPassword;

        UserLoginTask(String username, String password) {
            mUsername = username;
            mPassword = password;
        }

        @Override
        protected Boolean doInBackground(Void... params) {
            //Database
            QueueUtils queueUtils = new QueueUtils();
            queueUtils.initQueues();
            boolean check = false;
            try {
                check = queueUtils.attemptLogin(mUsername, mPassword);
            } catch (StorageException e) {
                Log.e("Loginerror", "Azure queue error");
            }

            return check;
        }

        //Handler for what to do with the response from server
        @Override
        protected void onPostExecute(final Boolean success) {
            mAuthTask = null;

            if (success) {
                User.INSTANCE.setEmail(mUsername);
                finish();
                Intent r = new Intent(LoginActivity.this, CameraActivity.class);
                startActivity(r);
                Toast.makeText(getApplicationContext(),"Log In success", Toast.LENGTH_SHORT).show();
                Log.d(TAG,"Log In Success");

            } else {
                Toast.makeText(getApplicationContext(),"Log In failed", Toast.LENGTH_SHORT).show();
            }
        }

        @Override
        protected void onCancelled() {
            mAuthTask = null;
        }
    }

}

