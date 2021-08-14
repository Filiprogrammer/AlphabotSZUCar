#!/usr/bin/env python

import socket
import time

sock = socket.socket(socket.AF_INET, socket.SOCK_STREAM)
sock.connect(("127.0.0.1", 9000))

# Wait for potential initialization and calibration
time.sleep(7.5)

sock.sendall("left".encode("ascii"))
time.sleep(20)
assert round(steer_direction_angle) == -10, "Expected steering angle 10 when steering to the left, got " + str(steer_direction_angle)

sock.sendall("right".encode("ascii"))
time.sleep(5)
assert round(steer_direction_angle) == 10, "Expected steering angle -10 when steering to the right, got " + str(steer_direction_angle)

sock.sendall("center".encode("ascii"))
time.sleep(2.5)
assert round(steer_direction_angle) == 0, "Expected steering angle 0 when steering to the center, got " + str(steer_direction_angle)
