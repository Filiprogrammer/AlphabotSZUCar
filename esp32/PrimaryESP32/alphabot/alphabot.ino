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
#include "MotionTracker.h"
#include "Compass.h"
#include "SaveFile.h"
#include "Navigator.h"
#include <BLECharacteristic.h>
#include "BLECharacteristicSender.h"

TwoMotorDrive* two_motor_drive = NULL;
DistanceMeter* distance_meter = NULL;
StepperMotor* stepper_motor = NULL;
DrivingAssistent* driving_assistent = NULL;
PositioningSystem* positioning_system = NULL;
MotionTracker* motion_tracker = NULL;
Compass* compass = NULL;
SaveFile* save_file = NULL;
Navigator* navigator = NULL;

float raw_dir = 0.0;
float dir = 0.0;
float anchor1_dist = 0.0;
float anchor2_dist = 0.0;
float anchor3_dist = 0.0;
float lps_x = 0;
float lps_y = 0;
int8_t drive_input_steer = 0;
int8_t drive_input_speed = 0;
uint16_t front_dist = 0;
uint16_t left_dist = 0;
uint16_t right_dist = 0;
uint16_t back_dist = 0;

struct settings {
    uint8_t unused : 3;
    bool explore_mode : 1;
    bool navigation_mode : 1;
    bool collision_avoidance : 1;
    bool positioning : 1;
    bool invite : 1;
} settings;

struct logging {
    bool unused : 1;
    bool imu : 1;
    bool wheel_speed : 1;
    bool anchor_distances : 1;
    bool compass_direction : 1;
    bool pathfinder_path : 1;
    bool obstacle_distance : 1;
    bool positioning : 1;
} logging;

bool connected = false;

BLEHandler* ble_handler = NULL;
BLECharacteristicSender* ble_char_toggle_sender = NULL;
BLECharacteristicSender* ble_char_add_obstacle_sender = NULL;

void charUpdateMotorsDataReceived(const char* data, size_t len) {
    if (len >= 2) {
        drive_input_speed = data[0];
        drive_input_steer = data[1];
    }
}

void charToggleDataReceived(const char* data, size_t len) {
    if (len >= 10) {
        static uint64_t last_toggle_send_time = 0;
        uint64_t send_time = *((uint64_t*)&(data[2]));

        if (last_toggle_send_time != send_time) {
            last_toggle_send_time = send_time;

            struct settings toggle_settings = *((struct settings*)data);
            struct logging toggle_logging = *((struct logging*)&(data[1]));

            if (settings.explore_mode != toggle_settings.explore_mode) {
                settings.explore_mode = toggle_settings.explore_mode;
            }

            if (settings.navigation_mode != toggle_settings.navigation_mode) {
                settings.navigation_mode = toggle_settings.navigation_mode;
            }

            if (settings.collision_avoidance != toggle_settings.collision_avoidance) {
                settings.collision_avoidance = toggle_settings.collision_avoidance;

                if (settings.collision_avoidance == false)
                    two_motor_drive->updateMotors(0, 0);
            }

            if (settings.positioning != toggle_settings.positioning)
                settings.positioning = toggle_settings.positioning;

            if (settings.invite != toggle_settings.invite) {
                settings.invite = toggle_settings.invite;

                if (settings.invite)
                    ble_handler->startAdvertising();
                else
                    ble_handler->stopAdvertising();
            }

            logging = toggle_logging;
        }
    }
}

void charToggleStateChanged(BLECharacteristicCallbacks::Status s, uint32_t code) {
    ble_char_toggle_sender->stateChanged(s, code);
}

void charPathfindingTargetDataReceived(const char* data, size_t len) {
    if (len >= 12) {
        static uint64_t last_pathfinding_target_send_time = 0;
        uint64_t send_time = *((uint64_t*)&(data[4]));

        if (last_pathfinding_target_send_time != send_time) {
            last_pathfinding_target_send_time = send_time;

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

void charCalibrateDataReceived(const char* data, size_t len) {
    if (len >= 9) {
        static uint64_t last_calibrate_send_time = 0;
        uint64_t send_time = *((uint64_t*)&(data[1]));

        if (last_calibrate_send_time != send_time) {
            last_calibrate_send_time = send_time;

            switch (data[0]) {
                case 0: // Calibrate steering
                    stepper_motor->calibrate();
                    break;
                case 1: // Automatic magnetometer calibration
                    // TODO: Automatic magnetometer calibration
                    break;
                case 2: // Begin manual magnetometer calibration
                    break;
                case 3: // Finish magnetometer calibration
                    break;
                case 4: // Calibrate compass direction
                    float correct_dir = atan2(positioning_system->getAnc1Y() - lps_y, positioning_system->getAnc1X() - lps_x) * (180.0 / PI);
                    float compass_angle_offset = fmodf(correct_dir - raw_dir + 540.0, 360.0) - 180.0;
                    compass->setAngleOffset(compass_angle_offset);
                    save_file->setCompassAngleOffset(compass_angle_offset);
                    save_file->write();
                    break;
            }
        }
    }
}

static uint16_t id = 0;

void charAddObstacleDataReceived(const char* data, size_t len) {
    if (len >= 16) {
        static uint64_t last_add_obstacle_send_time = 0;
        uint64_t send_time = *((uint64_t*)&(data[0]));

        if (last_add_obstacle_send_time != send_time) {
            last_add_obstacle_send_time = send_time;

            int16_t obstacle_x = data[8] | (data[9] << 8);
            int16_t obstacle_y = data[10] | (data[11] << 8);
            uint16_t obstacle_w = data[12] | (data[13] << 8);
            uint16_t obstacle_h = data[14] | (data[15] << 8);
            navigator->addObstacle(obstacle_x, obstacle_y, obstacle_w, obstacle_h, id);

            uint8_t val[18];

            for (uint8_t i = 0; i < 16; ++i)
                val[i] = data[i];

            val[16] = id;
            val[17] = id << 8;
            id++;
            ble_char_add_obstacle_sender->sendValue(val, 18);
        }
    }
}

void charAddObstacleStateChanged(BLECharacteristicCallbacks::Status s, uint32_t code) {
    ble_char_add_obstacle_sender->stateChanged(s, code);
}

void charRemoveObstacleDataReceived(const char* data, size_t len) {
    if (len == 0) {
        // Clear obstacles
        navigator->clearObstacles();
        ble_handler->charRemoveObstacle->notify();
    } else {
        static uint64_t last_remove_obstacle_send_time = 0;
        uint64_t send_time = *((uint64_t*)&(data[0]));

        if (last_remove_obstacle_send_time != send_time) {
            last_remove_obstacle_send_time = send_time;

            if (len == 12) {
                // Remove obstacle by position
                int16_t x = data[8] | (data[9] << 8);
                int16_t y = data[10] | (data[11] << 8);
                navigator->removeObstacleByPosition(x, y);
                ble_handler->charRemoveObstacle->notify();
            } else if (len == 10) {
                // Remove obstacle by ID
                uint16_t id = data[8] | (data[9] << 8);
                navigator->removeObstacleById(id);
                ble_handler->charRemoveObstacle->notify();
            }
        }
    }
}

void charAnchorLocationsDataReceived(const char* data, size_t len) {
    if (len == 20) {
        static uint64_t last_anchor_locations_send_time = 0;
        uint64_t send_time = *((uint64_t*)&(data[0]));

        if (last_anchor_locations_send_time != send_time) {
            last_anchor_locations_send_time = send_time;

            int16_t anc1_x = data[8] | (data[9] << 8);
            int16_t anc1_y = data[10] | (data[11] << 8);
            int16_t anc2_x = data[12] | (data[13] << 8);
            int16_t anc2_y = data[14] | (data[15] << 8);
            int16_t anc3_x = data[16] | (data[17] << 8);
            int16_t anc3_y = data[18] | (data[19] << 8);
            positioning_system->setAnchorPositions(anc1_x, anc1_y, anc2_x, anc2_y, anc3_x, anc3_y);
        }
    }
}

void onCharToggleArrive() {
    #ifdef DEBUG
    Serial.println("Toggle arrived");
    #endif
}

void onCharAddObstacleArrive() {
    #ifdef DEBUG
    Serial.println("Add Obstacle arrived");
    #endif
}

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
    Wire.begin(I2C_SDA, I2C_SCL, (uint32_t)400000);
    distance_meter = new DistanceMeter(5);
    ble_handler = new BLEHandler(&charUpdateMotorsDataReceived,
                                 &charToggleDataReceived,
                                 &charToggleStateChanged,
                                 &charPathfindingTargetDataReceived,
                                 &charCalibrateDataReceived,
                                 &charAddObstacleDataReceived,
                                 &charAddObstacleStateChanged,
                                 &charRemoveObstacleDataReceived,
                                 &charAnchorLocationsDataReceived,
                                 &onDisconnect, &onConnect);
    ble_char_toggle_sender = new BLECharacteristicSender(ble_handler->charToggle, &onCharToggleArrive);
    ble_char_add_obstacle_sender = new BLECharacteristicSender(ble_handler->charAddObstacle, &onCharAddObstacleArrive);
    positioning_system = new PositioningSystem(DW1000_ANCHOR_1_SHORT_ADDRESS, DW1000_ANCHOR_2_SHORT_ADDRESS, DW1000_ANCHOR_3_SHORT_ADDRESS);
    motion_tracker = new MotionTracker();
    compass = new Compass(motion_tracker);
    save_file = new SaveFile();
    compass->setAngleOffset(save_file->getCompassAngleOffset());
    navigator = new Navigator(two_motor_drive);

    settings.explore_mode = false;
    settings.navigation_mode = false;
    settings.collision_avoidance = true;
    settings.positioning = false;
    settings.invite = true;

    logging.imu = false;
    logging.wheel_speed = false;
    logging.anchor_distances = false;
    logging.compass_direction = false;
    logging.pathfinder_path = false;
    logging.obstacle_distance = false;
    logging.positioning = false;
}

void loop() {
    if (!connected) {
        delay(100);
        return;
    }

    uint32_t time_millis = millis();

    if (logging.obstacle_distance || settings.collision_avoidance || settings.explore_mode) {
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
    }

    if (logging.compass_direction || settings.positioning) {
        raw_dir = compass->getRawDirection();
        dir = fmodf(compass->getAngleOffset() + raw_dir + 360.0, 360.0);
    }

    if (settings.positioning) {
        positioning_system->readDistances(&anchor1_dist, &anchor2_dist, &anchor3_dist);
        positioning_system->calculatePosition(anchor1_dist, anchor2_dist, anchor3_dist, &lps_x, &lps_y);
        navigator->setOwnPosition(lps_x, lps_y);
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

        if (logging.anchor_distances) {
            uint16_t vals[3];
            vals[0] = anchor1_dist;
            vals[1] = anchor2_dist;
            vals[2] = anchor3_dist;
            ble_handler->charAnchorDistances->setValue((uint8_t*)vals, 6);
            ble_handler->charAnchorDistances->notify();
        }
    }

    if (settings.positioning && settings.navigation_mode) {
        if (navigator->hasTarget()) {
            std::list<Coordinate> path;
            navigator->navigateStep(dir, path);

            if (logging.pathfinder_path) {
                uint8_t vals[20];
                memset(vals, 0, 20);
                Coordinate previous_coords = {0, 0};
                bool isFirst = true;
                uint8_t step_offset = 6;

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

                        vals[2 + step_offset / 8] |= val << (step_offset % 8);

                        if ((step_offset % 8) > 5)
                            vals[2 + (step_offset + 3) / 8] |= val >> (8 - (step_offset % 8));

                        vals[2]++;
                        step_offset += 3;
                    }

                    previous_coords.x = coord.x;
                    previous_coords.y = coord.y;
                }

                ble_handler->charPathfindingPath->setValue(vals, 2 + (step_offset + 7) / 8);
                ble_handler->charPathfindingPath->notify();
            }
        }
    } else {
        if (settings.collision_avoidance) {
            driving_assistent->updateDistances(front_dist, left_dist, right_dist, back_dist);
            driving_assistent->updateSpeedAndSteer(two_motor_drive->getSpeed(), two_motor_drive->getSteerDirection());

            int8_t val_y = drive_input_speed;

            if (val_y > 0)
                val_y = driving_assistent->safeSpeedForward(val_y);
            else
                val_y = driving_assistent->safeSpeedBackward(val_y);

            two_motor_drive->updateMotors(drive_input_steer, val_y);
        } else {
            two_motor_drive->updateMotors(drive_input_steer, drive_input_speed);
        }
    }

    if (logging.positioning || logging.compass_direction || logging.obstacle_distance) {
        uint8_t sensor_logging_val[19];
        uint8_t sensor_logging_val_len = 2;
        uint8_t sensor_logging_counter = 0;
        sensor_logging_val[0] = 0;
        sensor_logging_val[1] = 0;

        if (logging.positioning) {
            sensor_logging_val[sensor_logging_val_len] = (uint8_t)lps_x;
            sensor_logging_val[sensor_logging_val_len + 1] = (((uint16_t)lps_x >> 8) & 0x07) | ((lps_x < 0) << 3);
            sensor_logging_val[sensor_logging_val_len + 1] |= (uint8_t)lps_y << 4;
            sensor_logging_val[sensor_logging_val_len + 2] = (((uint16_t)lps_y >> 4) & 0x7F) | ((lps_y < 0) << 7);
            sensor_logging_val_len += 3;
            sensor_logging_val[sensor_logging_counter / 4] |= 2 << ((sensor_logging_counter % 4) * 2);
            sensor_logging_counter++;
        }

        if (logging.compass_direction) {
            sensor_logging_val[sensor_logging_val_len] = (uint8_t)dir;
            sensor_logging_val[sensor_logging_val_len + 1] = (uint16_t)dir >> 8;
            sensor_logging_val_len += 2;
            sensor_logging_val[sensor_logging_counter / 4] |= 3 << ((sensor_logging_counter % 4) * 2);
            sensor_logging_counter++;
        }

        if (logging.obstacle_distance) {
            sensor_logging_val[sensor_logging_val_len] = 0;
            sensor_logging_val[sensor_logging_val_len + 1] = front_dist / 2;
            sensor_logging_val_len += 2;
            sensor_logging_val[sensor_logging_counter / 4] |= 1 << ((sensor_logging_counter % 4) * 2);
            sensor_logging_counter++;

            sensor_logging_val[sensor_logging_val_len] = 335 / 2;
            sensor_logging_val[sensor_logging_val_len + 1] = left_dist / 2;
            sensor_logging_val_len += 2;
            sensor_logging_val[sensor_logging_counter / 4] |= 1 << ((sensor_logging_counter % 4) * 2);
            sensor_logging_counter++;

            sensor_logging_val[sensor_logging_val_len] = 25 / 2;
            sensor_logging_val[sensor_logging_val_len + 1] = right_dist / 2;
            sensor_logging_val_len += 2;
            sensor_logging_val[sensor_logging_counter / 4] |= 1 << ((sensor_logging_counter % 4) * 2);
            sensor_logging_counter++;

            sensor_logging_val[sensor_logging_val_len] = 180 / 2;
            sensor_logging_val[sensor_logging_val_len + 1] = back_dist / 2;
            sensor_logging_val_len += 2;
            sensor_logging_val[sensor_logging_counter / 4] |= 1 << ((sensor_logging_counter % 4) * 2);
            sensor_logging_counter++;
        }

        ble_handler->charSensor->setValue(sensor_logging_val, sensor_logging_val_len);
        ble_handler->charSensor->notify();
    }

    if (logging.imu) {
        // TODO: Implement IMU logging
    }

    if (logging.wheel_speed) {
        int8_t vals[2];
        vals[0] = two_motor_drive->getLeftSpeed() / 1.44;
        vals[1] = two_motor_drive->getRightSpeed() / 1.44;
        ble_handler->charWheelSpeed->setValue((uint8_t*)vals, 2);
        ble_handler->charWheelSpeed->notify();
    }

    uint32_t time_diff = millis() - time_millis;

    #ifdef DEBUG
    Serial.print("time_diff: ");
    Serial.println(time_diff);
    #endif

    if (time_diff < 100)
        delay(100 - time_diff);
}

void onDisconnect() {
    two_motor_drive->updateMotors(0, 0);
    drive_input_speed = 0;
    drive_input_steer = 0;

    // If the last client is disconnecting, getConnectedCount still
    // returns 1, because the disconnecting client is still considered
    // connected at that point in time.
    if (ble_handler->getConnectedCount() <= 1) {
        connected = false;
    } else {
        settings.invite = true;
        sendToggleUpdate();
    }
}

void onConnect() {
    connected = true;
    settings.invite = false;

    xTaskCreate(
        sendInitialData,
        "sendInitialData",
        1536,           // Stack size of task
        NULL,           // parameter of the task
        1,              // priority of the task
        NULL);          // Task handle to keep track of created task
}

void sendInitialData(void* args) {
    delay(2000);

    sendToggleUpdate();

    std::list<struct Obstacle> obstacles = navigator->getObstacles();
    uint16_t data[9];
    uint32_t timestamp_millis = millis();
    data[0] = timestamp_millis;
    data[1] = timestamp_millis >> 16;
    data[2] = 0;
    data[3] = 0;

    for (Obstacle o : obstacles) {
        #ifdef DEBUG
        Serial.print("Sending obstacle ");
        Serial.println(o.id);
        #endif
        data[4] = o.x;
        data[5] = o.y;
        data[6] = o.width;
        data[7] = o.height;
        data[8] = o.id;
        ble_char_add_obstacle_sender->sendValue((uint8_t*)data, 18);
    }

    vTaskDelete(NULL);
}

void sendToggleUpdate() {
    uint8_t val[10] = { 0 };
    val[0] = *((uint8_t*)&settings);
    val[1] = *((uint8_t*)&logging);
    uint32_t timestamp_millis = millis();
    val[2] = timestamp_millis;
    val[3] = timestamp_millis >> 8;
    val[4] = timestamp_millis >> 16;
    val[5] = timestamp_millis >> 24;
    ble_char_toggle_sender->sendValue(val, sizeof(val));
}
