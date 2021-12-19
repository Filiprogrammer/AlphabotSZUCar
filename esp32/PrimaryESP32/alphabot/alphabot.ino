#include "TwoMotorDrive.h"
#include "StepperMotor.h"
#include "DistanceMeter.h"
#include "DrivingAssistent.h"
#include "BLEHandler.h"
#include "config.h"
#include <esp32-hal-cpu.h>
#include <soc/soc.h>
#include <soc/rtc_cntl_reg.h>
#include "PositioningSystem.h"
#include <math.h>
#include "Compass.h"
#include "SaveFile.h"
#include "Navigator.h"
//#include "MotionTracker.h"

TwoMotorDrive* two_motor_drive = NULL;
DistanceMeter* distance_meter = NULL;
StepperMotor* stepper_motor = NULL;
DrivingAssistent* driving_assistent = NULL;
PositioningSystem* positioning_system = NULL;
Compass* compass = NULL;
//MotionTracker* motion_tracker = NULL;
SaveFile* save_file = NULL;
Navigator* navigator = NULL;

float raw_dir = 0.0;
float dir = 0.0;
float anchor1_dist = 0.0;
float anchor2_dist = 0.0;
float anchor3_dist = 0.0;
float lps_x = 0;
float lps_y = 0;
uint8_t posupdate_i = 0;
int8_t drive_input_steer = 0;
int8_t drive_input_speed = 0;

bool lps = false;
bool ai_drive = false;
bool collision_avoidance = true;
bool magnet_sensor_calibration = false;
bool connected = false;

BLEHandler* ble_handler = NULL;

void setup() {
    REG_CLR_BIT(RTC_CNTL_BROWN_OUT_REG, RTC_CNTL_BROWN_OUT_ENA);
    setCpuFrequencyMhz(80);
    #ifdef DEBUG
    Serial.begin(115200);
    #endif
    Motor* motor_left = new Motor(MOTOR_LEFT_FORWARD, MOTOR_LEFT_BACKWARD, MOTOR_LEFT_SPEED, 0);
    Motor* motor_right = new Motor(MOTOR_RIGHT_FORWARD, MOTOR_RIGHT_BACKWARD, MOTOR_RIGHT_SPEED, 1);
    stepper_motor = new StepperMotor(STEPPER_IN1, STEPPER_IN2, STEPPER_IN3, STEPPER_IN4);
    stepper_motor->calibrate();
    two_motor_drive = new TwoMotorDrive(motor_left, motor_right, stepper_motor);
    driving_assistent = new DrivingAssistent();
    Wire.begin(I2C_SDA, I2C_SCL, 100000);
    distance_meter = new DistanceMeter(5);
    ble_handler = new BLEHandler(&dataReceived, &onDisconnect, &onConnect);
    positioning_system = new PositioningSystem(DW1000_ANCHOR_1_SHORT_ADDRESS, DW1000_ANCHOR_2_SHORT_ADDRESS, DW1000_ANCHOR_3_SHORT_ADDRESS);
    compass = new Compass();
    //motion_tracker = new MotionTracker();
    save_file = new SaveFile();
    compass->setMagnetSensorCalibratedValues(
        save_file->getMagnetSensorCalibratedMinX(),
        save_file->getMagnetSensorCalibratedMaxX(),
        save_file->getMagnetSensorCalibratedMinY(),
        save_file->getMagnetSensorCalibratedMaxY());
    compass->setAngleOffset(save_file->getCompassAngleOffset());
    navigator = new Navigator(two_motor_drive);
}

void loop() {
    /*if (motion_tracker->isInitialized()) {
        motion_tracker->fetchOrientationData();*/
        /*Serial.printf("quat_w: % 8.3f, quat_x: % 8.3f, quat_y: % 8.3f, quat_z: % 8.3f, accuracy: % 8.3f, roll: % 8.3f, pitch: % 8.3f, yaw: % 8.3f\n",
                      motion_tracker->getQuatW(),
                      motion_tracker->getQuatX(),
                      motion_tracker->getQuatY(),
                      motion_tracker->getQuatZ(),
                      motion_tracker->getAccuracy(),
                      motion_tracker->getRoll(),
                      motion_tracker->getPitch(),
                      motion_tracker->getYaw());*/
        /*Serial.print("quat_w: ");
        Serial.print(motion_tracker->getQuatW(), 3);
        Serial.print(", quat_x: ");
        Serial.print(motion_tracker->getQuatX(), 3);
        Serial.print(", quat_y: ");
        Serial.print(motion_tracker->getQuatY(), 3);
        Serial.print(", quat_z: ");
        Serial.print(motion_tracker->getQuatZ(), 3);
        Serial.print(", accuracy: ");
        Serial.print(motion_tracker->getAccuracy());
        Serial.print(", roll: ");
        Serial.print(motion_tracker->getRoll(), 3);
        Serial.print(", pitch: ");
        Serial.print(motion_tracker->getPitch(), 3);
        Serial.print(", yaw: ");
        Serial.println(motion_tracker->getYaw(), 3);*/

        //motion_tracker->fetchRawData();
        //Serial.printf("Scaled. Acc (mg) [ % 8.3f, % 8.3f, % 8.3f ], ", motion_tracker->getAccX(), motion_tracker->getAccY(), motion_tracker->getAccZ());
        //Serial.printf("Gyr (DPS) [ % 8.3f, % 8.3f, % 8.3f ]\n", motion_tracker->getGyrX(), motion_tracker->getGyrY(), motion_tracker->getGyrZ());

        //Serial.printf("Scaled. Acc (mg) [ % 8.2f, % 8.2f, % 8.2f ], ", motion_tracker->accX(), motion_tracker->accY(), motion_tracker->accZ());
        //Serial.printf("Gyr (DPS) [ % 8.2f, % 8.2f, % 8.2f ], ", motion_tracker->gyrX(), motion_tracker->gyrY(), motion_tracker->gyrZ());
    //}

    if (!connected) {
        delay(100);
        return;
    }

    uint16_t front_dist, left_dist, right_dist, back_dist;
    distance_meter->readValues(&front_dist, &left_dist, &right_dist, &back_dist);
    #ifdef DEBUG
    Serial.print("Front: ");
    Serial.print(front_dist);
    Serial.print("\tLeft: ");
    Serial.print(left_dist);
    Serial.print("\tRight: ");
    Serial.print(right_dist);
    Serial.print("\tBack: ");
    Serial.println(back_dist);
    #endif

    if (magnet_sensor_calibration)
        compass->magnetSensorCalibrate();

    if (lps) {
        positioning_system->readDistances(&anchor1_dist, &anchor2_dist, &anchor3_dist);
        positioning_system->calculatePosition(anchor1_dist, anchor2_dist, anchor3_dist, &lps_x, &lps_y);
        navigator->setOwnPosition(lps_x, lps_y);
        raw_dir = compass->getRawDirection();
        dir = compass->getAngleOffset() + raw_dir;
        #ifdef DEBUG
        Serial.print("anchor1_dist: ");
        Serial.print(anchor1_dist);
        Serial.print("\tanchor2_dist: ");
        Serial.print(anchor2_dist);
        Serial.print("\tanchor3_dist: ");
        Serial.print(anchor3_dist);
        Serial.print("\tlps_x: ");
        Serial.print(lps_x);
        Serial.print("\tlps_y: ");
        Serial.println(lps_y);
        #endif

        if (++posupdate_i == 5) {
            posupdate_i = 0;
            uint16_t val[10];
            val[0] = front_dist;
            val[1] = left_dist;
            val[2] = right_dist;
            val[3] = back_dist;
            val[4] = (uint16_t)anchor1_dist;
            val[5] = (uint16_t)anchor2_dist;
            val[6] = (uint16_t)anchor3_dist;
            val[7] = (int16_t)dir;
            val[8] = (int16_t)lps_x;
            val[9] = (int16_t)lps_y;
            ble_handler->charPosUpdate->setValue((uint8_t*)val, 20);
            ble_handler->charPosUpdate->notify();

            if (!collision_avoidance && navigator->hasTarget()) {
                std::list<Coordinate> path;
                navigator->navigateStep(dir, path);

                uint8_t vals[20];
                memset(vals, 0, 20);
                Coordinate previous_coords = {0, 0};
                bool isFirst = true;

                for (Coordinate coord : path) {
                    if (isFirst) {
                        vals[0] = coord.x / 10;
                        vals[1] = coord.y / 10;
                        isFirst = false;
                    } else {
                        uint8_t val = ((coord.x > previous_coords.x) ? 2 : ((coord.x < previous_coords.x) ? 0 : 1)) * 3 +
                                      ((coord.y > previous_coords.y) ? 2 : ((coord.y < previous_coords.y) ? 0 : 1));

                        if (val == 8)
                            val = 4;

                        vals[3 + (vals[2] * 3) / 8] |= val << ((vals[2] * 3) % 8);

                        if (((vals[2] * 3) % 8) > 5)
                            vals[3 + (vals[2] * 3 + 3) / 8] |= val >> (8 - ((vals[2] * 3) % 8));

                        vals[2]++;
                    }

                    previous_coords.x = coord.x;
                    previous_coords.y = coord.y;
                }

                ble_handler->charPathfindingPath->setValue(vals, 3 + (vals[2] * 3 + 7) / 8);
                ble_handler->charPathfindingPath->notify();
            }
        }
    }

    driving_assistent->updateDistances(front_dist, left_dist, right_dist, back_dist);
    driving_assistent->updateSpeedAndSteer(two_motor_drive->getSpeed(), two_motor_drive->getSteerDirection());

    if (ai_drive) {
        int8_t speed, steer_direction;
        driving_assistent->computeNextStep(&speed, &steer_direction);
        #ifdef DEBUG
        Serial.print("Speed: ");
        Serial.print(speed);
        Serial.print("\tSteerDirection: ");
        Serial.println(steer_direction);
        #endif
        two_motor_drive->updateMotors(steer_direction, speed);
    } else if (collision_avoidance) {
        int8_t val_y = drive_input_speed;

        if (val_y > 0)
            val_y = driving_assistent->safeSpeedForward(val_y);
        else
            val_y = driving_assistent->safeSpeedBackward(val_y);

        two_motor_drive->updateMotors(drive_input_steer, val_y);
    } else if (!lps) {
        two_motor_drive->updateMotors(drive_input_steer, drive_input_speed);
    }

    delay(100);
}

void onDisconnect() {
    two_motor_drive->updateMotors(0, 0);
    drive_input_speed = 0;
    drive_input_steer = 0;

    if (ble_handler->getConnectedCount() == 0) {
        connected = false;
    } else {
        uint8_t val = 1;
        ble_handler->charInvite->setValue(&val, 1);
        ble_handler->charInvite->notify();
    }
}

void onConnect() {
    connected = true;
    uint8_t val = 0;
    ble_handler->charInvite->setValue(&val, 1);
    ble_handler->charInvite->notify();
}

void dataReceived(BLEUUID charUUID, const char* data, size_t len) {
    #ifdef DEBUG
    Serial.print("BLE dataReceived ");
    Serial.print(charUUID.toString().c_str());
    Serial.print(": ");

    for (uint8_t i = 0; i < len; ++i) {
        char b[3];
        sprintf(b, "%02X", data[i]);
        Serial.print(b);
    }

    Serial.println();
    #endif

    if (charUUID.equals(BLEUUID(BLE_CHAR_UPDATEMOTORS_UUID))) {
        if (!ai_drive && len >= 2) {
            drive_input_speed = data[0];
            drive_input_steer = data[1];
        }
    } else if (charUUID.equals(BLEUUID(BLE_CHAR_COLLDETECT_UUID))) {
        if (len >= 1) {
            if (data[0]) {
                collision_avoidance = true;
            } else {
                collision_avoidance = false;
                two_motor_drive->updateMotors(0, 0);
            }
        }
    } else if (charUUID.equals(BLEUUID(BLE_CHAR_AIDRIVE_UUID))) {
        if (len >= 1) {
            if (data[0]) {
                ai_drive = true;
            } else {
                ai_drive = false;
                two_motor_drive->updateMotors(0, 0);
            }
        }
    } else if (charUUID.equals(BLEUUID(BLE_CHAR_LPS_UUID))) {
        if (len >= 1) {
            if (data[0]) {
                if (len == 13) {
                    int16_t anc1_x = data[1] | (data[2] << 8);
                    int16_t anc1_y = data[3] | (data[4] << 8);
                    int16_t anc2_x = data[5] | (data[6] << 8);
                    int16_t anc2_y = data[7] | (data[8] << 8);
                    int16_t anc3_x = data[9] | (data[10] << 8);
                    int16_t anc3_y = data[11] | (data[12] << 8);
                    positioning_system->setAnchorPositions(anc1_x, anc1_y, anc2_x, anc2_y, anc3_x, anc3_y);
                    lps = true;
                }
            } else {
                lps = false;
            }
        }
    } else if (charUUID.equals(BLEUUID(BLE_CHAR_CALIBRATE_UUID))) {
        if (len >= 9) {
            static uint64_t last_calibrate_send_time = 0;
            uint64_t send_time = *((uint64_t*)&(data[1]));

            if (last_calibrate_send_time != send_time) {
                last_calibrate_send_time = send_time;

                switch (data[0]) {
                    case 0: // Calibrate steering
                        stepper_motor->calibrate();
                        break;
                    case 1: // Begin magnetometer calibration
                        compass->beginMagnetSensorCalibration();
                        magnet_sensor_calibration = true;
                        break;
                    case 2: // Finish magnetometer calibration
                        magnet_sensor_calibration = false;
                        save_file->setMagnetSensorCalibratedMinX(compass->getMinX());
                        save_file->setMagnetSensorCalibratedMaxX(compass->getMaxX());
                        save_file->setMagnetSensorCalibratedMinY(compass->getMinY());
                        save_file->setMagnetSensorCalibratedMaxY(compass->getMaxY());
                        save_file->write();
                        break;
                    case 3: // Calibrate compass direction
                        float correct_dir = atan2(positioning_system->getAnc1Y() - lps_y, positioning_system->getAnc1X() - lps_x) * (180.0 / PI);
                        float compass_angle_offset = fmodf(correct_dir - raw_dir + 540.0, 360.0) - 180.0;
                        compass->setAngleOffset(compass_angle_offset);
                        save_file->setCompassAngleOffset(compass_angle_offset);
                        save_file->write();
                        break;
                }
            }
        }
    } else if (charUUID.equals(BLEUUID(BLE_CHAR_INVITE_UUID))) {
        if (len >= 1) {
            if (data[0])
                ble_handler->startAdvertising();
            else
                ble_handler->stopAdvertising();
        }
    } else if (charUUID.equals(BLEUUID(BLE_CHAR_PATHFINDING_OBSTACLES_UUID))) {
        if (len >= 9) {
            static uint64_t last_pathfinding_cmds_send_time = 0;
            uint64_t send_time = *((uint64_t*)&(data[1]));

            if (last_pathfinding_cmds_send_time != send_time) {
                last_pathfinding_cmds_send_time = send_time;

                if (data[0] == 0) {
                    // Clear obstacles
                    navigator->clearObstacles();
                } else if (data[0] == 1 && len >= 17) {
                    // Add obstalce
                    int16_t obstacle_x = data[9] | (data[10] << 8);
                    int16_t obstacle_y = data[11] | (data[12] << 8);
                    int16_t obstacle_w = data[13] | (data[14] << 8);
                    int16_t obstacle_h = data[15] | (data[16] << 8);
                    navigator->addObstacle(obstacle_x, obstacle_y, obstacle_w, obstacle_h);
                } else if (data[0] == 2) {
                    // TODO: Remove obstacle
                }
            }
        }
    } else if (charUUID.equals(BLEUUID(BLE_CHAR_PATHFINDING_TARGET_UUID))) {
        if (len >= 12) {
            static uint64_t last_pathfinding_cmds_send_time = 0;
            uint64_t send_time = *((uint64_t*)&(data[4]));

            if (last_pathfinding_cmds_send_time != send_time) {
                last_pathfinding_cmds_send_time = send_time;

                // Set target position
                int16_t target_x = data[0] | (data[1] << 8);
                int16_t target_y = data[2] | (data[3] << 8);
                navigator->setTarget(target_x, target_y);
                #ifdef DEBUG
                Serial.print("Set target position to: (");
                Serial.print(target_x);
                Serial.print(", \t");
                Serial.print(target_y);
                Serial.println(")");
                #endif
            }
        }
    }
}
