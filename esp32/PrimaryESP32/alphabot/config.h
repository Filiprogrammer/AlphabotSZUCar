#ifndef CONFIG_H
#define CONFIG_H

//#define DEBUG
#define BLE_DEVICE_NAME "Filip-ESP32"

#define BLE_SVC_UUID                                "4fafc201-1fb5-459e-8fcc-c5c9c331914b"
#define BLE_CHAR_PINGCLIENT_UUID                    "117ad3a5-b257-4465-abd4-7dc12a4cf77d"
#define BLE_CHAR_UPDATEMOTORS_UUID                  "a04295f7-eaa8-4536-b3a4-4e7ae4d72dc2"
#define BLE_CHAR_COLLDETECT_UUID                    "c9157183-c05f-4331-a74b-153c3723f69c"
#define BLE_CHAR_AIDRIVE_UUID                       "1a4032c8-e071-4f08-a4e6-688771a91a33"
#define BLE_CHAR_LPS_UUID                           "63bbadb7-bfb9-4e36-a028-1747d70bfbbc"
#define BLE_CHAR_POSUPDATE_UUID                     "11618450-ceec-438d-b376-ce666e612da1"
#define BLE_CHAR_CALIBRATE_UUID                     "d39e8d54-8019-46c8-a977-db13871bac59"
#define BLE_CHAR_INVITE_UUID                        "39d3a28f-b403-4c78-8f8f-f9549ff2d838"
#define BLE_CHAR_PATHFINDING_OBSTACLES_UUID         "60db37c7-afeb-4d40-bb17-a19a07d6fc95"
#define BLE_CHAR_PATHFINDING_TARGET_UUID            "f56f0a15-52ae-4ad5-bfe1-557eed983618"
#define BLE_CHAR_PATHFINDING_PATH_UUID              "8dad4c9a-1a1c-4a42-a522-ded592f4ed99"

#define COMPASS_DECLINATION 0.0759

#define DW1000_ANCHOR_1_SHORT_ADDRESS 0x0CBE
#define DW1000_ANCHOR_2_SHORT_ADDRESS 0x4CA6
#define DW1000_ANCHOR_3_SHORT_ADDRESS 0x1782

#endif
