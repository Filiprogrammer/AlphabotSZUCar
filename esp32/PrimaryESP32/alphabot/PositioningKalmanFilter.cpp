#include "PositioningKalmanFilter.h"
#include <algorithm>
#include <cmath>
#include <cstring>

void PositioningKalmanFilter::fuseaccel(const double accel[3], double accelCov) {
    double dv[28 * 3];
    double obj[3];
    double orientation_a_square = State.orientation.a * State.orientation.a;
    double orientation_b_square = State.orientation.b * State.orientation.b;
    double orientation_c_square = State.orientation.c * State.orientation.c;
    double orientation_d_square = State.orientation.d * State.orientation.d;
    double e_obj_tmp = 2.0 * State.orientation.a * State.orientation.d;
    double f_obj_tmp = 2.0 * State.orientation.b * State.orientation.c;
    double g_obj_tmp = 2.0 * State.orientation.a * State.orientation.c;
    double h_obj_tmp = 2.0 * State.orientation.b * State.orientation.d;
    double i_obj_tmp = g_obj_tmp - h_obj_tmp;
    obj[0] = State.accelerometerBias[0] - State.acceleration[0] * (orientation_a_square + orientation_b_square - orientation_c_square - orientation_d_square) + State.acceleration[2] * i_obj_tmp - State.acceleration[1] * (e_obj_tmp + f_obj_tmp);
    double j_obj_tmp = orientation_a_square - orientation_b_square;
    double k_obj_tmp = 2.0 * State.orientation.a * State.orientation.b;
    double l_obj_tmp = 2.0 * State.orientation.c * State.orientation.d;
    e_obj_tmp -= f_obj_tmp;
    obj[1] = State.accelerometerBias[1] - State.acceleration[1] * (j_obj_tmp + orientation_c_square - orientation_d_square) - State.acceleration[2] * (k_obj_tmp + l_obj_tmp) + State.acceleration[0] * e_obj_tmp;
    k_obj_tmp -= l_obj_tmp;
    obj[2] = State.accelerometerBias[2] - State.acceleration[2] * (j_obj_tmp - orientation_c_square + orientation_d_square) + State.acceleration[1] * k_obj_tmp - State.acceleration[0] * (g_obj_tmp + h_obj_tmp);
    g_obj_tmp = 2.0 * State.orientation.d * State.acceleration[1];
    j_obj_tmp = 2.0 * State.orientation.c * State.acceleration[2];
    double d = 2.0 * State.orientation.a * State.acceleration[0];
    double d1 = j_obj_tmp - g_obj_tmp - d;
    dv[0 + 0 * 3] = d1;
    double d2 = -2.0 * State.orientation.d * State.acceleration[2] - 2.0 * State.orientation.c * State.acceleration[1] - 2.0 * State.orientation.b * State.acceleration[0];
    dv[0 + 1 * 3] = d2;
    double d3 = 2.0 * State.orientation.b * State.acceleration[1];
    double d4 = 2.0 * State.orientation.a * State.acceleration[2];
    double d5 = 2.0 * State.orientation.c * State.acceleration[0];
    dv[0 + 2 * 3] = d4 - d3 + d5;
    double d6 = 2.0 * State.orientation.b * State.acceleration[2];
    double d7 = 2.0 * State.orientation.a * State.acceleration[1];
    double d8 = 2.0 * State.orientation.d * State.acceleration[0];
    double d9 = d8 - d7 - d6;
    dv[0 + 3 * 3] = d9;
    dv[0 + 13 * 3] = -orientation_a_square - orientation_b_square + orientation_c_square + orientation_d_square;
    dv[0 + 14 * 3] = -2.0 * State.orientation.a * State.orientation.d - f_obj_tmp;
    dv[0 + 15 * 3] = i_obj_tmp;
    dv[1 + 0 * 3] = d9;
    d3 = d3 - d4 - d5;
    dv[1 + 1 * 3] = d3;
    dv[1 + 2 * 3] = d2;
    dv[1 + 3 * 3] = g_obj_tmp - j_obj_tmp + d;
    dv[1 + 13 * 3] = e_obj_tmp;
    g_obj_tmp = -orientation_a_square + orientation_b_square;
    dv[1 + 14 * 3] = g_obj_tmp - orientation_c_square + orientation_d_square;
    dv[1 + 15 * 3] = -2.0 * State.orientation.a * State.orientation.b - l_obj_tmp;
    dv[2 + 0 * 3] = d3;
    dv[2 + 1 * 3] = d6 + d7 - d8;
    dv[2 + 2 * 3] = d1;
    dv[2 + 3 * 3] = d2;
    dv[2 + 13 * 3] = -2.0 * State.orientation.a * State.orientation.c - h_obj_tmp;
    dv[2 + 14 * 3] = k_obj_tmp;
    dv[2 + 15 * 3] = g_obj_tmp + orientation_c_square - orientation_d_square;

    double W[84];
    double b_H[84];
    for (int i = 0; i < 3; i++) {
        for (int j = 0; j < 28; j++) {
            int temp = i + 3 * j;
            W[j + 28 * i] = dv[temp];
            double maxval = 0.0;

            for (int k = 0; k < 4; ++k)
                maxval += dv[i + 3 * k] * StateCovariance[k + 28 * j];

            for (int k = 13; k < 16; k++)
                maxval += dv[i + 3 * k] * StateCovariance[k + 28 * j];

            maxval += StateCovariance[i + 16 + 28 * j];
            b_H[temp] = maxval;
        }
    }
    double innovCov[9];
    for (int i = 0; i < 3; i++) {
        for (int j = 0; j < 3; j++) {
            double maxval = 0.0;

            for (int k = 0; k < 4; k++)
                maxval += b_H[i + 3 * k] * W[k + 28 * j];

            for (int k = 13; k < 16; k++)
                maxval += b_H[i + 3 * k] * W[k + 28 * j];

            maxval += b_H[i + 3 * (j + 16)];
            innovCov[i + 3 * j] = maxval;
        }
    }
    innovCov[0 + 3 * 0] += accelCov;
    innovCov[1 + 3 * 1] += accelCov;
    innovCov[2 + 3 * 2] += accelCov;
    for (int i = 0; i < 28; i++) {
        for (int j = 0; j < 3; j++) {
            double maxval = 0.0;

            for (int k = 0; k < 4; k++)
                maxval += StateCovariance[i + 28 * k] * W[k + 28 * j];

            for (int k = 13; k < 16; k++)
                maxval += StateCovariance[i + 28 * k] * W[k + 28 * j];

            maxval += StateCovariance[i + 28 * (j + 16)];
            b_H[i + 28 * j] = maxval;
        }
    }
    int r1 = 0;
    int r2 = 1;
    int r3 = 2;
    double absInnovCov0 = fabs(innovCov[0]);
    double absInnovCov1 = fabs(innovCov[1]);
    double absInnovCov2 = fabs(innovCov[2]);
    if (absInnovCov1 > absInnovCov0) {
        absInnovCov0 = absInnovCov1;
        r1 = 1;
        r2 = 0;
    }
    if (absInnovCov2 > absInnovCov0) {
        r1 = 2;
        r2 = 1;
        r3 = 0;
    }
    innovCov[r2] /= innovCov[r1];
    innovCov[r3] /= innovCov[r1];
    innovCov[r2 + 3] -= innovCov[r2] * innovCov[r1 + 3];
    innovCov[r3 + 3] -= innovCov[r3] * innovCov[r1 + 3];
    innovCov[r2 + 6] -= innovCov[r2] * innovCov[r1 + 6];
    innovCov[r3 + 6] -= innovCov[r3] * innovCov[r1 + 6];
    if (fabs(innovCov[r3 + 3]) > fabs(innovCov[r2 + 3])) {
        std::swap(r2, r3);
    }
    innovCov[r3 + 3] /= innovCov[r2 + 3];
    innovCov[r3 + 6] -= innovCov[r3 + 3] * innovCov[r2 + 6];
    for (int k = 0; k < 28; k++) {
        int b_W_tmp = k + 28 * r1;
        W[b_W_tmp] = b_H[k] / innovCov[r1];
        int temp = k + 28 * r2;
        W[temp] = b_H[k + 28] - W[b_W_tmp] * innovCov[r1 + 3];
        int W_tmp = k + 28 * r3;
        W[W_tmp] = b_H[k + 56] - W[b_W_tmp] * innovCov[r1 + 6];
        W[temp] /= innovCov[r2 + 3];
        W[W_tmp] -= W[temp] * innovCov[r2 + 6];
        W[W_tmp] /= innovCov[r3 + 6];
        W[temp] -= W[W_tmp] * innovCov[r3 + 3];
        W[b_W_tmp] -= W[W_tmp] * innovCov[r3];
        W[b_W_tmp] -= W[temp] * innovCov[r2];
    }
    double innov[3];
    innov[0] = accel[0] - obj[0];
    innov[1] = accel[1] - obj[1];
    innov[2] = accel[2] - obj[2];
    double* b_P = (double*)malloc(784 * sizeof(double));
    double b_W_orientation[28 * 4];
    double b_W_accel[28 * 3];
    for (int i = 0; i < 28; i++) {
        State.State[i] += W[i] * innov[0] + W[i + 28] * innov[1] + W[i + 28 * 2] * innov[2];
        for (int j = 0; j < 4; j++)
            b_W_orientation[i + 28 * j] = W[i] * dv[3 * j] + W[i + 28] * dv[3 * j + 1] + W[i + 28 * 2] * dv[3 * j + 2];

        for (int j = 13; j < 16; j++)
            b_W_accel[i + 28 * (j - 13)] = W[i] * dv[3 * j] + W[i + 28] * dv[3 * j + 1] + W[i + 28 * 2] * dv[3 * j + 2];

        for (int j = 0; j < 28; j++) {
            double maxval = 0.0;

            for (int k = 0; k < 4; ++k)
                maxval += b_W_orientation[i + 28 * k] * StateCovariance[k + 28 * j];

            for (int k = 13; k < 16; k++)
                maxval += b_W_accel[i + 28 * (k - 13)] * StateCovariance[k + 28 * j];

            maxval += W[i + 28 * 0] * StateCovariance[16 + 28 * j];
            maxval += W[i + 28 * 1] * StateCovariance[17 + 28 * j];
            maxval += W[i + 28 * 2] * StateCovariance[18 + 28 * j];

            int temp = i + 28 * j;
            b_P[temp] = StateCovariance[temp] - maxval;
        }
    }
    std::copy(&b_P[0], &b_P[784], &StateCovariance[0]);
    free(b_P);
    repairQuaternion(&State.orientation);
}

void PositioningKalmanFilter::fusegps(const double rawPos[3], double posCov, const double vel[3], double velCov) {
    double A[6 * 6];

    for (int i = 0; i < 6; i++)
        for (int j = 0; j < 6; j++)
            A[i + 6 * j] = StateCovariance[i + 7 + 28 * (j + 7)];

    for (int i = 0; i < 3; i++)
        A[i + 6 * i] += posCov;

    for (int i = 3; i < 6; i++)
        A[i + 6 * i] += velCov;

    double W[28 * 6];

    for (int i = 0; i < 28; i++)
        for (int j = 0; j < 6; j++)
            W[i + 28 * j] = StateCovariance[i + 28 * (j + 7)];

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
                for (int k = 0; k < 28; k++)
                    W[k + 28 * i] -= smax * W[k + 28 * j];
        }

        double smax = 1.0 / A[i + 6 * i];

        for (int j = 0; j < 28; j++)
            W[j + 28 * i] *= smax;
    }
    for (int i = 5; i >= 0; i--) {
        for (int j = i + 1; j < 6; j++) {
            double smax = A[j + 6 * i];

            if (smax != 0.0)
                for (int k = 0; k < 28; k++)
                    W[k + 28 * i] -= smax * W[k + 28 * j];
        }
    }
    for (int i = 0; i < 5; i++) {
        signed char i1 = ipiv[i];

        if (i1 != i)
            for (int j = 0; j < 28; j++)
                std::swap(W[j + 28 * i], W[j + 28 * i1]);
    }
    double innov[6];
    innov[0] = rawPos[0] - State.position[0];
    innov[1] = rawPos[1] - State.position[1];
    innov[2] = rawPos[2] - State.position[2];
    innov[3] = vel[0] - State.velocity[0];
    innov[4] = vel[1] - State.velocity[1];
    innov[5] = vel[2] - State.velocity[2];
    double* b_P = (double*)malloc(784 * sizeof(double));
    for (int i = 0; i < 28; i++) {
        double smax = 0.0;

        for (int j = 0; j < 6; j++)
            smax += W[i + 28 * j] * innov[j];

        State.State[i] += smax;

        for (int j = 0; j < 28; j++) {
            smax = 0.0;

            for (int k = 7; k < 13; k++)
                smax += W[i + 28 * (k - 7)] * StateCovariance[k + 28 * j];

            int jA = i + 28 * j;
            b_P[jA] = StateCovariance[jA] - smax;
        }
    }
    repairQuaternion(&State.orientation);
    set_StateCovariance(b_P);
    free(b_P);
}

void PositioningKalmanFilter::fusevel(const double vel[3], double velCov) {
    double A[3 * 3];

    for (int i = 0; i < 3; i++)
        for (int j = 0; j < 3; j++)
            A[i + 3 * j] = StateCovariance[i + 10 + 28 * (j + 10)];

    for (int i = 0; i < 3; i++)
        A[i + 3 * i] += velCov;

    double W[28 * 3];

    for (int i = 0; i < 28; i++)
        for (int j = 0; j < 3; j++)
            W[i + 28 * j] = StateCovariance[i + 28 * (j + 10)];

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
                for (int k = 0; k < 28; k++)
                    W[k + 28 * i] -= smax * W[k + 28 * j];
        }

        double smax = 1.0 / A[i + 3 * i];

        for (int j = 0; j < 28; j++)
            W[j + 28 * i] *= smax;
    }
    for (int i = 2; i >= 0; i--) {
        for (int j = i + 1; j < 3; j++) {
            double smax = A[j + 3 * i];

            if (smax != 0.0)
                for (int k = 0; k < 28; k++)
                    W[k + 28 * i] -= smax * W[k + 28 * j];
        }
    }
    for (int i = 0; i < 2; i++) {
        signed char i1 = ipiv[i];

        if (i1 != i)
            for (int j = 0; j < 28; j++)
                std::swap(W[j + 28 * i], W[j + 28 * i1]);
    }
    double innov[3];
    innov[0] = vel[0] - State.velocity[0];
    innov[1] = vel[1] - State.velocity[1];
    innov[2] = vel[2] - State.velocity[2];
    double b_P[784];
    for (int i = 0; i < 28; i++) {
        double smax = 0.0;

        for (int j = 0; j < 3; j++)
            smax += W[i + 28 * j] * innov[j];

        State.State[i] += smax;

        for (int j = 0; j < 28; j++) {
            smax = 0.0;

            for (int k = 10; k < 13; k++)
                smax += W[i + 28 * (k - 10)] * StateCovariance[k + 28 * j];

            int jA = i + 28 * j;
            b_P[jA] = StateCovariance[jA] - smax;
        }
    }
    repairQuaternion(&State.orientation);
    set_StateCovariance(b_P);
}

void PositioningKalmanFilter::fusegyro(const double gyro[3], double gyroCov) {
    double obj[3];
    obj[0] = State.angularVelocity[0] + State.gyroscopeBias[0];
    obj[1] = State.angularVelocity[1] + State.gyroscopeBias[1];
    obj[2] = State.angularVelocity[2] + State.gyroscopeBias[2];

    double W[84];
    double b_H[84];
    double A[9];

    for (int i = 0; i < 3; i++) {
        for (int j = 0; j < 28; j++) {
            W[j + 28 * i] = 0.0;
            b_H[i + 3 * j] = StateCovariance[i + 4 + 28 * j] + StateCovariance[i + 19 + 28 * j];
        }
    }
    W[4 + 28 * 0] = 1.0;
    W[5 + 28 * 1] = 1.0;
    W[6 + 28 * 2] = 1.0;
    W[19 + 28 * 0] = 1.0;
    W[20 + 28 * 1] = 1.0;
    W[21 + 28 * 2] = 1.0;
    double innovCov[9];

    for (int i = 0; i < 3; i++)
        for (int j = 0; j < 3; j++)
            innovCov[i + 3 * j] = b_H[i + 3 * (j + 4)] + b_H[i + 3 * (j + 19)];

    for (int i = 0; i < 3; i++)
        innovCov[i + 3 * i] += gyroCov;

    for (int i = 0; i < 28; i++)
        for (int j = 0; j < 3; j++)
            b_H[i + 28 * j] = StateCovariance[i + 28 * (j + 4)] + StateCovariance[i + 28 * (j + 19)];

    std::copy(&innovCov[0], &innovCov[9], &A[0]);
    int r1 = 0;
    int r2 = 1;
    int r3 = 2;
    double absInnovCov0 = fabs(innovCov[0]);
    double absInnovCov1 = fabs(innovCov[1]);
    double absInnovCov2 = fabs(innovCov[2]);
    if (absInnovCov1 > absInnovCov0) {
        absInnovCov0 = absInnovCov1;
        r1 = 1;
        r2 = 0;
    }
    if (absInnovCov2 > absInnovCov0) {
        r1 = 2;
        r2 = 1;
        r3 = 0;
    }
    A[r2] = innovCov[r2] / innovCov[r1];
    A[r3] /= A[r1];
    A[r2 + 3] -= A[r2] * A[r1 + 3];
    A[r3 + 3] -= A[r3] * A[r1 + 3];
    A[r2 + 6] -= A[r2] * A[r1 + 6];
    A[r3 + 6] -= A[r3] * A[r1 + 6];
    if (fabs(A[r3 + 3]) > fabs(A[r2 + 3])) {
        std::swap(r2, r3);
    }
    A[r3 + 3] /= A[r2 + 3];
    A[r3 + 6] -= A[r3 + 3] * A[r2 + 6];
    for (int k = 0; k < 28; k++) {
        int b_W_tmp = k + 28 * r1;
        W[b_W_tmp] = b_H[k] / A[r1];
        int temp = k + 28 * r2;
        W[temp] = b_H[k + 28] - W[b_W_tmp] * A[r1 + 3];
        int W_tmp = k + 28 * r3;
        W[W_tmp] = b_H[k + 56] - W[b_W_tmp] * A[r1 + 6];
        W[temp] /= A[r2 + 3];
        W[W_tmp] -= W[temp] * A[r2 + 6];
        W[W_tmp] /= A[r3 + 6];
        W[temp] -= W[W_tmp] * A[r3 + 3];
        W[b_W_tmp] -= W[W_tmp] * A[r3];
        W[b_W_tmp] -= W[temp] * A[r2];
    }
    double innov[3];
    innov[0] = gyro[0] - obj[0];
    innov[1] = gyro[1] - obj[1];
    innov[2] = gyro[2] - obj[2];
    double* b_P = (double*)malloc(784 * sizeof(double));
    for (int i = 0; i < 28; i++) {
        State.State[i] += W[i] * innov[0] + W[i + 28] * innov[1] + W[i + 28 * 2] * innov[2];

        for (int j = 0; j < 28; j++) {
            double maxval = W[i + 0 * 28] * StateCovariance[4 + j * 28];
            maxval += W[i + 1 * 28] * StateCovariance[5 + j * 28];
            maxval += W[i + 2 * 28] * StateCovariance[6 + j * 28];
            maxval += W[i + 0 * 28] * StateCovariance[19 + j * 28];
            maxval += W[i + 1 * 28] * StateCovariance[20 + j * 28];
            maxval += W[i + 2 * 28] * StateCovariance[21 + j * 28];

            int temp = i + 28 * j;
            b_P[temp] = StateCovariance[temp] - maxval;
        }
    }
    std::copy(&b_P[0], &b_P[784], &StateCovariance[0]);
    free(b_P);
    repairQuaternion(&State.orientation);
}

void PositioningKalmanFilter::fusemag(const double mag[3], double magCov) {
    double dv_orientation[4 * 3];
    double dv_mag[3 * 3];
    double obj[3];
    double obj_tmp = State.orientation.a * State.orientation.a;
    double b_obj_tmp = State.orientation.b * State.orientation.b;
    double c_obj_tmp = State.orientation.c * State.orientation.c;
    double d_obj_tmp = State.orientation.d * State.orientation.d;
    double e_obj_tmp = 2.0 * State.orientation.a * State.orientation.d;
    double f_obj_tmp = 2.0 * State.orientation.b * State.orientation.c;
    double g_obj_tmp = 2.0 * State.orientation.a * State.orientation.c;
    double h_obj_tmp = 2.0 * State.orientation.b * State.orientation.d;
    double i_obj_tmp = obj_tmp + b_obj_tmp - c_obj_tmp - d_obj_tmp;
    double j_obj_tmp = e_obj_tmp + f_obj_tmp;
    obj[0] = State.magnetometerBias[0] + State.geomagneticFieldVector[0] * i_obj_tmp - State.geomagneticFieldVector[2] * (g_obj_tmp - h_obj_tmp) + State.geomagneticFieldVector[1] * j_obj_tmp;
    obj_tmp -= b_obj_tmp;
    b_obj_tmp = 2.0 * State.orientation.a * State.orientation.b;
    double k_obj_tmp = 2.0 * State.orientation.c * State.orientation.d;
    double l_obj_tmp = obj_tmp + c_obj_tmp - d_obj_tmp;
    double m_obj_tmp = b_obj_tmp + k_obj_tmp;
    obj[1] = State.magnetometerBias[1] + State.geomagneticFieldVector[1] * l_obj_tmp + State.geomagneticFieldVector[2] * m_obj_tmp - State.geomagneticFieldVector[0] * (e_obj_tmp - f_obj_tmp);
    double n_obj_tmp = g_obj_tmp + h_obj_tmp;
    obj_tmp = obj_tmp - c_obj_tmp + d_obj_tmp;
    obj[2] = State.magnetometerBias[2] + State.geomagneticFieldVector[2] * obj_tmp - State.geomagneticFieldVector[1] * (b_obj_tmp - k_obj_tmp) + State.geomagneticFieldVector[0] * n_obj_tmp;
    c_obj_tmp = 2.0 * State.geomagneticFieldVector[2] * State.orientation.c;
    d_obj_tmp = 2.0 * State.geomagneticFieldVector[1] * State.orientation.d;
    double d = 2.0 * State.geomagneticFieldVector[0] * State.orientation.a;
    double d1 = d_obj_tmp - c_obj_tmp + d;
    dv_orientation[0 + 3 * 0] = d1;
    double d2 = 2.0 * State.geomagneticFieldVector[2] * State.orientation.d + 2.0 * State.geomagneticFieldVector[1] * State.orientation.c + 2.0 * State.geomagneticFieldVector[0] * State.orientation.b;
    dv_orientation[0 + 3 * 1] = d2;
    double d3 = 2.0 * State.geomagneticFieldVector[2] * State.orientation.a;
    double d4 = 2.0 * State.geomagneticFieldVector[1] * State.orientation.b;
    double d5 = 2.0 * State.geomagneticFieldVector[0] * State.orientation.c;
    dv_orientation[0 + 3 * 2] = d4 - d3 - d5;
    double d6 = 2.0 * State.geomagneticFieldVector[0] * State.orientation.d;
    double d7 = 2.0 * State.geomagneticFieldVector[1] * State.orientation.a;
    double d8 = 2.0 * State.geomagneticFieldVector[2] * State.orientation.b;
    double d9 = d8 + d7 - d6;
    dv_orientation[0 + 3 * 3] = d9;
    dv_mag[0 + 3 * 0] = i_obj_tmp;
    dv_mag[0 + 3 * 1] = j_obj_tmp;
    dv_mag[0 + 3 * 2] = h_obj_tmp - g_obj_tmp;
    dv_orientation[1 + 3 * 0] = d9;
    d3 = d3 - d4 + d5;
    dv_orientation[1 + 3 * 1] = d3;
    dv_orientation[1 + 3 * 2] = d2;
    dv_orientation[1 + 3 * 3] = c_obj_tmp - d_obj_tmp - d;
    dv_mag[1 + 3 * 0] = f_obj_tmp - e_obj_tmp;
    dv_mag[1 + 3 * 1] = l_obj_tmp;
    dv_mag[1 + 3 * 2] = m_obj_tmp;
    dv_orientation[2 + 3 * 0] = d3;
    dv_orientation[2 + 3 * 1] = d6 - d7 - d8;
    dv_orientation[2 + 3 * 2] = d1;
    dv_orientation[2 + 3 * 3] = d2;
    dv_mag[2 + 3 * 0] = n_obj_tmp;
    dv_mag[2 + 3 * 1] = k_obj_tmp - b_obj_tmp;
    dv_mag[2 + 3 * 2] = obj_tmp;

    double W[84];
    double b_H[84];
    for (int i = 0; i < 3; i++) {
        for (int j = 0; j < 28; j++) {
            double maxval = 0.0;

            for (int k = 0; k < 4; k++)
                maxval += dv_orientation[i + 3 * k] * StateCovariance[k + 28 * j];

            for (int k = 22; k < 25; k++)
                maxval += dv_mag[i + 3 * (k - 22)] * StateCovariance[k + 28 * j];

            maxval += StateCovariance[25 + i + 28 * j];

            b_H[i + 3 * j] = maxval;
        }
        for (int j = 0; j < 4; j++) {
            W[j + 28 * i] = dv_orientation[i + 3 * j];
        }
        for (int j = 4; j < 22; j++) {
            W[j + 28 * i] = 0;
        }
        for (int j = 22; j < 25; j++) {
            W[j + 28 * i] = dv_mag[i + 3 * (j - 22)];
        }
        W[25 + 28 * i] = (i == 0);
        W[26 + 28 * i] = (i == 1);
        W[27 + 28 * i] = (i == 2);
    }
    double innovCov[9];
    for (int i = 0; i < 3; i++) {
        for (int j = 0; j < 3; j++) {
            double maxval = 0.0;

            for (int k = 0; k < 4; k++)
                maxval += b_H[i + 3 * k] * W[k + 28 * j];

            for (int k = 22; k < 25; k++)
                maxval += b_H[i + 3 * k] * W[k + 28 * j];

            maxval += b_H[i + 3 * (25 + i)];
            innovCov[i + 3 * j] = maxval;
        }
    }
    innovCov[0 + 3 * 0] += magCov;
    innovCov[1 + 3 * 1] += magCov;
    innovCov[2 + 3 * 2] += magCov;
    for (int i = 0; i < 28; i++) {
        for (int j = 0; j < 3; j++) {
            double maxval = 0.0;

            for (int k = 0; k < 4; k++)
                maxval += StateCovariance[i + 28 * k] * W[k + 28 * j];

            for (int k = 22; k < 25; k++)
                maxval += StateCovariance[i + 28 * k] * W[k + 28 * j];

            maxval += StateCovariance[i + 28 * (25 + j)];
            b_H[i + 28 * j] = maxval;
        }
    }
    int r1 = 0;
    int r2 = 1;
    int r3 = 2;
    double absInnovCov0 = fabs(innovCov[0]);
    double absInnovCov1 = fabs(innovCov[1]);
    double absInnovCov2 = fabs(innovCov[2]);
    if (absInnovCov1 > absInnovCov0) {
        absInnovCov0 = absInnovCov1;
        r1 = 1;
        r2 = 0;
    }
    if (absInnovCov2 > absInnovCov0) {
        r1 = 2;
        r2 = 1;
        r3 = 0;
    }
    innovCov[r2] /= innovCov[r1];
    innovCov[r3] /= innovCov[r1];
    innovCov[r2 + 3] -= innovCov[r2] * innovCov[r1 + 3];
    innovCov[r3 + 3] -= innovCov[r3] * innovCov[r1 + 3];
    innovCov[r2 + 6] -= innovCov[r2] * innovCov[r1 + 6];
    innovCov[r3 + 6] -= innovCov[r3] * innovCov[r1 + 6];
    if (fabs(innovCov[r3 + 3]) > fabs(innovCov[r2 + 3])) {
        std::swap(r2, r3);
    }
    innovCov[r3 + 3] /= innovCov[r2 + 3];
    innovCov[r3 + 6] -= innovCov[r3 + 3] * innovCov[r2 + 6];
    for (int k = 0; k < 28; k++) {
        int b_W_tmp = k + 28 * r1;
        W[b_W_tmp] = b_H[k] / innovCov[r1];
        int temp = k + 28 * r2;
        W[temp] = b_H[k + 28] - W[b_W_tmp] * innovCov[r1 + 3];
        int W_tmp = k + 28 * r3;
        W[W_tmp] = b_H[k + 56] - W[b_W_tmp] * innovCov[r1 + 6];
        W[temp] /= innovCov[r2 + 3];
        W[W_tmp] -= W[temp] * innovCov[r2 + 6];
        W[W_tmp] /= innovCov[r3 + 6];
        W[temp] -= W[W_tmp] * innovCov[r3 + 3];
        W[b_W_tmp] -= W[W_tmp] * innovCov[r3];
        W[b_W_tmp] -= W[temp] * innovCov[r2];
    }
    double innov[3];
    innov[0] = mag[0] - obj[0];
    innov[1] = mag[1] - obj[1];
    innov[2] = mag[2] - obj[2];
    double* b_P = (double*)malloc(784 * sizeof(double));
    double b_W_orientation[28 * 4];
    double b_W_mag[28 * 6];
    for (int i = 0; i < 28; i++) {
        State.State[i] += W[i] * innov[0] + W[i + 28] * innov[1] + W[i + 28 * 2] * innov[2];

        for (int j = 0; j < 4; j++)
            b_W_orientation[i + 28 * j] = W[i] * dv_orientation[3 * j] + W[i + 28] * dv_orientation[3 * j + 1] + W[i + 28 * 2] * dv_orientation[3 * j + 2];

        for (int j = 0; j < 3; j++)
            b_W_mag[i + 28 * j] = W[i] * dv_mag[3 * j] + W[i + 28] * dv_mag[3 * j + 1] + W[i + 28 * 2] * dv_mag[3 * j + 2];

        b_W_mag[i + 28 * 3] = W[i];
        b_W_mag[i + 28 * 4] = W[i + 28];
        b_W_mag[i + 28 * 5] = W[i + 28 * 2];

        for (int j = 0; j < 28; j++) {
            double maxval = 0.0;

            for (int k = 0; k < 4; k++)
                maxval += b_W_orientation[i + 28 * k] * StateCovariance[k + 28 * j];

            for (int k = 22; k < 28; k++)
                maxval += b_W_mag[i + 28 * (k - 22)] * StateCovariance[k + 28 * j];

            int temp = i + 28 * j;
            b_P[temp] = StateCovariance[temp] - maxval;
        }
    }
    std::copy(&b_P[0], &b_P[784], &StateCovariance[0]);
    free(b_P);
    repairQuaternion(&State.orientation);
}

void PositioningKalmanFilter::pose(double pos[3], quaternion* orient) const {
    orient->a = State.orientation.a;
    orient->b = State.orientation.b;
    orient->c = State.orientation.c;
    orient->d = State.orientation.d;
    pos[0] = State.position[0];
    pos[1] = State.position[1];
    pos[2] = State.position[2];
}

void PositioningKalmanFilter::predict(double dt) {
    double dfdx[28 * 7];
    memset(dfdx, 0, 28 * 7 * sizeof(double));
    dfdx[28 * 1 + 0] = -State.angularVelocity[0] / 2.0;
    dfdx[28 * 2 + 0] = -State.angularVelocity[1] / 2.0;
    dfdx[28 * 3 + 0] = -State.angularVelocity[2] / 2.0;
    dfdx[28 * 4 + 0] = -State.orientation.b / 2.0;
    dfdx[28 * 5 + 0] = -State.orientation.c / 2.0;
    dfdx[28 * 6 + 0] = -State.orientation.d / 2.0;
    dfdx[28 * 0 + 1] = State.angularVelocity[0] / 2.0;
    dfdx[28 * 2 + 1] = State.angularVelocity[2] / 2.0;
    dfdx[28 * 3 + 1] = -State.angularVelocity[1] / 2.0;
    dfdx[28 * 4 + 1] = State.orientation.a / 2.0;
    dfdx[28 * 5 + 1] = -State.orientation.d / 2.0;
    dfdx[28 * 6 + 1] = State.orientation.c / 2.0;
    dfdx[28 * 0 + 2] = State.angularVelocity[1] / 2.0;
    dfdx[28 * 1 + 2] = -State.angularVelocity[2] / 2.0;
    dfdx[28 * 3 + 2] = State.angularVelocity[0] / 2.0;
    dfdx[28 * 4 + 2] = State.orientation.d / 2.0;
    dfdx[28 * 5 + 2] = State.orientation.a / 2.0;
    dfdx[28 * 6 + 2] = -State.orientation.b / 2.0;
    dfdx[28 * 0 + 3] = State.angularVelocity[2] / 2.0;
    dfdx[28 * 1 + 3] = State.angularVelocity[1] / 2.0;
    dfdx[28 * 2 + 3] = -State.angularVelocity[0] / 2.0;
    dfdx[28 * 4 + 3] = -State.orientation.c / 2.0;
    dfdx[28 * 5 + 3] = State.orientation.b / 2.0;
    dfdx[28 * 6 + 3] = State.orientation.a / 2.0;

    double* Pdot = (double*)malloc(784 * sizeof(double));

    for (int i = 0; i < 7; i++) {
        for (int i1 = 0; i1 < 28; i1++) {
            double tmp = 0.0;

            for (int i2 = 0; i2 < 7; i2++)
                tmp += dfdx[i + 28 * i2] * StateCovariance[i2 + 28 * i1];

            Pdot[i + 28 * i1] = tmp;
        }
    }

    for (int i = 7; i < 13; i++) {
        for (int i1 = 0; i1 < 28; i1++) {
            Pdot[i + 28 * i1] = StateCovariance[i + 3 + 28 * i1];
        }
    }

    for (int i = 0; i < 13; i++) {
        for (int i1 = 0; i1 < 7; i1++) {
            double tmp = 0.0;

            for (int i2 = 0; i2 < 7; i2++)
                tmp += StateCovariance[i + 28 * i2] * dfdx[i1 + 28 * i2];

            Pdot[i + 28 * i1] += tmp;
        }

        for (int i1 = 7; i1 < 13; i1++)
            Pdot[i + 28 * i1] += StateCovariance[i + 28 * (i1 + 3)];
    }

    for (int i = 13; i < 28; i++) {
        for (int i1 = 0; i1 < 7; i1++) {
            double tmp = 0.0;

            for (int i2 = 0; i2 < 7; i2++)
                tmp += StateCovariance[i + 28 * i2] * dfdx[i1 + 28 * i2];

            Pdot[i + 28 * i1] = tmp;
        }

        for (int i1 = 7; i1 < 13; i1++)
            Pdot[i + 28 * i1] = StateCovariance[i + 28 * (i1 + 3)];
    }

    for (int i = 13; i < 28; i++) {
        for (int i1 = 13; i1 < 28; i1++) {
            Pdot[i + 28 * i1] = 0.0;
        }
    }

    Pdot[0 + 28 * 0] += QuaternionNoise[0];
    Pdot[1 + 28 * 1] += QuaternionNoise[1];
    Pdot[2 + 28 * 2] += QuaternionNoise[2];
    Pdot[3 + 28 * 3] += QuaternionNoise[3];
    Pdot[4 + 28 * 4] += AngularVelocityNoise[0];
    Pdot[5 + 28 * 5] += AngularVelocityNoise[1];
    Pdot[6 + 28 * 6] += AngularVelocityNoise[2];
    Pdot[7 + 28 * 7] += PositionNoise[0];
    Pdot[8 + 28 * 8] += PositionNoise[1];
    Pdot[9 + 28 * 9] += PositionNoise[2];
    Pdot[10 + 28 * 10] += VelocityNoise[0];
    Pdot[11 + 28 * 11] += VelocityNoise[1];
    Pdot[12 + 28 * 12] += VelocityNoise[2];
    Pdot[13 + 28 * 13] += AccelerationNoise[0];
    Pdot[14 + 28 * 14] += AccelerationNoise[1];
    Pdot[15 + 28 * 15] += AccelerationNoise[2];
    Pdot[16 + 28 * 16] += AccelerometerBiasNoise[0];
    Pdot[17 + 28 * 17] += AccelerometerBiasNoise[1];
    Pdot[18 + 28 * 18] += AccelerometerBiasNoise[2];
    Pdot[19 + 28 * 19] += GyroscopeBiasNoise[0];
    Pdot[20 + 28 * 20] += GyroscopeBiasNoise[1];
    Pdot[21 + 28 * 21] += GyroscopeBiasNoise[2];
    Pdot[22 + 28 * 22] += GeomagneticVectorNoise[0];
    Pdot[23 + 28 * 23] += GeomagneticVectorNoise[1];
    Pdot[24 + 28 * 24] += GeomagneticVectorNoise[2];
    Pdot[25 + 28 * 25] += MagnetometerBiasNoise[0];
    Pdot[26 + 28 * 26] += MagnetometerBiasNoise[1];
    Pdot[27 + 28 * 27] += MagnetometerBiasNoise[2];

    quaternion orientationTmp;
    orientationTmp.a = State.orientation.a + (-(State.orientation.b * State.angularVelocity[0]) / 2.0 - State.orientation.c * State.angularVelocity[1] / 2.0 - State.orientation.d * State.angularVelocity[2] / 2.0) * dt;
    orientationTmp.b = State.orientation.b + (State.orientation.a * State.angularVelocity[0] / 2.0 - State.orientation.d * State.angularVelocity[1] / 2.0 + State.orientation.c * State.angularVelocity[2] / 2.0) * dt;
    orientationTmp.c = State.orientation.c + (State.orientation.d * State.angularVelocity[0] / 2.0 + State.orientation.a * State.angularVelocity[1] / 2.0 - State.orientation.b * State.angularVelocity[2] / 2.0) * dt;
    orientationTmp.d = State.orientation.d + (State.orientation.b * State.angularVelocity[1] / 2.0 - State.orientation.c * State.angularVelocity[0] / 2.0 + State.orientation.a * State.angularVelocity[2] / 2.0) * dt;
    repairQuaternion(&orientationTmp);
    State.orientation.a = orientationTmp.a;
    State.orientation.b = orientationTmp.b;
    State.orientation.c = orientationTmp.c;
    State.orientation.d = orientationTmp.d;
    State.position[0] += State.velocity[0] * dt;
    State.position[1] += State.velocity[1] * dt;
    State.position[2] += State.velocity[2] * dt;
    State.velocity[0] += State.acceleration[0] * dt;
    State.velocity[1] += State.acceleration[1] * dt;
    State.velocity[2] += State.acceleration[2] * dt;

    for (int i = 0; i < 28; i++) {
        for (int i1 = 0; i1 < 28; i1++) {
            int j = i1 + 28 * i;
            StateCovariance[j] += 0.5 * (Pdot[j] + Pdot[i + 28 * i1]) * dt;
        }
    }

    free(Pdot);
}

void PositioningKalmanFilter::repairQuaternion(quaternion* x) const {
    double n = sqrt(x->a * x->a + x->b * x->b + x->c * x->c + x->d * x->d);
    double qparts_idx_0 = x->a / n;
    double qparts_idx_1 = x->b / n;
    double qparts_idx_2 = x->c / n;
    double qparts_idx_3 = x->d / n;

    if (qparts_idx_0 < 0.0) {
        x->a = -qparts_idx_0;
        x->b = -qparts_idx_1;
        x->c = -qparts_idx_2;
        x->d = -qparts_idx_3;
    } else {
        x->a = qparts_idx_0;
        x->b = qparts_idx_1;
        x->c = qparts_idx_2;
        x->d = qparts_idx_3;
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

void PositioningKalmanFilter::set_GeomagneticVectorNoise(const double val[3]) {
    GeomagneticVectorNoise[0] = val[0];
    GeomagneticVectorNoise[1] = val[1];
    GeomagneticVectorNoise[2] = val[2];
}

void PositioningKalmanFilter::set_AccelerationNoise(const double val[3]) {
    // 100.0
    AccelerationNoise[0] = val[0];
    AccelerationNoise[1] = val[1];
    AccelerationNoise[2] = val[2];
}

void PositioningKalmanFilter::set_AccelerometerBiasNoise(const double val[3]) {
    // 1.0E-7
    AccelerometerBiasNoise[0] = val[0];
    AccelerometerBiasNoise[1] = val[1];
    AccelerometerBiasNoise[2] = val[2];
}

void PositioningKalmanFilter::set_AngularVelocityNoise(const double val[3]) {
    // 100.0
    AngularVelocityNoise[0] = val[0];
    AngularVelocityNoise[1] = val[1];
    AngularVelocityNoise[2] = val[2];
}

void PositioningKalmanFilter::set_GyroscopeBiasNoise(const double val[3]) {
    // 1.0E-7
    GyroscopeBiasNoise[0] = val[0];
    GyroscopeBiasNoise[1] = val[1];
    GyroscopeBiasNoise[2] = val[2];
}

void PositioningKalmanFilter::set_MagnetometerBiasNoise(const double val[3]) {
    // 1.0E-7
    MagnetometerBiasNoise[0] = val[0];
    MagnetometerBiasNoise[1] = val[1];
    MagnetometerBiasNoise[2] = val[2];
}

void PositioningKalmanFilter::set_QuaternionNoise(const double val[4]) {
    // 0.01
    QuaternionNoise[0] = val[0];
    QuaternionNoise[1] = val[1];
    QuaternionNoise[2] = val[2];
    QuaternionNoise[3] = val[3];
}

void PositioningKalmanFilter::set_StateCovariance(const double val[784]) {
    std::copy(&val[0], &val[784], &StateCovariance[0]);
}
