#ifndef TWOMOTORDRIVE_H
#define TWOMOTORDRIVE_H

#include "pinconfig.h"
#include <Arduino.h>
#include "Motor.h"
#include "StepperMotor.h"

class TwoMotorDrive {
protected:
    Motor* motor_left;
    Motor* motor_right;
    StepperMotor* motor_steer;
    int8_t speed;
    int8_t steer_direction;

public:
    void stop();
    void leftForward();
    void leftBackward();
    void rightForward();
    void rightBackward();
    void steer(int16_t dir);
    void updateMotors(int8_t x, int8_t y);
    int8_t getSpeed() const;
    int8_t getSteerDirection() const;

    TwoMotorDrive(Motor* motor_left, Motor* motor_right, StepperMotor* motor_steer);
};

#endif
