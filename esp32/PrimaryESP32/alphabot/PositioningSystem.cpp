#include "PositioningSystem.h"
#include "pinconfig.h"

void PositioningSystem::readDistances(float* anc1_dist, float* anc2_dist, float* anc3_dist) {
    while (Serial2.available()) {
        char c = Serial2.read();

        if (c == '\n') {
            serial2_buffer[serial2_buffer_position] = 0;
            uint16_t short_address;

            switch (serial2_buffer[0]) {
                #ifdef DEBUG
                case '+':
                    serial2_buffer[5] = 0;
                    short_address = (uint16_t)strtol(&(serial2_buffer[1]), NULL, 16);
                    Serial.print("DW1000 Device connected: ");
                    Serial.println(short_address, HEX);
                    break;
                case '-':
                    serial2_buffer[5] = 0;
                    short_address = (uint16_t)strtol(&(serial2_buffer[1]), NULL, 16);
                    Serial.print("DW1000 Device disconnected: ");
                    Serial.println(short_address, HEX);
                    break;
                #endif
                case 'u':
                    serial2_buffer[5] = 0;
                    short_address = (uint16_t)strtol(&(serial2_buffer[1]), NULL, 16);

                    if (short_address != anc1_short_address && short_address != anc2_short_address && short_address != anc3_short_address)
                        break;

                    char* tmp = strchr(&(serial2_buffer[6]), ':');

                    if (tmp == NULL)
                        break;

                    tmp[0] = 0;
                    float range = atof(&(serial2_buffer[6]));
                    range *= 100; // Convert from meters to centimeters

                    if (short_address == anc1_short_address)
                        (*anc1_dist) = ((*anc1_dist) + range) / 2.0;
                    else if (short_address == anc2_short_address)
                        (*anc2_dist) = ((*anc2_dist) + range) / 2.0;
                    else if (short_address == anc3_short_address)
                        (*anc3_dist) = ((*anc3_dist) + range) / 2.0;

                    #ifdef DEBUG
                    Serial.print("DW1000 Device update: ");
                    Serial.print(short_address, HEX);
                    Serial.print(" Range: ");
                    Serial.println(range);
                    #endif
                    break;
            }

            serial2_buffer_position = 0;
        } else {
            serial2_buffer[serial2_buffer_position++] = c;
        }
    }
}

void PositioningSystem::setAnchorPositions(float anc1_x, float anc1_y, float anc2_x, float anc2_y, float anc3_x, float anc3_y) {
    this->anc1_x = anc1_x;
    this->anc1_y = anc1_y;
    this->anc2_x = anc2_x;
    this->anc2_y = anc2_y;
    this->anc3_x = anc3_x;
    this->anc3_y = anc3_y;
}

inline void r2_subtract(float* v1, float* v2, float* result) {
    result[0] = v1[0] - v2[0];
    result[1] = v1[1] - v2[1];
}

inline void r2_scale(float* v, float r, float* result) {
    result[0] = r * v[0];
    result[1] = r * v[1];
}

inline float r2_dot(float* v1, float* v2) {
    return v1[0] * v2[0] + v1[1] * v2[1];
}

inline float r2_norm(float* v) {
    return sqrt(sq(v[0]) + sq(v[1]));
}

void PositioningSystem::calculatePosition(float anc1_dist, float anc2_dist, float anc3_dist, float* x, float* y) {
    float P1[] = {anc1_x, anc1_y};
    float P2[] = {anc2_x, anc2_y};
    float P3[] = {anc3_x, anc3_y};

    float P2minusP1[] = {0, 0};
    r2_subtract(P2, P1, P2minusP1);
    float ex[] = {0, 0};
    r2_scale(P2minusP1, 1 / r2_norm(P2minusP1), ex);
    float P3minusP1[] = {0, 0};
    r2_subtract(P3, P1, P3minusP1);
    float i = r2_dot(ex, P3minusP1);

    float exmuli[] = {0, 0};
    r2_scale(ex, i, exmuli);
    float diff[] = {0, 0};
    r2_subtract(P3minusP1, exmuli, diff);
    float ey[] = {0, 0};
    r2_scale(diff, 1 / r2_norm(diff), ey);
    float d = r2_norm(P2minusP1);
    float j = r2_dot(ey, P3minusP1);

    (*x) = anc1_x + (sq(anc1_dist) - sq(anc2_dist) + sq(d)) / (2 * d);
    (*y) = anc1_y + ((sq(anc1_dist) - sq(anc3_dist) + sq(i) + sq(j)) / (2 * j)) - ((i / j) * (*x));
}

float PositioningSystem::getAnc1X() const {
    return anc1_x;
}

float PositioningSystem::getAnc1Y() const {
    return anc1_y;
}

float PositioningSystem::getAnc2X() const {
    return anc2_x;
}

float PositioningSystem::getAnc2Y() const {
    return anc2_y;
}

float PositioningSystem::getAnc3X() const {
    return anc3_x;
}

float PositioningSystem::getAnc3Y() const {
    return anc3_y;
}

PositioningSystem::PositioningSystem(uint16_t anc1_short_address, uint16_t anc2_short_address, uint16_t anc3_short_address) {
    this->anc1_short_address = anc1_short_address;
    this->anc2_short_address = anc2_short_address;
    this->anc3_short_address = anc3_short_address;

    Serial2.begin(115200, SERIAL_8N1, SERIAL2_RX, SERIAL2_TX);
}
