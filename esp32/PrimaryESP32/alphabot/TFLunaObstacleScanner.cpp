#include "TFLunaObstacleScanner.h"

int16_t TFLunaObstacleScanner::scan() {
    int16_t stepperCurrentDirection = stepperMotor->getCurrentDirection();
    int16_t stepperDesiredDirection = stepperMotor->getDesiredDirection();

    if (stepperCurrentDirection == stepperDesiredDirection) {
        if (stepperDesiredDirection < 0)
            stepperMotor->turnTo(stepperSideSteps);
        else
            stepperMotor->turnTo(-stepperSideSteps);
    }

    if (millis() - lastTrigger >= 100) {
        #ifdef DEBUG
        Serial.println("TF-Luna timed out, retrying...");
        #endif
        tflunaI2C->setTriggerMode();
        tflunaI2C->trigger();
        triggerDirection = stepperCurrentDirection;
        lastTrigger = millis();
        return -1;
    }

    int16_t distance;

    if(tflunaI2C->getDistance(distance)) {
        int16_t angle = -1;

        if (tflunaI2C->getStatus() == TFL_STATUS_READY) {
            angle = (360 + ((int32_t)triggerDirection * (int32_t)360) / (int32_t)2048) % 360;
            obstacle_distances[angle] = distance;
        } else {
            #ifdef DEBUG
            Serial.println("TF-Luna failed to read distance");
            #endif
        }

        tflunaI2C->setTriggerMode();
        tflunaI2C->trigger();
        triggerDirection = stepperCurrentDirection;
        lastTrigger = millis();

        return angle;
    }

    #ifdef DEBUG
    Serial.println("Waiting for TF-Luna response...");
    #endif
    return -1;
}

uint16_t TFLunaObstacleScanner::getObstacleDistance(uint16_t angle) {
    return obstacle_distances[angle % 360];
}

TFLunaObstacleScanner::TFLunaObstacleScanner(StepperMotor* stepperMotor, TFLunaI2C* tflunaI2C, uint16_t stepperSideSteps) {
    this->stepperMotor = stepperMotor;
    this->tflunaI2C = tflunaI2C;
    this->stepperSideSteps = stepperSideSteps;
    stepperMotor->calibrate();
    memset(obstacle_distances, 0, sizeof(obstacle_distances));
    tflunaI2C->setTriggerMode();
    lastTrigger = 0 - 100;
}
