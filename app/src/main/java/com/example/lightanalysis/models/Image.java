package com.example.lightanalysis.models;

public class Image {
    private String url;
    private String email;
    private double lat;
    private double lon;

    public Image(String url, String email, double lat, double lon) {
        this.url = url;
        this.email = email;
        this.lat = lat;
        this.lon = lon;
    }

    public String getUrl() {
        return url;
    }

    public String getEmail() {
        return email;
    }

    public double getLat() {
        return lat;
    }

    public double getLon() {
        return lon;
    }
}
