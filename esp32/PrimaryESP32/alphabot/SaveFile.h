#ifndef SAVEFILE_H
#define SAVEFILE_H

#include <Arduino.h>

class SaveFile {
private:
    bool spiffs_mounted = false;
    int16_t magnetSensorCalibratedMinX = -1000;
    int16_t magnetSensorCalibratedMaxX = 1000;
    int16_t magnetSensorCalibratedMinY = -1000;
    int16_t magnetSensorCalibratedMaxY = 1000;
    float compassAngleOffset = 0.0;

    void read();

public:
    int16_t getMagnetSensorCalibratedMinX();
    int16_t getMagnetSensorCalibratedMaxX();
    int16_t getMagnetSensorCalibratedMinY();
    int16_t getMagnetSensorCalibratedMaxY();
    float getCompassAngleOffset();
    void setMagnetSensorCalibratedMinX(int16_t min_x);
    void setMagnetSensorCalibratedMaxX(int16_t max_x);
    void setMagnetSensorCalibratedMinY(int16_t min_y);
    void setMagnetSensorCalibratedMaxY(int16_t max_y);
    void setCompassAngleOffset(float angleOffset);
    void write();

    SaveFile();
};

#endif
