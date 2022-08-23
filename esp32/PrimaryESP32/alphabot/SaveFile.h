#ifndef SAVEFILE_H
#define SAVEFILE_H

#include <Arduino.h>

class SaveFile {
private:
    bool spiffs_mounted = false;
    float compassAngleOffset = 0.0;

    void read();

public:
    float getCompassAngleOffset() const;
    void setCompassAngleOffset(float angleOffset);
    void write();

    SaveFile();
};

#endif
