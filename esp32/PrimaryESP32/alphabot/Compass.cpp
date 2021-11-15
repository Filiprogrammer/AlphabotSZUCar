#include "Compass.h"
#include "config.h"

float Compass::getRawDirection() {
    int16_t x, y, z;
    magnet_sensor.read(&x, &y, &z);
    x = map(x, min_x, max_x, -1000, 1000);
    y = map(y, min_y, max_y, -1000, 1000);
    float heading = atan2(y, x);
    heading += COMPASS_DECLINATION;

    if(heading < 0.0)
        heading += 2 * PI;

    else if(heading >= 2 * PI)
        heading -= 2 * PI;

    return (heading * 180.0 / PI);
}

void Compass::setMagnetSensorCalibratedValues(int16_t min_x, int16_t max_x, int16_t min_y, int16_t max_y) {
    this->min_x = min_x;
    this->max_x = max_x;
    this->min_y = min_y;
    this->max_y = max_y;
}

void Compass::setAngleOffset(float offset) {
    angle_offset = offset;
}

float Compass::getAngleOffset() const {
    return angle_offset;
}

void Compass::magnetSensorCalibrate() {
    int16_t x, y, z;
    magnet_sensor.read(&x, &y, &z);

    if (min_max_set) {
        min_x = min(x, min_x);
        max_x = max(x, max_x);
        min_y = min(y, min_y);
        max_y = max(y, max_y);
    } else {
        min_x = x;
        max_x = x;
        min_y = y;
        max_y = y;
        min_max_set = true;
    }
}

void Compass::beginMagnetSensorCalibration() {
    min_max_set = false;
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
    magnet_sensor.init();
}
