#include "BLEHandler.h"
#include "config.h"

class BLEHandlerCallbacks : public BLECharacteristicCallbacks {
    BLECharacteristic* characteristic;
    void (*dataReceived)(const char*, size_t) = NULL;
    void (*stateChanged)(Status s, uint32_t code) = NULL;

public:
    BLEHandlerCallbacks(BLECharacteristic* characteristic, void (*dataReceived)(const char*, size_t), void (*stateChanged)(Status s, uint32_t code) = NULL) {
        this->characteristic = characteristic;
        this->dataReceived = dataReceived;
        this->stateChanged = stateChanged;
    }

    void onWrite(BLECharacteristic* pCharacteristic) {
        if (dataReceived != NULL) {
            std::string str = pCharacteristic->getValue();
            (*(dataReceived))(str.c_str(), str.length());
        }
    }

    void onStatus(BLECharacteristic* pCharacteristic, Status s, uint32_t code) {
        if (stateChanged != NULL)
            stateChanged(s, code);
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

BLEHandler::BLEHandler(void (*charUpdateMotorsDataReceived)(const char*, size_t),
                       void (*charToggleDataReceived)(const char*, size_t),
                       void (*charToggleStateChanged)(BLECharacteristicCallbacks::Status s, uint32_t code),
                       void (*charPathfindingTargetDataReceived)(const char*, size_t),
                       void (*charCalibrateDataReceived)(const char*, size_t),
                       void (*charAddObstacleDataReceived)(const char*, size_t),
                       void (*charAddObstacleStateChanged)(BLECharacteristicCallbacks::Status s, uint32_t code),
                       void (*charRemoveObstacleDataReceived)(const char*, size_t),
                       void (*charAnchorLocationsDataReceived)(const char*, size_t),
                       void (*disconnected)(), void (*connected)()) {
    BLEDevice::init(BLE_DEVICE_NAME);
    server = BLEDevice::createServer();
    advertising = server->getAdvertising();
    server->setCallbacks(new BLEHandlerConnectionCallbacks(advertising, connected, disconnected));
    service = server->createService(BLEUUID(BLE_SVC_UUID), 50);

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
    charUpdateMotors->setCallbacks(new BLEHandlerCallbacks(charUpdateMotors, charUpdateMotorsDataReceived));

    charToggle = service->createCharacteristic(BLE_CHAR_TOGGLE_UUID,
                       BLECharacteristic::PROPERTY_READ |
                       BLECharacteristic::PROPERTY_WRITE |
                       BLECharacteristic::PROPERTY_NOTIFY
                      );
    BLE2902* p2902Descriptor = new BLE2902();
    p2902Descriptor->setNotifications(true);
    charToggle->addDescriptor(p2902Descriptor);
    charToggle->setValue("");
    charToggle->setCallbacks(new BLEHandlerCallbacks(charToggle, charToggleDataReceived, charToggleStateChanged));

    charSensor = service->createCharacteristic(BLE_CHAR_SENSOR_UUID,
                       BLECharacteristic::PROPERTY_READ |
                       BLECharacteristic::PROPERTY_NOTIFY
                      );
    p2902Descriptor = new BLE2902();
    p2902Descriptor->setNotifications(true);
    charSensor->addDescriptor(p2902Descriptor);
    charSensor->setValue("");

    charPathfindingTarget = service->createCharacteristic(BLE_CHAR_PATHFINDING_TARGET_UUID,
                       BLECharacteristic::PROPERTY_READ |
                       BLECharacteristic::PROPERTY_WRITE
                      );
    charPathfindingTarget->addDescriptor(new BLE2902());
    charPathfindingTarget->setValue("");
    charPathfindingTarget->setCallbacks(new BLEHandlerCallbacks(charPathfindingTarget, charPathfindingTargetDataReceived));

    charCalibrate = service->createCharacteristic(BLE_CHAR_CALIBRATE_UUID,
                       BLECharacteristic::PROPERTY_READ |
                       BLECharacteristic::PROPERTY_WRITE |
                       BLECharacteristic::PROPERTY_NOTIFY
                      );
    p2902Descriptor = new BLE2902();
    p2902Descriptor->setNotifications(true);
    charCalibrate->addDescriptor(p2902Descriptor);
    charCalibrate->setValue("");
    charCalibrate->setCallbacks(new BLEHandlerCallbacks(charCalibrate, charCalibrateDataReceived));

    charAddObstacle = service->createCharacteristic(BLE_CHAR_ADD_OBSTACLE_UUID,
                       BLECharacteristic::PROPERTY_READ |
                       BLECharacteristic::PROPERTY_WRITE |
                       BLECharacteristic::PROPERTY_NOTIFY
                      );
    p2902Descriptor = new BLE2902();
    p2902Descriptor->setNotifications(true);
    charAddObstacle->addDescriptor(p2902Descriptor);
    charAddObstacle->setValue("");
    charAddObstacle->setCallbacks(new BLEHandlerCallbacks(charAddObstacle, charAddObstacleDataReceived, charAddObstacleStateChanged));

    charRemoveObstacle = service->createCharacteristic(BLE_CHAR_REMOVE_OBSTACLE_UUID,
                       BLECharacteristic::PROPERTY_READ |
                       BLECharacteristic::PROPERTY_WRITE |
                       BLECharacteristic::PROPERTY_NOTIFY
                      );
    p2902Descriptor = new BLE2902();
    p2902Descriptor->setNotifications(true);
    charRemoveObstacle->addDescriptor(p2902Descriptor);
    charRemoveObstacle->setValue("");
    charRemoveObstacle->setCallbacks(new BLEHandlerCallbacks(charRemoveObstacle, charRemoveObstacleDataReceived));

    charPathfindingPath = service->createCharacteristic(BLE_CHAR_PATHFINDING_PATH_UUID,
                       BLECharacteristic::PROPERTY_READ |
                       BLECharacteristic::PROPERTY_NOTIFY
                      );
    p2902Descriptor = new BLE2902();
    p2902Descriptor->setNotifications(true);
    charPathfindingPath->addDescriptor(p2902Descriptor);
    charPathfindingPath->setValue("");

    charAnchorLocations = service->createCharacteristic(BLE_CHAR_ANCHOR_LOCATIONS_UUID,
                       BLECharacteristic::PROPERTY_READ |
                       BLECharacteristic::PROPERTY_WRITE
                      );
    charAnchorLocations->addDescriptor(new BLE2902());
    charAnchorLocations->setValue("");
    charAnchorLocations->setCallbacks(new BLEHandlerCallbacks(charAnchorLocations, charAnchorLocationsDataReceived));

    charAnchorDistances = service->createCharacteristic(BLE_CHAR_ANCHOR_DISTANCES_UUID,
                       BLECharacteristic::PROPERTY_READ |
                       BLECharacteristic::PROPERTY_NOTIFY
                      );
    p2902Descriptor = new BLE2902();
    p2902Descriptor->setNotifications(true);
    charAnchorDistances->addDescriptor(p2902Descriptor);
    charAnchorDistances->setValue("");

    charImu = service->createCharacteristic(BLE_CHAR_IMU_UUID,
                       BLECharacteristic::PROPERTY_READ |
                       BLECharacteristic::PROPERTY_NOTIFY
                      );
    p2902Descriptor = new BLE2902();
    p2902Descriptor->setNotifications(true);
    charImu->addDescriptor(p2902Descriptor);
    charImu->setValue("");

    charWheelSpeed = service->createCharacteristic(BLE_CHAR_WHEEL_SPEED_UUID,
                       BLECharacteristic::PROPERTY_READ |
                       BLECharacteristic::PROPERTY_NOTIFY
                      );
    p2902Descriptor = new BLE2902();
    p2902Descriptor->setNotifications(true);
    charWheelSpeed->addDescriptor(p2902Descriptor);
    charWheelSpeed->setValue("");

    charError = service->createCharacteristic(BLE_CHAR_ERROR_UUID,
                       BLECharacteristic::PROPERTY_READ |
                       BLECharacteristic::PROPERTY_NOTIFY
                      );
    p2902Descriptor = new BLE2902();
    p2902Descriptor->setNotifications(true);
    charError->addDescriptor(p2902Descriptor);
    charError->setValue("");

    service->start();
    advertising->addServiceUUID(BLE_SVC_UUID);
    advertising->setScanResponse(true);
    advertising->start();
    #ifdef DEBUG
    Serial.println("BLE Advertising started");
    #endif
}

BLEHandler::~BLEHandler() {
    BLEDevice::deinit(true);
}
