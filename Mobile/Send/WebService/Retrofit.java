package com.example.natalin.speechtotext.WebService;

import retrofit2.converter.gson.GsonConverterFactory;

public class Retrofit {
    private ApiOperativos service;
    private final retrofit2.Retrofit retrofit;
    //La ruta raíz del servicio web //http://localhost:3000/
    private String API_ROOT= "http://172.24.29.223:8080";
    public Retrofit() {
        this.retrofit = new retrofit2.Retrofit.Builder()
                .baseUrl(API_ROOT).addConverterFactory(GsonConverterFactory.create()).build();
        this.service =retrofit.create(ApiOperativos. class);
    }

    public ApiOperativos getService() {
        return service;
    }

    public retrofit2.Retrofit getRetrofit() {
        return retrofit;
    }

    public String getAPI_ROOT() {
        return API_ROOT;
    }
}
