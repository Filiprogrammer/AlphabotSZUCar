#include "StepperMotor.h"
#include "pinconfig.h"
#include "config.h"
#include <esp32/ulp.h>
#include <driver/rtc_io.h>

#define RTC_MEM_VARIABLE_ADDRESS 0x1000
#define IS_CALIBRATING_OFFSET    0
#define CALIBRATION_STEP_OFFSET  1
#define CURRENT_DIRECTION_OFFSET 2
#define DESIRED_DIRECTION_OFFSET 3

void StepperMotor::turnTo(int16_t dir) {
    desired_direction = constrain(dir, -STEPPER_CALIBRATION_STEPS / 2, STEPPER_CALIBRATION_STEPS / 2);
    RTC_SLOW_MEM[RTC_MEM_VARIABLE_ADDRESS / sizeof(uint32_t) + DESIRED_DIRECTION_OFFSET] = desired_direction;
}

void StepperMotor::calibrate() {
    RTC_SLOW_MEM[RTC_MEM_VARIABLE_ADDRESS / sizeof(uint32_t) + IS_CALIBRATING_OFFSET] = true;
    RTC_SLOW_MEM[RTC_MEM_VARIABLE_ADDRESS / sizeof(uint32_t) + CALIBRATION_STEP_OFFSET] = 0;
    #ifdef DEBUG
    Serial.println("Calibrate");
    #endif
}

// 4.11 RTC_MUX Pin List https://www.espressif.com/sites/default/files/documentation/esp32_technical_reference_manual_en.pdf
const uint8_t gpio_to_rtc_gpio_conversion[] = {
    11,   0xFF,   12, 0xFF,   10, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF,
    0xFF, 0xFF,   15,   14,   16,   13, 0xFF, 0xFF, 0xFF, 0xFF,
    0xFF, 0xFF, 0xFF, 0xFF, 0xFF,    6,    7,   17, 0xFF, 0xFF,
    0xFF, 0xFF,    9,    8,    4,    5,    0,    1,    2,    3
};

void StepperMotor::startStepperTask() {
    uint8_t pin_in1_rtc_bit = gpio_to_rtc_gpio_conversion[pin_in1] + 14;
    uint8_t pin_in2_rtc_bit = gpio_to_rtc_gpio_conversion[pin_in2] + 14;
    uint8_t pin_in3_rtc_bit = gpio_to_rtc_gpio_conversion[pin_in3] + 14;
    uint8_t pin_in4_rtc_bit = gpio_to_rtc_gpio_conversion[pin_in4] + 14;

    const ulp_insn_t stepperUlpTask[] = {
        I_DELAY(37500),

        // if (isCalibrating) {
        I_MOVI(R3, RTC_MEM_VARIABLE_ADDRESS / sizeof(uint32_t)),            // R3 = IS_CALIBRATING_ADDR
        I_LD(R0, R3, IS_CALIBRATING_OFFSET),                                // R0 = RTC_SLOW_MEM[R3 + 0]
        M_BL(1, 1),                                                         // IF R0 < 1 THEN GOTO M_LABEL(1)

        I_LD(R0, R3, CALIBRATION_STEP_OFFSET),                              // R0 = RTC_SLOW_MEM[R3 + 4]
        M_BL(2, STEPPER_CALIBRATION_STEPS),                                 // IF R0 < STEPPER_CALIBRATION_STEPS THEN GOTO M_LABEL(2)
        M_BL(3, STEPPER_CALIBRATION_STEPS + STEPPER_CALIBRATION_STEPS / 2), // IF R0 < (STEPPER_CALIBRATION_STEPS + STEPPER_CALIBRATION_STEPS / 2) THEN GOTO M_LABEL(3)

        // Calibrated
        // current_direction = 0;
        I_MOVI(R0, 0),                                                      // R0 = 0
        I_ST(R0, R3, CURRENT_DIRECTION_OFFSET),                             // RTC_SLOW_MEM[R3 + 8] = R0
        // isCalibrating = false;
        I_ST(R0, R3, IS_CALIBRATING_OFFSET),                                // RTC_SLOW_MEM[R3 + 0] = R0
        M_LABEL(13),
        // digitalWrite(pin_in4, 0);
        I_WR_REG(RTC_GPIO_OUT_REG, pin_in4_rtc_bit, pin_in4_rtc_bit, 0),
        // digitalWrite(pin_in3, 0);
        I_WR_REG(RTC_GPIO_OUT_REG, pin_in3_rtc_bit, pin_in3_rtc_bit, 0),
        // digitalWrite(pin_in2, 0);
        I_WR_REG(RTC_GPIO_OUT_REG, pin_in2_rtc_bit, pin_in2_rtc_bit, 0),
        // digitalWrite(pin_in1, 0);
        I_WR_REG(RTC_GPIO_OUT_REG, pin_in1_rtc_bit, pin_in1_rtc_bit, 0),
        // continue;
        I_BXI(0),

        M_LABEL(2),
        // current_direction--;
        I_LD(R1, R3, CURRENT_DIRECTION_OFFSET),                             // R1 = RTC_SLOW_MEM[R3 + 8]
        I_SUBI(R1, R1, 1),                                                  // R1 = R1 - 1
        I_ST(R1, R3, CURRENT_DIRECTION_OFFSET),                             // RTC_SLOW_MEM[R3 + 8] = R1
        // digitalWrite(pin_in4, (current_direction % 4) == 0);
        M_LABEL(12),
        I_ANDI(R0, R1, 0x03),                                               // R0 = R1 & 0x03
        M_BL(4, 1),                                                         // IF R0 < 1 THEN GOTO M_LABEL(4)
        I_WR_REG(RTC_GPIO_OUT_REG, pin_in4_rtc_bit, pin_in4_rtc_bit, 0),    // PIN 4 --> LOW
        M_BX(5),                                                            // GOTO M_LABEL(5)
        M_LABEL(4),
        I_WR_REG(RTC_GPIO_OUT_REG, pin_in4_rtc_bit, pin_in4_rtc_bit, 1),    // PIN 4 --> HIGH
        M_LABEL(5),
        // digitalWrite(pin_in3, ((current_direction + 1) % 4) == 0);
        I_ADDI(R1, R1, 1),                                                  // R1 = R1 + 1
        I_ANDI(R0, R1, 0x03),                                               // R0 = R1 & 0x03
        M_BL(6, 1),                                                         // IF R0 < 1 THEN GOTO M_LABEL(6)
        I_WR_REG(RTC_GPIO_OUT_REG, pin_in3_rtc_bit, pin_in3_rtc_bit, 0),    // PIN 3 --> LOW
        M_BX(7),                                                            // GOTO M_LABEL(7)
        M_LABEL(6),
        I_WR_REG(RTC_GPIO_OUT_REG, pin_in3_rtc_bit, pin_in3_rtc_bit, 1),    // PIN 3 --> HIGH
        M_LABEL(7),
        // digitalWrite(pin_in2, ((current_direction + 2) % 4) == 0);
        I_ADDI(R1, R1, 1),                                                  // R1 = R1 + 1
        I_ANDI(R0, R1, 0x03),                                               // R0 = R1 & 0x03
        M_BL(8, 1),                                                         // IF R0 < 1 THEN GOTO M_LABEL(8)
        I_WR_REG(RTC_GPIO_OUT_REG, pin_in2_rtc_bit, pin_in2_rtc_bit, 0),    // PIN 2 --> LOW
        M_BX(9),                                                            // GOTO M_LABEL(9)
        M_LABEL(8),
        I_WR_REG(RTC_GPIO_OUT_REG, pin_in2_rtc_bit, pin_in2_rtc_bit, 1),    // PIN 2 --> HIGH
        M_LABEL(9),
        // digitalWrite(pin_in1, ((current_direction + 3) % 4) == 0);
        I_ADDI(R1, R1, 1),                                                  // R1 = R1 + 1
        I_ANDI(R0, R1, 0x03),                                               // R0 = R1 & 0x03
        M_BL(10, 1),                                                        // IF R0 < 1 THEN GOTO M_LABEL(10)
        I_WR_REG(RTC_GPIO_OUT_REG, pin_in1_rtc_bit, pin_in1_rtc_bit, 0),    // PIN 1 --> LOW
        M_BX(11),                                                           // GOTO M_LABEL(11)
        M_LABEL(10),
        I_WR_REG(RTC_GPIO_OUT_REG, pin_in1_rtc_bit, pin_in1_rtc_bit, 1),    // PIN 1 --> HIGH

        M_LABEL(11),
        // calibration_step++;
        I_LD(R0, R3, CALIBRATION_STEP_OFFSET),
        I_ADDI(R0, R0, 1),
        I_ST(R0, R3, CALIBRATION_STEP_OFFSET),
        // continue;
        I_BXI(0),

        M_LABEL(3),
        // current_direction++;
        I_LD(R1, R3, CURRENT_DIRECTION_OFFSET),                             // R1 = RTC_SLOW_MEM[R3 + 8]
        I_ADDI(R1, R1, 1),                                                  // R1 = R1 + 1
        I_ST(R1, R3, CURRENT_DIRECTION_OFFSET),                             // RTC_SLOW_MEM[R3 + 8] = R1
        // digitalWrite(pin_in4, (current_direction % 4) == 0);
        // digitalWrite(pin_in3, ((current_direction + 1) % 4) == 0);
        // digitalWrite(pin_in2, ((current_direction + 2) % 4) == 0);
        // digitalWrite(pin_in1, ((current_direction + 3) % 4) == 0);
        M_BX(12),

        // } // if (isCalibrating)
        M_LABEL(1),
        // if (desired_direction == current_direction) {
        I_LD(R1, R3, CURRENT_DIRECTION_OFFSET),                             // R1 = RTC_SLOW_MEM[R3 + 8]
        I_LD(R2, R3, DESIRED_DIRECTION_OFFSET),                             // R2 = RTC_SLOW_MEM[R3 + 12]
        I_SUBR(R0, R1, R2),                                                 // R0 = R1 - R2
        M_BXZ(13),
        //     digitalWrite(pin_in4, 0);
        //     digitalWrite(pin_in3, 0);
        //     digitalWrite(pin_in2, 0);
        //     digitalWrite(pin_in1, 0);
        //     continue;
        // }

        // if (desired_direction > current_direction)
        I_SUBR(R2, R1, R2),                                                 // R2 = R1 - R2     (current_direction - desired_direction)
        I_RSHI(R0, R2, 15),                                                 // R0 = R2 >> 15    if (desired_direction > current_direction) --> R0 = 1
                                                                            //                  if (desired_direction <= current_direction) --> R0 = 0
        M_BL(14, 1),
        //     current_direction++;
        I_ADDI(R0, R1, 1),                                                  // R0 = R1 + 1
        M_BX(15),
        // else if (desired_direction < current_direction)
        M_LABEL(14),
        //     current_direction--;
        I_SUBI(R0, R1, 1),                                                  // R0 = R1 - 1
        M_LABEL(15),
        I_ST(R0, R3, CURRENT_DIRECTION_OFFSET),                             // RTC_SLOW_MEM[R3 + 8] = R0

        // digitalWrite(pin_in4, (current_direction % 4) == 0);
        I_MOVR(R1, R0),                                                     // R1 = R0
        I_ANDI(R0, R1, 0x03),                                               // R0 = R1 & 0x03
        M_BL(16, 1),                                                        // IF R0 < 1 THEN GOTO M_LABEL(16)
        I_WR_REG(RTC_GPIO_OUT_REG, pin_in4_rtc_bit, pin_in4_rtc_bit, 0),    // PIN 4 --> LOW
        M_BX(17),                                                           // GOTO M_LABEL(17)
        M_LABEL(16),
        I_WR_REG(RTC_GPIO_OUT_REG, pin_in4_rtc_bit, pin_in4_rtc_bit, 1),    // PIN 4 --> HIGH
        M_LABEL(17),
        // digitalWrite(pin_in3, ((current_direction + 1) % 4) == 0);
        I_ADDI(R1, R1, 1),                                                  // R1 = R1 + 1
        I_ANDI(R0, R1, 0x03),                                               // R0 = R1 & 0x03
        M_BL(18, 1),                                                        // IF R0 < 1 THEN GOTO M_LABEL(18)
        I_WR_REG(RTC_GPIO_OUT_REG, pin_in3_rtc_bit, pin_in3_rtc_bit, 0),    // PIN 3 --> LOW
        M_BX(19),                                                           // GOTO M_LABEL(19)
        M_LABEL(18),
        I_WR_REG(RTC_GPIO_OUT_REG, pin_in3_rtc_bit, pin_in3_rtc_bit, 1),    // PIN 3 --> HIGH
        M_LABEL(19),
        // digitalWrite(pin_in2, ((current_direction + 2) % 4) == 0);
        I_ADDI(R1, R1, 1),                                                  // R1 = R1 + 1
        I_ANDI(R0, R1, 0x03),                                               // R0 = R1 & 0x03
        M_BL(20, 1),                                                        // IF R0 < 1 THEN GOTO M_LABEL(20)
        I_WR_REG(RTC_GPIO_OUT_REG, pin_in2_rtc_bit, pin_in2_rtc_bit, 0),    // PIN 2 --> LOW
        M_BX(21),                                                           // GOTO M_LABEL(21)
        M_LABEL(20),
        I_WR_REG(RTC_GPIO_OUT_REG, pin_in2_rtc_bit, pin_in2_rtc_bit, 1),    // PIN 2 --> HIGH
        M_LABEL(21),
        // digitalWrite(pin_in1, ((current_direction + 3) % 4) == 0);
        I_ADDI(R1, R1, 1),                                                  // R1 = R1 + 1
        I_ANDI(R0, R1, 0x03),                                               // R0 = R1 & 0x03
        M_BL(22, 1),                                                        // IF R0 < 1 THEN GOTO M_LABEL(22)
        I_WR_REG(RTC_GPIO_OUT_REG, pin_in1_rtc_bit, pin_in1_rtc_bit, 0),    // PIN 1 --> LOW
        M_BX(23),                                                           // GOTO M_LABEL(23)
        M_LABEL(22),
        I_WR_REG(RTC_GPIO_OUT_REG, pin_in1_rtc_bit, pin_in1_rtc_bit, 1),    // PIN 1 --> HIGH

        M_LABEL(23),
        I_BXI(0)
    };

    rtc_gpio_init((gpio_num_t)pin_in1);
    rtc_gpio_set_direction((gpio_num_t)pin_in1, RTC_GPIO_MODE_OUTPUT_ONLY);
    rtc_gpio_set_level((gpio_num_t)pin_in1, 0);
    rtc_gpio_init((gpio_num_t)pin_in2);
    rtc_gpio_set_direction((gpio_num_t)pin_in2, RTC_GPIO_MODE_OUTPUT_ONLY);
    rtc_gpio_set_level((gpio_num_t)pin_in2, 0);
    rtc_gpio_init((gpio_num_t)pin_in3);
    rtc_gpio_set_direction((gpio_num_t)pin_in3, RTC_GPIO_MODE_OUTPUT_ONLY);
    rtc_gpio_set_level((gpio_num_t)pin_in3, 0);
    rtc_gpio_init((gpio_num_t)pin_in4);
    rtc_gpio_set_direction((gpio_num_t)pin_in4, RTC_GPIO_MODE_OUTPUT_ONLY);
    rtc_gpio_set_level((gpio_num_t)pin_in4, 0);

    size_t size = sizeof(stepperUlpTask) / sizeof(ulp_insn_t);
    ulp_process_macros_and_load(0, stepperUlpTask, &size);
    memset(&(RTC_SLOW_MEM[RTC_MEM_VARIABLE_ADDRESS / sizeof(uint32_t)]), 0, 16);
    ulp_run(0);
}

StepperMotor::StepperMotor(uint8_t pin_in1, uint8_t pin_in2, uint8_t pin_in3, uint8_t pin_in4) {
    this->pin_in1 = pin_in1;
    this->pin_in2 = pin_in2;
    this->pin_in3 = pin_in3;
    this->pin_in4 = pin_in4;
    this->desired_direction = 0;

    pinMode(pin_in1, OUTPUT);
    pinMode(pin_in2, OUTPUT);
    pinMode(pin_in3, OUTPUT);
    pinMode(pin_in4, OUTPUT);

    startStepperTask();
}
