#include "Compass.h"
#include "config.h"

float Compass::getRawDirection() {
    motion_tracker->fetchOrientationData();
    return motion_tracker->getYaw();
}

void Compass::setMagnetSensorCalibratedValues(int16_t min_x, int16_t max_x, int16_t min_y, int16_t max_y) {
    /*this->min_x = min_x;
    this->max_x = max_x;
    this->min_y = min_y;
    this->max_y = max_y;*/
}

void Compass::setAngleOffset(float offset) {
    angle_offset = offset;
}

float Compass::getAngleOffset() const {
    return angle_offset;
}

void Compass::magnetSensorCalibrate() {
}

void Compass::beginMagnetSensorCalibration() {
    //min_max_set = false;
}

int16_t Compass::getMinX() const {
    return min_x;
}

int16_t Compass::getMaxX() const {
    return max_x;
}

int16_t Compass::getMinY() const {
    return min_y;
}

int16_t Compass::getMaxY() const {
    return max_y;
}

float Compass::getDirection() {
    return angle_offset + getRawDirection();
}

Compass::Compass() {
    motion_tracker = new MotionTracker();
}
