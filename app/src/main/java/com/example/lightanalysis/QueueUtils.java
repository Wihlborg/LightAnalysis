package com.example.lightanalysis;

import com.example.lightanalysis.models.Account;
import com.example.lightanalysis.models.Response;
import com.example.lightanalysis.models.UserRequest;
import com.google.gson.Gson;
import com.microsoft.azure.storage.CloudStorageAccount;
import com.microsoft.azure.storage.StorageCredentials;
import com.microsoft.azure.storage.StorageCredentialsAccountAndKey;
import com.microsoft.azure.storage.StorageException;
import com.microsoft.azure.storage.queue.CloudQueue;
import com.microsoft.azure.storage.queue.CloudQueueClient;
import com.microsoft.azure.storage.queue.CloudQueueMessage;

import java.net.URISyntaxException;
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

    public QueueUtils() {
        try {
            initQueues();
        } catch (Exception e){
            e.printStackTrace();
        }

    }

    private void initQueues() throws URISyntaxException, StorageException
    {
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
    }

    public boolean attemptLogin(String email, String pw) throws StorageException {
        UserRequest request = new UserRequest();
        Account account = new Account();
        account.setEmail(email);
        account.setPassword(pw);
        Response response = null;

        request.setMethod("LOGIN");
        request.id = UUID.randomUUID().toString();

        Gson gson = new Gson();
        String jsonString = gson.toJson(request);

        outMessage = new CloudQueueMessage(jsonString);
        outqueue.addMessage(outMessage);


        inMessage = null;
        while (inMessage == null) {
            inMessage = inqueue.retrieveMessage();
        }
        String responseString = inMessage.getMessageContentAsString();
        response = gson.fromJson(responseString, Response.class);

        outqueue.clear();
        inqueue.clear();

        if (response.isSuccess()){
            return true;
        }
        return false;
    }
}
