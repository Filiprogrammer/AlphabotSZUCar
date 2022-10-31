#ifndef MOTION_TRACKER_H
#define MOTION_TRACKER_H

#include <Arduino-ICM20948.h>

class MotionTracker {
private:
    ArduinoICM20948 icm20948;
    float accX;
    float accY;
    float accZ;
    float linAccX;
    float linAccY;
    float linAccZ;
    float gyrX;
    float gyrY;
    float gyrZ;
    float uncalMagX;
    float uncalMagY;
    float uncalMagZ;
    float magX;
    float magY;
    float magZ;
    float roll6;
    float pitch6;
    float yaw6;
    float roll9;
    float pitch9;
    float yaw9;

public:
    void fetchData();
    float getRoll6() const;
    float getPitch6() const;
    float getYaw6() const;
    float getRoll9() const;
    float getPitch9() const;
    float getYaw9() const;
    float getAccX() const;
    float getAccY() const;
    float getAccZ() const;
    float getLinAccX() const;
    float getLinAccY() const;
    float getLinAccZ() const;
    float getGyrX() const;
    float getGyrY() const;
    float getGyrZ() const;
    float getUncalMagX() const;
    float getUncalMagY() const;
    float getUncalMagZ() const;
    float getMagX() const;
    float getMagY() const;
    float getMagZ() const;

    MotionTracker(bool enable_gyro, bool enable_acc, bool enable_uncal_mag, bool enable_mag, bool enable_linAcc, bool enable_quat6, bool enable_quat9);
};

#endif
