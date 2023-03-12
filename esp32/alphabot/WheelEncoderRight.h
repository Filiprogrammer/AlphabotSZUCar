#ifndef WHEEL_ENCODER_RIGHT_H
#define WHEEL_ENCODER_RIGHT_H

#include "WheelEncoder.h"

class WheelEncoderRight: public WheelEncoder {
private:
    static WheelEncoderRight* inst;

    WheelEncoderRight();

public:
    static WheelEncoderRight* getInstance();
};

#endif
