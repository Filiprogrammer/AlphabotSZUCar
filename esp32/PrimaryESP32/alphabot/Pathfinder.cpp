#include "Pathfinder.h"

void Pathfinder::updateMapDimensions() {
    map_x = 2147483647;
    map_y = 2147483647;
    map_width = 0;
    map_height = 0;

    for (Obstacle o : obstacles) {
        map_x = _min(map_x, o.x - 50);
        map_y = _min(map_y, o.y - 50);
    }

    map_x = _min(map_x, starting_pos_x - 30);
    map_y = _min(map_y, starting_pos_y - 30);
    map_x = _min(map_x, target_x - 30);
    map_y = _min(map_y, target_y - 30);

    for (Obstacle o : obstacles) {
        map_width = _max(map_x + map_width, o.x + o.width + 50) - map_x;
        map_height = _max(map_y + map_height, o.y + o.height + 50) - map_y;
    }

    map_width = _max(map_x + map_width, starting_pos_x + 30) - map_x;
    map_height = _max(map_y + map_height, starting_pos_y + 30) - map_y;
    map_width = _max(map_x + map_width, target_x + 30) - map_x;
    map_height = _max(map_y + map_height, target_y + 30) - map_y;

    map_x = roundf(map_x / 10.0f) * 10;
    map_y = roundf(map_y / 10.0f) * 10;
    map_x = roundf(map_x / 10.0f) * 10;
    map_y = roundf(map_y / 10.0f) * 10;
}

void Pathfinder::clearObstacles() {
    obstacles.clear();
}

void Pathfinder::addObstacle(float x, float y, float width, float height, uint16_t id) {
    struct Obstacle obstacle;
    obstacle.x = x;
    obstacle.y = y;
    obstacle.width = width;
    obstacle.height = height;
    obstacle.id = id;
    obstacles.push_back(obstacle);
    updateMapDimensions();
}

void Pathfinder::removeObstacleById(uint16_t id) {
    obstacles.erase(std::remove_if(obstacles.begin(), obstacles.end(), [&](const Obstacle& o) {
        return id == o.id;
    }), obstacles.end());
}

void Pathfinder::removeObstacleByPosition(float x, float y) {
    obstacles.erase(std::remove_if(obstacles.begin(), obstacles.end(), [&](const Obstacle& o) {
        return (x >= o.x && x <= (o.x + o.width) && y >= o.y && y <= (o.y + o.height));
    }), obstacles.end());
}

std::list<struct Obstacle> Pathfinder::getObstacles() {
    return obstacles;
}

void Pathfinder::setTarget(float x, float y, bool update) {
    target_x = x;
    target_y = y;

    if (update)
        updateMapDimensions();
}

void Pathfinder::setStartingPos(float x, float y, bool update) {
    starting_pos_x = x;
    starting_pos_y = y;

    if (update)
        updateMapDimensions();
}

void Pathfinder::calculatePath(std::list<Coordinate>& path) {
    int16_t nodes_width = (map_width + 9) / 10;
    int16_t nodes_height = (map_height + 9) / 10;

    if (nodes_width * nodes_height > nodes_size) {
        #ifdef DEBUG
        Serial.print("Not enough memory for Pathfinder (Tried to use ");
        Serial.print(nodes_width * nodes_height * sizeof(PathNode));
        Serial.print(" bytes) (Only ");
        Serial.print(nodes_size * sizeof(PathNode));
        Serial.println(" bytes allocated)");
        #endif
        return;
    }

    // Reset nodes
    for (int16_t x = 0; x < nodes_width; ++x)
        for (int16_t y = 0; y < nodes_height; ++y) {
            nodes[y * nodes_width + x].visited = false;
            nodes[y * nodes_width + x].global_goal = UINT16_MAX;
            nodes[y * nodes_width + x].local_goal = UINT16_MAX;
            nodes[y * nodes_width + x].parent = 0;
            nodes[y * nodes_width + x].obstacle = false;
            nodes[y * nodes_width + x].x = x;
            nodes[y * nodes_width + x].y = y;
        }

    for (Obstacle o : obstacles) {
        int16_t o_x1_rounded = roundf((o.x - map_x) / 10.0f);
        int16_t o_x2_rounded = roundf((o.x + o.width - map_x) / 10.0f);
        int16_t o_y1_rounded = roundf((o.y - map_y) / 10.0f);
        int16_t o_y2_rounded = roundf((o.y + o.height - map_y) / 10.0f);

        for (int16_t x = o_x1_rounded; x <= o_x2_rounded; ++x)
            for (int16_t y = o_y1_rounded; y <= o_y2_rounded; ++y)
                nodes[y * nodes_width + x].obstacle = true;
    }

    PathNode* node_start = &nodes[(int32_t)roundf((starting_pos_y - map_y) / 10.0f) * nodes_width + (int32_t)roundf((starting_pos_x - map_x) / 10.0f)];
    PathNode* node_target = &nodes[(int32_t)roundf((target_y - map_y) / 10.0f) * nodes_width + (int32_t)roundf((target_x - map_x) / 10.0f)];

    auto distance = [](PathNode* a, PathNode* b) {
        return sqrtf((a->x - b->x) * (a->x - b->x) + (a->y - b->y) * (a->y - b->y));
    };

    auto heuristic = [distance](PathNode* a, PathNode* b) {
        return distance(a, b);
    };

    // Setup starting conditions
    PathNode* node_current = node_start;
    node_start->local_goal = 0;
    node_start->global_goal = 10.0f * heuristic(node_start, node_target);

    // Add start node to not tested list - this will ensure it gets tested.
    // As the algorithm progresses, newly discovered nodes get added to this
    // list, and will themselves be tested later
    std::list<PathNode*> list_not_tested_nodes;
    list_not_tested_nodes.push_back(node_start);

    while (!list_not_tested_nodes.empty() && node_current != node_target) { // Find absolutely shortest path // && node_current != nodeEnd)
        // Sort Untested nodes by global goal, so lowest is first
        list_not_tested_nodes.sort([](const PathNode* lhs, const PathNode* rhs) { return lhs->global_goal < rhs->global_goal; });

        node_current = list_not_tested_nodes.front();
        list_not_tested_nodes.pop_front();
        node_current->visited = true; // We only explore a node once

        // Check each of this node's neighbours...
        int16_t x = node_current->x;
        int16_t y = node_current->y;

        if (y > 0)
            astar(node_current, &nodes[(y - 1) * nodes_width + (x + 0)], node_target, &list_not_tested_nodes, nodes, nodes_width, nodes_height);
        if (y < nodes_height - 1)
            astar(node_current, &nodes[(y + 1) * nodes_width + (x + 0)], node_target, &list_not_tested_nodes, nodes, nodes_width, nodes_height);
        if (x > 0)
            astar(node_current, &nodes[(y + 0) * nodes_width + (x - 1)], node_target, &list_not_tested_nodes, nodes, nodes_width, nodes_height);
        if (x < nodes_width - 1)
            astar(node_current, &nodes[(y + 0) * nodes_width + (x + 1)], node_target, &list_not_tested_nodes, nodes, nodes_width, nodes_height);

        if (y > 0 && x > 0)
            astar(node_current, &nodes[(y - 1) * nodes_width + (x - 1)], node_target, &list_not_tested_nodes, nodes, nodes_width, nodes_height);
        if (y < nodes_height - 1 && x > 0)
            astar(node_current, &nodes[(y + 1) * nodes_width + (x - 1)], node_target, &list_not_tested_nodes, nodes, nodes_width, nodes_height);
        if (y > 0 && x < nodes_width - 1)
            astar(node_current, &nodes[(y - 1) * nodes_width + (x + 1)], node_target, &list_not_tested_nodes, nodes, nodes_width, nodes_height);
        if (y < nodes_height - 1 && x < nodes_width - 1)
            astar(node_current, &nodes[(y + 1) * nodes_width + (x + 1)], node_target, &list_not_tested_nodes, nodes, nodes_width, nodes_height);
    }

    if (node_target != nullptr) {
        PathNode* p = node_target;

        while (p->parent != 0) {
            if (path.size() > 50)
                path.pop_back();

            struct Coordinate coordinate = {(int16_t)(map_x + p->x * 10), (int16_t)(map_y + p->y * 10)};
            path.push_front(coordinate);

            // Set next node to this node's parent
            p = (PathNode*)(p->parent << 2);
        }

        struct Coordinate coordinate = {(int16_t)(map_x + p->x * 10), (int16_t)(map_y + p->y * 10)};
        path.push_front(coordinate);
    }
}

void Pathfinder::astar(PathNode* node_current, PathNode* node_neighbour, PathNode* node_target, std::list<PathNode*>* list_not_tested_nodes, PathNode* nodes, int16_t nodes_width, int16_t nodes_height) {
    auto distance = [](PathNode* a, PathNode* b) {
        return sqrtf((a->x - b->x) * (a->x - b->x) + (a->y - b->y) * (a->y - b->y));
    };

    auto heuristic = [distance](PathNode* a, PathNode* b) {
        return distance(a, b);
    };

    if (node_neighbour->obstacle)
        return;

    int16_t neighbour_x = node_neighbour->x;
    int16_t neighbour_y = node_neighbour->y;

    if (neighbour_y > 0) {
        if (nodes[(neighbour_y - 1) * nodes_width + (neighbour_x + 0)].obstacle)
            return;
        if (neighbour_x > 0 && nodes[(neighbour_y - 1) * nodes_width + (neighbour_x - 1)].obstacle)
            return;
        if (neighbour_x < nodes_width - 1 && nodes[(neighbour_y - 1) * nodes_width + (neighbour_x + 1)].obstacle)
            return;
    }

    if (neighbour_y < nodes_height - 1) {
        if (nodes[(neighbour_y + 1) * nodes_width + (neighbour_x + 0)].obstacle)
            return;
        if (neighbour_x > 0 && nodes[(neighbour_y + 1) * nodes_width + (neighbour_x - 1)].obstacle)
            return;
        if (neighbour_x < nodes_width - 1 && nodes[(neighbour_y + 1) * nodes_width + (neighbour_x + 1)].obstacle)
            return;
    }

    if (neighbour_x > 0)
        if (nodes[(neighbour_y + 0) * nodes_width + (neighbour_x - 1)].obstacle)
            return;
    if (neighbour_x < nodes_width - 1)
        if (nodes[(neighbour_y + 0) * nodes_width + (neighbour_x + 1)].obstacle)
            return;

    // ... and only if the neighbour is not visited and is not an obstacle,
    // add it to NotTested List if it is not already in that list
    if (!node_neighbour->visited) {
        bool contains_duplicate = std::any_of(list_not_tested_nodes->begin(), list_not_tested_nodes->end(), [&](const PathNode* node) {
            return node->x == neighbour_x && node->y == neighbour_y;
        });

        if (!contains_duplicate)
            (*list_not_tested_nodes).push_back(node_neighbour);
    }

    // Calculate the neighbours potential lowest parent distance
    float fPossiblyLowerGoal = node_current->local_goal + 10.0f * distance(node_current, node_neighbour);

    // If choosing to path through this node is a lower distance than what
    // the neighbour currently has set, update the neighbour to use this node
    // as the path source, and set its distance scores as necessary
    if (fPossiblyLowerGoal < node_neighbour->local_goal) {
        node_neighbour->parent = (uint32_t)node_current >> 2;
        node_neighbour->local_goal = fPossiblyLowerGoal;

        // The best path length to the neighbour being tested has changed, so
        // update the neighbour's score. The heuristic is used to globally bias
        // the path algorithm, so it knows if its getting better or worse. At some
        // point the algo will realise this path is worse and abandon it, and then go
        // and search along the next best path.
        node_neighbour->global_goal = node_neighbour->local_goal + 10.0f * heuristic(node_neighbour, node_target);
    }
}

Pathfinder::Pathfinder() {
    target_x = 0;
    target_y = 0;
    starting_pos_x = 0;
    starting_pos_y = 0;
    map_x = -30;
    map_y = -30;
    map_width = 60;
    map_height = 60;

    int32_t free_heap = heap_caps_get_largest_free_block(MALLOC_CAP_8BIT);
    nodes_size = max(free_heap - 24576, 10240) / sizeof(PathNode);
    nodes = new PathNode[nodes_size];
}
