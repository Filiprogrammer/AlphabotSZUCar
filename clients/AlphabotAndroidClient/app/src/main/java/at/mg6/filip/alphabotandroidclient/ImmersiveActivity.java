package at.mg6.filip.alphabotandroidclient;

import android.support.v7.app.AppCompatActivity;
import android.view.View;

public class ImmersiveActivity extends AppCompatActivity {
    @Override
    protected void onResume() {
        super.onResume();
        enableImmersiveMode();
    }

    protected void enableImmersiveMode() {
        getWindow().getDecorView().setSystemUiVisibility(View.SYSTEM_UI_FLAG_IMMERSIVE_STICKY | View.SYSTEM_UI_FLAG_FULLSCREEN | View.SYSTEM_UI_FLAG_HIDE_NAVIGATION);
    }
}
