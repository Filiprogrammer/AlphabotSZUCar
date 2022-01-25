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

void StepperMotor::stepperTask() {
    for (;;) {
        delay(5);

        if (state == calibrating) {
            if (calibration_step < STEPPER_CALIBRATION_STEPS) {
                current_direction--;
                digitalWrite(pin_in4, (current_direction % 4) == 0);
                digitalWrite(pin_in3, ((current_direction + 1) % 4) == 0);
                digitalWrite(pin_in2, ((current_direction + 2) % 4) == 0);
                digitalWrite(pin_in1, ((current_direction + 3) % 4) == 0);
            } else if (calibration_step < (STEPPER_CALIBRATION_STEPS + STEPPER_CALIBRATION_STEPS / 2)) {
                current_direction++;
                digitalWrite(pin_in4, (current_direction % 4) == 0);
                digitalWrite(pin_in3, ((current_direction + 1) % 4) == 0);
                digitalWrite(pin_in2, ((current_direction + 2) % 4) == 0);
                digitalWrite(pin_in1, ((current_direction + 3) % 4) == 0);
            } else {
                #ifdef DEBUG
                Serial.println("Calibrated");
                #endif
                current_direction = 0;
                state = inactive;
                digitalWrite(pin_in4, 0);
                digitalWrite(pin_in3, 0);
                digitalWrite(pin_in2, 0);
                digitalWrite(pin_in1, 0);
            }

            calibration_step++;
            continue;
        }

        if (desired_direction == current_direction) {
            state = inactive;
            digitalWrite(pin_in4, 0);
            digitalWrite(pin_in3, 0);
            digitalWrite(pin_in2, 0);
            digitalWrite(pin_in1, 0);
            continue;
        }

        state = turning;

        if (desired_direction > current_direction)
            current_direction++;
        else if (desired_direction < current_direction)
            current_direction--;

        digitalWrite(pin_in4, (current_direction % 4) == 0);
        digitalWrite(pin_in3, ((current_direction + 1) % 4) == 0);
        digitalWrite(pin_in2, ((current_direction + 2) % 4) == 0);
        digitalWrite(pin_in1, ((current_direction + 3) % 4) == 0);
    }

    vTaskDelete(NULL);
}

void StepperMotor::startStepperTask() {
    xTaskCreate(
        [](void* o) { static_cast<StepperMotor*>(o)->stepperTask(); },
        "stepperTask",
        768,            // Stack size of task
        this,           // parameter of the task
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

    startStepperTask();
}
