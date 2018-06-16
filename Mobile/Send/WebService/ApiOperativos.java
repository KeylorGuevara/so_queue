package com.example.natalin.speechtotext.WebService;

import java.util.Map;

import retrofit2.Call;
import retrofit2.http.Body;
import retrofit2.http.POST;

public interface ApiOperativos {

    @POST("tikets")
    Call<Map<String,String>> sendMessage(@Body String ticket);
}
