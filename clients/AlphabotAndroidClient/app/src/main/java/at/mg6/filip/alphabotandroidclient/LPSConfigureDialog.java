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

public class LPSConfigureDialog extends AppCompatDialogFragment {
    private TextInputEditText edTxt0x;
    private TextInputEditText edTxt0y;
    private TextInputEditText edTxt1x;
    private TextInputEditText edTxt1y;
    private TextInputEditText edTxt2x;
    private TextInputEditText edTxt2y;
    private LPSConfigureDialogListener listener;

    @Override
    public Dialog onCreateDialog(Bundle savedInstanceState) {
        setCancelable(false);
        AlertDialog.Builder builder = new AlertDialog.Builder(getActivity());

        LayoutInflater inflater = getActivity().getLayoutInflater();
        View view = inflater.inflate(R.layout.dialog_lps_configure, null);

        builder.setView(view)
                .setTitle("LPS Configure")
                .setNegativeButton("cancel", new DialogInterface.OnClickListener() {
                    @Override
                    public void onClick(DialogInterface dialogInterface, int i) {
                        listener.onLPSConfigureDialogCancel();
                    }
                })
                .setPositiveButton("ok", new DialogInterface.OnClickListener() {
                    @Override
                    public void onClick(DialogInterface dialogInterface, int i) {
                        short anchor0x = tryParseShort(edTxt0x.getText().toString(), (short)0);
                        short anchor0y = tryParseShort(edTxt0y.getText().toString(), (short)0);
                        short anchor1x = tryParseShort(edTxt1x.getText().toString(), (short)0);
                        short anchor1y = tryParseShort(edTxt1y.getText().toString(), (short)0);
                        short anchor2x = tryParseShort(edTxt2x.getText().toString(), (short)0);
                        short anchor2y = tryParseShort(edTxt2y.getText().toString(), (short)0);
                        listener.onLPSConfigureDialogConfirm(anchor0x, anchor0y, anchor1x, anchor1y, anchor2x, anchor2y);
                    }
                });

        edTxt0x = view.findViewById(R.id.edTxt0x);
        edTxt0y = view.findViewById(R.id.edTxt0y);
        edTxt1x = view.findViewById(R.id.edTxt1x);
        edTxt1y = view.findViewById(R.id.edTxt1y);
        edTxt2x = view.findViewById(R.id.edTxt2x);
        edTxt2y = view.findViewById(R.id.edTxt2y);

        Bundle args = getArguments();
        edTxt0x.setText("" + args.getShort("anchor0x"));
        edTxt0y.setText("" + args.getShort("anchor0y"));
        edTxt1x.setText("" + args.getShort("anchor1x"));
        edTxt1y.setText("" + args.getShort("anchor1y"));
        edTxt2x.setText("" + args.getShort("anchor2x"));
        edTxt2y.setText("" + args.getShort("anchor2y"));

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
            listener = (LPSConfigureDialogListener) context;
        } catch (ClassCastException e) {
            throw new ClassCastException(context.toString() + " must implement LPSConfigureDialogListener");
        }
    }

    public interface LPSConfigureDialogListener {
        void onLPSConfigureDialogConfirm(short anchor0x, short anchor0y, short anchor1x, short anchor1y, short anchor2x, short anchor2y);
        void onLPSConfigureDialogCancel();
    }

    private short tryParseShort(String value, short defaultVal) {
        try {
            return Short.parseShort(value);
        } catch (NumberFormatException e) {
            return defaultVal;
        }
    }
}
