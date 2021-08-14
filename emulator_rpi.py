#!/usr/bin/env python

SSH_PORT = 5022
QTEST_PORT = 5332
ALPHABOT_PORT = 9000
ULN2003_PIN1 = 13
ULN2003_PIN2 = 19
ULN2003_PIN3 = 26
ULN2003_PIN4 = 16
HCSR04_TRIG = 23
HCSR04_ECHO = 24
#L298N_ENA = 
L298N_IN1 = 5
L298N_IN2 = 6
L298N_IN3 = 27
L298N_IN4 = 22
#L298N_ENB = 

car_x = 300
car_y = 300
car_angle = 35
steer_direction_angle = 0

room_width = 600
room_height = 600
gui_zoom = 1
fps = 20

car_wheel_circumference = 24.5
#car_wheel_max_rpm = 240 # According to the Amazon Product Description
car_wheel_max_rpm = 300 # Accoring to tests with a 12V power supply

import os
import socket
import subprocess
import sys
import time
import math
from threading import Thread
from emulator_devices import ULN2003_Steering
from emulator_devices import HCSR04
from emulator_devices import L298N_Driving
from colorama import init, Fore, Back, Style
import tkinter
from tkinter import *

def usage(arg0):
    print("Usage: " + arg0 + " <options>")
    print("Options:")
    print("  --headless     Run the emulator without a GUI")
    print("  --help         Display this help and exit")
    print("  --test-all     Run all tests in emulator_tests")
    print("  --test <file>  Run a certain test")
    print("  --auto-exit    Exit automatically once either all tests pass or a test fails")

def handle_socket_communication():
    sock = socket.socket(socket.AF_INET, socket.SOCK_STREAM)
    sock.bind(('127.0.0.1', QTEST_PORT))
    sock.listen(1)
    print("Waiting for connection...")
    global connection
    connection, client_address = sock.accept()
    global is_connected
    is_connected = True
    print(str(client_address) + " connected")

    try:
        while True:
            data = connection.recv(64)

            if len(data) == 0:
                break

            print(data)
    finally:
        print("Closing connection...")
        connection.close()

def hcsr04_front_pinEchoHandler(state):
    print("Front Echo State: " + str(state))
    if is_connected:
        m = 0x3f200000 + int(HCSR04_ECHO / 32)
        if state:
            m += 0x1c
        else:
            m += 0x28
        gpio = 1 << (HCSR04_ECHO % 32)
        connection.sendall('writel 0x{:x} 0x{:x}\n'.format(m, gpio).encode("ascii"))

def qemu_stdout_print():
    alphabot_dotnet_started = False
    line = ""

    while True:
        out = qemu_pipe.stdout.read(1)

        if not alphabot_dotnet_started:
            if out == '\n':
                line = '\n'
            else:
                line = line + out.decode("utf-8", 'ignore')

        if len(out) == 0 and qemu_pipe.poll() != None:
            break
        if out != '':
            print(out.decode('utf-8', 'ignore'), end = '')

            if not alphabot_dotnet_started:
                if line[-78:] == ": Information : ClientHandler:AcceptClients:waiting for incoming connection...":
                    tests_thread = Thread(target=run_tests)
                    tests_thread.start()
                    alphabot_dotnet_started = True

def qemu_stderr_print():
    while True:
        err = qemu_pipe.stderr.readline()

        if len(err) == 0 and qemu_pipe.poll() != None:
            break

        err_str = err.decode("utf-8", 'ignore')

        if err_str != '':
            try:
                if err_str.startswith("gpset "):
                    gpionum = int(err_str[6:])
                    uln2003.updatePin(gpionum, True)
                    hcsr04_front.updatePin(gpionum, True)
                    l298n_driving.updatePin(gpionum, True)
                elif err_str.startswith("gpclr "):
                    gpionum = int(err_str[6:])
                    uln2003.updatePin(gpionum, False)
                    hcsr04_front.updatePin(gpionum, False)
                    l298n_driving.updatePin(gpionum, False)
            except ValueError:
                pass

            print(Fore.RED + err_str + Style.RESET_ALL, end = '')

def run_tests():
    for file in test_files:
        print(Fore.YELLOW + "Running test " + file + Style.RESET_ALL)
        try:
            exec(open(file).read())
            print(Fore.YELLOW + "Test " + file + Fore.GREEN + " PASSED" + Style.RESET_ALL)
        except Exception as e:
            print(e)
            print(Fore.YELLOW + "Test " + file + Fore.RED + " FAILED" + Style.RESET_ALL)

            if auto_exit:
                os._exit(2)

    if auto_exit:
        os._exit(0)

def rotate_verticies(points, angle, center):
    angle = math.radians(angle)
    cos_val = math.cos(angle)
    sin_val = math.sin(angle)
    cx, cy = center
    new_points = []
    for x_old, y_old in points:
        x_old -= cx
        y_old -= cy
        x_new = x_old * cos_val - y_old * sin_val
        y_new = x_old * sin_val + y_old * cos_val
        new_points.append([x_new + cx, y_new + cy])
    return new_points

def move_verticies(points, vec):
    vx, vy = vec
    new_points = []
    for x, y in points:
        new_points.append([x + vx, y + vy])
    return new_points

def scale_verticies(points, scale):
    new_points = []
    for x, y in points:
        new_points.append([x * scale, y * scale])
    return new_points

def on_closing():
    global window
    qemu_pipe.kill()
    os._exit(0)

def run_car_simulation(headless):
    if not headless:
        global window
        window = tkinter.Tk()
        window.title("Alphabot Raspberry Pi Emulator")
        window.geometry(str(room_width * gui_zoom) + "x" + str(room_height * gui_zoom + 20))
        window.protocol("WM_DELETE_WINDOW", on_closing)
        label = Label(window, text="Simulation Time: 0")
        label.pack()

    simulation_time = 0
    start_time = time.time()
    car_speed = 0

    while True:
        if not headless:
            canvas = Canvas(window)
            canvas.pack(fill=BOTH, expand=1)

        desired_car_speed = car_wheel_circumference * (car_wheel_max_rpm / 60 / fps) * ((l298n_driving.getLeftWheelSpeed() + l298n_driving.getRightWheelSpeed()) / 2)
        car_speed = (desired_car_speed + car_speed * 4) / 5
        global steer_direction_angle
        global car_x
        global car_y
        global car_angle
        steer_direction_angle = uln2003.getSteerDirection() / 5.7
        car_x += car_speed * math.sin(math.radians(car_angle + steer_direction_angle))
        car_y -= car_speed * math.cos(math.radians(car_angle + steer_direction_angle))
        car_angle += car_speed * steer_direction_angle / 20.0

        if not headless:
            for x in range(100 * gui_zoom, room_width * gui_zoom, 100 * gui_zoom):
                canvas.create_line(x, 0, x, room_height * gui_zoom, fill="dim gray")

            for y in range(100 * gui_zoom, room_height * gui_zoom, 100 * gui_zoom):
                canvas.create_line(0, y, room_width * gui_zoom, y, fill="dim gray")

            car_base_verticies = [[-45, -80], [0, -100], [45, -80], [20, -80], [20, -40], [60, -25], [25, -10], [25, 20], [65, 60], [65, 180], [30, 180], [20, 210], [75, 210], [75, 265], [-75, 265], [-75, 210], [-20, 210], [-30, 180], [-65, 180], [-65, 60], [-25, 20], [-25, -10], [-60, -25], [-20, -40], [-20, -80]]
            car_wheel_verticies = [[-18, -38], [18, -38], [18, 38], [-18, 38]]

            car_wheel_left_front_verticies = rotate_verticies(car_wheel_verticies, steer_direction_angle, [20, 0])
            car_wheel_right_front_verticies = rotate_verticies(car_wheel_verticies, steer_direction_angle, [-20, 0])

            car_wheel_left_front_verticies = move_verticies(car_wheel_left_front_verticies, [-80, -25])
            car_wheel_right_front_verticies = move_verticies(car_wheel_right_front_verticies, [80, -25])
            car_wheel_left_back_verticies = move_verticies(car_wheel_verticies, [-85, 160])
            car_wheel_right_back_verticies = move_verticies(car_wheel_verticies, [85, 160])

            car_base = canvas.create_polygon(scale_verticies(move_verticies(rotate_verticies(car_base_verticies, car_angle, [0, 0]), [car_x * 10, car_y * 10]), 0.1 * gui_zoom), fill='deep sky blue')
            car_wheel_left_front = canvas.create_polygon(scale_verticies(move_verticies(rotate_verticies(car_wheel_left_front_verticies, car_angle, [0, 0]), [car_x * 10, car_y * 10]), 0.1 * gui_zoom), fill="black")
            car_wheel_right_front = canvas.create_polygon(scale_verticies(move_verticies(rotate_verticies(car_wheel_right_front_verticies, car_angle, [0, 0]), [car_x * 10, car_y * 10]), 0.1 * gui_zoom), fill="black")
            car_wheel_left_back = canvas.create_polygon(scale_verticies(move_verticies(rotate_verticies(car_wheel_left_back_verticies, car_angle, [0, 0]), [car_x * 10, car_y * 10]), 0.1 * gui_zoom), fill="black")
            car_wheel_right_back = canvas.create_polygon(scale_verticies(move_verticies(rotate_verticies(car_wheel_right_back_verticies, car_angle, [0, 0]), [car_x * 10, car_y * 10]), 0.1 * gui_zoom), fill="black")
            label.config(text="Simulation Time: " + str("%.2f" % simulation_time))
            window.update()
            canvas.destroy()

        new_time = time.time()

        if (start_time + simulation_time) > new_time:
            time.sleep((start_time + simulation_time) - new_time)

        last_time = new_time

        simulation_time += (1 / fps)

headless = False
test_files = []
tests_all = False
auto_exit = False

i = 0

while i < (len(sys.argv) - 1):
    arg = sys.argv[i + 1]

    if arg == "--headless":
        headless = True
    elif arg == "--test-all":
        tests_all = True

        for file in os.listdir("emulator_tests"):
            if file.endswith(".py"):
                test_files.append(os.path.join("emulator_tests", file))
    elif arg == "--test":
        if (i + 2) == len(sys.argv):
            print("Missing test file after --test")
            sys.exit(1)

        i += 1
        file = os.path.join("emulator_tests", sys.argv[i + 1]) + ".py"

        if not tests_all:
            if os.path.isfile(file):
                test_files.append(file)
            else:
                print("Test file \"" + file + "\" does not exist")
                sys.exit(1)
    elif arg == "--auto-exit":
        auto_exit = True
    elif arg == "--help":
        usage(sys.argv[0])
        sys.exit(0)
    else:
        print("Unknown argument: \"" + str(arg) + "\"")
        usage(sys.argv[0])
        sys.exit(1)

    i += 1

init()

os.chdir(os.path.dirname(os.path.realpath(__file__)))

qemu_bin = "qemu_rpi/build/qemu-system-aarch64"

if os.name == 'nt':
    qemu_bin = qemu_bin + ".exe"

if not os.path.isfile(qemu_bin):
    print("qemu-system-aarch64 binary not found. Build qemu_rpi first!")
    sys.exit(1)

uln2003 = ULN2003_Steering.ULN2003_Steering(ULN2003_PIN1, ULN2003_PIN2, ULN2003_PIN3, ULN2003_PIN4)
hcsr04_front = HCSR04.HCSR04(HCSR04_TRIG, HCSR04_ECHO, hcsr04_front_pinEchoHandler)
l298n_driving = L298N_Driving.L298N_Driving(L298N_IN1, L298N_IN2, L298N_IN3, L298N_IN4)

is_connected = False

socket_communication_thread = Thread(target=handle_socket_communication)
socket_communication_thread.setDaemon(True)
socket_communication_thread.start()

qemu_cmd = [qemu_bin,
            "-kernel", "rpi/sd_image/vmlinuz-5.10.10-arm64",
            "-initrd", "rpi/sd_image/initrd.img",
            "-dtb", "rpi/sd_image/bcm2837-rpi-3-b.dtb",
            "-m", "1024",
            "-M", "raspi3",
            "-nographic",
            "-append", "rw root=/dev/mmcblk0p2 console=ttyAMA0,115200 loglevel=8 rootwait fsck.repair=yes memtest=1 net.ifnames=0",
            "-drive", "file=rpi/sd_image/raspi_alphabot.img,format=raw,if=sd,index=0",
            "-device", "usb-net,netdev=net0",
            "-netdev", "user,id=net0,hostfwd=tcp::" + str(SSH_PORT) + "-:22,hostfwd=tcp::" + str(ALPHABOT_PORT) + "-:9000",
            "-no-reboot",
            "-qtest", "tcp:127.0.0.1:" + str(QTEST_PORT)]

qemu_pipe = subprocess.Popen(qemu_cmd, stdin=subprocess.PIPE, stdout=subprocess.PIPE, stderr=subprocess.PIPE)

qemu_stdout_print_thread = Thread(target=qemu_stdout_print)
qemu_stdout_print_thread.setDaemon(True)
qemu_stdout_print_thread.start()

qemu_stderr_print_thread = Thread(target=qemu_stderr_print)
qemu_stderr_print_thread.setDaemon(True)
qemu_stderr_print_thread.start()

gui_thread = Thread(target=run_car_simulation, args=[headless])
gui_thread.setDaemon(True)
gui_thread.start()

err_line = ""

try:
    qemu_pipe.wait()
except KeyboardInterrupt:
    os._exit(0)
