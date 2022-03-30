#include <Arduino.h>
#include <BLEDevice.h>
#include <BLEServer.h>
#include <BLE2902.h>

BLERemoteCharacteristic* char_sensor;
BLERemoteCharacteristic* char_pathfinding_target;
BLERemoteCharacteristic* char_add_obstacle;
BLERemoteCharacteristic* char_remove_obstacle;
BLERemoteCharacteristic* char_pathfinding_path;
BLERemoteCharacteristic* char_anchor_locations;

static void notifySensorCallback(
  BLERemoteCharacteristic* ble_remote_characteristic,
  uint8_t* data,
  size_t length,
  bool is_notify) {
    Serial.print("SENSOR: ");
    char buff[3];

    for (uint32_t i = 0; i < length; ++i) {
        sprintf(buff, "%02X", data[i]);
        Serial.print(buff);
    }

    Serial.println();
}

static void notifyPathfindingTargetCallback(
  BLERemoteCharacteristic* ble_remote_characteristic,
  uint8_t* data,
  size_t length,
  bool is_notify) {
    Serial.print("PATHFINDING_TARGET: ");
    char buff[3];

    for (uint32_t i = 0; i < length; ++i) {
        sprintf(buff, "%02X", data[i]);
        Serial.print(buff);
    }

    Serial.println();
}

static void notifyAddObstacleCallback(
  BLERemoteCharacteristic* ble_remote_characteristic,
  uint8_t* data,
  size_t length,
  bool is_notify) {
    if (length != 18)
        return;

    Serial.print("ADD_OBSTACLE: ");
    char buff[3];

    for (uint32_t i = 0; i < length; ++i) {
        sprintf(buff, "%02X", data[i]);
        Serial.print(buff);
    }

    Serial.println();
}

static void notifyRemoveObstacleCallback(
  BLERemoteCharacteristic* ble_remote_characteristic,
  uint8_t* data,
  size_t length,
  bool is_notify) {
    Serial.print("REMOVE_OBSTACLE: ");
    char buff[3];

    for (uint32_t i = 0; i < length; ++i) {
        sprintf(buff, "%02X", data[i]);
        Serial.print(buff);
    }

    Serial.println();
}

static void notifyPathfindingPathCallback(
  BLERemoteCharacteristic* ble_remote_characteristic,
  uint8_t* data,
  size_t length,
  bool is_notify) {
    Serial.print("PATHFINDING_PATH: ");
    char buff[3];

    for (uint32_t i = 0; i < length; ++i) {
        sprintf(buff, "%02X", data[i]);
        Serial.print(buff);
    }

    Serial.println();
}

static void notifyAnchorLocationsCallback(
  BLERemoteCharacteristic* ble_remote_characteristic,
  uint8_t* data,
  size_t length,
  bool is_notify) {
    Serial.print("ANCHOR_LOCATIONS: ");
    char buff[3];

    for (uint32_t i = 0; i < length; ++i) {
        sprintf(buff, "%02X", data[i]);
        Serial.print(buff);
    }

    Serial.println();
}

class MyClientCallback : public BLEClientCallbacks {
    void onConnect(BLEClient* client) {}

    void onDisconnect(BLEClient* client) {
        ESP.restart();
    }
};

void setup() {
    Serial.begin(115200);
    BLEDevice::init("");
    BLEClient* client = BLEDevice::createClient();
    client->setClientCallbacks(new MyClientCallback());
    Serial.println(" - Created client");

    while (!client->connect(BLEAddress("30:AE:A4:74:93:9A")))
        Serial.println(" - Failed to connect to Alphabot");

    Serial.println(" - Connected to Alphabot");

    BLERemoteService* remote_service = client->getService("4fafc201-1fb5-459e-8fcc-c5c9c331914b");

    if (remote_service == nullptr) {
        Serial.println(" - Could not find service");
        ESP.restart();
    }

    Serial.println(" - Found service");

    char_sensor = remote_service->getCharacteristic("4c999381-35e2-4af4-8443-ee8b9fe56ba0");

    if (char_sensor == nullptr) {
        Serial.println(" - Did not find SENSOR characteristic");
        ESP.restart();
    }

    Serial.println(" - Found SENSOR characteristic");

    char_sensor->registerForNotify(notifySensorCallback, true);
    Serial.println(" - Registered SENSOR characteristic for notify");

    char_pathfinding_target = remote_service->getCharacteristic("f56f0a15-52ae-4ad5-bfe1-557eed983618");

    if (char_pathfinding_target == nullptr) {
        Serial.println(" - Did not find PATHFINDING_TARGET characteristic");
        ESP.restart();
    }

    Serial.println(" - Found PATHFINDING_TARGET characteristic");

    char_pathfinding_target->registerForNotify(notifyPathfindingTargetCallback, true);
    Serial.println(" - Registered PATHFINDING_TARGET characteristic for notify");

    char_add_obstacle = remote_service->getCharacteristic("60db37c7-afeb-4d40-bb17-a19a07d6fc95");

    if (char_add_obstacle == nullptr) {
        Serial.println(" - Did not find ADD_OBSTACLE characteristic");
        ESP.restart();
    }

    Serial.println(" - Found ADD_OBSTACLE characteristic");

    char_add_obstacle->registerForNotify(notifyAddObstacleCallback, true);
    Serial.println(" - Registered ADD_OBSTACLE characteristic for notify");

    char_remove_obstacle = remote_service->getCharacteristic("6d43e0df-682b-45ef-abb7-814ecf475771");

    if (char_remove_obstacle == nullptr) {
        Serial.println(" - Did not find REMOVE_OBSTACLE characteristic");
        ESP.restart();
    }

    Serial.println(" - Found REMOVE_OBSTACLE characteristic");

    char_remove_obstacle->registerForNotify(notifyRemoveObstacleCallback, true);
    Serial.println(" - Registered REMOVE_OBSTACLE characteristic for notify");

    char_pathfinding_path = remote_service->getCharacteristic("8dad4c9a-1a1c-4a42-a522-ded592f4ed99");

    if (char_pathfinding_path == nullptr) {
        Serial.println(" - Did not find PATHFINDING_PATH characteristic");
        ESP.restart();
    }

    Serial.println(" - Found PATHFINDING_PATH characteristic");

    char_pathfinding_path->registerForNotify(notifyPathfindingPathCallback, true);
    Serial.println(" - Registered PATHFINDING_PATH characteristic for notify");

    char_anchor_locations = remote_service->getCharacteristic("8a55dd30-463b-40f6-8f21-d68efcc386b2");

    if (char_anchor_locations == nullptr) {
        Serial.println(" - Did not find ANCHOR_LOCATIONS characteristic");
        ESP.restart();
    }

    Serial.println(" - Found ANCHOR_LOCATIONS characteristic");

    char_anchor_locations->registerForNotify(notifyAnchorLocationsCallback, true);
    Serial.println(" - Registered ANCHOR_LOCATIONS characteristic for notify");

    printAnchorLocations();
}

void loop() {
    char buffer[256];
    getLine(buffer);

    if (strncmp(buffer, "anchors", 7) == 0)
        printAnchorLocations();
    else if (strncmp(buffer, "target: ", 8) == 0) {
        char* target_x_str = strtok(buffer + 8, ";");
        int32_t target_x = 0;
        int32_t target_y = 0;

        if (target_x_str != NULL) {
            target_x = atoi(target_x_str);
            char* target_y_str = strtok(NULL, ";");

            if (target_y_str != NULL)
                target_y = atoi(target_y_str);
        }

        uint8_t data[12];
        data[0] = target_x;
        data[1] = target_x >> 8;
        data[2] = target_y;
        data[3] = target_y >> 8;
        uint32_t current_time = millis();
        data[4] = current_time;
        data[5] = current_time >> 8;
        data[6] = current_time >> 16;
        data[7] = current_time >> 24;
        data[8] = 0;
        data[9] = 0;
        data[10] = 0;
        data[11] = 0;
        char_pathfinding_target->writeValue(data, 12);
    }
}

void printAnchorLocations() {
    std::string value = char_anchor_locations->readValue();
    size_t len = value.length();
    const char* cvalue = value.c_str();
    char hex_buff[3];
    Serial.print("ANCHOR_LOCATIONS: ");

    for (uint32_t i = 0; i < len; ++i) {
        sprintf(hex_buff, "%02X", cvalue[i]);
        Serial.print(hex_buff);
    }

    Serial.println();
}

void getLine(char* buffer) {
    uint8_t idx = 0;
    char c;

    do {
        while (Serial.available() == 0);
        c = Serial.read();
        Serial.print("just read: ");
        Serial.println(c);
        buffer[idx++] = c;
    } while (c != '\n' && c != '\r');

    buffer[idx] = 0;
}
