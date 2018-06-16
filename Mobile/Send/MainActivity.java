package com.example.natalin.speechtotext;

import android.app.Dialog;
import android.content.Context;
import android.content.Intent;
import android.net.ConnectivityManager;
import android.net.NetworkInfo;
import android.speech.RecognizerIntent;
import android.support.v7.app.AppCompatActivity;
import android.os.Bundle;
import android.view.View;
import android.widget.AdapterView;
import android.widget.ArrayAdapter;
import android.widget.Button;
import android.widget.ListView;
import android.widget.TextView;
import android.widget.Toast;

import com.example.natalin.speechtotext.WebService.Retrofit;

import org.apache.http.NameValuePair;
import org.apache.http.client.HttpClient;
import org.apache.http.client.entity.UrlEncodedFormEntity;
import org.apache.http.client.methods.HttpPost;
import org.apache.http.impl.client.DefaultHttpClient;
import org.apache.http.message.BasicNameValuePair;

import java.io.UnsupportedEncodingException;
import java.util.ArrayList;
import java.util.List;
import java.util.Map;

import retrofit2.Call;
import retrofit2.Callback;
import retrofit2.Response;

public class MainActivity extends AppCompatActivity {

    Retrofit retrofit;
    private static final int REQUEST_CODE = 1234;
    Button startButton;
    TextView speechTextView;
    Dialog matchTextDialog;
    ListView textListView;
    ArrayList<String> matchesText;


    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_main);
        retrofit =  new Retrofit();
        startButton=(Button) findViewById(R.id.button1);
        speechTextView= (TextView) findViewById(R.id.textView2);

        startButton.setOnClickListener(new View.OnClickListener() {
            @Override
            public void onClick(View view) {
                if(isConnected()){
                    Intent intent = new Intent(RecognizerIntent.ACTION_RECOGNIZE_SPEECH);
                    intent.putExtra(RecognizerIntent.EXTRA_LANGUAGE_MODEL, RecognizerIntent.LANGUAGE_MODEL_FREE_FORM);
                    startActivityForResult(intent, REQUEST_CODE);
                } else {
                    Toast.makeText(MainActivity.this, "", Toast.LENGTH_SHORT).show();
                }
            }
        });
    }

    @Override
    protected void onActivityResult(int requestCode, int resultCode, Intent data) {
        super.onActivityResult(requestCode, resultCode, data);
        matchTextDialog = new Dialog(MainActivity.this); //Create a Dialog
        matchTextDialog.setContentView(R.layout.dialog_matches_frag); //Link the new Dialog with the dialog_matches frag
        matchTextDialog.setTitle("Select Matching Text"); //Add title to the Dialog
        textListView = (ListView) matchTextDialog.findViewById(R.id.listView1);
        matchesText = data.getStringArrayListExtra(RecognizerIntent.EXTRA_RESULTS); //Get data of data
        ArrayAdapter<String> adapter = new ArrayAdapter<String>(this, android.R.layout.simple_list_item_1, matchesText );
        textListView.setAdapter(adapter);
        textListView.setOnItemClickListener(new AdapterView.OnItemClickListener() {
            @Override
            public void onItemClick(AdapterView<?> adapterView, View view, int i, long l) {
                speechTextView.setText("Se han comprado: " + matchesText.get(i) + " entradas. Disfrutalas.");
                matchTextDialog.hide();
                int cantidad_tiquetes = 0;
                String tiquetes = matchesText.get(i);
                try {
                    //cantidad_tiquetes = Integer.parseInt(tiquetes.toString());
                    sendMessage();
                } catch (NumberFormatException nfe) {
                    System.out.println("Could not parse " + nfe);
                }
            }
        });
        matchTextDialog.show();
    }
    public void sendMessage(){
        this.retrofit.getService().sendMessage("prueba").enqueue(new Callback<Map<String, String>>() {
            @Override
            public void onResponse(Call<Map<String, String>> call, Response<Map<String, String>> response) {
                if (response.isSuccessful()){
                    System.out.println(response.body());
                } else {
                    System.out.println("Error "+response.message());
                }
            }

            @Override
            public void onFailure(Call<Map<String, String>> call, Throwable t) {
                System.out.println("Error2 " + t.getMessage());
            }
        });
    }
    /**
     To Check if the net is available and conected
     * @return true if the net is available and conected
     * and false in other case
     */
    //Verificar conexion para que la app no se vaya a caer
    public boolean isConnected(){
        ConnectivityManager cm = (ConnectivityManager) getSystemService(Context.CONNECTIVITY_SERVICE);
        NetworkInfo net = cm.getActiveNetworkInfo();
        if (net!= null && net.isAvailable() && net.isConnected()){
            return true;
        }   else {
            return false;
        }
    }
}
