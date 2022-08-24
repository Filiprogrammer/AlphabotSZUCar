#include "Compass.h"
#include "config.h"

float Compass::getRawDirection() {
    motion_tracker->fetchData();
    return 360.0 - motion_tracker->getYaw();
}

void Compass::setAngleOffset(float offset) {
    angle_offset = offset;
}

float Compass::getAngleOffset() const {
    return angle_offset;
}

float Compass::getDirection() {
    return angle_offset + getRawDirection();
}

Compass::Compass(MotionTracker* motion_tracker) {
    this->motion_tracker = motion_tracker;
}
