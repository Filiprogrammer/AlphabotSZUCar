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
    public final UUID CHAR_UUID_TOGGLE = UUID.fromString("fce001d4-864a-48f4-9c95-de928f1da07b");
    public final UUID CHAR_UUID_SENSOR = UUID.fromString("4c999381-35e2-4af4-8443-ee8b9fe56ba0");
    public final UUID CHAR_UUID_PATHFINDING_TARGET = UUID.fromString("f56f0a15-52ae-4ad5-bfe1-557eed983618");
    public final UUID CHAR_UUID_CALIBRATE = UUID.fromString("d39e8d54-8019-46c8-a977-db13871bac59");
    public final UUID CHAR_UUID_ADD_OBSTACLE = UUID.fromString("60db37c7-afeb-4d40-bb17-a19a07d6fc95");
    public final UUID CHAR_UUID_REMOVE_OBSTACLE = UUID.fromString("6d43e0df-682b-45ef-abb7-814ecf475771");
    public final UUID CHAR_UUID_PATHFINDING_PATH = UUID.fromString("8dad4c9a-1a1c-4a42-a522-ded592f4ed99");
    public final UUID CHAR_UUID_ANCHOR_LOCATIONS = UUID.fromString("8a55dd30-463b-40f6-8f21-d68efcc386b2");
    public final UUID CHAR_UUID_ANCHOR_DISTANCES = UUID.fromString("254492a2-9324-469b-b1e2-4d4590972c35");
    public final UUID CHAR_UUID_IMU = UUID.fromString("93758afa-ce6f-4670-9562-ce92bda84d49");
    public final UUID CHAR_UUID_WHEEL_SPEED = UUID.fromString("8efafa16-15de-461f-bde1-493261201e2b");
    public final UUID CHAR_UUID_ERROR = UUID.fromString("dc458f08-ea3e-4fe1-adb3-25c840be081a");

    public BluetoothLeService bleClient;
    public BluetoothGattService bleService;
    public BluetoothGattCharacteristic bleCharPingclient;
    public BluetoothGattCharacteristic bleCharUpdateMotors;
    public BluetoothGattCharacteristic bleCharToggle;
    public BluetoothGattCharacteristic bleCharSensor;
    public BluetoothGattCharacteristic bleCharPathfindingTarget;
    public BluetoothGattCharacteristic bleCharCalibrate;
    public BluetoothGattCharacteristic bleCharAddObstacle;
    public BluetoothGattCharacteristic bleCharRemoveObstacle;
    public BluetoothGattCharacteristic bleCharPathfindingPath;
    public BluetoothGattCharacteristic bleCharAnchorLocations;
    public BluetoothGattCharacteristic bleCharAnchorDistances;
    public BluetoothGattCharacteristic bleCharImu;
    public BluetoothGattCharacteristic bleCharWheelSpeed;
    public BluetoothGattCharacteristic bleCharError;

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

        System.out.println("Discovered PINGCLIENT");

        // Characteristic UPDATEMOTORS
        bleCharUpdateMotors = tryGetBLECharacteristic(CHAR_UUID_UPDATEMOTORS, 10);

        if (bleCharUpdateMotors == null)
            return ConnectResult.UNABLE_TO_FIND_ALL_CHARS;

        System.out.println("Discovered UPDATEMOTORS");

        // Characteristic TOGGLE
        bleCharToggle = tryGetBLECharacteristic(CHAR_UUID_TOGGLE, 10);

        if (bleCharToggle == null)
            return ConnectResult.UNABLE_TO_FIND_ALL_CHARS;

        System.out.println("Discovered TOGGLE");

        bleClient.setCharacteristicNotification(bleCharToggle, true);

        // Characteristic SENSOR
        bleCharSensor = tryGetBLECharacteristic(CHAR_UUID_SENSOR, 10);

        if (bleCharSensor == null)
            return ConnectResult.UNABLE_TO_FIND_ALL_CHARS;

        System.out.println("Discovered SENSOR");

        bleClient.setCharacteristicNotification(bleCharSensor, true);

        // Characteristic PATHFINDING_TARGET
        bleCharPathfindingTarget = tryGetBLECharacteristic(CHAR_UUID_PATHFINDING_TARGET, 10);

        if (bleCharPathfindingTarget == null)
            return ConnectResult.UNABLE_TO_FIND_ALL_CHARS;

        System.out.println("Discovered PATHFINDING_TARGET");

        // Characteristic CALIBRATE
        bleCharCalibrate = tryGetBLECharacteristic(CHAR_UUID_CALIBRATE, 10);

        if (bleCharCalibrate == null)
            return ConnectResult.UNABLE_TO_FIND_ALL_CHARS;

        System.out.println("Discovered CALIBRATE");

        bleClient.setCharacteristicNotification(bleCharCalibrate, true);

        // Characteristic ADD_OBSTACLE
        bleCharAddObstacle = tryGetBLECharacteristic(CHAR_UUID_ADD_OBSTACLE, 10);

        if (bleCharAddObstacle == null)
            return ConnectResult.UNABLE_TO_FIND_ALL_CHARS;

        System.out.println("Discovered ADD_OBSTACLE");

        bleClient.setCharacteristicNotification(bleCharAddObstacle, true);

        // Characteristic REMOVE_OBSTACLE
        bleCharRemoveObstacle = tryGetBLECharacteristic(CHAR_UUID_REMOVE_OBSTACLE, 10);

        if (bleCharRemoveObstacle == null)
            return ConnectResult.UNABLE_TO_FIND_ALL_CHARS;

        System.out.println("Discovered REMOVE_OBSTACLE");

        bleClient.setCharacteristicNotification(bleCharRemoveObstacle, true);

        // Characteristic PATHFINDING_PATH
        bleCharPathfindingPath = tryGetBLECharacteristic(CHAR_UUID_PATHFINDING_PATH, 10);

        if (bleCharPathfindingPath == null)
            return ConnectResult.UNABLE_TO_FIND_ALL_CHARS;

        System.out.println("Discovered PATHFINDING_PATH");

        bleClient.setCharacteristicNotification(bleCharPathfindingPath, true);

        // Characteristic ANCHOR_LOCATIONS
        bleCharAnchorLocations = tryGetBLECharacteristic(CHAR_UUID_ANCHOR_LOCATIONS, 10);

        if (bleCharAnchorLocations == null)
            return ConnectResult.UNABLE_TO_FIND_ALL_CHARS;

        System.out.println("Discovered ANCHOR_LOCATIONS");

        // Characteristic ANCHOR_DISTANCES
        bleCharAnchorDistances = tryGetBLECharacteristic(CHAR_UUID_ANCHOR_DISTANCES, 10);

        if (bleCharAnchorDistances == null)
            return ConnectResult.UNABLE_TO_FIND_ALL_CHARS;

        System.out.println("Discovered ANCHOR_DISTANCES");

        bleClient.setCharacteristicNotification(bleCharAnchorDistances, true);

        // Characteristic IMU
        bleCharImu = tryGetBLECharacteristic(CHAR_UUID_IMU, 10);

        if (bleCharImu == null)
            return ConnectResult.UNABLE_TO_FIND_ALL_CHARS;

        System.out.println("Discovered IMU");

        bleClient.setCharacteristicNotification(bleCharImu, true);

        // Characteristic WHEEL_SPEED
        bleCharWheelSpeed = tryGetBLECharacteristic(CHAR_UUID_WHEEL_SPEED, 10);

        if (bleCharWheelSpeed == null)
            return ConnectResult.UNABLE_TO_FIND_ALL_CHARS;

        System.out.println("Discovered WHEEL_SPEED");

        bleClient.setCharacteristicNotification(bleCharWheelSpeed, true);

        // Characteristic ERROR
        bleCharError = tryGetBLECharacteristic(CHAR_UUID_ERROR, 10);

        if (bleCharError == null)
            return ConnectResult.UNABLE_TO_FIND_ALL_CHARS;

        System.out.println("Discovered ERROR");

        bleClient.setCharacteristicNotification(bleCharError, true);

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
