#include "SaveFile.h"
#include "FS.h"
#include "SPIFFS.h"

float SaveFile::getCompassAngleOffset() const {
    return compassAngleOffset;
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

    uint8_t buffer[4];
    union Float f;
    f.f = compassAngleOffset;
    buffer[0] = f.bytes[0];
    buffer[1] = f.bytes[1];
    buffer[2] = f.bytes[2];
    buffer[3] = f.bytes[3];
    file.write(buffer, sizeof(buffer));
}

void SaveFile::read() {
    if (!spiffs_mounted)
        return;

    File file = SPIFFS.open("/savefile");

    if (!file || file.isDirectory())
        return;

    uint8_t buffer[4];
    file.read(buffer, sizeof(buffer));

    if (file.size() >= 4) {
        union Float f;
        f.bytes[0] = buffer[0];
        f.bytes[1] = buffer[1];
        f.bytes[2] = buffer[2];
        f.bytes[3] = buffer[3];
        compassAngleOffset = f.f;
    }
}

SaveFile::SaveFile() {
    spiffs_mounted = SPIFFS.begin(true);
    read();
}
