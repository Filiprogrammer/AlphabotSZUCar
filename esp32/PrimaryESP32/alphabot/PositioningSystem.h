#ifndef POSITIONINGSYSTEM_H
#define POSITIONINGSYSTEM_H

#include <Arduino.h>

class PositioningSystem {
private:
    uint16_t anc1_short_address;
    uint16_t anc2_short_address;
    uint16_t anc3_short_address;
    char serial2_buffer[256] = {0};
    uint8_t serial2_buffer_position = 0;
    float anc1_x;
    float anc1_y;
    float anc2_x;
    float anc2_y;
    float anc3_x;
    float anc3_y;

public:
    void readDistances(float* anc1_dist, float* anc2_dist, float* anc3_dist);
    void setAnchorPositions(float anc1_x, float anc1_y, float anc2_x, float anc2_y, float anc3_x, float anc3_y);
    void calculatePosition(float anc1_dist, float anc2_dist, float anc3_dist, float* x, float* y);
    float getAnc1X() const;
    float getAnc1Y() const;
    float getAnc2X() const;
    float getAnc2Y() const;
    float getAnc3X() const;
    float getAnc3Y() const;

    PositioningSystem(uint16_t anc1_short_address, uint16_t anc2_short_address, uint16_t anc3_short_address);
};

#endif
