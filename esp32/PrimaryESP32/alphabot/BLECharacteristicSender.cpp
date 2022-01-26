#include "BLECharacteristicSender.h"

void BLECharacteristicSender::sendValue(const uint8_t* data, uint8_t len) {
    struct DataToSend* dataToSend = (struct DataToSend*)malloc(sizeof(struct DataToSend));
    dataToSend->len = len;
    memcpy(dataToSend->data, data, len);
    toSendQueue.push(dataToSend);
}

void BLECharacteristicSender::sendCharacteristicTask() {
    for (;;) {
        if (currentlySending == NULL && !toSendQueue.empty()) {
            currentlySending = toSendQueue.front();
            toSendQueue.pop();
        }

        if (currentlySending != NULL && xSemaphoreTake(currentlySendingMutex, 0) == pdTRUE) {
            characteristic->setValue(currentlySending->data, currentlySending->len);
            xSemaphoreGive(currentlySendingMutex);
            characteristic->notify();
        }

        delay(300);
    }
}

void BLECharacteristicSender::stateChanged(BLECharacteristicCallbacks::Status s, uint32_t code) {
    if (s == BLECharacteristicCallbacks::Status::SUCCESS_NOTIFY) {
        while (xSemaphoreTake(currentlySendingMutex, 0) != pdTRUE)
            delay(10);

        free(currentlySending);
        currentlySending = NULL;
        xSemaphoreGive(currentlySendingMutex);
        valueArrived();
    }
}

BLECharacteristicSender::BLECharacteristicSender(BLECharacteristic* characteristic, void (*valueArrived)()) {
    this->characteristic = characteristic;
    this->valueArrived = valueArrived;
    currentlySending = NULL;
    currentlySendingMutex = xSemaphoreCreateBinary();
    xSemaphoreGive(currentlySendingMutex);

    xTaskCreate(
        [](void* o) { static_cast<BLECharacteristicSender*>(o)->sendCharacteristicTask(); },
        "sendCharacteristicTask",
        2048,           // Stack size of task
        this,           // parameter of the task
        1,              // priority of the task
        &sendTask);     // Task handle to keep track of created task
}

BLECharacteristicSender::~BLECharacteristicSender() {
    vTaskDelete(sendTask);
    vSemaphoreDelete(currentlySendingMutex);

    if (currentlySending != NULL)
        free(currentlySending);

    while (!toSendQueue.empty()) {
        DataToSend* element = toSendQueue.front();
        toSendQueue.pop();
        free(element);
    }
}
