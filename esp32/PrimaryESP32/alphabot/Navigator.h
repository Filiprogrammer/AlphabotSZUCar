#ifndef NAVIGATOR_H
#define NAVIGATOR_H

#include "Pathfinder.h"
#include "TwoMotorDrive.h"

class Navigator {
private:
    float pos_x;
    float pos_y;
    bool has_target;
    float target_x;
    float target_y;
    Pathfinder* pathfinder;
    TwoMotorDrive* two_motor_drive;

public:
    void setOwnPosition(float x, float y);
    void setTarget(float x, float y);
    bool hasTarget() const;
    void clearObstacles();
    void addObstacle(float x, float y, float width, float height, uint16_t id);
    void removeObstacleById(uint16_t id);
    void removeObstacleByPosition(float x, float y);
    std::list<struct Obstacle> getObstacles();
    void navigateStep(float dir, std::list<Coordinate>& path);

    Navigator(TwoMotorDrive* two_motor_drive);
    ~Navigator();
};

#endif
