#include "MotionTracker.h"

bool MotionTracker::isInitialized() const {
    return initialized;
}

void MotionTracker::fetchOrientationData() {
    if (initialized && icm20948.dataReady())
        icm20948.getAGMT();


    icm_20948_DMP_data_t data;
    icm20948.readDMPdataFromFIFO(&data);

    if ((icm20948.status == ICM_20948_Stat_Ok) || (icm20948.status == ICM_20948_Stat_FIFOMoreDataAvail)) { // Was valid data available?
        if ((data.header & DMP_header_bitmap_Accel) > 0) {
            /*Serial.print("Linear_Accel: X: ");
            Serial.print(data.Raw_Accel.Data.X);
            Serial.print("\tY: ");
            Serial.print(data.Raw_Accel.Data.Y);
            Serial.print("\tZ: ");
            Serial.println(data.Raw_Accel.Data.Z);*/
        }

        if ((data.header & DMP_header_bitmap_Quat9) > 0) { // We have asked for orientation data so we should receive Quat9
            // Q0 value is computed from this equation: Q0^2 + Q1^2 + Q2^2 + Q3^2 = 1.
            // In case of drift, the sum will not add to 1, therefore, quaternion data need to be corrected with right bias values.
            // The quaternion data is scaled by 2^30.

            // Scale to +/- 1
            quat_x = ((double)data.Quat9.Data.Q1) / 1073741824.0; // Convert to double. Divide by 2^30
            quat_y = ((double)data.Quat9.Data.Q2) / 1073741824.0; // Convert to double. Divide by 2^30
            quat_z = ((double)data.Quat9.Data.Q3) / 1073741824.0; // Convert to double. Divide by 2^30
            quat_w = sqrt(1.0 - ((quat_x * quat_x) + (quat_y * quat_y) + (quat_z * quat_z)));
            accuracy = data.Quat9.Data.Accuracy;
            /*icm_20948_DMP_Activity_t activity_state_start = data.Activity_Recognition.Data.State_Start;
            icm_20948_DMP_Activity_t activity_state_end = data.Activity_Recognition.Data.State_End;
            uint32_t activity_timestamp = data.Activity_Recognition.Data.Timestamp;
            Serial.print("activity_state_start:");

            if (activity_state_start.Bike)
                Serial.print(" Bike");

            if (activity_state_start.Drive)
                Serial.print(" Drive");

            if (activity_state_start.Run)
                Serial.print(" Run");

            if (activity_state_start.Still)
                Serial.print(" Still");

            if (activity_state_start.Tilt)
                Serial.print(" Tilt");

            if (activity_state_start.Walk)
                Serial.print(" Walk");

            Serial.print("\tactivity_state_end:");

            if (activity_state_end.Bike)
                Serial.print(" Bike");

            if (activity_state_end.Drive)
                Serial.print(" Drive");

            if (activity_state_end.Run)
                Serial.print(" Run");

            if (activity_state_end.Still)
                Serial.print(" Still");

            if (activity_state_end.Tilt)
                Serial.print(" Tilt");

            if (activity_state_end.Walk)
                Serial.print(" Walk");

            Serial.print("\tactivity_timestamp: ");
            Serial.println(activity_timestamp);*/
        }
    }
}

void MotionTracker::fetchRawData() {
    if (initialized && icm20948.dataReady())
        icm20948.getAGMT();
}

float MotionTracker::getAccX() {
    return icm20948.accX();
}

float MotionTracker::getAccY() {
    return icm20948.accY();
}

float MotionTracker::getAccZ() {
    return icm20948.accZ();
}

float MotionTracker::getGyrX() {
    return icm20948.gyrX();
}

float MotionTracker::getGyrY() {
    return icm20948.gyrY();
}

float MotionTracker::getGyrZ() {
    return icm20948.gyrZ();
}

float MotionTracker::getMagX() {
    return icm20948.magX();
}

float MotionTracker::getMagY() {
    return icm20948.magY();
}

float MotionTracker::getMagZ() {
    return icm20948.magZ();
}

float MotionTracker::getTemp() {
    return icm20948.temp();
}

double MotionTracker::getQuatW() const {
    return quat_w;
}

double MotionTracker::getQuatX() const {
    return quat_x;
}

double MotionTracker::getQuatY() const {
    return quat_y;
}

double MotionTracker::getQuatZ() const {
    return quat_z;
}

int16_t MotionTracker::getAccuracy() const {
    return accuracy;
}

double MotionTracker::getRoll() const {
    double q2sqr = quat_y * quat_y;

    // roll (x-axis rotation)
    double t0 = +2.0 * (quat_w * quat_x + quat_y * quat_z);
    double t1 = +1.0 - 2.0 * (quat_x * quat_x + q2sqr);
    double roll = atan2(t0, t1) * 180.0 / PI;

    return roll;
}

double MotionTracker::getPitch() const {
    // pitch (y-axis rotation)
    double t2 = +2.0 * (quat_w * quat_y - quat_z * quat_x);
    t2 = t2 > 1.0 ? 1.0 : t2;
    t2 = t2 < -1.0 ? -1.0 : t2;
    double pitch = asin(t2) * 180.0 / PI;

    return pitch;
}

double MotionTracker::getYaw() const {
    double q2sqr = quat_y * quat_y;

    // yaw (z-axis rotation)
    double t3 = +2.0 * (quat_w * quat_z + quat_x * quat_y);
    double t4 = +1.0 - 2.0 * (q2sqr + quat_z * quat_z);
    double yaw = atan2(t3, t4) * 180.0 / PI;

    return yaw;
}

MotionTracker::MotionTracker() {
    ICM_20948_Status_e status = icm20948.begin(Wire, 0);

    if (status != ICM_20948_Stat_Ok) {
        initialized = false;
        return;
    }

    initialized = true;

    Serial.println("A");

    // Initialize the DMP. initializeDMP is a weak function. You can overwrite it if you want to e.g. to change the sample rate
    initialized &= (icm20948.initializeDMP() == ICM_20948_Stat_Ok);

    Serial.print("B");
    Serial.println(initialized);

    // Enable the DMP orientation sensor
    //initialized &= (icm20948.enableDMPSensor(INV_ICM20948_SENSOR_ORIENTATION) == ICM_20948_Stat_Ok);

    //Serial.print("C");
    //Serial.println(initialized);

    initialized &= (icm20948.enableDMPSensor(INV_ICM20948_SENSOR_LINEAR_ACCELERATION) == ICM_20948_Stat_Ok);
    initialized &= (icm20948.setDMPODRrate(DMP_ODR_Reg_Accel, 10) == ICM_20948_Stat_Ok);

    //initialized &= (icm20948.enableDMPSensor(INV_ICM20948_SENSOR_ACTIVITY_CLASSIFICATON) == ICM_20948_Stat_Ok);

    Serial.print("D");
    Serial.println(initialized);

    // Configuring DMP to output data at multiple ODRs:
    // DMP is capable of outputting multiple sensor data at different rates to FIFO.
    // Setting value can be calculated as follows:
    // Value = (DMP running rate / ODR ) - 1
    // E.g. For a 5Hz ODR rate when DMP is running at 55Hz, value = (55/5) - 1 = 10.
    initialized &= (icm20948.setDMPODRrate(DMP_ODR_Reg_Quat9, 10) == ICM_20948_Stat_Ok); // Set to the maximum

    Serial.print("E");
    Serial.println(initialized);

    // Enable the FIFO
    initialized &= (icm20948.enableFIFO() == ICM_20948_Stat_Ok);

    Serial.print("F");
    Serial.println(initialized);

    // Enable the DMP
    initialized &= (icm20948.enableDMP() == ICM_20948_Stat_Ok);

    Serial.print("G");
    Serial.println(initialized);

    // Reset DMP
    initialized &= (icm20948.resetDMP() == ICM_20948_Stat_Ok);

    Serial.print("H");
    Serial.println(initialized);

    // Reset FIFO
    initialized &= (icm20948.resetFIFO() == ICM_20948_Stat_Ok);

    Serial.print("I");
    Serial.println(initialized);
}
