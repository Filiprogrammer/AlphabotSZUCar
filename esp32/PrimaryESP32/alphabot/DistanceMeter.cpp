#include "DistanceMeter.h"

void DistanceMeter::readValues(uint16_t* front, uint16_t* left, uint16_t* right, uint16_t* back) {
    Wire.beginTransmission(this->address);
    Wire.write(0x52);
    int err = Wire.endTransmission();

    if (err)
        return;

    uint8_t size = Wire.requestFrom(this->address, 9U);

    if (size < 9)
        return;

    uint8_t bus_num = Wire.read();

    if (bus_num != this->address)
        return;

    (*front) = (uint16_t)(Wire.read() | (Wire.read() << 8));
    (*left)  = (uint16_t)(Wire.read() | (Wire.read() << 8));
    (*right) = (uint16_t)(Wire.read() | (Wire.read() << 8));
    (*back)  = (uint16_t)(Wire.read() | (Wire.read() << 8));
}

DistanceMeter::DistanceMeter(uint8_t addr) {
    this->address = addr;
}
