package at.mg6.filip.alphabotandroidclient;

import android.bluetooth.BluetoothGattCharacteristic;

import java.util.ArrayList;
import java.util.List;
import java.util.Timer;
import java.util.TimerTask;

public class BLECharacteristicSender {
    private BluetoothGattCharacteristic bleCharacteristic;
    private Timer sendTimer;
    private TimerTask sendTimerTask;
    private List<BLECharacteristicSenderListener> listeners = new ArrayList<>();

    public BLECharacteristicSender(BluetoothLeService bleClient, BluetoothGattCharacteristic characteristic) {
        bleCharacteristic = characteristic;

        BluetoothLeServiceListener bleListener = new BluetoothLeServiceListener() {
            @Override
            public void onCharacteristicChanged(BluetoothGattCharacteristic characteristic) {}

            @Override
            public void onCharacteristicWrite(BluetoothGattCharacteristic characteristic) {
                if (characteristic.getUuid().compareTo(bleCharacteristic.getUuid()) == 0) {
                    if (sendTimer != null)
                        sendTimer.cancel();

                    for (BLECharacteristicSenderListener listener : listeners)
                        listener.onValueArrive(characteristic.getValue());
                }
            }

            @Override
            public void onDisconnect() {
                if (sendTimer != null) {
                    sendTimer.cancel();
                    sendTimerTask.cancel();
                }
            }
        };
        bleClient.addListener(bleListener);
    }

    public void sendValue(byte[] value) {
        bleCharacteristic.setValue(value);

        if (sendTimer != null)
            sendTimer.cancel();

        sendTimer = new Timer();
        sendTimerTask = new TimerTask() {
            @Override
            public void run() {
                BLEHandler.getInstance().bleClient.sendCharacteristic(bleCharacteristic);
            }
        };
        sendTimer.schedule(sendTimerTask, 0, 300);
    }

    public void addListener(BLECharacteristicSenderListener listener) {
        listeners.add(listener);
    }

    public void removeListener(BLECharacteristicSenderListener listener) {
        listeners.remove(listener);
    }

    public interface BLECharacteristicSenderListener {
        void onValueArrive(byte[] value);
    }
}
