#ifndef TFLUNA_I2C_H
#define TFLUNA_I2C_H

#include <Arduino.h>

#define TFL_DEFAULT_ADDRESS 0x10
#define TFL_DEFAULT_FPS     100

// FPS (Power saving mode)
#define TFL_FPS_1             1
#define TFL_FPS_2             2
#define TFL_FPS_3             3
#define TFL_FPS_4             4
#define TFL_FPS_5             5
#define TFL_FPS_6             6
#define TFL_FPS_7             7
#define TFL_FPS_8             8
#define TFL_FPS_9             9
#define TFL_FPS_10           10

// FPS (Normal mode)
#define TFL_FPS_35           35
#define TFL_FPS_50           50
#define TFL_FPS_100         100
#define TFL_FPS_125         125
#define TFL_FPS_250         250

enum TFLunaStatus {
    TFL_STATUS_READY,
    TFL_STATUS_I2C_READ_ERROR,
    TFL_STATUS_I2C_WRITE_ERROR,
    TFL_STATUS_SIGNAL_TOO_WEAK,
    TFL_STATUS_SIGNAL_TOO_STRONG
};

class TFLunaI2C {
private:
    uint8_t status;
    uint8_t i2c_address;

    bool readReg(uint8_t reg_address, uint8_t &val);
    bool writeReg(uint8_t reg_address, uint8_t val);

public:
    bool getDistanceAndStrength(int16_t &distance, int16_t &amp);
    bool getDistance(int16_t &distance);
    bool getTemperature(uint16_t &temperature);
    bool getTime(uint16_t &time);
    bool getFirmwareVersion(uint8_t ver[3]);
    bool getProductionCode(uint8_t production_code[14]);
    bool getFrameRate(uint16_t &fps);
    bool setFrameRate(uint16_t &fps);
    bool setI2CAddr(uint8_t new_address);
    bool setLowPower(bool low_power);
    bool setMinDist(uint16_t min_dist);
    bool setMaxDist(uint16_t max_dist);
    bool setContinuousMode();
    bool setTriggerMode();
    bool enable();
    bool disable();
    bool softReset();
    bool hardReset();
    bool saveSettings();
    bool trigger();
    uint8_t getStatus();

    TFLunaI2C(uint8_t i2c_address);
};

#endif
