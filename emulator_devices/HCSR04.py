#!/usr/bin/env python

from threading import Timer, current_thread
from datetime import datetime

class HCSR04:
    distance = 100
    pinTrig_state = False
    pinEcho_state = False

    def __init__(self, pinTrig, pinEcho, pinEchoHandler):
        self.pinTrig = pinTrig
        self.pinEcho = pinEcho
        self.pinEchoHandler = pinEchoHandler

    def updatePin(self, pin, state):
        if pin != self.pinTrig:
            return

        self.pinTrig_state = state

        if state == True and self.pinEcho_state == False:
            self.pinEcho_state = True
            self.pinEchoHandler(True)
            timer = Timer(self.distance / 34300.0, self.__echo)
            timer.start()

    def __echo(self):
        self.pinEcho_state = False
        self.pinEchoHandler(False)
