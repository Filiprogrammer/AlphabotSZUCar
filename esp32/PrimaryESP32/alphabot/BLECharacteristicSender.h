#ifndef BLECHARACTERISTICSENDER_H
#define BLECHARACTERISTICSENDER_H

#include <Arduino.h>
#include <BLECharacteristic.h>
#include <queue>

struct DataToSend {
    uint8_t len;
    uint8_t data[20];
};

class BLECharacteristicSender {
private:
    BLECharacteristic* characteristic;
    void (*valueArrived)();
    TaskHandle_t sendTask;
    std::queue<struct DataToSend*> toSendQueue;
    struct DataToSend* currentlySending;
    SemaphoreHandle_t currentlySendingMutex;

    void sendCharacteristicTask();

public:
    void sendValue(const uint8_t* data, uint8_t len);
    void stateChanged(BLECharacteristicCallbacks::Status s, uint32_t code);

    BLECharacteristicSender(BLECharacteristic* characteristic, void (*valueArrived)());
    ~BLECharacteristicSender();
};

#endif
