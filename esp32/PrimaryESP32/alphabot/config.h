#ifndef CONFIG_H
#define CONFIG_H

//#define DEBUG
#define BLE_DEVICE_NAME "Filip-ESP32"

#define BLE_SVC_UUID                                "4fafc201-1fb5-459e-8fcc-c5c9c331914b"
#define BLE_CHAR_PINGCLIENT_UUID                    "117ad3a5-b257-4465-abd4-7dc12a4cf77d"
#define BLE_CHAR_UPDATEMOTORS_UUID                  "a04295f7-eaa8-4536-b3a4-4e7ae4d72dc2"
#define BLE_CHAR_TOGGLE_UUID                        "fce001d4-864a-48f4-9c95-de928f1da07b"
#define BLE_CHAR_SENSOR_UUID                        "4c999381-35e2-4af4-8443-ee8b9fe56ba0"
#define BLE_CHAR_PATHFINDING_TARGET_UUID            "f56f0a15-52ae-4ad5-bfe1-557eed983618"
#define BLE_CHAR_CALIBRATE_UUID                     "d39e8d54-8019-46c8-a977-db13871bac59"
#define BLE_CHAR_ADD_OBSTACLE_UUID                  "60db37c7-afeb-4d40-bb17-a19a07d6fc95"
#define BLE_CHAR_REMOVE_OBSTACLE_UUID               "6d43e0df-682b-45ef-abb7-814ecf475771"
#define BLE_CHAR_PATHFINDING_PATH_UUID              "8dad4c9a-1a1c-4a42-a522-ded592f4ed99"
#define BLE_CHAR_ANCHOR_LOCATIONS_UUID              "8a55dd30-463b-40f6-8f21-d68efcc386b2"
#define BLE_CHAR_ANCHOR_DISTANCES_UUID              "254492a2-9324-469b-b1e2-4d4590972c35"
#define BLE_CHAR_IMU_UUID                           "93758afa-ce6f-4670-9562-ce92bda84d49"
#define BLE_CHAR_WHEEL_SPEED_UUID                   "8efafa16-15de-461f-bde1-493261201e2b"
#define BLE_CHAR_ERROR_UUID                         "dc458f08-ea3e-4fe1-adb3-25c840be081a"

#define DW1000_ANCHOR_1_SHORT_ADDRESS 0x0CBE
#define DW1000_ANCHOR_2_SHORT_ADDRESS 0x4CA6
#define DW1000_ANCHOR_3_SHORT_ADDRESS 0x1782

#define LOOP_FREQUENCY_HZ 10

#endif
