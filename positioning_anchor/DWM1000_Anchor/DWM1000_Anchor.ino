#include <SPI.h>
#include "DW1000Ranging.h"

// connection pins
const uint8_t PIN_RST = 9; // reset pin
const uint8_t PIN_IRQ = 2; // irq pin
const uint8_t PIN_SS = SS; // spi select pin

void setup() {
    Serial.begin(115200);
    delay(1000);
    // init the configuration
    DW1000Ranging.initCommunication(PIN_RST, PIN_SS, PIN_IRQ); //Reset, CS, IRQ pin
    // define the sketch as anchor. It will be great to dynamically change the type of module
    DW1000Ranging.attachNewRange(newRange);
    DW1000Ranging.attachBlinkDevice(newBlink);
    DW1000Ranging.attachInactiveDevice(inactiveDevice);
    // Enable the filter to smooth the distance
    // DW1000Ranging.useRangeFilter(true);

    DW1000.setAntennaDelay(16407);

    // we start the module as an anchor
    // Anchor 1 "BE:0C:61:69:A4:1B:41:D7"
    // Anchor 2 "A6:4C:F6:E8:12:29:4F:F4"
    // Anchor 3 "82:17:88:05:A9:9A:58:3D"
    DW1000Ranging.startAsAnchor("BE:0C:61:69:A4:1B:41:D7", DW1000.MODE_LONGDATA_RANGE_ACCURACY, false);
}

void loop() {
    DW1000Ranging.loop();
}

void newRange() {
    Serial.print("u: "); Serial.print(DW1000Ranging.getDistantDevice()->getShortAddress(), HEX);
    Serial.print(":"); Serial.print(DW1000Ranging.getDistantDevice()->getRange());
    Serial.print(":"); Serial.println(DW1000Ranging.getDistantDevice()->getRXPower());
}

void newBlink(DW1000Device* device) {
    Serial.print("+");
    Serial.println(device->getShortAddress(), HEX);
}

void inactiveDevice(DW1000Device* device) {
    Serial.print("-");
    Serial.println(device->getShortAddress(), HEX);
}
