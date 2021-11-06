#include "BLEHandler.h"
#include "config.h"

class BLEHandlerCallbacks : public BLECharacteristicCallbacks {
    BLEUUID charUUID;
    void (*dataReceived)(BLEUUID, const char*, size_t) = NULL;

public:
    BLEHandlerCallbacks(BLEUUID charUUID, void (*dataReceived)(BLEUUID, const char*, size_t)) : charUUID(charUUID) {
        this->dataReceived = dataReceived;
    }

    void onWrite(BLECharacteristic* pCharacteristic) {
        std::string str = pCharacteristic->getValue();
        (*(dataReceived))(charUUID, str.c_str(), str.length());
    }
};

class BLEHandlerConnectionCallbacks : public BLEServerCallbacks {
    void (*disconnected)();
    void (*connected)();
    BLEAdvertising* advertising;

public:
    BLEHandlerConnectionCallbacks(BLEAdvertising* advertising, void (*connected)(), void (*disconnected)()) {
        this->advertising = advertising;
        this->connected = connected;
        this->disconnected = disconnected;
    }

    void onConnect(BLEServer* pServer) {
        (*(connected))();
        advertising->stop();
        #ifdef DEBUG
        Serial.println("BLE Connected");
        #endif
    }

    void onDisconnect(BLEServer* pServer) {
        (*(disconnected))();
        advertising->start();
        #ifdef DEBUG
        Serial.println("BLE Disconnected");
        #endif
    }
};

void BLEHandler::startAdvertising() {
    advertising->start();
}

void BLEHandler::stopAdvertising() {
    advertising->stop();
}

uint32_t BLEHandler::getConnectedCount() {
    return server->getConnectedCount();
}

BLEHandler::BLEHandler(void (*dataReceived)(BLEUUID, const char*, size_t), void (*disconnected)(), void (*connected)()) {
    BLEDevice::init(BLE_DEVICE_NAME);
    server = BLEDevice::createServer();
    advertising = server->getAdvertising();
    server->setCallbacks(new BLEHandlerConnectionCallbacks(advertising, connected, disconnected));
    service = server->createService(BLEUUID(BLE_SVC_UUID), 40);

    charClientPing = service->createCharacteristic(BLE_CHAR_PINGCLIENT_UUID,
                       BLECharacteristic::PROPERTY_READ |
                       BLECharacteristic::PROPERTY_WRITE
                      );
    charClientPing->addDescriptor(new BLE2902());
    charClientPing->setValue("");

    charUpdateMotors = service->createCharacteristic(BLE_CHAR_UPDATEMOTORS_UUID,
                       BLECharacteristic::PROPERTY_READ |
                       BLECharacteristic::PROPERTY_WRITE
                      );
    charUpdateMotors->addDescriptor(new BLE2902());
    charUpdateMotors->setValue("");
    charUpdateMotors->setCallbacks(new BLEHandlerCallbacks(BLEUUID(BLE_CHAR_UPDATEMOTORS_UUID), dataReceived));

    charCollDetect = service->createCharacteristic(BLE_CHAR_COLLDETECT_UUID,
                       BLECharacteristic::PROPERTY_READ |
                       BLECharacteristic::PROPERTY_WRITE
                      );
    BLE2902* p2902Descriptor = new BLE2902();
    p2902Descriptor->setNotifications(true);
    charCollDetect->addDescriptor(p2902Descriptor);
    charCollDetect->setValue("1");
    charCollDetect->setCallbacks(new BLEHandlerCallbacks(BLEUUID(BLE_CHAR_COLLDETECT_UUID), dataReceived));

    charAIDrive = service->createCharacteristic(BLE_CHAR_AIDRIVE_UUID,
                       BLECharacteristic::PROPERTY_READ |
                       BLECharacteristic::PROPERTY_WRITE
                      );
    p2902Descriptor = new BLE2902();
    p2902Descriptor->setNotifications(true);
    charAIDrive->addDescriptor(p2902Descriptor);
    charAIDrive->setValue("");
    charAIDrive->setCallbacks(new BLEHandlerCallbacks(BLEUUID(BLE_CHAR_AIDRIVE_UUID), dataReceived));

    charLPS = service->createCharacteristic(BLE_CHAR_LPS_UUID,
                       BLECharacteristic::PROPERTY_READ |
                       BLECharacteristic::PROPERTY_WRITE
                      );
    p2902Descriptor = new BLE2902();
    p2902Descriptor->setNotifications(true);
    charLPS->addDescriptor(p2902Descriptor);
    charLPS->setValue("");
    charLPS->setCallbacks(new BLEHandlerCallbacks(BLEUUID(BLE_CHAR_LPS_UUID), dataReceived));

    charPosUpdate = service->createCharacteristic(BLE_CHAR_POSUPDATE_UUID,
                       BLECharacteristic::PROPERTY_READ |
                       BLECharacteristic::PROPERTY_WRITE |
                       BLECharacteristic::PROPERTY_NOTIFY
                      );
    p2902Descriptor = new BLE2902();
    p2902Descriptor->setNotifications(true);
    charPosUpdate->addDescriptor(p2902Descriptor);
    charPosUpdate->setValue("");

    charCalibrate = service->createCharacteristic(BLE_CHAR_CALIBRATE_UUID,
                       BLECharacteristic::PROPERTY_READ |
                       BLECharacteristic::PROPERTY_WRITE
                      );
    p2902Descriptor = new BLE2902();
    p2902Descriptor->setNotifications(true);
    charCalibrate->addDescriptor(p2902Descriptor);
    charCalibrate->setValue("");
    charCalibrate->setCallbacks(new BLEHandlerCallbacks(BLEUUID(BLE_CHAR_CALIBRATE_UUID), dataReceived));

    charInvite = service->createCharacteristic(BLE_CHAR_INVITE_UUID,
                       BLECharacteristic::PROPERTY_READ |
                       BLECharacteristic::PROPERTY_WRITE |
                       BLECharacteristic::PROPERTY_NOTIFY
                      );
    p2902Descriptor = new BLE2902();
    p2902Descriptor->setNotifications(true);
    charInvite->addDescriptor(p2902Descriptor);
    charInvite->setValue("");
    charInvite->setCallbacks(new BLEHandlerCallbacks(BLEUUID(BLE_CHAR_INVITE_UUID), dataReceived));

    charPathfindingObstacles = service->createCharacteristic(BLE_CHAR_PATHFINDING_OBSTACLES_UUID,
                       BLECharacteristic::PROPERTY_READ |
                       BLECharacteristic::PROPERTY_WRITE |
                       BLECharacteristic::PROPERTY_NOTIFY
                      );
    p2902Descriptor = new BLE2902();
    p2902Descriptor->setNotifications(true);
    charPathfindingObstacles->addDescriptor(p2902Descriptor);
    charPathfindingObstacles->setValue("");
    charPathfindingObstacles->setCallbacks(new BLEHandlerCallbacks(BLEUUID(BLE_CHAR_PATHFINDING_OBSTACLES_UUID), dataReceived));

    charPathfindingTarget = service->createCharacteristic(BLE_CHAR_PATHFINDING_TARGET_UUID,
                       BLECharacteristic::PROPERTY_READ |
                       BLECharacteristic::PROPERTY_WRITE |
                       BLECharacteristic::PROPERTY_NOTIFY
                      );
    p2902Descriptor = new BLE2902();
    p2902Descriptor->setNotifications(true);
    charPathfindingTarget->addDescriptor(p2902Descriptor);
    charPathfindingTarget->setValue("");
    charPathfindingTarget->setCallbacks(new BLEHandlerCallbacks(BLEUUID(BLE_CHAR_PATHFINDING_TARGET_UUID), dataReceived));

    charPathfindingPath = service->createCharacteristic(BLE_CHAR_PATHFINDING_PATH_UUID,
                       BLECharacteristic::PROPERTY_READ |
                       BLECharacteristic::PROPERTY_WRITE |
                       BLECharacteristic::PROPERTY_NOTIFY
                      );
    p2902Descriptor = new BLE2902();
    p2902Descriptor->setNotifications(true);
    charPathfindingPath->addDescriptor(p2902Descriptor);
    charPathfindingPath->setValue("");

    service->start();
    advertising->start();
    #ifdef DEBUG
    Serial.println("BLE Advertising started");
    #endif
}

BLEHandler::~BLEHandler() {
    BLEDevice::deinit(true);
}
