#ifndef DRIVINGASSISTENT_H
#define DRIVINGASSISTENT_H

#include <Arduino.h>

#define MIN_DIST_TO_OBSTACLE 20
#define FULL_SPEED_ROLL 40

class DrivingAssistent {
private:
    uint16_t dist_front;
    uint16_t dist_left;
    uint16_t dist_right;
    uint16_t dist_back;
    int8_t speed;
    int8_t steer;
    int8_t shieb_back;

public:
    void updateDistances(uint16_t front, uint16_t left, uint16_t right, uint16_t back);
    void updateDistanceFront(uint16_t front);
    void updateDistanceLeft(uint16_t left);
    void updateDistanceRight(uint16_t right);
    void updateDistanceBack(uint16_t back);
    void updateSpeedAndSteer(int8_t spd, int8_t steer);
    void computeNextStep(int8_t* spd, int8_t* steer);
    int8_t safeSpeedForward(int8_t speed) const;
    int8_t safeSpeedBackward(int8_t speed) const;

    DrivingAssistent();
};

#endif
