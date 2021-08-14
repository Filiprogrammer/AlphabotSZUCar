package at.mg6.filip.alphabotandroidclient;

import android.bluetooth.BluetoothGattCharacteristic;
import android.bluetooth.BluetoothGattService;
import android.content.Context;

import java.util.UUID;

public class BLEHandler {
    private static BLEHandler instance = null;
    public final UUID SERVICE_UUID = UUID.fromString("4fafc201-1fb5-459e-8fcc-c5c9c331914b");
    public final UUID CHAR_UUID_PINGCLIENT = UUID.fromString("117ad3a5-b257-4465-abd4-7dc12a4cf77d");
    public final UUID CHAR_UUID_UPDATEMOTORS = UUID.fromString("a04295f7-eaa8-4536-b3a4-4e7ae4d72dc2");
    public final UUID CHAR_UUID_COLLDETECT = UUID.fromString("c9157183-c05f-4331-a74b-153c3723f69c");
    public final UUID CHAR_UUID_AIDRIVE = UUID.fromString("1a4032c8-e071-4f08-a4e6-688771a91a33");
    public final UUID CHAR_UUID_LPS = UUID.fromString("63bbadb7-bfb9-4e36-a028-1747d70bfbbc");
    public final UUID CHAR_UUID_POSUPDATE = UUID.fromString("11618450-ceec-438d-b376-ce666e612da1");
    public final UUID CHAR_UUID_CALIBRATE = UUID.fromString("d39e8d54-8019-46c8-a977-db13871bac59");
    public final UUID CHAR_UUID_INVITE = UUID.fromString("39d3a28f-b403-4c78-8f8f-f9549ff2d838");
    public final UUID CHAR_UUID_PATHFINDING_OBSTACLES = UUID.fromString("60db37c7-afeb-4d40-bb17-a19a07d6fc95");
    public final UUID CHAR_UUID_PATHFINDING_TARGET = UUID.fromString("f56f0a15-52ae-4ad5-bfe1-557eed983618");
    public final UUID CHAR_UUID_PATHFINDING_PATH = UUID.fromString("8dad4c9a-1a1c-4a42-a522-ded592f4ed99");
    public BluetoothLeService bleClient;
    public BluetoothGattService bleService;
    public BluetoothGattCharacteristic bleCharPingclient;
    public BluetoothGattCharacteristic bleCharUpdateMotors;
    public BluetoothGattCharacteristic bleCharCollDetect;
    public BluetoothGattCharacteristic bleCharAIDrive;
    public BluetoothGattCharacteristic bleCharLPS;
    public BluetoothGattCharacteristic bleCharPosUpdate;
    public BluetoothGattCharacteristic bleCharCalibrate;
    public BluetoothGattCharacteristic bleCharInvite;
    public BluetoothGattCharacteristic bleCharPathfindingObstacles;
    public BluetoothGattCharacteristic bleCharPathfindingTarget;
    public BluetoothGattCharacteristic bleCharPathfindingPath;

    enum ConnectResult {
        SUCCESS,
        UNABLE_TO_INIT_BT,
        UNABLE_TO_FIND_ALL_CHARS,
        COULD_NOT_CONNECT
    }

    private BLEHandler() {}

    public ConnectResult connect(Context context, String connectTo) {
        bleClient = new BluetoothLeService();

        if (!bleClient.initialize(context))
            return ConnectResult.UNABLE_TO_INIT_BT;

        if (!bleClient.connect(connectTo))
            return ConnectResult.COULD_NOT_CONNECT;

        for (int i = 0; i < 100; ++i) {
            if (bleClient.isConnected())
                break;

            try {
                Thread.sleep(100);
            } catch (InterruptedException e) {
                e.printStackTrace();
            }
        }

        if (!bleClient.isConnected())
            return ConnectResult.COULD_NOT_CONNECT;

        bleClient.waitUntilServicesDiscovered();
        bleClient.getSupportedGattServices();
        bleService = bleClient.getGattService(SERVICE_UUID);

        // Characteristic PINGCLIENT
        bleCharPingclient = tryGetBLECharacteristic(CHAR_UUID_PINGCLIENT, 10);

        if (bleCharPingclient == null)
            return ConnectResult.UNABLE_TO_FIND_ALL_CHARS;

        // Characteristic UPDATEMOTORS
        bleCharUpdateMotors = tryGetBLECharacteristic(CHAR_UUID_UPDATEMOTORS, 10);

        if (bleCharUpdateMotors == null)
            return ConnectResult.UNABLE_TO_FIND_ALL_CHARS;

        // Characteristic COLLDETECT
        bleCharCollDetect = tryGetBLECharacteristic(CHAR_UUID_COLLDETECT, 10);

        if (bleCharCollDetect == null)
            return ConnectResult.UNABLE_TO_FIND_ALL_CHARS;

        bleClient.setCharacteristicNotification(bleCharCollDetect, true);

        // Characteristic AIDRIVE
        bleCharAIDrive = tryGetBLECharacteristic(CHAR_UUID_AIDRIVE, 10);

        if (bleCharAIDrive == null)
            return ConnectResult.UNABLE_TO_FIND_ALL_CHARS;

        bleClient.setCharacteristicNotification(bleCharAIDrive, true);

        // Characteristic LPS
        bleCharLPS = tryGetBLECharacteristic(CHAR_UUID_LPS, 10);

        if (bleCharLPS == null)
            return ConnectResult.UNABLE_TO_FIND_ALL_CHARS;

        bleClient.setCharacteristicNotification(bleCharLPS, true);

        // Characteristic POSUPDATE
        bleCharPosUpdate = tryGetBLECharacteristic(CHAR_UUID_POSUPDATE, 10);

        if (bleCharPosUpdate == null)
            return ConnectResult.UNABLE_TO_FIND_ALL_CHARS;

        bleClient.setCharacteristicNotification(bleCharPosUpdate, true);

        // Characteristic CALIBRATE
        bleCharCalibrate = tryGetBLECharacteristic(CHAR_UUID_CALIBRATE, 10);

        if (bleCharCalibrate == null)
            return ConnectResult.UNABLE_TO_FIND_ALL_CHARS;

        bleClient.setCharacteristicNotification(bleCharCalibrate, true);

        // Characteristic INVITE
        bleCharInvite = tryGetBLECharacteristic(CHAR_UUID_INVITE, 10);

        if (bleCharInvite == null)
            return ConnectResult.UNABLE_TO_FIND_ALL_CHARS;

        bleClient.setCharacteristicNotification(bleCharInvite, true);

        // Characteristic PATHFINDING_OBSTACLES
        bleCharPathfindingObstacles = tryGetBLECharacteristic(CHAR_UUID_PATHFINDING_OBSTACLES, 10);

        if (bleCharPathfindingObstacles == null)
            return ConnectResult.UNABLE_TO_FIND_ALL_CHARS;

        // Characteristic PATHFINDING_TARGET
        bleCharPathfindingTarget = tryGetBLECharacteristic(CHAR_UUID_PATHFINDING_TARGET, 10);

        if (bleCharPathfindingTarget == null)
            return ConnectResult.UNABLE_TO_FIND_ALL_CHARS;

        // Characteristic PATHFINDING_PATH
        bleCharPathfindingPath = tryGetBLECharacteristic(CHAR_UUID_PATHFINDING_PATH, 10);

        if (bleCharPathfindingPath == null)
            return ConnectResult.UNABLE_TO_FIND_ALL_CHARS;

        bleClient.setCharacteristicNotification(bleCharPathfindingPath, true);

        return ConnectResult.SUCCESS;
    }

    public void disconnect() {
        if (bleClient != null)
            bleClient.disconnect();
    }

    public static BLEHandler getInstance() {
        if (instance == null)
            instance = new BLEHandler();

        return instance;
    }

    private BluetoothGattCharacteristic tryGetBLECharacteristic(UUID uuid, int attempts) {
        int i;
        BluetoothGattCharacteristic characteristic = null;

        for (i = 0; i < attempts; ++i) {
            try{
                characteristic = bleService.getCharacteristic(uuid);
            } catch(Exception e) { }

            if (characteristic != null)
                break;
        }

        return characteristic;
    }
}
