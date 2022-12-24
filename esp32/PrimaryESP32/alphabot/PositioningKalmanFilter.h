#ifndef POSITIONING_KALMAN_FILTER_H
#define POSITIONING_KALMAN_FILTER_H

// State Vector
// This tracks the pose states in a 9-element vector.
// The states are:
//
//    States                Units    Index
//    Position (XY)         m        0:1
//    Velocity (XY)         m/s      2:3
//    Acceleration (XY)     m/s^2    4:5
//
// Ground truth is used to help initialize the filter states, so the filter
// converges to good answers quickly.

union state {
    double State[6];
    struct {
        double position[2];
        double velocity[2];
        double acceleration[2];
    };
};

class PositioningKalmanFilter {
private:
    double StateCovariance[6 * 6];
    double PositionNoise[2];
    double VelocityNoise[2];
    double AccelerationNoise[2];

public:
    union state State;

    void setPositionNoise(const double val[2]);
    void setVelocityNoise(const double val[2]);
    void setAccelerationNoise(const double val[2]);
    void setStateCovariance(const double val[6 * 6]);
    void predict(double dt);
    void fusegps(const double rawPos[2], double posCov, const double vel[2], double velCov);
    void fusevel(const double vel[2], double velCov);
    void pose(double pos[2]) const;

    PositioningKalmanFilter();

};

#endif
