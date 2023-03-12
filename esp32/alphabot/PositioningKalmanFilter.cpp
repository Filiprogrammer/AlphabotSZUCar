#include "PositioningKalmanFilter.h"
#include <algorithm>
#include <cmath>
#include <cstring>

void PositioningKalmanFilter::fusegps(const double rawPos[2], double posCov, const double vel[2], double velCov) {
    double A[4 * 4];

    for (int i = 0; i < 4; i++)
        for (int j = 0; j < 4; j++)
            A[i + 4 * j] = StateCovariance[i + 6 * j];

    for (int i = 0; i < 2; i++)
        A[i + 4 * i] += posCov;

    for (int i = 2; i < 4; i++)
        A[i + 4 * i] += velCov;

    double W[6 * 4];

    for (int i = 0; i < 6; i++)
        for (int j = 0; j < 4; j++)
            W[i + 6 * j] = StateCovariance[i + 6 * j];

    signed char ipiv[4] = { 0, 1, 2, 3 };
    for (int i = 0; i < 3; i++) {
        int jBcol = 0;
        double smax = abs(A[i + 4 * i]);
        for (int j = 1; j < 4 - i; j++) {
            double s = abs(A[i + j + 4 * i]);
            if (s > smax) {
                jBcol = j;
                smax = s;
            }
        }
        if (A[i + jBcol + 4 * i] != 0.0) {
            if (jBcol != 0) {
                ipiv[i] = i + jBcol;

                for (int j = 0; j < 4; j++)
                    std::swap(A[i + j * 4], A[i + jBcol + j * 4]);
            }

            for (int j = i + 1; j < 4; j++)
                A[j + 4 * i] /= A[i + 4 * i];
        }
        for (int j = 1; j < 4 - i; j++) {
            smax = A[i + 4 * (i + j)];

            if (smax != 0.0) {
                for (int k = i + 1; k < 4; k++)
                    A[k + 4 * (i + j)] += A[k + 4 * i] * -smax;
            }
        }
    }
    for (int i = 0; i < 4; i++) {
        for (int j = 0; j < i; j++) {
            double smax = A[j + 4 * i];

            if (smax != 0.0)
                for (int k = 0; k < 6; k++)
                    W[k + 6 * i] -= smax * W[k + 6 * j];
        }

        double smax = 1.0 / A[i + 4 * i];

        for (int j = 0; j < 6; j++)
            W[j + 6 * i] *= smax;
    }
    for (int i = 3; i >= 0; i--) {
        for (int j = i + 1; j < 4; j++) {
            double smax = A[j + 4 * i];

            if (smax != 0.0)
                for (int k = 0; k < 6; k++)
                    W[k + 6 * i] -= smax * W[k + 6 * j];
        }
    }
    for (int i = 0; i < 3; i++) {
        signed char i1 = ipiv[i];

        if (i1 != i)
            for (int j = 0; j < 6; j++)
                std::swap(W[j + 6 * i], W[j + 6 * i1]);
    }
    double innov[4];
    innov[0] = rawPos[0] - State.position[0];
    innov[1] = rawPos[1] - State.position[1];
    innov[2] = vel[0] - State.velocity[0];
    innov[3] = vel[1] - State.velocity[1];
    double b_P[6 * 6];
    for (int i = 0; i < 6; i++) {
        double smax = 0.0;

        for (int j = 0; j < 4; j++)
            smax += W[i + 6 * j] * innov[j];

        State.State[i] += smax;

        for (int j = 0; j < 6; j++) {
            smax = 0.0;

            for (int k = 0; k < 4; k++)
                smax += W[i + 6 * k] * StateCovariance[k + 6 * j];

            int jA = i + 6 * j;
            b_P[jA] = StateCovariance[jA] - smax;
        }
    }
    setStateCovariance(b_P);
}

void PositioningKalmanFilter::fusevel(const double vel[2], double velCov) {
    double A[2 * 2];

    for (int i = 0; i < 2; i++)
        for (int j = 0; j < 2; j++)
            A[i + 2 * j] = StateCovariance[i + 2 + 6 * (j + 2)];

    for (int i = 0; i < 2; i++)
        A[i + 2 * i] += velCov;

    double W[6 * 2];

    for (int i = 0; i < 6; i++)
        for (int j = 0; j < 2; j++)
            W[i + 6 * j] = StateCovariance[i + 6 * (j + 2)];

    signed char ipiv[2] = { 0, 1 };
    for (int i = 0; i < 1; i++) {
        int jBcol = 0;
        double smax = abs(A[i + 2 * i]);
        for (int j = 1; j < 2 - i; j++) {
            double s = abs(A[i + j + 2 * i]);
            if (s > smax) {
                jBcol = j;
                smax = s;
            }
        }
        if (A[i + jBcol + 2 * i] != 0.0) {
            if (jBcol != 0) {
                ipiv[i] = i + jBcol;

                for (int j = 0; j < 2; j++)
                    std::swap(A[i + j * 2], A[i + jBcol + j * 2]);
            }

            for (int j = i + 1; j < 2; j++)
                A[j + 2 * i] /= A[i + 2 * i];
        }
        for (int j = 1; j < 2 - i; j++) {
            smax = A[i + 2 * (i + j)];

            if (smax != 0.0) {
                for (int k = i + 1; k < 2; k++)
                    A[k + 2 * (i + j)] += A[k + 2 * i] * -smax;
            }
        }
    }
    for (int i = 0; i < 2; i++) {
        for (int j = 0; j < i; j++) {
            double smax = A[j + 2 * i];

            if (smax != 0.0)
                for (int k = 0; k < 6; k++)
                    W[k + 6 * i] -= smax * W[k + 6 * j];
        }

        double smax = 1.0 / A[i + 2 * i];

        for (int j = 0; j < 6; j++)
            W[j + 6 * i] *= smax;
    }
    for (int i = 1; i >= 0; i--) {
        for (int j = i + 1; j < 2; j++) {
            double smax = A[j + 2 * i];

            if (smax != 0.0)
                for (int k = 0; k < 6; k++)
                    W[k + 6 * i] -= smax * W[k + 6 * j];
        }
    }
    for (int i = 0; i < 1; i++) {
        signed char i1 = ipiv[i];

        if (i1 != i)
            for (int j = 0; j < 6; j++)
                std::swap(W[j + 6 * i], W[j + 6 * i1]);
    }
    double innov[2];
    innov[0] = vel[0] - State.velocity[0];
    innov[1] = vel[1] - State.velocity[1];
    double b_P[6 * 6];
    for (int i = 0; i < 6; i++) {
        double smax = 0.0;

        for (int j = 0; j < 2; j++)
            smax += W[i + 6 * j] * innov[j];

        State.State[i] += smax;

        for (int j = 0; j < 6; j++) {
            smax = 0.0;

            for (int k = 2; k < 4; k++)
                smax += W[i + 6 * (k - 2)] * StateCovariance[k + 6 * j];

            int jA = i + 6 * j;
            b_P[jA] = StateCovariance[jA] - smax;
        }
    }
    setStateCovariance(b_P);
}

void PositioningKalmanFilter::pose(double pos[2]) const {
    pos[0] = State.position[0];
    pos[1] = State.position[1];
}

void PositioningKalmanFilter::predict(double dt) {
    double Pdot[6 * 6];

    for (int i = 0; i < 4; i++) {
        for (int i1 = 0; i1 < 6; i1++)
            Pdot[i + 6 * i1] = StateCovariance[i + 2 + 6 * i1];

        for (int i1 = 0; i1 < 4; i1++)
            Pdot[i + 6 * i1] += StateCovariance[i + 6 * (i1 + 2)];
    }

    for (int i = 4; i < 6; i++) {
        for (int i1 = 0; i1 < 4; i1++)
            Pdot[i + 6 * i1] = StateCovariance[i + 6 * (i1 + 2)];

        for (int i1 = 4; i1 < 6; i1++)
            Pdot[i + 6 * i1] = 0.0;
    }

    Pdot[0 + 6 * 0] += PositionNoise[0];
    Pdot[1 + 6 * 1] += PositionNoise[1];
    Pdot[2 + 6 * 2] += VelocityNoise[0];
    Pdot[3 + 6 * 3] += VelocityNoise[1];
    Pdot[4 + 6 * 4] += AccelerationNoise[0];
    Pdot[5 + 6 * 5] += AccelerationNoise[1];

    State.position[0] += State.velocity[0] * dt;
    State.position[1] += State.velocity[1] * dt;
    State.velocity[0] += State.acceleration[0] * dt;
    State.velocity[1] += State.acceleration[1] * dt;

    for (int i = 0; i < 6; i++) {
        for (int i1 = 0; i1 < 6; i1++) {
            StateCovariance[i1 + 6 * i] += 0.5 * (Pdot[i1 + 6 * i] + Pdot[i + 6 * i1]) * dt;
        }
    }
}

void PositioningKalmanFilter::setPositionNoise(const double val[2]) {
    PositionNoise[0] = val[0];
    PositionNoise[1] = val[1];
}

void PositioningKalmanFilter::setVelocityNoise(const double val[2]) {
    VelocityNoise[0] = val[0];
    VelocityNoise[1] = val[1];
}

void PositioningKalmanFilter::setAccelerationNoise(const double val[2]) {
    AccelerationNoise[0] = val[0];
    AccelerationNoise[1] = val[1];
}

void PositioningKalmanFilter::setStateCovariance(const double val[6 * 6]) {
    std::copy(&val[0], &val[6 * 6], &StateCovariance[0]);
}

PositioningKalmanFilter::PositioningKalmanFilter() {
    PositionNoise[0] = 0.0;
    PositionNoise[1] = 0.0;
    VelocityNoise[0] = 0.0;
    VelocityNoise[1] = 0.0;
    AccelerationNoise[0] = 0.0;
    AccelerationNoise[1] = 0.0;
    memset(StateCovariance, 0, sizeof(StateCovariance));
}
