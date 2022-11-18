#include "Compass.h"
#include "config.h"

float fmap(float x, float in_min, float in_max, float out_min, float out_max) {
    float dividend = out_max - out_min;
    float divisor = in_max - in_min;
    float delta = x - in_min;

    if(divisor == 0)
        return 0;

    return (delta * dividend + (divisor / 2)) / divisor + out_min - 0.5;
}

float Compass::getRawDirection() {
    motion_tracker->fetchData();
    float x = motion_tracker->getUncalMagX();
    float y = motion_tracker->getUncalMagY();
    float z = motion_tracker->getUncalMagZ();

    x = fmap(x, min_x, max_x, -1.0, 1.0);
    y = fmap(y, min_y, max_y, -1.0, 1.0);
    z = fmap(z, min_z, max_z, -1.0, 1.0);

    float heading = atan2(y, x);

    if (heading < 0.0)
        heading += 2 * PI;
    else if (heading >= 2 * PI)
        heading -= 2 * PI;

    float mag_dir = heading * 180.0 / PI;
    float quat6_dir = 360.0 - motion_tracker->getYaw6();

    // If mag values go far beyond the min max values, do not let the mag contribute
    if (fabs(x) > 1.5 || fabs(y) > 1.5 || fabs(z) > 1.5) {
        #ifdef DEBUG
        Serial.println("Magnetic interference detected");
        #endif
    } else {
        double diff = fmod(mag_dir - quat6_dir - avg_diff_quat6_mag + 540, 360) - 180;
        avg_diff_quat6_mag = fmod(360 + avg_diff_quat6_mag + diff / 1000, 360);
    }

    return fmod(avg_diff_quat6_mag + quat6_dir, 360);
}

void Compass::setMagnetSensorCalibratedValues(float min_x, float max_x, float min_y, float max_y, float min_z, float max_z) {
    this->min_x = min_x;
    this->max_x = max_x;
    this->min_y = min_y;
    this->max_y = max_y;
    this->min_z = min_z;
    this->max_z = max_z;
}

void Compass::magnetSensorCalibrate() {
    motion_tracker->fetchData();
    float x = motion_tracker->getUncalMagX();
    float y = motion_tracker->getUncalMagY();
    float z = motion_tracker->getUncalMagZ();

    if (min_max_set) {
        min_x = min(x, min_x);
        max_x = max(x, max_x);
        min_y = min(y, min_y);
        max_y = max(y, max_y);
        min_z = min(z, min_z);
        max_z = max(z, max_z);
    } else {
        min_x = x;
        max_x = x;
        min_y = y;
        max_y = y;
        min_z = z;
        max_z = z;
        min_max_set = true;
    }
}

void Compass::beginMagnetSensorCalibration() {
    min_max_set = false;
}

void Compass::finishMagnetSensorCalibration() {
    // Initialize avg_diff_quat6_mag quickly, to prevent having to wait for the value to converge.
    motion_tracker->fetchData();
    float x = motion_tracker->getUncalMagX();
    float y = motion_tracker->getUncalMagY();

    x = fmap(x, min_x, max_x, -1.0, 1.0);
    y = fmap(y, min_y, max_y, -1.0, 1.0);

    float heading = atan2(y, x);

    if (heading < 0.0)
        heading += 2 * PI;
    else if (heading >= 2 * PI)
        heading -= 2 * PI;

    float mag_dir = heading * 180.0 / PI;
    float quat6_dir = 360.0 - motion_tracker->getYaw6();

    double diff = fmod(mag_dir - quat6_dir - avg_diff_quat6_mag + 540, 360) - 180;
    avg_diff_quat6_mag = fmod(360 + avg_diff_quat6_mag + diff, 360);
}

float Compass::getMinX() const {
    return min_x;
}

float Compass::getMaxX() const {
    return max_x;
}

float Compass::getMinY() const {
    return min_y;
}

float Compass::getMaxY() const {
    return max_y;
}

float Compass::getMinZ() const {
    return min_z;
}

float Compass::getMaxZ() const {
    return max_z;
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
