#ifndef TFL_OBSTACLE_SCANNER_H
#define TFL_OBSTACLE_SCANNER_H

#include "StepperMotor.h"
#include "TFLunaI2C.h"

class TFLunaObstacleScanner {
private:
    StepperMotor* stepperMotor;
    TFLunaI2C* tflunaI2C;
    uint16_t obstacle_distances[360];
    uint16_t stepperSideSteps;
    int16_t triggerDirection;
    unsigned long lastTrigger;

public:
    int16_t scan();
    uint16_t getObstacleDistance(uint16_t angle);

    TFLunaObstacleScanner(StepperMotor* stepperMotor, TFLunaI2C* tflunaI2C, uint16_t stepperSideSteps);
};

#endif
