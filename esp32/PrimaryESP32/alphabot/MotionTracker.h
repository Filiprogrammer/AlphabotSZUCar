#ifndef MOTION_TRACKER_H
#define MOTION_TRACKER_H

#include <Arduino-ICM20948.h>

class MotionTracker {
private:
    ArduinoICM20948 icm20948;
    float accX;
    float accY;
    float accZ;
    float gyrX;
    float gyrY;
    float gyrZ;
    float magX;
    float magY;
    float magZ;
    float roll;
    float pitch;
    float yaw;

public:
    void fetchData();
    float getRoll() const;
    float getPitch() const;
    float getYaw() const;
    float getAccX() const;
    float getAccY() const;
    float getAccZ() const;
    float getGyrX() const;
    float getGyrY() const;
    float getGyrZ() const;
    float getMagX() const;
    float getMagY() const;
    float getMagZ() const;

    MotionTracker();
};

#endif
