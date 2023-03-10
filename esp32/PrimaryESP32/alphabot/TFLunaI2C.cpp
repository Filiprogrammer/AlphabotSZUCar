#include "TFLunaI2C.h"
#include <Wire.h>

// I2C Registers
#define TFL_REG_DIST_LOW        0x00
#define TFL_REG_DIST_HIGH       0x01
#define TFL_REG_AMP_LOW         0x02
#define TFL_REG_AMP_HIGH        0x03
#define TFL_REG_TEMP_LOW        0x04
#define TFL_REG_TEMP_HIGH       0x05
#define TFL_REG_TICK_LOW        0x06
#define TFL_REG_TICK_HIGH       0x07
#define TFL_REG_ERROR_LOW       0x08
#define TFL_REG_ERROR_HIGH      0x09
#define TFL_REG_VER_REV         0x0A
#define TFL_REG_VER_MIN         0x0B
#define TFL_REG_VER_MAJ         0x0C
#define TFL_REG_PROD_CODE       0x10
#define TFL_REG_ULTRA_LOW_POWER 0x1F
#define TFL_REG_SAVE_SETTINGS   0x20
#define TFL_REG_SOFT_RESET      0x21
#define TFL_REG_I2C_ADDR        0x22
#define TFL_REG_SET_TRIG_MODE   0x23
#define TFL_REG_TRIGGER         0x24
#define TFL_REG_ENABLE          0x25
#define TFL_REG_FPS_LOW         0x26
#define TFL_REG_FPS_HIGH        0x27
#define TFL_REG_SET_LOW_POWER   0x28
#define TFL_REG_HARD_RESET      0x29
#define TFL_REG_AMP_THR_LOW     0x2A
#define TFL_REG_AMP_THR_HIGH    0x2B
#define TFL_REG_DUMMY_DIST_LOW  0x2C
#define TFL_REG_DUMMY_DIST_HIGH 0x2D
#define TFL_REG_MIN_DIST_LOW    0x2E
#define TFL_REG_MIN_DIST_HIGH   0x2F
#define TFL_REG_MAX_DIST_LOW    0x30
#define TFL_REG_MAX_DIST_HIGH   0x31

bool TFLunaI2C::readReg(uint8_t reg_address, uint8_t &val) {
    Wire.beginTransmission(i2c_address);
    Wire.write(reg_address);

    if (Wire.endTransmission() != 0) {
        // Write error
        status = TFL_STATUS_I2C_WRITE_ERROR;
        return false;
    }

    Wire.requestFrom((uint16_t)i2c_address, (uint8_t)1, true);

    if (Wire.peek() == -1) {
        // Read error
        status = TFL_STATUS_I2C_READ_ERROR;
        return false;
    }

    val = (uint8_t)Wire.read();
    return true;
}

bool TFLunaI2C::writeReg(uint8_t reg_address, uint8_t val) {
    Wire.beginTransmission(i2c_address);
    Wire.write(reg_address);
    Wire.write(val);

    if (Wire.endTransmission() != 0) {
        // Write error
        status = TFL_STATUS_I2C_WRITE_ERROR;
        return false;
    }

    return true;
}

/**
 * @brief Get the measured distance value along with the signal strength.
 * 
 * @param dist measured distance in centimeters
 * @param amp signal strength
 * @return true read succeeded.
 * @return false read failed.
 */
bool TFLunaI2C::getDistanceAndStrength(int16_t &distance, int16_t &amp) {
    uint8_t sensor_data[4];

    for (uint8_t reg = TFL_REG_DIST_LOW; reg <= TFL_REG_AMP_HIGH; reg++)
        if (!readReg(reg, sensor_data[reg]))
            return false;

    distance = sensor_data[0] + (sensor_data[1] << 8);
    amp = sensor_data[2] + (sensor_data[3] << 8);

    if (amp < 100) {
        // Received signal is too low
        status = TFL_STATUS_SIGNAL_TOO_WEAK;
        return false;
    }

    if (amp == 0xFFFF) {
        // Received signal is overexposed
        status = TFL_STATUS_SIGNAL_TOO_STRONG;
        return false;
    }

    status = TFL_STATUS_READY;
    return true;
}

/**
 * @brief Get the measured distance value.
 * 
 * @param distance measured distance in centimeters
 * @return true read succeeded.
 * @return false read failed.
 */
bool TFLunaI2C::getDistance(int16_t &distance) {
    int16_t amp;
    return getDistanceAndStrength(distance, amp);
}

/**
 * @brief Get the device temperature in (Celsius + 256) * 8.
 * 
 * @param temperature temperature value
 * @return true read succeeded.
 * @return false read failed.
 */
bool TFLunaI2C::getTemperature(uint16_t &temperature) {
    uint8_t* ptemp = (uint8_t*)&temperature;

    if (!readReg(TFL_REG_TEMP_LOW, ptemp[0]))
        return false;

    return readReg(TFL_REG_TEMP_HIGH, ptemp[1]);
}

/**
 * @brief Get the device time in milliseconds.
 * 
 * @param time device time in milliseconds
 * @return true read succeeded.
 * @return false read failed.
 */
bool TFLunaI2C::getTime(uint16_t &time) {
    uint8_t* ptime = (uint8_t*)&time;

    if (!readReg(TFL_REG_TICK_LOW, ptime[0]))
        return false;

    return readReg(TFL_REG_TICK_HIGH, ptime[1]);
}

/**
 * @brief Get the firmware version.
 * 
 * @param version 3 byte array to hold the firmware version in the follwing format (revision, minor, major)
 * @return true read succeeded.
 * @return false read failed.
 */
bool TFLunaI2C::getFirmwareVersion(uint8_t version[3]) {
    for (uint8_t i = 0; i < 3; ++i)
        if (!readReg(0x0A + i, version[i]))
            return false;

    return true;
}

/**
 * @brief Get the production code. (serial number)
 * 
 * @param production_code 14 byte array to hold the production code as an ASCII string
 * @return true read succeeded.
 * @return false read failed.
 */
bool TFLunaI2C::getProductionCode(uint8_t production_code[14]) {
    for (uint8_t i = 0; i < 14; ++i)
        if (!readReg(0x10 + i, production_code[i]))
            return false;

    return true;
}

/**
 * @brief Get the rate at which the device measures in continuous mode.
 * 
 * @param fps number of measurements per second
 * @return true read succeeded.
 * @return false read failed.
 */
bool TFLunaI2C::getFrameRate(uint16_t &fps) {
    uint8_t * pfps = (uint8_t *) &fps;

    if (!readReg(TFL_REG_FPS_LOW, pfps[0]))
        return false;

    return readReg(TFL_REG_FPS_HIGH, pfps[1]);
}

/**
 * @brief Set the rate at which the device measures in continuous mode.
 * 
 * @param fps number of measurements per second
 * @return true write succeeded.
 * @return false write failed.
 */
bool TFLunaI2C::setFrameRate(uint16_t &fps) {
    uint8_t* pfps = (uint8_t*)&fps;

    if (!writeReg(TFL_REG_FPS_LOW, pfps[0]))
        return false;

    return writeReg(TFL_REG_FPS_HIGH, pfps[1]);
}

/**
 * @brief Set the I2C address of the device. The address must be within
 * the range of 0x08 to 0x77. The device must reboot for this change to
 * take effect.
 * 
 * @param new_address new I2C address
 * @return true write succeeded.
 * @return false write failed.
 */
bool TFLunaI2C::setI2CAddr(uint8_t new_address) {
    if (writeReg(TFL_REG_I2C_ADDR, new_address)) {
        i2c_address = new_address;
        return true;
    }

    return false;
}

/**
 * @brief Set whether the device should operate in power saving mode.
 * 
 * @param low_power true for power saving mode, otherwise false
 * @return true write succeeded.
 * @return false write failed.
 */
bool TFLunaI2C::setLowPower(bool low_power) {
    return writeReg(TFL_REG_SET_LOW_POWER, (uint8_t)low_power);
}

/**
 * @brief Set the minimum distance that the device should measure.
 * 
 * @param min_dist minimum distance in centimeters
 * @return true write succeeded.
 * @return false write failed.
 */
bool TFLunaI2C::setMinDist(uint16_t min_dist) {
    if (!writeReg(TFL_REG_MIN_DIST_LOW, (uint8_t)min_dist))
        return false;

    return writeReg(TFL_REG_MIN_DIST_HIGH, min_dist >> 8);
}

/**
 * @brief Set the maximum distance that the device should measure.
 * 
 * @param max_dist maximum distance in centimeters
 * @return true write succeeded.
 * @return false write failed.
 */
bool TFLunaI2C::setMaxDist(uint16_t max_dist) {
    if (!writeReg(TFL_REG_MAX_DIST_LOW, (uint8_t)max_dist))
        return false;

    return writeReg(TFL_REG_MAX_DIST_HIGH, max_dist >> 8);
}

/**
 * @brief Put the device into continuous mode. This will make the device
 * sample continuously at the configured frame rate.
 * 
 * @return true write succeeded.
 * @return false write failed.
 */
bool TFLunaI2C::setContinuousMode() {
    return writeReg(TFL_REG_SET_TRIG_MODE, 0);
}

/**
 * @brief Put the device into trigger mode. This will make the device
 * only sample once when it is triggered.
 * 
 * @return true write succeeded.
 * @return false write failed.
 */
bool TFLunaI2C::setTriggerMode() {
    return writeReg(TFL_REG_SET_TRIG_MODE, 1);
}

/**
 * @brief Enable the device.
 * 
 * @return true write succeeded.
 * @return false write failed.
 */
bool TFLunaI2C::enable() {
    return writeReg(TFL_REG_ENABLE, 1);
}

/**
 * @brief Disable the device.
 * 
 * @return true write succeeded.
 * @return false write failed.
 */
bool TFLunaI2C::disable() {
    return writeReg(TFL_REG_ENABLE, 0);
}

/**
 * @brief Reboot the device.
 * 
 * @return true write succeeded.
 * @return false write failed.
 */
bool TFLunaI2C::softReset() {
    return writeReg(TFL_REG_SOFT_RESET, 2);
}

/**
 * @brief Restore the device to factory defaults.
 * 
 * @return true write succeeded.
 * @return false write failed.
 */
bool TFLunaI2C::hardReset() {
    return writeReg(TFL_REG_HARD_RESET, 1);
}

/**
 * @brief Save the settings.
 * 
 * @return true write succeeded.
 * @return false write failed.
 */
bool TFLunaI2C::saveSettings() {
    return writeReg(TFL_REG_SAVE_SETTINGS, 1);
}

/**
 * @brief Trigger the device to sample once.
 * 
 * @return true device was triggered successfully.
 * @return false device is in continuous mode, hence it can not be triggered.
 */
bool TFLunaI2C::trigger() {
    return writeReg(TFL_REG_TRIGGER, 1);
}

uint8_t TFLunaI2C::getStatus() {
    return status;
}

TFLunaI2C::TFLunaI2C(uint8_t i2c_address) {
    this->i2c_address = i2c_address;
    status = TFL_STATUS_READY;
}
