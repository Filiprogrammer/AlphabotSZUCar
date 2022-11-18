#ifndef SAVEFILE_H
#define SAVEFILE_H

#include <Arduino.h>

class SaveFile {
private:
    bool spiffs_mounted = false;
    float magnetSensorCalibratedMinX = -1000;
    float magnetSensorCalibratedMaxX = 1000;
    float magnetSensorCalibratedMinY = -1000;
    float magnetSensorCalibratedMaxY = 1000;
    float magnetSensorCalibratedMinZ = -1000;
    float magnetSensorCalibratedMaxZ = 1000;
    float compassAngleOffset = 0.0;

    void read();

public:
    float getMagnetSensorCalibratedMinX() const;
    float getMagnetSensorCalibratedMaxX() const;
    float getMagnetSensorCalibratedMinY() const;
    float getMagnetSensorCalibratedMaxY() const;
    float getMagnetSensorCalibratedMinZ() const;
    float getMagnetSensorCalibratedMaxZ() const;
    float getCompassAngleOffset() const;
    void setMagnetSensorCalibratedMinX(float min_x);
    void setMagnetSensorCalibratedMaxX(float max_x);
    void setMagnetSensorCalibratedMinY(float min_y);
    void setMagnetSensorCalibratedMaxY(float max_y);
    void setMagnetSensorCalibratedMinZ(float min_z);
    void setMagnetSensorCalibratedMaxZ(float max_z);
    void setCompassAngleOffset(float angleOffset);
    void write();

    SaveFile();
};

#endif
