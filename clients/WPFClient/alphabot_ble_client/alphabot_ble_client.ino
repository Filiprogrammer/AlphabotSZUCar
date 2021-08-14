#include <Arduino.h>
#include <BLEDevice.h>
#include <BLEServer.h>
#include <BLE2902.h>

BLERemoteCharacteristic* char_posupdate;
BLERemoteCharacteristic* char_pathfinding_obstacles;
BLERemoteCharacteristic* char_pathfinding_target;
BLERemoteCharacteristic* char_pathfinding_path;
BLERemoteCharacteristic* char_lps;

static void notifyPosupdateCallback(
  BLERemoteCharacteristic* ble_remote_characteristic,
  uint8_t* data,
  size_t length,
  bool is_notify) {
    uint16_t* val = (uint16_t*)data;
    uint16_t front_dist = val[0];
    uint16_t left_dist = val[1];
    uint16_t right_dist = val[2];
    uint16_t back_dist = val[3];
    uint16_t anchor1_dist = val[4];
    uint16_t anchor2_dist = val[5];
    uint16_t anchor3_dist = val[6];
    int16_t dir = val[7];
    int16_t lps_x = val[8];
    int16_t lps_y = val[9];
    Serial.print("POSUPDATE: ");
    Serial.print(front_dist);
    Serial.print(";");
    Serial.print(left_dist);
    Serial.print(";");
    Serial.print(right_dist);
    Serial.print(";");
    Serial.print(back_dist);
    Serial.print(";");
    Serial.print(anchor1_dist);
    Serial.print(";");
    Serial.print(anchor2_dist);
    Serial.print(";");
    Serial.print(anchor3_dist);
    Serial.print(";");
    Serial.print(dir);
    Serial.print(";");
    Serial.print(lps_x);
    Serial.print(";");
    Serial.println(lps_y);
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

static void notifyPathfindingObstaclesCallback(
  BLERemoteCharacteristic* ble_remote_characteristic,
  uint8_t* data,
  size_t length,
  bool is_notify) {
    Serial.print("PATHFINDING_OBSTACLES: ");
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

static void notifyLpsCallback(
  BLERemoteCharacteristic* ble_remote_characteristic,
  uint8_t* data,
  size_t length,
  bool is_notify) {
    char hex_buff[3];
    Serial.print("LPS: ");

    for (uint32_t i = 0; i < length; ++i) {
        sprintf(hex_buff, "%02X", data[i]);
        Serial.print(hex_buff);
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

    char_posupdate = remote_service->getCharacteristic("11618450-ceec-438d-b376-ce666e612da1");

    if (char_posupdate == nullptr) {
        Serial.println(" - Did not find POSUPDATE characteristic");
        ESP.restart();
    }

    Serial.println(" - Found POSUPDATE characteristic");

    char_posupdate->registerForNotify(notifyPosupdateCallback, true);
    Serial.println(" - Registered POSUPDATE characteristic for notify");

    char_pathfinding_obstacles = remote_service->getCharacteristic("60db37c7-afeb-4d40-bb17-a19a07d6fc95");

    if (char_pathfinding_obstacles == nullptr) {
        Serial.println(" - Did not find PATHFINDING_OBSTACLES characteristic");
        ESP.restart();
    }

    Serial.println(" - Found PATHFINDING_OBSTACLES characteristic");

    char_pathfinding_obstacles->registerForNotify(notifyPathfindingObstaclesCallback, true);
    Serial.println(" - Registered PATHFINDING_OBSTACLES characteristic for notify");

    char_pathfinding_target = remote_service->getCharacteristic("f56f0a15-52ae-4ad5-bfe1-557eed983618");

    if (char_pathfinding_target == nullptr) {
        Serial.println(" - Did not find PATHFINDING_TARGET characteristic");
        ESP.restart();
    }

    Serial.println(" - Found PATHFINDING_TARGET characteristic");

    char_pathfinding_target->registerForNotify(notifyPathfindingTargetCallback, true);
    Serial.println(" - Registered PATHFINDING_TARGET characteristic for notify");

    char_pathfinding_path = remote_service->getCharacteristic("8dad4c9a-1a1c-4a42-a522-ded592f4ed99");

    if (char_pathfinding_path == nullptr) {
        Serial.println(" - Did not find PATHFINDING_PATH characteristic");
        ESP.restart();
    }

    Serial.println(" - Found PATHFINDING_PATH characteristic");

    char_pathfinding_path->registerForNotify(notifyPathfindingPathCallback, true);
    Serial.println(" - Registered PATHFINDING_PATH characteristic for notify");

    char_lps = remote_service->getCharacteristic("63bbadb7-bfb9-4e36-a028-1747d70bfbbc");

    if (char_lps == nullptr) {
        Serial.println(" - Did not find LPS characteristic");
        ESP.restart();
    }

    Serial.println(" - Found LPS characteristic");

    char_lps->registerForNotify(notifyLpsCallback, true);
    Serial.println(" - Registered LPS characteristic for notify");

    printLPS();
}

void loop() {
    char buffer[256];
    getLine(buffer);

    if (strncmp(buffer, "anchors", 7) == 0)
        printLPS();
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

void printLPS() {
    std::string value = char_lps->readValue();
    size_t len = value.length();
    const char* cvalue = value.c_str();
    char hex_buff[3];
    Serial.print("LPS: ");

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
