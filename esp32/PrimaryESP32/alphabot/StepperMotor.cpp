#include "StepperMotor.h"
#include "pinconfig.h"
#include "config.h"
#include <esp32/ulp.h>
#include <driver/rtc_io.h>

#define JUMP_TABLE_PIN_LOW_ADDRESS  0x400
#define JUMP_TABLE_PIN_HIGH_ADDRESS 0x500
#define RTC_MEM_ID_ADDRESS          0x0FF8
#define RTC_MEM_ID_COUNT_ADDRESS    0x0FFC
#define RTC_MEM_VARIABLE_ADDRESS    0x1000
#define IS_CALIBRATING_OFFSET       0
#define CALIBRATION_STEP_OFFSET     1
#define CURRENT_DIRECTION_OFFSET    2
#define DESIRED_DIRECTION_OFFSET    3
#define CALIBRATION_STEPS_OFFSET    4
#define PIN_IN1_SET_LOW_OFFSET      5
#define PIN_IN2_SET_LOW_OFFSET      6
#define PIN_IN3_SET_LOW_OFFSET      7
#define PIN_IN4_SET_LOW_OFFSET      8
#define PIN_IN1_SET_HIGH_OFFSET     9
#define PIN_IN2_SET_HIGH_OFFSET     10
#define PIN_IN3_SET_HIGH_OFFSET     11
#define PIN_IN4_SET_HIGH_OFFSET     12

static uint8_t id_counter = 0;
static bool ulp_task_running = false;

void StepperMotor::turnTo(int16_t dir) {
    uint16_t stepper_calibration_steps = RTC_SLOW_MEM[RTC_MEM_VARIABLE_ADDRESS / sizeof(uint32_t) + id * 16 + CALIBRATION_STEPS_OFFSET];
    desired_direction = constrain(dir, -stepper_calibration_steps / 2, stepper_calibration_steps / 2);
    RTC_SLOW_MEM[RTC_MEM_VARIABLE_ADDRESS / sizeof(uint32_t) + id * 16 + DESIRED_DIRECTION_OFFSET] = desired_direction;
}

void StepperMotor::calibrate() {
    RTC_SLOW_MEM[RTC_MEM_VARIABLE_ADDRESS / sizeof(uint32_t) + id * 16 + IS_CALIBRATING_OFFSET] = true;
    RTC_SLOW_MEM[RTC_MEM_VARIABLE_ADDRESS / sizeof(uint32_t) + id * 16 + CALIBRATION_STEP_OFFSET] = 0;
    #ifdef DEBUG
    Serial.println("Calibrate");
    #endif
}

// 4.11 RTC_MUX Pin List https://www.espressif.com/sites/default/files/documentation/esp32_technical_reference_manual_en.pdf
static const uint8_t gpio_to_rtc_gpio_conversion[] = {
    11,   0xFF,   12, 0xFF,   10, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF,
    0xFF, 0xFF,   15,   14,   16,   13, 0xFF, 0xFF, 0xFF, 0xFF,
    0xFF, 0xFF, 0xFF, 0xFF, 0xFF,    6,    7,   17, 0xFF, 0xFF,
    0xFF, 0xFF,    9,    8,    4,    5,    0,    1,    2,    3
};

void StepperMotor::startStepperTask() {
    const ulp_insn_t stepperUlpTask[] = {
        I_MOVI(R3, RTC_MEM_ID_ADDRESS / sizeof(uint32_t)),
        I_LD(R0, R3, 0),                                                    // R0 = id
        I_LD(R1, R3, 1),                                                    // R1 = id_count

        // if (id > id_count)
        I_SUBR(R1, R1, R0),                                                 // R1 = R1 - R0     (id_count - id)
        M_BXZ(26),                                                          // IF R1 == 0 THEN GOTO M_LABEL(26)
        M_BX(27),                                                           // GOTO M_LABEL(27)

        M_LABEL(26),
        I_MOVI(R0, 0),                                                      // R0 = 0
        I_DELAY(37500),

        M_LABEL(27),
        I_MOVR(R2, R0),                                                     // R2 = R0
        I_ADDI(R0, R0, 1),                                                  // R0 = R0 + 1      (id++)
        I_ST(R0, R3, 0),                                                    // RTC_SLOW_MEM[R3 + 0] = R0
        // R3 = RTC_MEM_VARIABLE_ADDRESS / sizeof(uint32_t) + id * 16
        I_LSHI(R3, R2, 4),                                                  // R3 = R2 << 4
        I_ADDI(R3, R3, RTC_MEM_VARIABLE_ADDRESS / sizeof(uint32_t)),        // R3 = R3 + 0x1000 / 4

        // if (isCalibrating) {
        I_LD(R0, R3, IS_CALIBRATING_OFFSET),                                // R0 = RTC_SLOW_MEM[R3 + 0]
        M_BL(1, 1),                                                         // IF R0 < 1 THEN GOTO M_LABEL(1)

        I_LD(R1, R3, CALIBRATION_STEP_OFFSET),                              // R1 = RTC_SLOW_MEM[R3 + 4]
        I_LD(R2, R3, CALIBRATION_STEPS_OFFSET),                             // R2 = RTC_SLOW_MEM[R3 + 16]

        // if (calibration_steps > calibration_step)
        I_SUBR(R2, R1, R2),                                                 // R2 = R1 - R2     (calibration_step - calibration_steps)
        M_BXF(2),                                                           // IF OVERFLOW THEN GOTO M_LABEL(2)

        I_LD(R2, R3, CALIBRATION_STEPS_OFFSET),                             // R2 = RTC_SLOW_MEM[R3 + 16]
        I_RSHI(R0, R2, 1),                                                  // R0 = R2 >> 1
        I_ADDR(R2, R0, R2),                                                 // R2 = R0 + R2

        // else if ((calibration_steps + calibration_steps / 2) > calibration_step)
        I_SUBR(R2, R1, R2),                                                 // R2 = R1 - R2     (calibration_step - (calibration_steps + calibration_steps / 2))
        M_BXF(3),                                                           // IF OVERFLOW THEN GOTO M_LABEL(3)

        // Calibrated
        // current_direction = 0;
        I_MOVI(R0, 0),                                                      // R0 = 0
        I_ST(R0, R3, CURRENT_DIRECTION_OFFSET),                             // RTC_SLOW_MEM[R3 + 8] = R0
        // isCalibrating = false;
        I_ST(R0, R3, IS_CALIBRATING_OFFSET),                                // RTC_SLOW_MEM[R3 + 0] = R0
        M_LABEL(13),
        // digitalWrite(pin_in4, 0);
        I_LD(R0, R3, PIN_IN4_SET_LOW_OFFSET),                               // R0 = RTC_SLOW_MEM[R3 + 32]
        M_MOVL(R2, 23),                                                     // R2 = M_LABEL(23) (return address)
        I_BXR(R0),                                                          // GOTO R0
        // digitalWrite(pin_in3, 0);
        M_LABEL(23),
        I_LD(R0, R3, PIN_IN3_SET_LOW_OFFSET),                               // R0 = RTC_SLOW_MEM[R3 + 28]
        M_MOVL(R2, 24),                                                     // R2 = M_LABEL(24) (return address)
        I_BXR(R0),                                                          // GOTO R0
        // digitalWrite(pin_in2, 0);
        M_LABEL(24),
        I_LD(R0, R3, PIN_IN2_SET_LOW_OFFSET),                               // R0 = RTC_SLOW_MEM[R3 + 24]
        M_MOVL(R2, 25),                                                     // R2 = M_LABEL(25) (return address)
        I_BXR(R0),                                                          // GOTO R0
        // digitalWrite(pin_in1, 0);
        M_LABEL(25),
        I_LD(R0, R3, PIN_IN1_SET_LOW_OFFSET),                               // R0 = RTC_SLOW_MEM[R3 + 20]
        I_MOVI(R2, 0),                                                      // R2 = 0 (return address)
        I_BXR(R0),                                                          // GOTO R0
        // continue;

        M_LABEL(2),
        // current_direction--;
        I_LD(R1, R3, CURRENT_DIRECTION_OFFSET),                             // R1 = RTC_SLOW_MEM[R3 + 8]
        I_SUBI(R1, R1, 1),                                                  // R1 = R1 - 1
        I_ST(R1, R3, CURRENT_DIRECTION_OFFSET),                             // RTC_SLOW_MEM[R3 + 8] = R1
        // calibration_step++;
        M_LABEL(12),
        I_LD(R0, R3, CALIBRATION_STEP_OFFSET),                              // R0 = RTC_SLOW_MEM[R3 + 4]
        I_ADDI(R0, R0, 1),                                                  // R0 = R0 + 1
        I_ST(R0, R3, CALIBRATION_STEP_OFFSET),                              // RTC_SLOW_MEM[R3 + 4] = R0
        // digitalWrite(pin_in4, (current_direction % 4) == 0);
        M_LABEL(11),
        I_ANDI(R0, R1, 0x03),                                               // R0 = R1 & 0x03
        M_BL(4, 1),                                                         // IF R0 < 1 THEN GOTO M_LABEL(4)
        I_LD(R0, R3, PIN_IN4_SET_LOW_OFFSET),                               // R0 = RTC_SLOW_MEM[R3 + 32]
        M_MOVL(R2, 5),                                                      // R2 = M_LABEL(5) (return address)
        I_BXR(R0),                                                          // GOTO R0
        M_LABEL(4),
        I_LD(R0, R3, PIN_IN4_SET_HIGH_OFFSET),                              // R0 = RTC_SLOW_MEM[R3 + 48]
        M_MOVL(R2, 5),                                                      // R2 = M_LABEL(5) (return address)
        I_BXR(R0),                                                          // GOTO R0
        // digitalWrite(pin_in3, ((current_direction + 1) % 4) == 0);
        M_LABEL(5),
        I_ADDI(R1, R1, 1),                                                  // R1 = R1 + 1
        I_ANDI(R0, R1, 0x03),                                               // R0 = R1 & 0x03
        M_BL(6, 1),                                                         // IF R0 < 1 THEN GOTO M_LABEL(6)
        I_LD(R0, R3, PIN_IN3_SET_LOW_OFFSET),                               // R0 = RTC_SLOW_MEM[R3 + 28]
        M_MOVL(R2, 7),                                                      // R2 = M_LABEL(7) (return address)
        I_BXR(R0),                                                          // GOTO R0
        M_LABEL(6),
        I_LD(R0, R3, PIN_IN3_SET_HIGH_OFFSET),                              // R0 = RTC_SLOW_MEM[R3 + 44]
        M_MOVL(R2, 7),                                                      // R2 = M_LABEL(7) (return address)
        I_BXR(R0),                                                          // GOTO R0
        // digitalWrite(pin_in2, ((current_direction + 2) % 4) == 0);
        M_LABEL(7),
        I_ADDI(R1, R1, 1),                                                  // R1 = R1 + 1
        I_ANDI(R0, R1, 0x03),                                               // R0 = R1 & 0x03
        M_BL(8, 1),                                                         // IF R0 < 1 THEN GOTO M_LABEL(8)
        I_LD(R0, R3, PIN_IN2_SET_LOW_OFFSET),                               // R0 = RTC_SLOW_MEM[R3 + 24]
        M_MOVL(R2, 9),                                                      // R2 = M_LABEL(9) (return address)
        I_BXR(R0),                                                          // GOTO R0
        M_LABEL(8),
        I_LD(R0, R3, PIN_IN2_SET_HIGH_OFFSET),                              // R0 = RTC_SLOW_MEM[R3 + 40]
        M_MOVL(R2, 9),                                                      // R2 = M_LABEL(9) (return address)
        I_BXR(R0),                                                          // GOTO R0
        // digitalWrite(pin_in1, ((current_direction + 3) % 4) == 0);
        M_LABEL(9),
        I_ADDI(R1, R1, 1),                                                  // R1 = R1 + 1
        I_ANDI(R0, R1, 0x03),                                               // R0 = R1 & 0x03
        M_BL(10, 1),                                                        // IF R0 < 1 THEN GOTO M_LABEL(10)
        I_LD(R0, R3, PIN_IN1_SET_LOW_OFFSET),                               // R0 = RTC_SLOW_MEM[R3 + 20]
        I_MOVI(R2, 0),                                                      // R2 = 0 (return address)
        I_BXR(R0),                                                          // GOTO R0
        M_LABEL(10),
        I_LD(R0, R3, PIN_IN1_SET_HIGH_OFFSET),                              // R0 = RTC_SLOW_MEM[R3 + 36]
        I_MOVI(R2, 0),                                                      // R2 = 0 (return address)
        I_BXR(R0),                                                          // GOTO R0

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
        I_ADDI(R1, R1, 1),                                                  // R1 = R1 + 1
        M_BX(15),                                                           // GOTO M_LABEL(15)
        // else if (desired_direction < current_direction)
        M_LABEL(14),
        //     current_direction--;
        I_SUBI(R1, R1, 1),                                                  // R1 = R1 - 1
        M_LABEL(15),
        I_ST(R1, R3, CURRENT_DIRECTION_OFFSET),                             // RTC_SLOW_MEM[R3 + 8] = R1
        M_BX(11)                                                            // GOTO M_LABEL(11)
    };

    size_t size = sizeof(stepperUlpTask) / sizeof(ulp_insn_t);
    ulp_process_macros_and_load(0, stepperUlpTask, &size);

    // Create jump table for setting pins low
    for (uint8_t i = 14; i < 32; ++i) {
        RTC_SLOW_MEM[JUMP_TABLE_PIN_LOW_ADDRESS / sizeof(uint32_t) + i * 2] = 0x10000100 | (i << 18) | (i << 23); // PIN i --> LOW
        RTC_SLOW_MEM[JUMP_TABLE_PIN_LOW_ADDRESS / sizeof(uint32_t) + i * 2 + 1] = 0x80200002; // GOTO R2
    }

    // Create jump table for setting pins high
    for (uint8_t i = 14; i < 32; ++i) {
        RTC_SLOW_MEM[JUMP_TABLE_PIN_HIGH_ADDRESS / sizeof(uint32_t) + i * 2] = 0x10000500 | (i << 18) | (i << 23); // PIN i --> HIGH
        RTC_SLOW_MEM[JUMP_TABLE_PIN_HIGH_ADDRESS / sizeof(uint32_t) + i * 2 + 1] = 0x80200002; // GOTO R2
    }

    RTC_SLOW_MEM[RTC_MEM_ID_ADDRESS / sizeof(uint32_t)] = 0;
    ulp_run(0);
}

StepperMotor::StepperMotor(uint8_t pin_in1, uint8_t pin_in2, uint8_t pin_in3, uint8_t pin_in4, uint16_t calibration_steps) {
    this->id = id_counter++;
    this->pin_in1 = pin_in1;
    this->pin_in2 = pin_in2;
    this->pin_in3 = pin_in3;
    this->pin_in4 = pin_in4;
    this->desired_direction = 0;

    pinMode(pin_in1, OUTPUT);
    pinMode(pin_in2, OUTPUT);
    pinMode(pin_in3, OUTPUT);
    pinMode(pin_in4, OUTPUT);

    uint8_t pin_in1_rtc_bit = gpio_to_rtc_gpio_conversion[pin_in1] + 14;
    uint8_t pin_in2_rtc_bit = gpio_to_rtc_gpio_conversion[pin_in2] + 14;
    uint8_t pin_in3_rtc_bit = gpio_to_rtc_gpio_conversion[pin_in3] + 14;
    uint8_t pin_in4_rtc_bit = gpio_to_rtc_gpio_conversion[pin_in4] + 14;

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

    memset(&(RTC_SLOW_MEM[RTC_MEM_VARIABLE_ADDRESS / sizeof(uint32_t) + id * 16]), 0, 16);
    RTC_SLOW_MEM[RTC_MEM_VARIABLE_ADDRESS / sizeof(uint32_t) + id * 16 + CALIBRATION_STEPS_OFFSET] = calibration_steps;
    RTC_SLOW_MEM[RTC_MEM_VARIABLE_ADDRESS / sizeof(uint32_t) + id * 16 + PIN_IN1_SET_LOW_OFFSET] = JUMP_TABLE_PIN_LOW_ADDRESS / 4 + pin_in1_rtc_bit * 2;
    RTC_SLOW_MEM[RTC_MEM_VARIABLE_ADDRESS / sizeof(uint32_t) + id * 16 + PIN_IN2_SET_LOW_OFFSET] = JUMP_TABLE_PIN_LOW_ADDRESS / 4 + pin_in2_rtc_bit * 2;
    RTC_SLOW_MEM[RTC_MEM_VARIABLE_ADDRESS / sizeof(uint32_t) + id * 16 + PIN_IN3_SET_LOW_OFFSET] = JUMP_TABLE_PIN_LOW_ADDRESS / 4 + pin_in3_rtc_bit * 2;
    RTC_SLOW_MEM[RTC_MEM_VARIABLE_ADDRESS / sizeof(uint32_t) + id * 16 + PIN_IN4_SET_LOW_OFFSET] = JUMP_TABLE_PIN_LOW_ADDRESS / 4 + pin_in4_rtc_bit * 2;
    RTC_SLOW_MEM[RTC_MEM_VARIABLE_ADDRESS / sizeof(uint32_t) + id * 16 + PIN_IN1_SET_HIGH_OFFSET] = JUMP_TABLE_PIN_HIGH_ADDRESS / 4 + pin_in1_rtc_bit * 2;
    RTC_SLOW_MEM[RTC_MEM_VARIABLE_ADDRESS / sizeof(uint32_t) + id * 16 + PIN_IN2_SET_HIGH_OFFSET] = JUMP_TABLE_PIN_HIGH_ADDRESS / 4 + pin_in2_rtc_bit * 2;
    RTC_SLOW_MEM[RTC_MEM_VARIABLE_ADDRESS / sizeof(uint32_t) + id * 16 + PIN_IN3_SET_HIGH_OFFSET] = JUMP_TABLE_PIN_HIGH_ADDRESS / 4 + pin_in3_rtc_bit * 2;
    RTC_SLOW_MEM[RTC_MEM_VARIABLE_ADDRESS / sizeof(uint32_t) + id * 16 + PIN_IN4_SET_HIGH_OFFSET] = JUMP_TABLE_PIN_HIGH_ADDRESS / 4 + pin_in4_rtc_bit * 2;
    RTC_SLOW_MEM[RTC_MEM_ID_COUNT_ADDRESS / sizeof(uint32_t)] = id_counter;

    if (!ulp_task_running) {
        ulp_task_running = true;
        startStepperTask();
    }
}
