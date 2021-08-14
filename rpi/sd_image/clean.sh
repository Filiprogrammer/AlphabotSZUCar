#!/bin/sh

docker stop debian
docker rm debian
umount /tmp/raspi_debian/.//boot/firmware
umount /tmp/raspi_debian
kpartx -dsv raspi_alphabot.img
rm -f raspi_alphabot.img
rm -f dotnet-sdk-5.0.202-linux-arm64.tar.gz
rm -f vmlinuz-*-arm64
rm -f initrd.img
rm -f bcm2*-rpi-*.dtb
