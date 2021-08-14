#!/bin/sh

cd "$(dirname "$(readlink -f "$0")")"

set -e
set -x

IMAGE_SIZE=1G
BOOT_PARTITION_SIZE=64MiB
IMAGE_OUTPUT_FILE=raspi_alphabot.img
RASPI_DEBIAN_ROOT=$(mktemp -d)

dd bs=$IMAGE_SIZE seek=1 of=$IMAGE_OUTPUT_FILE count=0
parted -s $IMAGE_OUTPUT_FILE mklabel msdos
parted -m $IMAGE_OUTPUT_FILE print
parted -s $IMAGE_OUTPUT_FILE -- mkpart primary fat32 0% $BOOT_PARTITION_SIZE
parted -m $IMAGE_OUTPUT_FILE print
parted -s $IMAGE_OUTPUT_FILE -- mkpart primary ext2 $BOOT_PARTITION_SIZE 100%
parted -m $IMAGE_OUTPUT_FILE print

KPARTX_OUTPUT=$(kpartx -asv $IMAGE_OUTPUT_FILE)
RASPI_DEB_P1=$(echo $KPARTX_OUTPUT | awk '{print $3}')
RASPI_DEB_P2=$(echo $KPARTX_OUTPUT | awk '{print $12}')

/sbin/mkfs -t vfat -n RASPIFIRM /dev/mapper/$RASPI_DEB_P1
/sbin/mkfs -t ext4 -L RASPIROOT /dev/mapper/$RASPI_DEB_P2

mkdir -p $RASPI_DEBIAN_ROOT
mount /dev/mapper/$RASPI_DEB_P2 $RASPI_DEBIAN_ROOT
mkdir -p $RASPI_DEBIAN_ROOT/boot/firmware
mount /dev/mapper/$RASPI_DEB_P1 $RASPI_DEBIAN_ROOT/boot/firmware

DOCKER_CONTAINER_ID=$(docker run --privileged -it -d -v "$(pwd)":/root/build-debian -v "$(pwd)/../Alphabot.Net:/root/Alphabot.Net" -v $RASPI_DEBIAN_ROOT:$RASPI_DEBIAN_ROOT debian bash)

docker exec $DOCKER_CONTAINER_ID /root/build-debian/build_in_container.sh $RASPI_DEBIAN_ROOT

docker stop $DOCKER_CONTAINER_ID
docker rm $DOCKER_CONTAINER_ID

# Copy vmlinuz, initrd.img and device tree blobs to build directory.
rm -f vmlinuz-*-arm64
cp $RASPI_DEBIAN_ROOT/boot/firmware/vmlinuz-*-arm64 .
rm -f initrd.img
cp $RASPI_DEBIAN_ROOT/boot/firmware/initrd.img .
rm -f bcm2*-rpi-*.dtb
cp $RASPI_DEBIAN_ROOT/boot/firmware/bcm2*-rpi-*.dtb .

umount $RASPI_DEBIAN_ROOT/boot/firmware
umount $RASPI_DEBIAN_ROOT
kpartx -dsv $IMAGE_OUTPUT_FILE
