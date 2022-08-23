#ifndef COMPASS_H
#define COMPASS_H

#include <Arduino.h>
#include "MotionTracker.h"

class Compass {
private:
    MotionTracker* motion_tracker;
    float angle_offset = 0.0;
    float dir = 0.0;

public:
    float getRawDirection();
    void setAngleOffset(float offset);
    float getAngleOffset() const;
    float getDirection();

    Compass(MotionTracker* motion_tracker);
};

#endif
