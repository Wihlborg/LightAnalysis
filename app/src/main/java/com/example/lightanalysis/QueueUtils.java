package com.example.lightanalysis;

import android.graphics.Bitmap;
import android.graphics.BitmapFactory;
import android.media.Image;
import android.util.Log;

import androidx.camera.core.ImageProxy;

import com.example.lightanalysis.models.Account;
import com.example.lightanalysis.models.Response;
import com.example.lightanalysis.models.UserRequest;
import com.google.gson.Gson;
import com.microsoft.azure.storage.CloudStorageAccount;
import com.microsoft.azure.storage.StorageCredentials;
import com.microsoft.azure.storage.StorageCredentialsAccountAndKey;
import com.microsoft.azure.storage.StorageException;
import com.microsoft.azure.storage.blob.CloudBlobClient;
import com.microsoft.azure.storage.blob.CloudBlobContainer;
import com.microsoft.azure.storage.blob.CloudBlockBlob;
import com.microsoft.azure.storage.queue.CloudQueue;
import com.microsoft.azure.storage.queue.CloudQueueClient;
import com.microsoft.azure.storage.queue.CloudQueueMessage;

import java.io.ByteArrayInputStream;
import java.io.ByteArrayOutputStream;
import java.io.IOException;
import java.net.URISyntaxException;
import java.nio.ByteBuffer;
import java.util.UUID;

public class QueueUtils {


    //Strings
    private String accountName = "rallestorage";
    private String accountKey = "OLPmb7rXZfl2e+z2xM46/auXeesW9b11JdbRBLzdGzBJnpRglUAHhFpMJAr/PG48AAZHyGfHWTyS9N/P2MSx2g==";
    private StorageCredentials creds;
    private CloudStorageAccount storageAccount;
    private CloudQueueClient queueClient;
    private CloudQueue inqueue, outqueue;
    private CloudQueueMessage inMessage, outMessage;

    Gson gson = new Gson();

    public QueueUtils() {
    }

    public void initQueues()
    {
        try {
            creds = new StorageCredentialsAccountAndKey(accountName, accountKey);
            storageAccount = new CloudStorageAccount(creds, true);

            // Create the queue client
            queueClient = storageAccount.createCloudQueueClient();

            // Retrieve a reference to a queue
            inqueue = queueClient.getQueueReference("authresponsequeue");

            // Create the queue if it doesn't already exist
            inqueue.createIfNotExists();

            // Retrieve a reference to a queue
            outqueue = queueClient.getQueueReference("authrequestqueue");

            // Create the queue if it doesn't already exist
            outqueue.createIfNotExists();
        } catch (Exception e){
            e.printStackTrace();
        }
    }

    public boolean attemptLogin(String email, String pw) throws StorageException {
        UserRequest request = new UserRequest();
        Account account = new Account();
        account.setEmail(email);
        account.setPassword(pw);
        Response response = null;

        request.setMethod("LOGIN");
        String id = UUID.randomUUID().toString();
        request.setId(id);
        request.setAccount(account);


        String jsonString = gson.toJson(request);

        outMessage = new CloudQueueMessage(jsonString);

        if (outqueue == null){
            Log.d("Queue", "Outqueue is null, initializing queues");
            initQueues();
        }

        outqueue.addMessage(outMessage);


        inMessage = null;
        while (inMessage == null) {
            inMessage = inqueue.retrieveMessage();
        }
        String responseString = inMessage.getMessageContentAsString();
        response = gson.fromJson(responseString, Response.class);

        outqueue.clear();
        inqueue.clear();

        return response.isSuccess();
    }

    public boolean attemptRegistration(String email, String password) throws StorageException{
        UserRequest request = new UserRequest();
        Response response = null;
        Account account = new Account();
        account.setEmail(email);
        account.setPassword(password);

        String id = UUID.randomUUID().toString();
        request.setId(id);
        request.setAccount(account);
        request.setMethod("REGISTER");

        String jsonString = gson.toJson(request);

        outMessage = new CloudQueueMessage(jsonString);

        if (outqueue == null){
            Log.d("Queue", "Outqueue is null, initializing queues");
            initQueues();
        }

        outqueue.addMessage(outMessage);

        inMessage = null;
        while (inMessage == null) {
            inMessage = inqueue.retrieveMessage();
        }
        String responseString = inMessage.getMessageContentAsString();
        response = gson.fromJson(responseString, Response.class);

        outqueue.clear();
        inqueue.clear();

        return response.isSuccess();

    }

    public void uploadImageToStorage(ImageProxy imageProxy, String fileName) throws URISyntaxException, StorageException, IOException {
        //Connect with the blob storage and create the blob where the file is created
        CloudBlobClient blobClient = storageAccount.createCloudBlobClient();

        CloudBlobContainer container = blobClient.getContainerReference("images");

        CloudBlockBlob blob = container.getBlockBlobReference(fileName + ".jpg");

        //Create a bitmap from the image
        Image image =  imageProxy.getImage();
        ByteBuffer buffer = image.getPlanes()[0].getBuffer();
        byte[] bytes = new byte[buffer.remaining()];
        buffer.get(bytes);
        Bitmap bitmap = BitmapFactory.decodeByteArray(bytes, 0, bytes.length, null);

        //Compress the image to a jpeg and create an inputstream from it
        ByteArrayOutputStream bos = new ByteArrayOutputStream();
        bitmap.compress(Bitmap.CompressFormat.JPEG, 50, bos);
        byte[] bitmapdata = bos.toByteArray();
        Log.d("UPLOAD", "Bitmap size: "+bitmapdata.length);
        ByteArrayInputStream bis = new ByteArrayInputStream(bitmapdata);

        //Upload the image using the inputstream
        blob.upload(bis, bitmapdata.length);
        Log.d("UPLOAD", "uploadImageToStorage END");


    }
}
