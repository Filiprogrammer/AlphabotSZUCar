#include "DrivingAssistent.h"

void DrivingAssistent::updateDistances(uint16_t front, uint16_t left, uint16_t right, uint16_t back) {
    this->dist_front = front;
    this->dist_left = left;
    this->dist_right = right;
    this->dist_back = back;
}

void DrivingAssistent::updateDistanceFront(uint16_t front) {
    this->dist_front = front;
}

void DrivingAssistent::updateDistanceLeft(uint16_t left) {
    this->dist_left = left;
}

void DrivingAssistent::updateDistanceRight(uint16_t right) {
    this->dist_right = right;
}

void DrivingAssistent::updateDistanceBack(uint16_t back) {
    this->dist_back = back;
}

void DrivingAssistent::updateSpeedAndSteer(int8_t spd, int8_t steer) {
    this->speed = spd;
    this->steer = steer;
}

/**
 * @brief 
 * 
 * @param spd Pointer to the speed (positive is forward)
 * @param steer Pointer to the steering value (positive is right)
 */
void DrivingAssistent::computeNextStep(int8_t* spd, int8_t* steer) {
    // Positive if there is more space on the right
    // Negative if there is more space on the left
    int16_t dist_to_middle = ((int16_t)(this->dist_right) - (int16_t)(this->dist_left)) / 2;
    this->steer = constrain(dist_to_middle, -100, 100);

    int8_t speed = 0;

    if (this->shieb_back > 0) {
        speed = random(-70, -65);
        speed = this->safeSpeedBackward(speed);

        // if current speed is faster than safe speed, calculate the safe speed for the current speed
        if (this->speed < speed)
            speed = max(this->safeSpeedBackward(this->speed), speed);

        #ifdef DEBUG
        Serial.print("shieb_back: ");
        Serial.println(this->shieb_back);
        #endif
        this->shieb_back--;
        this->steer = -(this->steer);
    } else {
        speed = random(70, 75);
        speed = this->safeSpeedForward(speed);

        // if current speed is faster than safe speed, calculate the safe speed for the current speed
        if (this->speed > speed)
            speed = min(this->safeSpeedForward(this->speed), speed);

        if (speed < 65) {
            speed = this->safeSpeedBackward(-75);
            this->shieb_back = 20;
        }
    }

    *spd = speed;
    this->speed = speed;
    *steer = this->steer;
}

int8_t DrivingAssistent::safeSpeedForward(int8_t speed) {
    float stoppingDistance = (speed * FULL_SPEED_ROLL) / 128.0;

    if (min(this->dist_front, min(this->dist_left, this->dist_right)) <= (MIN_DIST_TO_OBSTACLE + stoppingDistance)) {
        float frontFactor = ((float)(this->dist_front)) / ((float)(MIN_DIST_TO_OBSTACLE + FULL_SPEED_ROLL));
        float leftFactor = ((float)(this->dist_left) + min((int8_t)(-(this->steer)), (int8_t)0)) / ((float)(MIN_DIST_TO_OBSTACLE + FULL_SPEED_ROLL));
        float rightFactor = ((float)(this->dist_right) - max((int8_t)(-(this->steer)), (int8_t)0)) / ((float)(MIN_DIST_TO_OBSTACLE + FULL_SPEED_ROLL));

        frontFactor = constrain(frontFactor, 0.0, 1.0);
        leftFactor = constrain(leftFactor, 0.0, 1.0);
        rightFactor = constrain(rightFactor, 0.0, 1.0);

        speed *= (frontFactor + leftFactor + rightFactor) / 3.0;
    }

    return speed;
}

int8_t DrivingAssistent::safeSpeedBackward(int8_t speed) {
    float stoppingDistance = (-speed * FULL_SPEED_ROLL) / 128.0;

    if (this->dist_back <= (MIN_DIST_TO_OBSTACLE + stoppingDistance)) {
        float factor = ((float)(this->dist_back)) / ((float)(MIN_DIST_TO_OBSTACLE + FULL_SPEED_ROLL));
        factor = constrain(factor, 0.0, 1.0);
        speed *= factor;
    }

    return speed;
}

DrivingAssistent::DrivingAssistent() {
    this->dist_front = 0;
    this->dist_left = 0;
    this->dist_right = 0;
    this->dist_back = 0;
    this->speed = 0;
    this->steer = 0;
    this->shieb_back = 0;
}
