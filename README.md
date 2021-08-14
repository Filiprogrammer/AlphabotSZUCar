AlphabotSZUCar
==============

[![CI](https://github.com/Filiprogrammer/AlphabotSZUCar/actions/workflows/main.yml/badge.svg)](https://github.com/Filiprogrammer/AlphabotSZUCar/actions/workflows/main.yml)

The AlphabotSZUCar is a remote-controlled model car with some functions that also let it act autonomously in various ways.

There exist two major variations of this car:

- Controlled by Raspberry Pi 3B/3B+/4B (single-board computer)
- Controlled by ESP32 (microcontroller)

Features
--------

* Communication through WiFi
* Communication through Bluetooth Low Energy
* Remote controlled manual driving
* Obstacle detection & Collision avoidance
* Position and orientation system
* Pathfinding with obstacle avoidance
* Autonomous driving

Assembling the hardware
-----------------------

*TODO*

Building the software
---------------------

### Raspberry Pi 3B/3B+/4B

#### Prerequisites

Make sure that you have at least 3GB of free storage space.

Make sure that you have installed the following prerequisites:

* Docker - [Docker Engine Install Instructions](https://docs.docker.com/engine/install/)
* parted

Debian:

```console
sudo apt-get install parted
```

Arch Linux:

```console
sudo pacman -Sy parted
```

Fedora:

```console
sudo yum install parted
```

* kpartx

Debian:

```console
sudo apt install kpartx
```

Arch Linux:

```console
sudo pacman -Sy multipath-tools
```

Fedora:

```console
sudo yum install kpartx
```

#### Build Image

```console
sudo rpi/sd_image/build.sh
```

### ESP32

*TODO*

Running on real hardware
------------------------

### Raspberry Pi 3B 3B/3B+/4B

Flash rpi/sd_image/raspi_alphabot.img onto an SD card with a size of at least 1GiB.

### ESP32

*TODO*

Emulating
---------

### Prerequisites

* Python3
* colorama python package

```console
pip install colorama
```

#### Building the Raspberry Pi emulator

```console
mkdir qemu_rpi/build
cd qemu_rpi/build
../configure --target-list=aarch64-softmmu --static
make -j$(nproc)
```

#### Building the ESP32 emulator

*TODO*

### Run Raspberry Pi Emulator

```console
python3 emulator_rpi.py
```

To automatically test the functionality of the system run:

```console
python3 emulator_rpi.py --test-all
```

To see more options use:

```console
python3 emulator_rpi.py --help
```

### Run ESP32 Emulator

*TODO*

Connecting to Alphabot with a client
------------------------------------

### WiFi

Make sure that your client runs in the same network as the Alphabot.

#### AlphabotTcpClient

```console
dotnet clients/AlphabotTcpClient/AlphabotTcpClient/bin/Release/netcoreapp2.1/AlphabotTcpClient.dll alphabot 9000
```

#### Netcat

```console
nc alphabot 9000
```

### Bluetooth Low Energy

#### AlphabotAndroidClient

To open the project in Android Studio, click **File > New > Import Project** and select the AlphabotAndroidClient project folder.

#### WPFClient
