#include "Navigator.h"
#include "config.h"

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

void Navigator::addObstacle(float x, float y, float width, float height, uint16_t id) {
    pathfinder->addObstacle(x, y, width, height, id);
}

void Navigator::removeObstacleById(uint16_t id) {
    pathfinder->removeObstacleById(id);
}

void Navigator::removeObstacleByPosition(float x, float y) {
    pathfinder->removeObstacleByPosition(x, y);
}

void Navigator::clearObstacles() {
    pathfinder->clearObstacles();
}

std::list<struct Obstacle> Navigator::getObstacles() {
    return pathfinder->getObstacles();
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

    if (path.size() <= 1) {
        // Target reached
        has_target = false;
        two_motor_drive->updateMotors(0, 0);
        #ifdef DEBUG
        Serial.println("Target reached!");
        #endif

        // Just in case the path happens to be empty for some reason
        if (path.empty()) {
            struct Coordinate coordinate = {(int32_t)pos_x, (int32_t)pos_y};
            path.push_back(coordinate);
        }
    } else {
        Coordinate first_four_coords[4];
        int8_t i = -1;

        for (Coordinate coord : path) {
            ++i;
            first_four_coords[i].x = coord.x;
            first_four_coords[i].y = coord.y;

            if (i == 3)
                break;
        }

        float next_dir;

        if (path.size() <= 2) // O(1)
            next_dir = atan2(target_y - pos_y, target_x - pos_x) * 57.2957795;
        else
            next_dir = atan2(first_four_coords[i].y - first_four_coords[0].y, first_four_coords[i].x - first_four_coords[0].x) * 57.2957795;

        #ifdef DEBUG
        Serial.print("next_dir: ");
        Serial.println(next_dir);
        #endif

        int relative_dir = ((int)(next_dir - (((int)dir + 720) % 360) + 540) % 360) - 180;
        int8_t speed = 65;

        bool driveBackward = false;

        // If car would have to turn more than 90 degrees, drive backwards instead
        if (abs(relative_dir) > 90)
            driveBackward = true;

        if (driveBackward) {
            // If the navigator would switch from driving forward to driving backward and we do not steer quite that far, simply continue forward.
            if (two_motor_drive->getSpeed() > 0 && abs(relative_dir) < (90 + NAVIGATOR_DIRECTION_TOLERANCE))
                driveBackward = false;
        } else {
            // If the navigator would switch from driving backward to driving forward and we do not steer quite that far, simply continue backward.
            if (two_motor_drive->getSpeed() < 0 && abs(relative_dir) > (90 - NAVIGATOR_DIRECTION_TOLERANCE))
                driveBackward = true;
        }

        float to_x, to_y;

        if (driveBackward) {
            to_x = pos_x - cosf(dir * (PI / 180)) * 10;
            to_y = pos_y - sinf(dir * (PI / 180)) * 10;
        } else {
            to_x = pos_x + cosf(dir * (PI / 180)) * 10;
            to_y = pos_y + sinf(dir * (PI / 180)) * 10;
        }

        // Check wether car would collide with an obstacle
        std::list<Obstacle> obstacles = pathfinder->getObstacles();
        bool wouldCollide = std::any_of(obstacles.begin(), obstacles.end(), [&](const Obstacle& o) {
            return to_x >= o.x && to_x <= o.x + o.width && to_y >= o.y && to_y <= o.y + o.height;
        });

        if (wouldCollide) {
            driveBackward = !driveBackward;
            #ifdef DEBUG
            Serial.println("Reversing because I would collide otherwise");
            #endif
        }

        if (driveBackward) {
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
