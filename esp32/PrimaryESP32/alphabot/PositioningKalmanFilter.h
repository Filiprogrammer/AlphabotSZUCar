#ifndef POSITIONING_KALMAN_FILTER_H
#define POSITIONING_KALMAN_FILTER_H

#include "quaternion.h"

// State Vector
// This tracks the pose states in a 28-element vector.
// The states are:
//
//    States                            Units    Index
//    Orientation (quaternion parts)             1:4
//    Angular Velocity (XYZ)            rad/s    5:7
//    Position (XYZ)                    m        8:10
//    Velocity (XYZ)                    m/s      11:13
//    Acceleration (XYZ)                m/s^2    14:16
//    Accelerometer Bias (XYZ)          m/s^2    17:19
//    Gyroscope Bias (XYZ)              rad/s    20:22
//    Geomagnetic Field Vector (NED)    uT       23:25
//    Magnetometer Bias (XYZ)           uT       26:28
//
// Ground truth is used to help initialize the filter states, so the filter
// converges to good answers quickly.

union state {
    double State[28];
    struct {
        quaternion orientation;
        double angularVelocity[3];
        double position[3];
        double velocity[3];
        double acceleration[3];
        double accelerometerBias[3];
        double gyroscopeBias[3];
        double geomagneticFieldVector[3];
        double magnetometerBias[3];
    };
};

class PositioningKalmanFilter {
public:
    void set_PositionNoise(const double val[3]);
    void set_VelocityNoise(const double val[3]);
    void set_GeomagneticVectorNoise(const double val[3]);
    void set_QuaternionNoise(const double val[4]);
    void set_AngularVelocityNoise(const double val[3]);
    void set_AccelerationNoise(const double val[3]);
    void set_MagnetometerBiasNoise(const double val[3]);
    void set_AccelerometerBiasNoise(const double val[3]);
    void set_GyroscopeBiasNoise(const double val[3]);
    void set_StateCovariance(const double val[784]);
    void predict(double dt);
    void fuseaccel(const double accel[3], double accelCov);
    void fusegyro(const double gyro[3], double gyroCov);
    void fusemag(const double mag[3], double magCov);
    void fusegps(const double rawPos[3], double posCov, const double vel[3], double velCov);
    void fusevel(const double vel[3], double velCov);
    void pose(double pos[3], quaternion* orient) const;
    void repairQuaternion(quaternion* x) const;

public:
    double StateCovariance[784];
    double QuaternionNoise[4];
    double AngularVelocityNoise[3];
    double PositionNoise[3];
    double VelocityNoise[3];
    double AccelerationNoise[3];
    double GyroscopeBiasNoise[3];
    double AccelerometerBiasNoise[3];
    double GeomagneticVectorNoise[3];
    double MagnetometerBiasNoise[3];
    union state State;
};

#endif
