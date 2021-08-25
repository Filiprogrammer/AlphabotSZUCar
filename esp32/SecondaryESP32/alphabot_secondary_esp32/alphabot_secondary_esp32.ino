#include <NewPing.h>
#include <Wire.h>
#include <WiFi.h>
#include <driver/i2c.h>
#include <esp_wifi.h>
#include <esp_bt.h>
#include <esp32-hal-cpu.h>
#include <driver/adc.h>
#include <soc/soc.h>
#include <soc/rtc_cntl_reg.h>

//#define DEBUG

// Distance sensor front - values
#define FRONT_TRIG 2
#define FRONT_ECHO 4

// Distance sensor left - values
#define LEFT_TRIG 27
#define LEFT_ECHO 26

// Distance sensor right - values
#define RIGHT_TRIG 19
#define RIGHT_ECHO 18

// Distance sensor back - values
#define BACK_TRIG 14
#define BACK_ECHO 12

#define MAX_DIST 400
#define I2C_SLAVE_SDA_IO 21
#define I2C_SLAVE_SCL_IO 22
#define ESP_SLAVE_ADDR 5
#define I2C_SLAVE_NUM I2C_NUM_0

int front_dist = 0;
int left_dist = 0;
int right_dist = 0;
int back_dist = 0;

NewPing front_sensor(FRONT_TRIG, FRONT_ECHO, MAX_DIST);
NewPing left_sensor(LEFT_TRIG, LEFT_ECHO, MAX_DIST);
NewPing right_sensor(RIGHT_TRIG, RIGHT_ECHO, MAX_DIST);
NewPing back_sensor(BACK_TRIG, BACK_ECHO, MAX_DIST);

i2c_port_t i2c_slave_port = I2C_SLAVE_NUM;

uint8_t i2c_in_buffer[512];
uint8_t i2c_out_buffer[512];

void setup() {
    REG_CLR_BIT(RTC_CNTL_BROWN_OUT_REG, RTC_CNTL_BROWN_OUT_ENA);
    WiFi.mode(WIFI_OFF);
    btStop();
    adc_power_off();
    //esp_wifi_stop();
    esp_bt_controller_disable();
    setCpuFrequencyMhz(10);
    #ifdef DEBUG
    Serial.begin(115200);
    Serial.println("Hello");
    #endif
    i2c_slave_init();
}

void loop() {
    measure_distance(&front_sensor, &front_dist);
    delay(5);
    measure_distance(&left_sensor, &left_dist);
    delay(5);
    measure_distance(&right_sensor, &right_dist);
    delay(5);
    measure_distance(&back_sensor, &back_dist);

    #ifdef DEBUG
    Serial.print("Front: ");
    Serial.print(front_dist);
    Serial.print("\tLeft: ");
    Serial.print(left_dist);
    Serial.print("\tRight: ");
    Serial.print(right_dist);
    Serial.print("\tBack: ");
    Serial.println(back_dist);
    #endif

    for(uint8_t i = 0; i < 2; ++i) {
        handle_i2c_communication();
        delay(20);
    }
    //esp_sleep_enable_timer_wakeup(200000);
    //esp_light_sleep_start();
}

void measure_distance(NewPing* sensor, int* dist) {
    unsigned int duration = sensor->ping_median(5);
    int new_dist;

    if(duration == 0)
        new_dist = MAX_DIST;
    else
        new_dist = (duration / 2) / 29.1; // Distance in cm

    (*dist) = ((*dist) + new_dist*3) / 4;
}

void handle_i2c_communication() {
    int size = i2c_slave_read_buffer(i2c_slave_port, i2c_in_buffer, 1, 50 / portTICK_PERIOD_MS);

    if(size != 0 && i2c_in_buffer[0] == 0x52) {
        i2c_out_buffer[0] = ESP_SLAVE_ADDR;
        i2c_out_buffer[1] = (uint8_t) front_dist;
        i2c_out_buffer[2] = (uint8_t) (front_dist >> 8);
        i2c_out_buffer[3] = (uint8_t) left_dist;
        i2c_out_buffer[4] = (uint8_t) (left_dist >> 8);
        i2c_out_buffer[5] = (uint8_t) right_dist;
        i2c_out_buffer[6] = (uint8_t) (right_dist >> 8);
        i2c_out_buffer[7] = (uint8_t) back_dist;
        i2c_out_buffer[8] = (uint8_t) (back_dist >> 8);
        i2c_slave_write_buffer(i2c_slave_port, i2c_out_buffer, 9, 50 / portTICK_PERIOD_MS);
    }
}

esp_err_t i2c_slave_init(void) {
    i2c_config_t conf_slave;
    conf_slave.sda_io_num = (gpio_num_t) I2C_SLAVE_SDA_IO;
    conf_slave.sda_pullup_en = GPIO_PULLUP_ENABLE;
    conf_slave.scl_io_num = (gpio_num_t) I2C_SLAVE_SCL_IO;
    conf_slave.scl_pullup_en = GPIO_PULLUP_ENABLE;
    conf_slave.mode = I2C_MODE_SLAVE;
    conf_slave.slave.addr_10bit_en = 0;
    conf_slave.slave.slave_addr = ESP_SLAVE_ADDR;
    i2c_param_config(i2c_slave_port, &conf_slave);
    return i2c_driver_install(i2c_slave_port, conf_slave.mode, 512, 512, 0);
}
