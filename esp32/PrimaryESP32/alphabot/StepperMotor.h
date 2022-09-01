#ifndef STEPPERMOTOR_H
#define STEPPERMOTOR_H

#include <Arduino.h>

#define STEPPER_CALIBRATION_STEPS 625

class StepperMotor {
private:
    uint8_t pin_in1;
    uint8_t pin_in2;
    uint8_t pin_in3;
    uint8_t pin_in4;
    int16_t desired_direction;

    void startStepperTask();

public:
    void turnTo(int16_t dir);
    void calibrate();

    StepperMotor(uint8_t pin_in1, uint8_t pin_in2, uint8_t pin_in3, uint8_t pin_in4);
};

#endif
