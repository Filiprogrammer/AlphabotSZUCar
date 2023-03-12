#include "Motor.h"
#include "pinconfig.h"
#include "motorconfig.h"

void Motor::stop() {
    ledcWrite(pwm_channel, 0);
}

void Motor::setSpeed(uint8_t val) {
    uint16_t balancing_percentage = 256 + MOTOR_MINIMUM_DUTY_CYCLE;
    uint16_t speed = min(MOTOR_MINIMUM_DUTY_CYCLE + ((uint16_t)val * 256) / balancing_percentage, 255);
    ledcWrite(pwm_channel, speed);
}

void Motor::forward() {
    digitalWrite(pin_backward, LOW);
    digitalWrite(pin_forward, HIGH);
}

void Motor::backward() {
    digitalWrite(pin_backward, HIGH);
    digitalWrite(pin_forward, LOW);
}

Motor::Motor(uint8_t pin_forward, uint8_t pin_backward, uint8_t pin_speed, uint8_t pwm_channel) {
    this->pin_forward = pin_forward;
    this->pin_backward = pin_backward;
    this->pin_speed = pin_speed;
    this->pwm_channel = pwm_channel;

    ledcSetup(pwm_channel, PWM_FREQ, PWM_RESOLUTION);
    ledcAttachPin(pin_speed, pwm_channel);
    pinMode(pin_forward, OUTPUT);
    pinMode(pin_backward, OUTPUT);
}
