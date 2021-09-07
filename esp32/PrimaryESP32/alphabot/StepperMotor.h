#ifndef STEPPERMOTOR_H
#define STEPPERMOTOR_H

#include <Arduino.h>

#define STEPPER_CALIBRATION_STEPS 625

typedef enum {
    inactive,
    turning,
    calibrating
} StepperState;

class StepperMotor {
private:
    uint8_t pin_in1;
    uint8_t pin_in2;
    uint8_t pin_in3;
    uint8_t pin_in4;
    int16_t current_direction;
    int16_t desired_direction;
    StepperState state;
    uint16_t calibration_step;

public:
    void turnTo(int16_t dir);
    void calibrate();
    void stepperTask();
    void startStepperTask();

    StepperMotor(uint8_t pin_in1, uint8_t pin_in2, uint8_t pin_in3, uint8_t pin_in4);
};

#endif
