#include "pinconfig.h"
#include <Arduino.h>
#include "TwoMotorDrive.h"
#include "config.h"

#define MAX(x, y) (((x) > (y)) ? (x) : (y))
#define MIN(x, y) (((x) < (y)) ? (x) : (y))

void TwoMotorDrive::stop() {
    motor_left->stop();
    motor_right->stop();
}

void TwoMotorDrive::leftForward() {
    this->motor_left->forward();
}

void TwoMotorDrive::leftBackward() {
    this->motor_left->backward();
}

void TwoMotorDrive::rightForward() {
    this->motor_right->forward();
}

void TwoMotorDrive::rightBackward() {
    this->motor_right->backward();
}

void TwoMotorDrive::steer(int16_t dir) {
    this->motor_steer->turnTo(dir);
}

void TwoMotorDrive::updateMotors(int8_t x, int8_t y) {
    int16_t speed_left = y;
    int16_t speed_right = y;

    float dir = 35.0 * tan((double)x / 250.0);

    speed_left -= ((double)y / 127.0) * MIN(dir, 0.0);
    speed_right += ((double)y / 127.0) * MAX(dir, 0.0);

    if (speed_left > 0)
        this->motor_left->forward();
    else
        this->motor_left->backward();

    if (speed_right > 0)
        this->motor_right->forward();
    else
        this->motor_right->backward();

    #ifdef DEBUG
    Serial.print("speed_left: ");
    Serial.println(speed_left);

    Serial.print("speed_right: ");
    Serial.println(speed_right);

    Serial.print("steer: ");
    Serial.println(dir);
    #endif

    this->motor_left->setSpeed(MIN(abs(speed_left * 2), 255));
    this->motor_right->setSpeed(MIN(abs(speed_right * 2), 255));
    this->steer(dir * 14.0);
    this->speed = y;
    this->steer_direction = x;
}

int8_t TwoMotorDrive::getSpeed() {
    return this->speed;
}

int8_t TwoMotorDrive::getSteerDirection() {
    return this->steer_direction;
}

TwoMotorDrive::TwoMotorDrive(Motor* motor_left, Motor* motor_right, StepperMotor* motor_steer) {
    this->motor_left = motor_left;
    this->motor_right = motor_right;
    this->motor_steer = motor_steer;
}
