#ifndef MOTOR_H
#define MOTOR_H

#include <Arduino.h>

class Motor {
private:
    uint8_t pin_forward;
    uint8_t pin_backward;
    uint8_t pin_speed;
    uint8_t pwm_channel;

public:
    void stop();
    void setSpeed(uint8_t val);
    void forward();
    void backward();

    Motor(uint8_t pin_forward, uint8_t pin_backward, uint8_t pin_speed, uint8_t pwm_channel);
};

#endif
