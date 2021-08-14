#include "SaveFile.h"
#include "FS.h"
#include "SPIFFS.h"

int16_t SaveFile::getMagnetSensorCalibratedMinX() {
    return magnetSensorCalibratedMinX;
}

int16_t SaveFile::getMagnetSensorCalibratedMaxX() {
    return magnetSensorCalibratedMaxX;
}

int16_t SaveFile::getMagnetSensorCalibratedMinY() {
    return magnetSensorCalibratedMinY;
}

int16_t SaveFile::getMagnetSensorCalibratedMaxY() {
    return magnetSensorCalibratedMaxY;
}

float SaveFile::getCompassAngleOffset() {
    return compassAngleOffset;
}

void SaveFile::setMagnetSensorCalibratedMinX(int16_t min_x) {
    magnetSensorCalibratedMinX = min_x;
}

void SaveFile::setMagnetSensorCalibratedMaxX(int16_t max_x) {
    magnetSensorCalibratedMaxX = max_x;
}

void SaveFile::setMagnetSensorCalibratedMinY(int16_t min_y) {
    magnetSensorCalibratedMinY = min_y;
}

void SaveFile::setMagnetSensorCalibratedMaxY(int16_t max_y) {
    magnetSensorCalibratedMaxY = max_y;
}

void SaveFile::setCompassAngleOffset(float angleOffset) {
    compassAngleOffset = angleOffset;
}

union Float {
    float f;
    uint8_t bytes[sizeof(float)];
};

void SaveFile::write() {
    if (!spiffs_mounted)
        return;

    File file = SPIFFS.open("/savefile", FILE_WRITE);

    if (!file)
        return;

    uint8_t buffer[12];
    buffer[0] = magnetSensorCalibratedMinX;
    buffer[1] = magnetSensorCalibratedMinX >> 8;
    buffer[2] = magnetSensorCalibratedMaxX;
    buffer[3] = magnetSensorCalibratedMaxX >> 8;
    buffer[4] = magnetSensorCalibratedMinY;
    buffer[5] = magnetSensorCalibratedMinY >> 8;
    buffer[6] = magnetSensorCalibratedMaxY;
    buffer[7] = magnetSensorCalibratedMaxY >> 8;
    union Float f;
    f.f = compassAngleOffset;
    buffer[8] = f.bytes[0];
    buffer[9] = f.bytes[1];
    buffer[10] = f.bytes[2];
    buffer[11] = f.bytes[3];
    file.write(buffer, sizeof(buffer));
}

void SaveFile::read() {
    if (!spiffs_mounted)
        return;

    File file = SPIFFS.open("/savefile");

    if (!file || file.isDirectory())
        return;

    uint8_t buffer[12];
    file.read(buffer, sizeof(buffer));

    if (file.size() >= 2)
        magnetSensorCalibratedMinX = buffer[0] | (buffer[1] << 8);

    if (file.size() >= 4)
        magnetSensorCalibratedMaxX = buffer[2] | (buffer[3] << 8);

    if (file.size() >= 6)
        magnetSensorCalibratedMinY = buffer[4] | (buffer[5] << 8);

    if (file.size() >= 8)
        magnetSensorCalibratedMaxY = buffer[6] | (buffer[7] << 8);

    if (file.size() >= 12) {
        union Float f;
        f.bytes[0] = buffer[8];
        f.bytes[1] = buffer[9];
        f.bytes[2] = buffer[10];
        f.bytes[3] = buffer[11];
        compassAngleOffset = f.f;
    }
}

SaveFile::SaveFile() {
    spiffs_mounted = SPIFFS.begin(true);
    read();
}
