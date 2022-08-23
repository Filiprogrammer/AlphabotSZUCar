#ifndef BLEHANDLER_H
#define BLEHANDLER_H

#include <Arduino.h>
#include <BLEDevice.h>
#include <BLEServer.h>
#include <BLE2902.h>

class BLEHandler : public BLEServerCallbacks {
private:
    BLEServer* server;
    BLEService* service;
    BLEAdvertising* advertising;

public:
    BLECharacteristic* charClientPing;
    BLECharacteristic* charUpdateMotors;
    BLECharacteristic* charToggle;
    BLECharacteristic* charSensor;
    BLECharacteristic* charPathfindingTarget;
    BLECharacteristic* charCalibrate;
    BLECharacteristic* charAddObstacle;
    BLECharacteristic* charRemoveObstacle;
    BLECharacteristic* charPathfindingPath;
    BLECharacteristic* charAnchorLocations;
    BLECharacteristic* charAnchorDistances;
    BLECharacteristic* charImu;
    BLECharacteristic* charWheelSpeed;
    BLECharacteristic* charError;

    void startAdvertising();
    void stopAdvertising();
    uint32_t getConnectedCount();

    BLEHandler(void (*charUpdateMotorsDataReceived)(const char*, size_t),
               void (*charToggleDataReceived)(const char*, size_t),
               void (*charToggleStateChanged)(BLECharacteristicCallbacks::Status, uint32_t),
               void (*charPathfindingTargetDataReceived)(const char*, size_t),
               void (*charCalibrateDataReceived)(const char*, size_t),
               void (*charAddObstacleDataReceived)(const char*, size_t),
               void (*charAddObstacleStateChanged)(BLECharacteristicCallbacks::Status, uint32_t),
               void (*charRemoveObstacleDataReceived)(const char*, size_t),
               void (*charAnchorLocationsDataReceived)(const char*, size_t),
               void (*disconnected)(), void (*connected)());
    ~BLEHandler();
};

#endif
