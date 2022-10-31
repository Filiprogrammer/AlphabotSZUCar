#include "SaveFile.h"
#include "FS.h"
#include "SPIFFS.h"

float SaveFile::getMagnetSensorCalibratedMinX() const {
    return magnetSensorCalibratedMinX;
}

float SaveFile::getMagnetSensorCalibratedMaxX() const {
    return magnetSensorCalibratedMaxX;
}

float SaveFile::getMagnetSensorCalibratedMinY() const {
    return magnetSensorCalibratedMinY;
}

float SaveFile::getMagnetSensorCalibratedMaxY() const {
    return magnetSensorCalibratedMaxY;
}

float SaveFile::getMagnetSensorCalibratedMinZ() const {
    return magnetSensorCalibratedMinZ;
}

float SaveFile::getMagnetSensorCalibratedMaxZ() const {
    return magnetSensorCalibratedMaxZ;
}

float SaveFile::getCompassAngleOffset() const {
    return compassAngleOffset;
}

void SaveFile::setMagnetSensorCalibratedMinX(float min_x) {
    magnetSensorCalibratedMinX = min_x;
}

void SaveFile::setMagnetSensorCalibratedMaxX(float max_x) {
    magnetSensorCalibratedMaxX = max_x;
}

void SaveFile::setMagnetSensorCalibratedMinY(float min_y) {
    magnetSensorCalibratedMinY = min_y;
}

void SaveFile::setMagnetSensorCalibratedMaxY(float max_y) {
    magnetSensorCalibratedMaxY = max_y;
}

void SaveFile::setMagnetSensorCalibratedMinZ(float min_z) {
    magnetSensorCalibratedMinY = min_z;
}

void SaveFile::setMagnetSensorCalibratedMaxZ(float max_z) {
    magnetSensorCalibratedMaxY = max_z;
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

    uint8_t buffer[28];
    union Float f;
    f.f = compassAngleOffset;
    buffer[0] = f.bytes[0];
    buffer[1] = f.bytes[1];
    buffer[2] = f.bytes[2];
    buffer[3] = f.bytes[3];
    f.f = magnetSensorCalibratedMinX;
    buffer[4] = f.bytes[0];
    buffer[5] = f.bytes[1];
    buffer[6] = f.bytes[2];
    buffer[7] = f.bytes[3];
    f.f = magnetSensorCalibratedMaxX;
    buffer[8] = f.bytes[0];
    buffer[9] = f.bytes[1];
    buffer[10] = f.bytes[2];
    buffer[11] = f.bytes[3];
    f.f = magnetSensorCalibratedMinY;
    buffer[12] = f.bytes[0];
    buffer[13] = f.bytes[1];
    buffer[14] = f.bytes[2];
    buffer[15] = f.bytes[3];
    f.f = magnetSensorCalibratedMaxY;
    buffer[16] = f.bytes[0];
    buffer[17] = f.bytes[1];
    buffer[18] = f.bytes[2];
    buffer[19] = f.bytes[3];
    f.f = magnetSensorCalibratedMinZ;
    buffer[20] = f.bytes[0];
    buffer[21] = f.bytes[1];
    buffer[22] = f.bytes[2];
    buffer[23] = f.bytes[3];
    f.f = magnetSensorCalibratedMaxZ;
    buffer[24] = f.bytes[0];
    buffer[25] = f.bytes[1];
    buffer[26] = f.bytes[2];
    buffer[27] = f.bytes[3];
    file.write(buffer, sizeof(buffer));
}

void SaveFile::read() {
    if (!spiffs_mounted)
        return;

    File file = SPIFFS.open("/savefile");

    if (!file || file.isDirectory())
        return;

    uint8_t buffer[28];
    file.read(buffer, sizeof(buffer));
    union Float f;

    if (file.size() >= 4) {
        f.bytes[0] = buffer[0];
        f.bytes[1] = buffer[1];
        f.bytes[2] = buffer[2];
        f.bytes[3] = buffer[3];
        compassAngleOffset = f.f;
    }

    if (file.size() >= 28) {
        f.bytes[4] = buffer[0];
        f.bytes[5] = buffer[1];
        f.bytes[6] = buffer[2];
        f.bytes[7] = buffer[3];
        magnetSensorCalibratedMinX = f.f;
        f.bytes[8] = buffer[0];
        f.bytes[9] = buffer[1];
        f.bytes[10] = buffer[2];
        f.bytes[11] = buffer[3];
        magnetSensorCalibratedMaxX = f.f;
        f.bytes[12] = buffer[0];
        f.bytes[13] = buffer[1];
        f.bytes[14] = buffer[2];
        f.bytes[15] = buffer[3];
        magnetSensorCalibratedMinY = f.f;
        f.bytes[16] = buffer[0];
        f.bytes[17] = buffer[1];
        f.bytes[18] = buffer[2];
        f.bytes[19] = buffer[3];
        magnetSensorCalibratedMaxY = f.f;
        f.bytes[20] = buffer[0];
        f.bytes[21] = buffer[1];
        f.bytes[22] = buffer[2];
        f.bytes[23] = buffer[3];
        magnetSensorCalibratedMinZ = f.f;
        f.bytes[24] = buffer[0];
        f.bytes[25] = buffer[1];
        f.bytes[26] = buffer[2];
        f.bytes[27] = buffer[3];
        magnetSensorCalibratedMaxZ = f.f;
    }
}

SaveFile::SaveFile() {
    spiffs_mounted = SPIFFS.begin(true);
    read();
}
