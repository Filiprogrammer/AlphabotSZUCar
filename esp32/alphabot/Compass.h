#ifndef COMPASS_H
#define COMPASS_H

#include <Arduino.h>
#include "MotionTracker.h"

class Compass {
private:
    MotionTracker* motion_tracker;
    float min_x = -1000.0;
    float max_x = 1000.0;
    float min_y = -1000.0;
    float max_y = 1000.0;
    float min_z = -1000.0;
    float max_z = 1000.0;
    bool min_max_set = false;
    float angle_offset = 0.0;
    float dir = 0.0;
    float avg_diff_quat6_mag = 0.0;

public:
    float getRawDirection();
    void setMagnetSensorCalibratedValues(float min_x, float max_x, float min_y, float max_y, float min_z, float max_z);
    void magnetSensorCalibrate();
    void beginMagnetSensorCalibration();
    void finishMagnetSensorCalibration();
    float getMinX() const;
    float getMaxX() const;
    float getMinY() const;
    float getMaxY() const;
    float getMinZ() const;
    float getMaxZ() const;
    void setAngleOffset(float offset);
    float getAngleOffset() const;
    float getDirection();

    Compass(MotionTracker* motion_tracker);
};

#endif
