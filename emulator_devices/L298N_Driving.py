#!/usr/bin/env python

class L298N_Driving:
    left_speed = 0.15
    right_speed = 0.15
    left_direction = 0
    right_direction = 0
    ena_state = False
    in1_state = False
    in2_state = False
    in3_state = False
    in4_state = False
    enb_state = False

    def __init__(self, in1, in2, in3, in4):
        self.in1 = in1
        self.in2 = in2
        self.in3 = in3
        self.in4 = in4

    def updatePin(self, pin, state):
        if pin == self.in1:
            self.in1_state = state
        elif pin == self.in2:
            self.in2_state = state
        elif pin == self.in3:
            self.in3_state = state
        elif pin == self.in4:
            self.in4_state = state

        left_direction_prev = self.left_direction
        right_direction_prev = self.right_direction

        if self.in1_state == False and self.in2_state == True:
            self.left_direction = 1
        elif self.in1_state == True and self.in2_state == False:
            self.left_direction = -1
        else:
            self.left_direction = 0

        if self.in3_state == False and self.in4_state == True:
            self.right_direction = 1
        elif self.in3_state == True and self.in4_state == False:
            self.right_direction = -1
        else:
            self.right_direction = 0

        if left_direction_prev != self.left_direction:
            print("left_direction: " + str(self.left_direction))

        if right_direction_prev != self.right_direction:
            print("right_direction: " + str(self.right_direction))

    def getLeftWheelSpeed(self):
        return self.left_speed * self.left_direction

    def getRightWheelSpeed(self):
        return self.right_speed * self.right_direction
