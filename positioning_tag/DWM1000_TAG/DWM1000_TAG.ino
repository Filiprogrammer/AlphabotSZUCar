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
    DW1000Ranging.attachNewDevice(newDevice);
    DW1000Ranging.attachInactiveDevice(inactiveDevice);
    // Enable the filter to smooth the distance
    // DW1000Ranging.useRangeFilter(true);

    DW1000.setAntennaDelay(16480);

    // we start the module as a tag
    DW1000Ranging.startAsTag("B2:04:C5:78:05:5E:E6:2F", DW1000.MODE_LONGDATA_RANGE_ACCURACY);
}

void loop() {
    DW1000Ranging.loop();
}

void newRange() {
    Serial.print("u");
    char shortAddressHex[5];
    sprintf(shortAddressHex, "%04X", DW1000Ranging.getDistantDevice()->getShortAddress());
    Serial.print(shortAddressHex);
    Serial.print(":"); Serial.print(DW1000Ranging.getDistantDevice()->getRange());
    Serial.print(":"); Serial.println(DW1000Ranging.getDistantDevice()->getRXPower());
}

void newDevice(DW1000Device* device) {
    Serial.print("+");
    char shortAddressHex[5];
    sprintf(shortAddressHex, "%04X", device->getShortAddress());
    Serial.println(shortAddressHex);
}

void inactiveDevice(DW1000Device* device) {
    Serial.print("-");
    char shortAddressHex[5];
    sprintf(shortAddressHex, "%04X", device->getShortAddress());
    Serial.println(shortAddressHex);
}
