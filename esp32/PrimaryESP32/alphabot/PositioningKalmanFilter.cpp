#include "PositioningKalmanFilter.h"
#include <algorithm>
#include <cmath>
#include <cstring>

void PositioningKalmanFilter::fusegps(const double rawPos[3], double posCov, const double vel[3], double velCov) {
    double A[6 * 6];

    for (int i = 0; i < 6; i++)
        for (int j = 0; j < 6; j++)
            A[i + 6 * j] = StateCovariance[i + 9 * j];

    for (int i = 0; i < 3; i++)
        A[i + 6 * i] += posCov;

    for (int i = 3; i < 6; i++)
        A[i + 6 * i] += velCov;

    double W[9 * 6];

    for (int i = 0; i < 9; i++)
        for (int j = 0; j < 6; j++)
            W[i + 9 * j] = StateCovariance[i + 9 * j];

    signed char ipiv[6] = { 0, 1, 2, 3, 4, 5 };
    for (int i = 0; i < 5; i++) {
        int jBcol = 0;
        double smax = abs(A[i + 6 * i]);
        for (int j = 1; j < 6 - i; j++) {
            double s = abs(A[i + j + 6 * i]);
            if (s > smax) {
                jBcol = j;
                smax = s;
            }
        }
        if (A[i + jBcol + 6 * i] != 0.0) {
            if (jBcol != 0) {
                ipiv[i] = i + jBcol;

                for (int j = 0; j < 6; j++)
                    std::swap(A[i + j * 6], A[i + jBcol + j * 6]);
            }

            for (int j = i + 1; j < 6; j++)
                A[j + 6 * i] /= A[i + 6 * i];
        }
        for (int j = 1; j < 6 - i; j++) {
            smax = A[i + 6 * (i + j)];

            if (smax != 0.0) {
                for (int k = i + 1; k < 6; k++)
                    A[k + 6 * (i + j)] += A[k + 6 * i] * -smax;
            }
        }
    }
    for (int i = 0; i < 6; i++) {
        for (int j = 0; j < i; j++) {
            double smax = A[j + 6 * i];

            if (smax != 0.0)
                for (int k = 0; k < 9; k++)
                    W[k + 9 * i] -= smax * W[k + 9 * j];
        }

        double smax = 1.0 / A[i + 6 * i];

        for (int j = 0; j < 9; j++)
            W[j + 9 * i] *= smax;
    }
    for (int i = 5; i >= 0; i--) {
        for (int j = i + 1; j < 6; j++) {
            double smax = A[j + 6 * i];

            if (smax != 0.0)
                for (int k = 0; k < 9; k++)
                    W[k + 9 * i] -= smax * W[k + 9 * j];
        }
    }
    for (int i = 0; i < 5; i++) {
        signed char i1 = ipiv[i];

        if (i1 != i)
            for (int j = 0; j < 9; j++)
                std::swap(W[j + 9 * i], W[j + 9 * i1]);
    }
    double innov[6];
    innov[0] = rawPos[0] - State.position[0];
    innov[1] = rawPos[1] - State.position[1];
    innov[2] = rawPos[2] - State.position[2];
    innov[3] = vel[0] - State.velocity[0];
    innov[4] = vel[1] - State.velocity[1];
    innov[5] = vel[2] - State.velocity[2];
    double b_P[9 * 9];
    for (int i = 0; i < 9; i++) {
        double smax = 0.0;

        for (int j = 0; j < 6; j++)
            smax += W[i + 9 * j] * innov[j];

        State.State[i] += smax;

        for (int j = 0; j < 9; j++) {
            smax = 0.0;

            for (int k = 0; k < 6; k++)
                smax += W[i + 9 * k] * StateCovariance[k + 9 * j];

            int jA = i + 9 * j;
            b_P[jA] = StateCovariance[jA] - smax;
        }
    }
    set_StateCovariance(b_P);
}

void PositioningKalmanFilter::fusevel(const double vel[3], double velCov) {
    double A[3 * 3];

    for (int i = 0; i < 3; i++)
        for (int j = 0; j < 3; j++)
            A[i + 3 * j] = StateCovariance[i + 3 + 9 * (j + 3)];

    for (int i = 0; i < 3; i++)
        A[i + 3 * i] += velCov;

    double W[9 * 3];

    for (int i = 0; i < 9; i++)
        for (int j = 0; j < 3; j++)
            W[i + 9 * j] = StateCovariance[i + 9 * (j + 3)];

    signed char ipiv[3] = { 0, 1, 2 };
    for (int i = 0; i < 2; i++) {
        int jBcol = 0;
        double smax = abs(A[i + 3 * i]);
        for (int j = 1; j < 3 - i; j++) {
            double s = abs(A[i + j + 3 * i]);
            if (s > smax) {
                jBcol = j;
                smax = s;
            }
        }
        if (A[i + jBcol + 3 * i] != 0.0) {
            if (jBcol != 0) {
                ipiv[i] = i + jBcol;

                for (int j = 0; j < 3; j++)
                    std::swap(A[i + j * 3], A[i + jBcol + j * 3]);
            }

            for (int j = i + 1; j < 3; j++)
                A[j + 3 * i] /= A[i + 3 * i];
        }
        for (int j = 1; j < 3 - i; j++) {
            smax = A[i + 3 * (i + j)];

            if (smax != 0.0) {
                for (int k = i + 1; k < 3; k++)
                    A[k + 3 * (i + j)] += A[k + 3 * i] * -smax;
            }
        }
    }
    for (int i = 0; i < 3; i++) {
        for (int j = 0; j < i; j++) {
            double smax = A[j + 3 * i];

            if (smax != 0.0)
                for (int k = 0; k < 9; k++)
                    W[k + 9 * i] -= smax * W[k + 9 * j];
        }

        double smax = 1.0 / A[i + 3 * i];

        for (int j = 0; j < 9; j++)
            W[j + 9 * i] *= smax;
    }
    for (int i = 2; i >= 0; i--) {
        for (int j = i + 1; j < 3; j++) {
            double smax = A[j + 3 * i];

            if (smax != 0.0)
                for (int k = 0; k < 9; k++)
                    W[k + 9 * i] -= smax * W[k + 9 * j];
        }
    }
    for (int i = 0; i < 2; i++) {
        signed char i1 = ipiv[i];

        if (i1 != i)
            for (int j = 0; j < 9; j++)
                std::swap(W[j + 9 * i], W[j + 9 * i1]);
    }
    double innov[3];
    innov[0] = vel[0] - State.velocity[0];
    innov[1] = vel[1] - State.velocity[1];
    innov[2] = vel[2] - State.velocity[2];
    double b_P[9 * 9];
    for (int i = 0; i < 9; i++) {
        double smax = 0.0;

        for (int j = 0; j < 3; j++)
            smax += W[i + 9 * j] * innov[j];

        State.State[i] += smax;

        for (int j = 0; j < 9; j++) {
            smax = 0.0;

            for (int k = 3; k < 6; k++)
                smax += W[i + 9 * (k - 3)] * StateCovariance[k + 9 * j];

            int jA = i + 9 * j;
            b_P[jA] = StateCovariance[jA] - smax;
        }
    }
    set_StateCovariance(b_P);
}

void PositioningKalmanFilter::pose(double pos[3]) const {
    pos[0] = State.position[0];
    pos[1] = State.position[1];
    pos[2] = State.position[2];
}

void PositioningKalmanFilter::predict(double dt) {
    double Pdot[9 * 9];

    for (int i = 0; i < 6; i++) {
        for (int i1 = 0; i1 < 9; i1++)
            Pdot[i + 9 * i1] = StateCovariance[i + 3 + 9 * i1];

        for (int i1 = 0; i1 < 6; i1++)
            Pdot[i + 9 * i1] += StateCovariance[i + 9 * (i1 + 3)];
    }

    for (int i = 6; i < 9; i++) {
        for (int i1 = 0; i1 < 6; i1++)
            Pdot[i + 9 * i1] = StateCovariance[i + 9 * (i1 + 3)];

        for (int i1 = 6; i1 < 9; i1++)
            Pdot[i + 9 * i1] = 0.0;
    }

    Pdot[0 + 9 * 0] += PositionNoise[0];
    Pdot[1 + 9 * 1] += PositionNoise[1];
    Pdot[2 + 9 * 2] += PositionNoise[2];
    Pdot[3 + 9 * 3] += VelocityNoise[0];
    Pdot[4 + 9 * 4] += VelocityNoise[1];
    Pdot[5 + 9 * 5] += VelocityNoise[2];
    Pdot[6 + 9 * 6] += AccelerationNoise[0];
    Pdot[7 + 9 * 7] += AccelerationNoise[1];
    Pdot[8 + 9 * 8] += AccelerationNoise[2];

    State.position[0] += State.velocity[0] * dt;
    State.position[1] += State.velocity[1] * dt;
    State.position[2] += State.velocity[2] * dt;
    State.velocity[0] += State.acceleration[0] * dt;
    State.velocity[1] += State.acceleration[1] * dt;
    State.velocity[2] += State.acceleration[2] * dt;

    for (int i = 0; i < 9; i++) {
        for (int i1 = 0; i1 < 9; i1++) {
            StateCovariance[i1 + 9 * i] += 0.5 * (Pdot[i1 + 9 * i] + Pdot[i + 9 * i1]) * dt;
        }
    }
}

void PositioningKalmanFilter::set_PositionNoise(const double val[3]) {
    PositionNoise[0] = val[0];
    PositionNoise[1] = val[1];
    PositionNoise[2] = val[2];
}

void PositioningKalmanFilter::set_VelocityNoise(const double val[3]) {
    VelocityNoise[0] = val[0];
    VelocityNoise[1] = val[1];
    VelocityNoise[2] = val[2];
}

void PositioningKalmanFilter::set_AccelerationNoise(const double val[3]) {
    // 100.0
    AccelerationNoise[0] = val[0];
    AccelerationNoise[1] = val[1];
    AccelerationNoise[2] = val[2];
}

void PositioningKalmanFilter::set_StateCovariance(const double val[9 * 9]) {
    std::copy(&val[0], &val[9 * 9], &StateCovariance[0]);
}
