#ifndef STEPPERMOTOR_H
#define STEPPERMOTOR_H

#include <Arduino.h>

class StepperMotor {
private:
    uint8_t id;
    uint8_t pin_in1;
    uint8_t pin_in2;
    uint8_t pin_in3;
    uint8_t pin_in4;
    int16_t desired_direction;

    void startStepperTask();

public:
    int16_t getCurrentDirection() const;
    int16_t getDesiredDirection() const;
    void turnTo(int16_t dir);
    void calibrate();

    StepperMotor(uint8_t pin_in1, uint8_t pin_in2, uint8_t pin_in3, uint8_t pin_in4, uint16_t calibration_steps);
};

#endif
