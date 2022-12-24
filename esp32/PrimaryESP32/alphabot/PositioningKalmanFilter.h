#ifndef POSITIONING_KALMAN_FILTER_H
#define POSITIONING_KALMAN_FILTER_H

// State Vector
// This tracks the pose states in a 9-element vector.
// The states are:
//
//    States                Units    Index
//    Position (XYZ)        m        0:2
//    Velocity (XYZ)        m/s      3:5
//    Acceleration (XYZ)    m/s^2    6:8
//
// Ground truth is used to help initialize the filter states, so the filter
// converges to good answers quickly.

union state {
    double State[9];
    struct {
        double position[3];
        double velocity[3];
        double acceleration[3];
    };
};

class PositioningKalmanFilter {
private:
    double StateCovariance[9 * 9];
    double PositionNoise[3];
    double VelocityNoise[3];
    double AccelerationNoise[3];

public:
    union state State;

    void setPositionNoise(const double val[3]);
    void setVelocityNoise(const double val[3]);
    void setAccelerationNoise(const double val[3]);
    void setStateCovariance(const double val[9 * 9]);
    void predict(double dt);
    void fusegps(const double rawPos[3], double posCov, const double vel[3], double velCov);
    void fusevel(const double vel[3], double velCov);
    void pose(double pos[3]) const;

};

#endif
