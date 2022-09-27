#ifndef PATHFINDER_H
#define PATHFINDER_H

#include <Arduino.h>
#include <list>

struct Obstacle {
    float x;
    float y;
    float width;
    float height;
    uint16_t id;
};

struct PathNode {
    uint16_t global_goal;
    uint16_t local_goal;
    int16_t x;
    int16_t y;
    uint32_t parent : 30;
    bool obstacle : 1;
    bool visited : 1;
};

struct Coordinate {
    int32_t x;
    int32_t y;
};

class Pathfinder {
private:
    std::list<struct Obstacle> obstacles;
    int32_t map_x;
    int32_t map_y;
    int32_t map_width;
    int32_t map_height;
    float target_x;
    float target_y;
    float starting_pos_x;
    float starting_pos_y;

    void updateMapDimensions();
    void astar(PathNode* node_current, PathNode* node_neighbour, PathNode* node_target, std::list<PathNode*>* list_not_tested_nodes, PathNode* nodes, int16_t nodes_width, int16_t nodes_height);

public:
    void clearObstacles();
    void addObstacle(float x, float y, float width, float height, uint16_t id);
    void removeObstacleById(uint16_t id);
    void removeObstacleByPosition(float x, float y);
    std::list<struct Obstacle> getObstacles();
    void setTarget(float x, float y, bool update = true);
    void setStartingPos(float x, float y, bool update = true);
    void calculatePath(std::list<Coordinate>& path);

    Pathfinder();
};

#endif
