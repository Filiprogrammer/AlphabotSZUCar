package at.mg6.filip.alphabotandroidclient;

import android.bluetooth.BluetoothAdapter;
import android.content.Intent;
import android.os.AsyncTask;
import android.support.v7.app.AppCompatActivity;
import android.os.Bundle;
import android.view.View;
import android.widget.Button;
import android.widget.EditText;
import android.widget.Toast;

public class MainActivity extends AppCompatActivity {
    Button btnConnectBLE;
    EditText bleMacText;

    View.OnClickListener onBtnConnectBLEClick = new View.OnClickListener() {
        @Override
        public void onClick(View view) {
            if (BluetoothAdapter.getDefaultAdapter().isEnabled()) {
                btnConnectBLE.setEnabled(false);
                new ConnectBleTask().execute();
            } else {
                Intent turnOn = new Intent(BluetoothAdapter.ACTION_REQUEST_ENABLE);
                startActivityForResult(turnOn, 0);
            }
        }
    };

    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_main);

        btnConnectBLE = findViewById(R.id.btnConnectBLE);
        btnConnectBLE.setOnClickListener(onBtnConnectBLEClick);
        bleMacText = findViewById(R.id.edTxtBleMac);
        setTitle("Alphabot Android Client");
    }

    public class ConnectBleTask extends AsyncTask<Void, Void, Void> {
        @Override
        protected Void doInBackground(Void... args) {
            final BLEHandler.ConnectResult connectResult = BLEHandler.getInstance().connect(getApplicationContext(), bleMacText.getText().toString());

            runOnUiThread(new Runnable() {
                @Override
                public void run() {
                    switch (connectResult) {
                        case COULD_NOT_CONNECT:
                            Toast.makeText(getApplicationContext(), "Could not connect", Toast.LENGTH_SHORT).show();
                            btnConnectBLE.setEnabled(true);
                            break;
                        case UNABLE_TO_FIND_ALL_CHARS:
                            Toast.makeText(getApplicationContext(), "Unable to find all characteristics", Toast.LENGTH_SHORT).show();
                            btnConnectBLE.setEnabled(true);
                            break;
                        case UNABLE_TO_INIT_BT:
                            Toast.makeText(getApplicationContext(), "Unable to initialize Bluetooth", Toast.LENGTH_SHORT).show();
                            btnConnectBLE.setEnabled(true);
                            break;
                        case SUCCESS:
                            Toast.makeText(getApplicationContext(), "Connected", Toast.LENGTH_SHORT).show();
                            Intent ctrlActivityIntent = new Intent(getApplicationContext(), ControlActivity.class);
                            startActivity(ctrlActivityIntent);
                            btnConnectBLE.setEnabled(true);
                            break;
                    }
                }
            });

            return null;
        }
    }
}
