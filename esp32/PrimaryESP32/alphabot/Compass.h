#ifndef COMPASS_H
#define COMPASS_H

#include <Arduino.h>
#include "MotionTracker.h"

class Compass {
private:
    MotionTracker* motion_tracker;
    int16_t min_x = -1000.0;
    int16_t max_x = 1000.0;
    int16_t min_y = -1000.0;
    int16_t max_y = 1000.0;
    bool min_max_set = false;
    float angle_offset = 0.0;
    float dir = 0.0;

public:
    float getRawDirection();
    void setMagnetSensorCalibratedValues(int16_t min_x, int16_t max_x, int16_t min_y, int16_t max_y);
    void setAngleOffset(float offset);
    float getAngleOffset() const;
    void magnetSensorCalibrate();
    void beginMagnetSensorCalibration();
    int16_t getMinX() const;
    int16_t getMaxX() const;
    int16_t getMinY() const;
    int16_t getMaxY() const;
    float getDirection();

    Compass();
};

#endif
