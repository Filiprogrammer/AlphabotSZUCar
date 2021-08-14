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
        (*(this->dataReceived))(this->charUUID, str.c_str(), str.length());
    }
};

void BLEHandler::onConnect(BLEServer* pServer) {
    (*(this->connected))();
    this->advertising->stop();
    #ifdef DEBUG
    Serial.println("BLE Connected");
    #endif
}

void BLEHandler::onDisconnect(BLEServer* pServer) {
    (*(this->disconnected))();
    this->advertising->start();
    #ifdef DEBUG
    Serial.println("BLE Disconnected");
    #endif
}

BLEHandler::BLEHandler(void (*dataReceived)(BLEUUID, const char*, size_t), void (*disconnected)(), void (*connected)()) {
    BLEDevice::init(BLE_DEVICE_NAME);
    this->server = BLEDevice::createServer();
    this->server->setCallbacks((BLEServerCallbacks*)this);
    this->service = this->server->createService(BLEUUID(BLE_SVC_UUID), 40);

    this->charClientPing = this->service->createCharacteristic(BLE_CHAR_PINGCLIENT_UUID,
                       BLECharacteristic::PROPERTY_READ |
                       BLECharacteristic::PROPERTY_WRITE
                      );
    this->charClientPing->addDescriptor(new BLE2902());
    this->charClientPing->setValue("");

    this->charUpdateMotors = this->service->createCharacteristic(BLE_CHAR_UPDATEMOTORS_UUID,
                       BLECharacteristic::PROPERTY_READ |
                       BLECharacteristic::PROPERTY_WRITE
                      );
    this->charUpdateMotors->addDescriptor(new BLE2902());
    this->charUpdateMotors->setValue("");
    this->charUpdateMotors->setCallbacks(new BLEHandlerCallbacks(BLEUUID(BLE_CHAR_UPDATEMOTORS_UUID), dataReceived));

    this->charCollDetect = this->service->createCharacteristic(BLE_CHAR_COLLDETECT_UUID,
                       BLECharacteristic::PROPERTY_READ |
                       BLECharacteristic::PROPERTY_WRITE
                      );
    BLE2902* p2902Descriptor = new BLE2902();
    p2902Descriptor->setNotifications(true);
    this->charCollDetect->addDescriptor(p2902Descriptor);
    this->charCollDetect->setValue("1");
    this->charCollDetect->setCallbacks(new BLEHandlerCallbacks(BLEUUID(BLE_CHAR_COLLDETECT_UUID), dataReceived));

    this->charAIDrive = this->service->createCharacteristic(BLE_CHAR_AIDRIVE_UUID,
                       BLECharacteristic::PROPERTY_READ |
                       BLECharacteristic::PROPERTY_WRITE
                      );
    p2902Descriptor = new BLE2902();
    p2902Descriptor->setNotifications(true);
    this->charAIDrive->addDescriptor(p2902Descriptor);
    this->charAIDrive->setValue("");
    this->charAIDrive->setCallbacks(new BLEHandlerCallbacks(BLEUUID(BLE_CHAR_AIDRIVE_UUID), dataReceived));

    this->charLPS = this->service->createCharacteristic(BLE_CHAR_LPS_UUID,
                       BLECharacteristic::PROPERTY_READ |
                       BLECharacteristic::PROPERTY_WRITE
                      );
    p2902Descriptor = new BLE2902();
    p2902Descriptor->setNotifications(true);
    this->charLPS->addDescriptor(p2902Descriptor);
    this->charLPS->setValue("");
    this->charLPS->setCallbacks(new BLEHandlerCallbacks(BLEUUID(BLE_CHAR_LPS_UUID), dataReceived));

    this->charPosUpdate = this->service->createCharacteristic(BLE_CHAR_POSUPDATE_UUID,
                       BLECharacteristic::PROPERTY_READ |
                       BLECharacteristic::PROPERTY_WRITE |
                       BLECharacteristic::PROPERTY_NOTIFY
                      );
    p2902Descriptor = new BLE2902();
    p2902Descriptor->setNotifications(true);
    this->charPosUpdate->addDescriptor(p2902Descriptor);
    this->charPosUpdate->setValue("");

    this->charCalibrate = this->service->createCharacteristic(BLE_CHAR_CALIBRATE_UUID,
                       BLECharacteristic::PROPERTY_READ |
                       BLECharacteristic::PROPERTY_WRITE
                      );
    p2902Descriptor = new BLE2902();
    p2902Descriptor->setNotifications(true);
    this->charCalibrate->addDescriptor(p2902Descriptor);
    this->charCalibrate->setValue("");
    this->charCalibrate->setCallbacks(new BLEHandlerCallbacks(BLEUUID(BLE_CHAR_CALIBRATE_UUID), dataReceived));

    this->charInvite = this->service->createCharacteristic(BLE_CHAR_INVITE_UUID,
                       BLECharacteristic::PROPERTY_READ |
                       BLECharacteristic::PROPERTY_WRITE |
                       BLECharacteristic::PROPERTY_NOTIFY
                      );
    p2902Descriptor = new BLE2902();
    p2902Descriptor->setNotifications(true);
    this->charInvite->addDescriptor(p2902Descriptor);
    this->charInvite->setValue("");
    this->charInvite->setCallbacks(new BLEHandlerCallbacks(BLEUUID(BLE_CHAR_INVITE_UUID), dataReceived));

    this->charPathfindingObstacles = this->service->createCharacteristic(BLE_CHAR_PATHFINDING_OBSTACLES_UUID,
                       BLECharacteristic::PROPERTY_READ |
                       BLECharacteristic::PROPERTY_WRITE |
                       BLECharacteristic::PROPERTY_NOTIFY
                      );
    p2902Descriptor = new BLE2902();
    p2902Descriptor->setNotifications(true);
    this->charPathfindingObstacles->addDescriptor(p2902Descriptor);
    this->charPathfindingObstacles->setValue("");
    this->charPathfindingObstacles->setCallbacks(new BLEHandlerCallbacks(BLEUUID(BLE_CHAR_PATHFINDING_OBSTACLES_UUID), dataReceived));

    this->charPathfindingTarget = this->service->createCharacteristic(BLE_CHAR_PATHFINDING_TARGET_UUID,
                       BLECharacteristic::PROPERTY_READ |
                       BLECharacteristic::PROPERTY_WRITE |
                       BLECharacteristic::PROPERTY_NOTIFY
                      );
    p2902Descriptor = new BLE2902();
    p2902Descriptor->setNotifications(true);
    this->charPathfindingTarget->addDescriptor(p2902Descriptor);
    this->charPathfindingTarget->setValue("");
    this->charPathfindingTarget->setCallbacks(new BLEHandlerCallbacks(BLEUUID(BLE_CHAR_PATHFINDING_TARGET_UUID), dataReceived));

    this->charPathfindingPath = this->service->createCharacteristic(BLE_CHAR_PATHFINDING_PATH_UUID,
                       BLECharacteristic::PROPERTY_READ |
                       BLECharacteristic::PROPERTY_WRITE |
                       BLECharacteristic::PROPERTY_NOTIFY
                      );
    p2902Descriptor = new BLE2902();
    p2902Descriptor->setNotifications(true);
    this->charPathfindingPath->addDescriptor(p2902Descriptor);
    this->charPathfindingPath->setValue("");

    this->disconnected = disconnected;
    this->connected = connected;

    this->service->start();
    this->advertising = this->server->getAdvertising();
    this->advertising->start();
    #ifdef DEBUG
    Serial.println("BLE Advertising started");
    #endif
}

BLEHandler::~BLEHandler() {
    BLEDevice::deinit(true);
}
