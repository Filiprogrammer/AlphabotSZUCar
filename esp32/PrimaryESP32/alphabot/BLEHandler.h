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
    BLECharacteristic* charCollDetect;
    BLECharacteristic* charAIDrive;
    BLECharacteristic* charLPS;
    BLECharacteristic* charPosUpdate;
    BLECharacteristic* charCalibrate;
    BLECharacteristic* charInvite;
    BLECharacteristic* charPathfindingObstacles;
    BLECharacteristic* charPathfindingTarget;
    BLECharacteristic* charPathfindingPath;

    void startAdvertising();
    void stopAdvertising();
    uint32_t getConnectedCount();

    BLEHandler(void (*dataReceived)(BLEUUID, const char*, size_t), void (*disconnected)(), void (*connected)());
    ~BLEHandler();
};

#endif
