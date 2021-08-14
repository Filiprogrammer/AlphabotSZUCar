package at.mg6.filip.alphabotandroidclient;

import android.os.Handler;
import android.view.MotionEvent;
import android.view.View;

public class HoldDownListener implements View.OnTouchListener {
    private View downView;
    private Handler handler = new Handler();
    private View.OnClickListener clickListener;
    private Runnable handlerRunnable = new Runnable() {
        @Override
        public void run() {
            clickListener.onClick(downView);
            handler.postDelayed(this, 100);
        }
    };

    public HoldDownListener(View.OnClickListener clickListener) {
        if (clickListener == null)
            throw new IllegalArgumentException("clickListener must not be null");

        this.clickListener = clickListener;
    }

    @Override
    public boolean onTouch(View view, MotionEvent motionEvent) {
        switch(motionEvent.getAction()) {
            case MotionEvent.ACTION_DOWN:
                if (handler != null)
                    return true;

                downView = view;
                handler = new Handler();
                handler.post(handlerRunnable);
                break;
            case MotionEvent.ACTION_UP:
                if (handler == null)
                    return true;

                handler.removeCallbacks(handlerRunnable);
                handler = null;
                break;
        }

        return false;
    }
}
