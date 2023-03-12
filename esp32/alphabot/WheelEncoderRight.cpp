#include "WheelEncoderRight.h"
#include "pinconfig.h"

static uint8_t* pencoder_pin;
static volatile uint32_t debounce;
static volatile uint32_t* plast_pulse_time;
static volatile uint32_t* ptime_between_pulses;

WheelEncoderRight* WheelEncoderRight::inst = nullptr;

WheelEncoderRight* WheelEncoderRight::getInstance() {
    return (!inst) ? inst = new WheelEncoderRight : inst;
}

static void IRAM_ATTR handleWheelEncoderInterrupt() {
    if(digitalRead(*pencoder_pin) && (micros() - debounce > 500)) {
        debounce = micros();
        *ptime_between_pulses = debounce - *plast_pulse_time;
        *plast_pulse_time = debounce;
    }
}

WheelEncoderRight::WheelEncoderRight(): WheelEncoder(RIGHT_WHEEL_ENCODER_D0, handleWheelEncoderInterrupt) {
    pencoder_pin = &encoder_pin;
    debounce = 0;
    plast_pulse_time = &last_pulse_time;
    ptime_between_pulses = &time_between_pulses;
}
