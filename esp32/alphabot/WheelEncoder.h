#ifndef WHEEL_ENCODER_H
#define WHEEL_ENCODER_H

#include <Arduino.h>

class WheelEncoder {
protected:
    uint8_t encoder_pin;
    volatile uint32_t last_pulse_time = 0;
    volatile uint32_t time_between_pulses = UINT32_MAX;

public:
    float getRotationsPerMinute();
    float getWheelSpeedMps();

    WheelEncoder(uint8_t pin_d0, void (*handleWheelEncoderInterrupt)());
    ~WheelEncoder();
};

#endif
