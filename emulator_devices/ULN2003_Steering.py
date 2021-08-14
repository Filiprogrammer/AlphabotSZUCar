#!/usr/bin/env python

class ULN2003_Steering:
    steer_direction = 0
    pin1_state = False
    pin2_state = False
    pin3_state = False
    pin4_state = False

    def __init__(self, pin1, pin2, pin3, pin4):
        self.pin1 = pin1
        self.pin2 = pin2
        self.pin3 = pin3
        self.pin4 = pin4

    def updatePin(self, pin, state):
        i = -1

        if pin == self.pin1:
            pin1_state = state
            if state == True:
                i = 0
        elif pin == self.pin2:
            pin2_state = state
            if state == True:
                i = 1
        elif pin == self.pin3:
            pin3_state = state
            if state == True:
                i = 2
        elif pin == self.pin4:
            pin4_state = state
            if state == True:
                i = 3

        if i == -1:
            return

        stepper_direction = self.steer_direction % 4

        if ((i - stepper_direction) % 2) == 1:
            if i > stepper_direction and not (i == 3 and stepper_direction == 0) or (i == 0 and stepper_direction == 3):
                self.steer_direction += 1
            else:
                self.steer_direction -= 1

            # Under perfect circumstances this would be 289.16, but use 312 to account for bendable plastic.
            self.steer_direction = max(min(self.steer_direction, 312), -312)
            print("steer_direction: " + str(self.steer_direction))

    def getSteerDirection(self):
        return -self.steer_direction
