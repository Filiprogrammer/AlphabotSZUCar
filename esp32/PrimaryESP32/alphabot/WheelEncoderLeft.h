#ifndef WHEEL_ENCODER_LEFT_H
#define WHEEL_ENCODER_LEFT_H

#include "WheelEncoder.h"

class WheelEncoderLeft: public WheelEncoder {
private:
    static WheelEncoderLeft* inst;

    WheelEncoderLeft();

public:
    static WheelEncoderLeft* getInstance();
};

#endif
