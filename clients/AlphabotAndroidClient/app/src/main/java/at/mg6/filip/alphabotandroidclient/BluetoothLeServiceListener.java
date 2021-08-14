package at.mg6.filip.alphabotandroidclient;

import android.bluetooth.BluetoothGattCharacteristic;

public interface BluetoothLeServiceListener {
    void onCharacteristicChanged(BluetoothGattCharacteristic characteristic);
    void onCharacteristicWrite(BluetoothGattCharacteristic characteristic);
    void onDisconnect();
}
