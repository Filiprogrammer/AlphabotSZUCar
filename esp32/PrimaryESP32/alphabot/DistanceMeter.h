#ifndef DISTANCEMETER_H
#define DISTANCEMETER_H

#include <Arduino.h>
#include <Wire.h>

class DistanceMeter {
private:
    uint8_t address;

public:
    void readValues(uint16_t* front, uint16_t* left, uint16_t* right, uint16_t* back);

    DistanceMeter(uint8_t addr);
};

#endif
