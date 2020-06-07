package com.example.lightanalysis.models;

public class UserRequest {


    public String LOGIN = "LOGIN";
    public String REGISTER = "REGISTER";
    public String CHECKIN = "CHECKIN";
    public String LOGOUT = "LOGOUT";
    public String RETRIEVE = "RETRIEVE";
    public String RETRIEVEALL = "RETRIEVEALL";
    public String DELETE = "DELETE";

    public String method;
    public String id;
    public Account account;

    public String getMethod() {
        return method;
    }

    public void setMethod(String method) {
        this.method = method;
    }

    public String getId() {
        return id;
    }

    public void setId(String id) {
        this.id = id;
    }

    public Account getAccount() {
        return account;
    }

    public void setAccount(Account account) {
        this.account = account;
    }


}
