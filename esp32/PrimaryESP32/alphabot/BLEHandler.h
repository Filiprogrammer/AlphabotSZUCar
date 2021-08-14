#ifndef BLEHANDLER_H
#define BLEHANDLER_H

#include <Arduino.h>
#include <BLEDevice.h>
#include <BLEServer.h>
#include <BLE2902.h>

class BLEHandler : public BLEServerCallbacks {
private:
    void (*disconnected)();
    void (*connected)();
    BLEService* service;

public:
    BLEServer* server;
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
    BLEAdvertising* advertising;

    void onConnect(BLEServer* pServer);
    void onDisconnect(BLEServer* pServer);

    BLEHandler(void (*dataReceived)(BLEUUID, const char*, size_t), void (*disconnected)(), void (*connected)());
    ~BLEHandler();
};

#endif
