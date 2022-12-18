# Building the software for the Raspberry Pi variation of the Alphabot

## Prerequisites

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

## Build Image

```console
sudo rpi/sd_image/build.sh
```

## Running on real hardware

Flash rpi/sd_image/raspi_alphabot.img onto an SD card with a size of at least 1GiB. Plug the flashed SD card into your Raspberry Pi 3B/3B+/4B.
