package at.mg6.filip.alphabotandroidclient;

import android.app.Dialog;
import android.content.Context;
import android.content.DialogInterface;
import android.os.Bundle;
import android.support.design.widget.TextInputEditText;
import android.support.v7.app.AlertDialog;
import android.support.v7.app.AppCompatDialogFragment;
import android.view.LayoutInflater;
import android.view.View;
import android.view.Window;
import android.view.WindowManager;

public class ObstacleConfigureDialog extends AppCompatDialogFragment {
    private TextInputEditText edTxtX;
    private TextInputEditText edTxtY;
    private TextInputEditText edTxtWidth;
    private TextInputEditText edTxtHeight;
    private ObstacleConfigureDialogListener listener;

    @Override
    public Dialog onCreateDialog(Bundle savedInstanceState) {
        setCancelable(false);
        AlertDialog.Builder builder = new AlertDialog.Builder(getActivity());

        LayoutInflater inflater = getActivity().getLayoutInflater();
        View view = inflater.inflate(R.layout.dialog_obstacle_configure, null);

        builder.setView(view)
                .setTitle("Obstacle Configure")
                .setNegativeButton("cancel", new DialogInterface.OnClickListener() {
                    @Override
                    public void onClick(DialogInterface dialogInterface, int i) {
                        listener.onObstacleConfigureDialogCancel();
                    }
                })
                .setPositiveButton("ok", new DialogInterface.OnClickListener() {
                    @Override
                    public void onClick(DialogInterface dialogInterface, int i) {
                        short x = tryParseShort(edTxtX.getText().toString(), (short)0);
                        short y = tryParseShort(edTxtY.getText().toString(), (short)0);
                        short width = tryParseShort(edTxtWidth.getText().toString(), (short)0);
                        short height = tryParseShort(edTxtHeight.getText().toString(), (short)0);
                        listener.onObstacleConfigureDialogConfirm(x, y, width, height);
                    }
                });

        edTxtX = view.findViewById(R.id.edTxtX);
        edTxtY = view.findViewById(R.id.edTxtY);
        edTxtWidth = view.findViewById(R.id.edTxtWidth);
        edTxtHeight = view.findViewById(R.id.edTxtHeight);

        Bundle args = getArguments();
        edTxtX.setText("" + args.getShort("x"));
        edTxtY.setText("" + args.getShort("y"));
        edTxtWidth.setText("" + args.getShort("width"));
        edTxtHeight.setText("" + args.getShort("height"));

        AlertDialog dialog = builder.create();
        final Window dialogWindow = dialog.getWindow();

        dialogWindow.setFlags(WindowManager.LayoutParams.FLAG_NOT_FOCUSABLE, WindowManager.LayoutParams.FLAG_NOT_FOCUSABLE);
        dialogWindow.getDecorView().setSystemUiVisibility(getActivity().getWindow().getDecorView().getSystemUiVisibility());

        dialog.setOnShowListener(new DialogInterface.OnShowListener() {
            @Override
            public void onShow(DialogInterface dialogInterface) {
                dialogWindow.clearFlags(WindowManager.LayoutParams.FLAG_NOT_FOCUSABLE);
                WindowManager wm = (WindowManager) getActivity().getSystemService(Context.WINDOW_SERVICE);
                wm.updateViewLayout(dialogWindow.getDecorView(), dialogWindow.getAttributes());
            }
        });

        return dialog;
    }

    @Override
    public void onAttach(Context context) {
        super.onAttach(context);

        try {
            listener = (ObstacleConfigureDialogListener) context;
        } catch (ClassCastException e) {
            throw new ClassCastException(context.toString() + " must implement ObstacleConfigureDialogListener");
        }
    }

    public interface ObstacleConfigureDialogListener {
        void onObstacleConfigureDialogConfirm(short x, short y, short width, short height);
        void onObstacleConfigureDialogCancel();
    }

    private short tryParseShort(String value, short defaultVal) {
        try {
            return Short.parseShort(value);
        } catch (NumberFormatException e) {
            return defaultVal;
        }
    }
}
