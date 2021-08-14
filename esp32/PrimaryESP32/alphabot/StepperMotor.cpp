#include "StepperMotor.h"
#include "pinconfig.h"
#include "config.h"

void StepperMotor::turnTo(int16_t dir) {
    this->desired_direction = constrain(dir, -STEPPER_CALIBRATION_STEPS / 2, STEPPER_CALIBRATION_STEPS / 2);
}

void StepperMotor::calibrate() {
    this->calibration_step = 0;
    this->state = calibrating;
    #ifdef DEBUG
    Serial.println("Calibrate");
    #endif
}

void stepperTask(void* parameter) {
    StepperMotor* _this = (StepperMotor*)parameter;

    for (;;) {
        delay(5);

        if (_this->state == calibrating) {
            if (_this->calibration_step < STEPPER_CALIBRATION_STEPS) {
                _this->current_direction--;
                digitalWrite(_this->pin_in4, (_this->current_direction % 4) == 0);
                digitalWrite(_this->pin_in3, ((_this->current_direction + 1) % 4) == 0);
                digitalWrite(_this->pin_in2, ((_this->current_direction + 2) % 4) == 0);
                digitalWrite(_this->pin_in1, ((_this->current_direction + 3) % 4) == 0);
            } else if (_this->calibration_step < (STEPPER_CALIBRATION_STEPS + STEPPER_CALIBRATION_STEPS / 2)) {
                _this->current_direction++;
                digitalWrite(_this->pin_in4, (_this->current_direction % 4) == 0);
                digitalWrite(_this->pin_in3, ((_this->current_direction + 1) % 4) == 0);
                digitalWrite(_this->pin_in2, ((_this->current_direction + 2) % 4) == 0);
                digitalWrite(_this->pin_in1, ((_this->current_direction + 3) % 4) == 0);
            } else {
                #ifdef DEBUG
                Serial.println("Calibrated");
                #endif
                _this->current_direction = 0;
                _this->state = inactive;
                digitalWrite(_this->pin_in4, 0);
                digitalWrite(_this->pin_in3, 0);
                digitalWrite(_this->pin_in2, 0);
                digitalWrite(_this->pin_in1, 0);
            }

            _this->calibration_step++;
            continue;
        }

        if (_this->desired_direction == _this->current_direction) {
            _this->state = inactive;
            digitalWrite(_this->pin_in4, 0);
            digitalWrite(_this->pin_in3, 0);
            digitalWrite(_this->pin_in2, 0);
            digitalWrite(_this->pin_in1, 0);
            continue;
        }

        _this->state = turning;

        if (_this->desired_direction > _this->current_direction)
            _this->current_direction++;
        else if (_this->desired_direction < _this->current_direction)
            _this->current_direction--;

        digitalWrite(_this->pin_in4, (_this->current_direction % 4) == 0);
        digitalWrite(_this->pin_in3, ((_this->current_direction + 1) % 4) == 0);
        digitalWrite(_this->pin_in2, ((_this->current_direction + 2) % 4) == 0);
        digitalWrite(_this->pin_in1, ((_this->current_direction + 3) % 4) == 0);
    }

    vTaskDelete(NULL);
}

void startStepperTask(StepperMotor* _this) {
    xTaskCreate(
        &stepperTask,
        "stepperTask",
        8192,           // Stack size of task
        _this,          // parameter of the task
        1,              // priority of the task
        NULL);          // Task handle to keep track of created task
}

StepperMotor::StepperMotor(uint8_t pin_in1, uint8_t pin_in2, uint8_t pin_in3, uint8_t pin_in4) {
    this->pin_in1 = pin_in1;
    this->pin_in2 = pin_in2;
    this->pin_in3 = pin_in3;
    this->pin_in4 = pin_in4;
    this->current_direction = 0;
    this->desired_direction = 0;
    this->state = inactive;

    pinMode(pin_in1, OUTPUT);
    pinMode(pin_in2, OUTPUT);
    pinMode(pin_in3, OUTPUT);
    pinMode(pin_in4, OUTPUT);

    startStepperTask(this);
}
