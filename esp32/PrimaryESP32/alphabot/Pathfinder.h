#ifndef PATHFINDER_H
#define PATHFINDER_H

#include <Arduino.h>
#include <list>

struct Obstacle {
    float x;
    float y;
    float width;
    float height;
};

struct PathNode {
    bool obstacle = false;
    bool visited = false;
    float global_goal;
    float local_goal;
    int32_t x;
    int32_t y;
    PathNode* parent;
};

struct Coordinate {
    int32_t x;
    int32_t y;
};

class Pathfinder {
private:
    std::vector<struct Obstacle> obstacles;
    int32_t map_x;
    int32_t map_y;
    int32_t map_width;
    int32_t map_height;
    float target_x;
    float target_y;
    float starting_pos_x;
    float starting_pos_y;

    void updateMapDimensions();
    void astar(PathNode* node_current, PathNode* node_neighbour, PathNode* node_target, std::list<PathNode*>* list_not_tested_nodes, PathNode* nodes, int32_t nodes_width, int32_t nodes_height);

public:
    void clearObstacles();
    void addObstacle(float x, float y, float width, float height);
    void setTarget(float x, float y, bool update = true);
    void setStartingPos(float x, float y, bool update = true);
    void calculatePath(std::list<Coordinate>& path);

    Pathfinder();
};

#endif
