#include "WheelEncoder.h"
#include "config.h"

#define PULSES_PER_TURN 12

float WheelEncoder::getRotationsPerMinute() {
    uint32_t time_between_pulses_snapshot = time_between_pulses;
    uint32_t time_since_last_pulse = micros() - last_pulse_time;
    float rpm = -1.0;

    if (time_since_last_pulse > time_between_pulses_snapshot)
        rpm = 1.0 / ((PULSES_PER_TURN * time_since_last_pulse) / (60.0 * 1000000.0));
    else
        rpm = 1.0 / ((PULSES_PER_TURN * time_between_pulses_snapshot) / (60.0 * 1000000.0));

    return rpm;
}

/**
 * @brief Get the speed of the wheel in m/s.
 * 
 * @return float the wheel speed in m/s
 */
float WheelEncoder::getWheelSpeedMps() {
    return getRotationsPerMinute() * PI * WHEEL_DIAMETER_MM / 60000.0;
}

WheelEncoder::WheelEncoder(uint8_t pin_d0, void (*handleWheelEncoderInterrupt)()) {
    encoder_pin = pin_d0;
    pinMode(pin_d0, INPUT);
    attachInterrupt(pin_d0, handleWheelEncoderInterrupt, RISING);
    last_pulse_time = 0;
    time_between_pulses = UINT32_MAX;
}

WheelEncoder::~WheelEncoder() {
    detachInterrupt(encoder_pin);
}
