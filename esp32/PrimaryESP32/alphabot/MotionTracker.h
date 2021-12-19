#ifndef MOTION_TRACKER_H
#define MOTION_TRACKER_H

#include <ICM_20948.h>

class MotionTracker {
private:
    ICM_20948_I2C icm20948;
    bool initialized = false;
    double quat_w;
    double quat_x;
    double quat_y;
    double quat_z;
    int16_t accuracy;

public:
    bool isInitialized() const;
    void fetchOrientationData();
    void fetchRawData();
    double getQuatW() const;
    double getQuatX() const;
    double getQuatY() const;
    double getQuatZ() const;
    int16_t getAccuracy() const;
    double getRoll() const;
    double getPitch() const;
    double getYaw() const;
    float getAccX();
    float getAccY();
    float getAccZ();
    float getGyrX();
    float getGyrY();
    float getGyrZ();
    float getMagX();
    float getMagY();
    float getMagZ();
    float getTemp();

    MotionTracker();
};

#endif
