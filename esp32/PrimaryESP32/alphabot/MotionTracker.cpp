#include "MotionTracker.h"

void MotionTracker::fetchData() {
    icm20948.task();

    if (icm20948.accelDataIsReady())
        icm20948.readAccelData(&accX, &accY, &accZ);

    if (icm20948.linearAccelDataIsReady())
        icm20948.readLinearAccelData(&linAccX, &linAccY, &linAccZ);

    if (icm20948.gyroDataIsReady())
        icm20948.readGyroData(&gyrX, &gyrY, &gyrZ);

    if (icm20948.uncalMagDataIsReady())
        icm20948.readUncalMagData(&uncalMagX, &uncalMagY, &uncalMagZ);

    if (icm20948.magDataIsReady())
        icm20948.readMagData(&magX, &magY, &magZ);

    if (icm20948.euler6DataIsReady())
        icm20948.readEuler6Data(&roll6, &pitch6, &yaw6);

    if (icm20948.euler9DataIsReady())
        icm20948.readEuler9Data(&roll9, &pitch9, &yaw9);
}

float MotionTracker::getAccX() const {
    return accX;
}

float MotionTracker::getAccY() const {
    return accY;
}

float MotionTracker::getAccZ() const {
    return accZ;
}

float MotionTracker::getLinAccX() const {
    return linAccX;
}

float MotionTracker::getLinAccY() const {
    return linAccY;
}

float MotionTracker::getLinAccZ() const {
    return linAccZ;
}

float MotionTracker::getGyrX() const {
    return gyrX;
}

float MotionTracker::getGyrY() const {
    return gyrY;
}

float MotionTracker::getGyrZ() const {
    return gyrZ;
}

float MotionTracker::getUncalMagX() const {
    return uncalMagX;
}

float MotionTracker::getUncalMagY() const {
    return uncalMagY;
}

float MotionTracker::getUncalMagZ() const {
    return uncalMagZ;
}

float MotionTracker::getMagX() const {
    return magX;
}

float MotionTracker::getMagY() const {
    return magY;
}

float MotionTracker::getMagZ() const {
    return magZ;
}

float MotionTracker::getRoll6() const {
    return roll6;
}

float MotionTracker::getPitch6() const {
    return pitch6;
}

float MotionTracker::getYaw6() const {
    return yaw6;
}

float MotionTracker::getRoll9() const {
    return roll9;
}

float MotionTracker::getPitch9() const {
    return pitch9;
}

float MotionTracker::getYaw9() const {
    return yaw9;
}

MotionTracker::MotionTracker(bool enable_gyro, bool enable_acc, bool enable_uncal_mag, bool enable_mag, bool enable_linAcc, bool enable_quat6, bool enable_quat9) {
    accX = 0;
    accY = 0;
    accZ = 0;
    linAccX = 0;
    linAccY = 0;
    linAccZ = 0;
    gyrX = 0;
    gyrY = 0;
    gyrZ = 0;
    magX = 0;
    magY = 0;
    magZ = 0;
    roll6 = 0;
    pitch6 = 0;
    yaw6 = 0;
    roll9 = 0;
    pitch9 = 0;
    yaw9 = 0;

    ArduinoICM20948Settings icmSettings =
    {
        .i2c_speed = 400000,                            // i2c clock speed
        .is_SPI = false,                                // Enable SPI, if disable use i2c
        .cs_pin = 10,                                   // SPI chip select pin
        .spi_speed = 7000000,                           // SPI clock speed in Hz, max speed is 7MHz
        .mode = 1,                                      // 0 = low power mode, 1 = high performance mode
        .enable_gyroscope = enable_gyro,                // Enables gyroscope output
        .enable_accelerometer = enable_acc,             // Enables accelerometer output
        .enable_uncal_magnetometer = enable_uncal_mag,  // Enables uncalibrated magnetometer output
        .enable_magnetometer = enable_mag,              // Enables magnetometer output
        .enable_gravity = false,                        // Enables gravity vector output
        .enable_linearAcceleration = enable_linAcc,     // Enables linear acceleration output
        .enable_quaternion6 = enable_quat6,             // Enables quaternion 6DOF output
        .enable_quaternion9 = enable_quat9,             // Enables quaternion 9DOF output
        .enable_har = false,                            // Enables activity recognition
        .enable_steps = false,                          // Enables step counter
        .gyroscope_frequency = 50,                      // Max frequency = 225, min frequency = 1
        .accelerometer_frequency = 50,                  // Max frequency = 225, min frequency = 1
        .uncal_magnetometer_frequency = 50,             // Max frequency = 70, min frequency = 1
        .magnetometer_frequency = 50,                   // Max frequency = 70, min frequency = 1
        .gravity_frequency = 50,                        // Max frequency = 225, min frequency = 1
        .linearAcceleration_frequency = 50,             // Max frequency = 225, min frequency = 1
        .quaternion6_frequency = 50,                    // Max frequency = 225, min frequency = 50
        .quaternion9_frequency = 50,                    // Max frequency = 225, min frequency = 50
        .har_frequency = 50,                            // Max frequency = 225, min frequency = 50
        .steps_frequency = 50                           // Max frequency = 225, min frequency = 50
    };

    icm20948.init(icmSettings);
}
