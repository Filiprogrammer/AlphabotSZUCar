#include "Navigator.h"

void Navigator::setOwnPosition(float x, float y) {
    pos_x = x;
    pos_y = y;
}

void Navigator::setTarget(float x, float y) {
    target_x = x;
    target_y = y;
    has_target = true;
}

bool Navigator::hasTarget() const {
    return has_target;
}

void Navigator::addObstacle(float x, float y, float width, float height) {
    pathfinder->addObstacle(x, y, width, height);
}

void Navigator::clearObstacles() {
    pathfinder->clearObstacles();
}

void Navigator::navigateStep(float dir, std::list<Coordinate>& path) {
    if (!has_target)
        return;

    pathfinder->setStartingPos(pos_x, pos_y, false);
    pathfinder->setTarget(target_x, target_y);

    #ifdef DEBUG
    unsigned long time_pre = micros();
    #endif

    pathfinder->calculatePath(path);
    #ifdef DEBUG
    Serial.print("Path (");
    Serial.print(micros() - time_pre);
    Serial.println("µs)");
    #endif

    if (path.empty()) {
        // Target reached
        has_target = false;
        two_motor_drive->updateMotors(0, 0);
        #ifdef DEBUG
        Serial.println("Target reached!");
        #endif
        struct Coordinate coordinate = {(int32_t)pos_x, (int32_t)pos_y};
        path.push_back(coordinate);
    } else {
        Coordinate first_three_coords[3];
        uint32_t i = 0;

        for (Coordinate coord : path) {
            first_three_coords[i].x = coord.x;
            first_three_coords[i].y = coord.y;

            if (i == 2)
                break;

            ++i;
        }

        float next_dir;

        if (path.size() == 1) // O(1)
            next_dir = atan2(first_three_coords[1].y - first_three_coords[0].y, first_three_coords[1].x - first_three_coords[0].x) * 57.2957795;
        else
            next_dir = atan2(first_three_coords[2].y - first_three_coords[0].y, first_three_coords[2].x - first_three_coords[0].x) * 57.2957795;

        #ifdef DEBUG
        Serial.print("next_dir: ");
        Serial.println(next_dir);
        #endif

        int relative_dir = ((int)(next_dir - (((int)dir + 720) % 360) + 540) % 360) - 180;
        int8_t speed = -65;

        // If car would have to turn more than 90 degrees, drive backwards instead
        if (abs(relative_dir) > 90) {
            speed = -speed;
            relative_dir = -(((relative_dir + 360) % 360) - 180);
        }

        two_motor_drive->updateMotors(constrain(relative_dir, -127, 127), speed);

        #ifdef DEBUG
        Serial.print("ich muss nach: ");
        Serial.println(constrain(relative_dir, -127, 127));
        #endif
    }
}

Navigator::Navigator(TwoMotorDrive* two_motor_drive) {
    this->two_motor_drive = two_motor_drive;
    this->pathfinder = new Pathfinder();
    pos_x = 0;
    pos_y = 0;
    has_target = false;
}

Navigator::~Navigator() {
    delete pathfinder;
}
