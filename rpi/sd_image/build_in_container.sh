#!/bin/sh

set -e
set -x

RASPI_DEBIAN_ROOT=$1

echo "deb http://ftp.de.debian.org/debian buster main" >> /etc/apt/sources.list
apt-get -o Acquire::Check-Valid-Until=false -o Acquire::Check-Date=false update
apt-get install -y --no-install-recommends git make gcc device-tree-compiler bison flex libssl-dev libncurses-dev bc wget ca-certificates xz-utils patch cpio

# Fetch ARM cross compiler.
wget https://developer.arm.com/-/media/Files/downloads/gnu-a/10.2-2020.11/binrel/gcc-arm-10.2-2020.11-x86_64-aarch64-none-linux-gnu.tar.xz -P /opt
tar -xf /opt/gcc-arm-10.2-2020.11-x86_64-aarch64-none-linux-gnu.tar.xz -C /opt
rm /opt/gcc-arm-10.2-2020.11-x86_64-aarch64-none-linux-gnu.tar.xz

# Clone the linux kernel.
git clone --depth 1 --branch v5.10.10 git://git.kernel.org/pub/scm/linux/kernel/git/stable/linux.git
cd linux
rm -r .git
export ARCH=arm64
export CROSS_COMPILE=/opt/gcc-arm-10.2-2020.11-x86_64-aarch64-none-linux-gnu/bin/aarch64-none-linux-gnu-
cp /root/build-debian/alphabot_defconfig arch/arm64/configs/
make alphabot_defconfig

# Append gpiomem to the device tree so that the /dev/gpiomem device is created.
printf "/ {\n" >> arch/arm/boot/dts/bcm283x.dtsi
printf "\tsoc {\n" >> arch/arm/boot/dts/bcm283x.dtsi
printf "\t\tgpiomem {\n" >> arch/arm/boot/dts/bcm283x.dtsi
printf '\t\t\tcompatible = "brcm,bcm2835-gpiomem";\n' >> arch/arm/boot/dts/bcm283x.dtsi
printf "\t\t\treg = < 0x7e200000 0x1000 >;\n" >> arch/arm/boot/dts/bcm283x.dtsi
printf "\t\t};\n" >> arch/arm/boot/dts/bcm283x.dtsi
printf "\t};\n" >> arch/arm/boot/dts/bcm283x.dtsi
printf "};\n" >> arch/arm/boot/dts/bcm283x.dtsi

# Patch the kernel to allow the dialout group to access GPIO pins.
patch drivers/gpio/gpiolib-sysfs.c /root/build-debian/kernel-patches/gpiolib-sysfs.c.patch
patch include/linux/device/class.h /root/build-debian/kernel-patches/class.h.patch
patch fs/sysfs/dir.c /root/build-debian/kernel-patches/dir.c.patch

# Build the linux kernel, modules, device tree blobs...
make -j$(nproc)

cd ..

# Build the bcm2835-gpiomem kernel module, which is a driver for the /dev/gpiomem device.
cp -R /root/build-debian/bcm2835-gpiomem bcm2835-gpiomem
cd bcm2835-gpiomem
make KERNEL=$(pwd)/../linux

cd ..

# Remember the kernel release for later.
KERNEL_RELEASE=$(cat linux/include/config/kernel.release)

# Gather all of the important build artefacts and clean up.
mv bcm2835-gpiomem/bcm2835-gpiomem.ko bcm2835-gpiomem.ko
rm -r bcm2835-gpiomem
mv linux/arch/arm64/boot/Image vmlinuz-$KERNEL_RELEASE-arm64
mv linux/arch/arm64/boot/dts/broadcom/*.dtb .
mv linux/modules.builtin .
mv linux/modules.builtin.modinfo .
mv linux/modules.order .
mv linux/System.map .
cd linux
find * -name "*.ko" -exec /bin/sh -c 'mkdir -p ../modules/$(dirname {}) && mv {} ../modules/{}' \;
cd ..
rm -r linux
mkdir -p modules/drivers/char/broadcom
mv bcm2835-gpiomem.ko modules/drivers/char/broadcom/

# Install tools for building a debian image.
apt-get install -y --allow-unauthenticated dosfstools qemu-user-static debootstrap binfmt-support

# Setup basic debian filesystem.
debootstrap --verbose --foreign --arch arm64 --variant=minbase --include=ifupdown,iproute2,iputils-ping,isc-dhcp-client,libiptc0 --components main,contrib,non-free buster $RASPI_DEBIAN_ROOT http://deb.debian.org/debian

rm $RASPI_DEBIAN_ROOT/proc
mkdir $RASPI_DEBIAN_ROOT/proc

# Copy qemu to the target filesystem.
cp /usr/bin/qemu-aarch64-static $RASPI_DEBIAN_ROOT/usr/bin/

# Make sure that binfmt-support service is running, in order to be able to run arm binaries.
service binfmt-support start

# Run second stage of debootstrap
chroot $RASPI_DEBIAN_ROOT /usr/bin/qemu-aarch64-static /bin/sh /debootstrap/debootstrap --second-stage

# Clean up: Remove qemu from the target filesystem.
rm $RASPI_DEBIAN_ROOT/usr/bin/qemu-aarch64-static

# Add some mirrors to the package manager.
echo "deb http://deb.debian.org/debian buster main contrib non-free" > $RASPI_DEBIAN_ROOT/etc/apt/sources.list
echo "deb http://security.debian.org/debian-security buster/updates main contrib non-free" >> $RASPI_DEBIAN_ROOT/etc/apt/sources.list
echo "deb http://ftp.de.debian.org/debian buster main" >> $RASPI_DEBIAN_ROOT/etc/apt/sources.list

mkdir -p $RASPI_DEBIAN_ROOT/etc/initramfs-tools/hooks
cp /root/build-debian/rootfs/etc/initramfs-tools/hooks/rpi-resizerootfs $RASPI_DEBIAN_ROOT/etc/initramfs-tools/hooks/rpi-resizerootfs
chmod 755 $RASPI_DEBIAN_ROOT/etc/initramfs-tools/hooks/rpi-resizerootfs
mkdir -p $RASPI_DEBIAN_ROOT/etc/initramfs-tools/scripts/local-bottom
cp /root/build-debian/rootfs/etc/initramfs-tools/scripts/local-bottom/rpi-resizerootfs $RASPI_DEBIAN_ROOT/etc/initramfs-tools/scripts/local-bottom/rpi-resizerootfs
chmod 755 $RASPI_DEBIAN_ROOT/etc/initramfs-tools/scripts/local-bottom/rpi-resizerootfs
chroot $RASPI_DEBIAN_ROOT apt-get update
chroot $RASPI_DEBIAN_ROOT apt-get -y install ca-certificates dosfstools iw parted ssh xauth- ncurses-term- wpasupplicant raspi3-firmware initramfs-tools kmod linux-base firmware-brcm80211 chrony

# Clean up package manager cache to free up some space.
chroot $RASPI_DEBIAN_ROOT apt-get clean

# Copy over firmware files for the Raspberry Pi 4.
cp /root/build-debian/raspi4-firmware/* $RASPI_DEBIAN_ROOT/boot/firmware/

# Setup config.txt for the bootloader.
echo "arm_control=0x200" > $RASPI_DEBIAN_ROOT/boot/firmware/config.txt
echo "enable_uart=1" >> $RASPI_DEBIAN_ROOT/boot/firmware/config.txt
echo "upstream_kernel=1" >> $RASPI_DEBIAN_ROOT/boot/firmware/config.txt
echo "kernel=vmlinuz-$KERNEL_RELEASE-arm64" >> $RASPI_DEBIAN_ROOT/boot/firmware/config.txt
echo "initramfs initrd.img" >> $RASPI_DEBIAN_ROOT/boot/firmware/config.txt
echo "hdmi_safe=1" >> $RASPI_DEBIAN_ROOT/boot/firmware/config.txt

mkdir -p $RASPI_DEBIAN_ROOT/usr/lib/linux-image-$KERNEL_RELEASE-arm64/broadcom
install -m 755 -o root -g root *.dtb $RASPI_DEBIAN_ROOT/usr/lib/linux-image-$KERNEL_RELEASE-arm64/broadcom/
mv *.dtb $RASPI_DEBIAN_ROOT/boot/firmware/
install -D -m 644 -o root -g root modules.builtin $RASPI_DEBIAN_ROOT/usr/lib/modules/$KERNEL_RELEASE/modules.builtin
install -m 644 -o root -g root modules.builtin.modinfo $RASPI_DEBIAN_ROOT/usr/lib/modules/$KERNEL_RELEASE/
install -m 644 -o root -g root modules.order $RASPI_DEBIAN_ROOT/usr/lib/modules/$KERNEL_RELEASE/
mv modules $RASPI_DEBIAN_ROOT/usr/lib/modules/$KERNEL_RELEASE/kernel
find $RASPI_DEBIAN_ROOT/usr/lib/modules/$KERNEL_RELEASE/kernel/* -name "*.ko" -exec sh -c 'basename {} | cut -f 1 -d '.'' \; | tee -a $RASPI_DEBIAN_ROOT/etc/initramfs-tools/modules $RASPI_DEBIAN_ROOT/etc/modules
install -m 644 -o root -g root System.map $RASPI_DEBIAN_ROOT/boot/

install -m 644 -o root -g root vmlinuz-$KERNEL_RELEASE-arm64 $RASPI_DEBIAN_ROOT/boot/
mv vmlinuz-$KERNEL_RELEASE-arm64 $RASPI_DEBIAN_ROOT/boot/firmware/

cp /root/build-debian/brcm-firmware/* $RASPI_DEBIAN_ROOT/usr/lib/firmware/brcm/
cp /root/build-debian/fix_brcm_missing_firmware $RASPI_DEBIAN_ROOT/etc/initramfs-tools/hooks/fix_brcm_missing_firmware
chmod 755 $RASPI_DEBIAN_ROOT/etc/initramfs-tools/hooks/fix_brcm_missing_firmware

# Build initrd.img.
chroot $RASPI_DEBIAN_ROOT /sbin/mkinitramfs -v -o /boot/initrd.img $KERNEL_RELEASE

# Copy initrd.img to boot partition.
cp $RASPI_DEBIAN_ROOT/boot/initrd.img $RASPI_DEBIAN_ROOT/boot/firmware/initrd.img

# Setup symlinks to vmlinuz and initrd.img.
ln -s /boot/vmlinuz-$KERNEL_RELEASE-arm64 $RASPI_DEBIAN_ROOT/vmlinuz-$KERNEL_RELEASE-arm64
ln -s /boot/initrd.img $RASPI_DEBIAN_ROOT/initrd.img

echo "alphabot" > $RASPI_DEBIAN_ROOT/etc/hostname
sed -i 's,root:[^:]*:,root::,' $RASPI_DEBIAN_ROOT/etc/shadow

# Add alphabot user.
echo "alphabot:x:1000:1000:alphabot:/home/alphabot:/bin/bash" >> $RASPI_DEBIAN_ROOT/etc/passwd

# Set password "alphabot" for alphabot user.
echo 'alphabot:$6$O1jkdLGH5lbZOo5U$AIxWqyTNc3XvdMcHLRrS28qDMDxhC8Ysh8XKNATcuXx4QRO43wnGGmxWn4ccwiJsMMsH84C1PVr5KtyKQ2gBT.:18808:0:99999:0:::' >> $RASPI_DEBIAN_ROOT/etc/shadow

# Create home directory for alphabot user.
mkdir $RASPI_DEBIAN_ROOT/home/alphabot
chroot $RASPI_DEBIAN_ROOT chown alphabot /home/alphabot
chroot $RASPI_DEBIAN_ROOT chgrp 1000 /home/alphabot

install -m 644 -o root -g root /root/build-debian/rootfs/etc/fstab $RASPI_DEBIAN_ROOT/etc/fstab
install -D -m 644 -o root -g root /root/build-debian/rootfs/etc/network/interfaces.d/eth0 $RASPI_DEBIAN_ROOT/etc/network/interfaces.d/eth0
install -D -m 644 -o root -g root /root/build-debian/rootfs/etc/network/interfaces.d/usb0 $RASPI_DEBIAN_ROOT/etc/network/interfaces.d/usb0
install -D -m 600 -o root -g root /root/build-debian/rootfs/etc/network/interfaces.d/wlan0 $RASPI_DEBIAN_ROOT/etc/network/interfaces.d/wlan0
install -m 755 -o root -g root /root/build-debian/rootfs/usr/local/sbin/rpi-set-sysconf $RASPI_DEBIAN_ROOT/usr/local/sbin/rpi-set-sysconf
install -m 644 -o root -g root /root/build-debian/rootfs/etc/systemd/system/rpi-set-sysconf.service $RASPI_DEBIAN_ROOT/etc/systemd/system/
install -m 644 -o root -g root /root/build-debian/rootfs/boot/firmware/sysconf.txt $RASPI_DEBIAN_ROOT/boot/firmware/sysconf.txt
install -m 644 -o root -g root /root/build-debian/rootfs/etc/systemd/journald.conf $RASPI_DEBIAN_ROOT/etc/systemd/journald.conf

mkdir -p $RASPI_DEBIAN_ROOT/etc/systemd/system/basic.target.requires/
ln -s /etc/systemd/system/rpi-set-sysconf.service $RASPI_DEBIAN_ROOT/etc/systemd/system/basic.target.requires/rpi-set-sysconf.service

rm -f $RASPI_DEBIAN_ROOT/etc/initramfs-tools/hooks/rpi-resizerootfs
rm -f $RASPI_DEBIAN_ROOT/etc/initramfs-tools/scripts/local-bottom/rpi-resizerootfs
install -m 644 -o root -g root /root/build-debian/rootfs/etc/systemd/system/rpi-reconfigure-raspi-firmware.service $RASPI_DEBIAN_ROOT/etc/systemd/system/
mkdir -p $RASPI_DEBIAN_ROOT/etc/systemd/system/multi-user.target.requires/
ln -s /etc/systemd/system/rpi-reconfigure-raspi-firmware.service $RASPI_DEBIAN_ROOT/etc/systemd/system/multi-user.target.requires/rpi-reconfigure-raspi-firmware.service
install -m 644 -o root -g root /root/build-debian/rootfs/etc/systemd/system/rpi-generate-ssh-host-keys.service $RASPI_DEBIAN_ROOT/etc/systemd/system/
ln -s /etc/systemd/system/rpi-generate-ssh-host-keys.service $RASPI_DEBIAN_ROOT/etc/systemd/system/multi-user.target.requires/rpi-generate-ssh-host-keys.service
rm -f $RASPI_DEBIAN_ROOT/etc/ssh/ssh_host_*_key*

# Create the raspberrypi-net-mods service to automatically move the wpa_supplicant.conf file from the boot partition to /etc/wpa_supplicant/ on startup.
install -m 644 -o root -g root /root/build-debian/rootfs/etc/systemd/system/raspberrypi-net-mods.service $RASPI_DEBIAN_ROOT/etc/systemd/system/
ln -s /etc/systemd/system/raspberrypi-net-mods.service $RASPI_DEBIAN_ROOT/etc/systemd/system/multi-user.target.requires/raspberrypi-net-mods.service

# Create the wlan-hack service to automatically establish a WLAN connection on startup.
install -m 644 -o root -g root /root/build-debian/rootfs/etc/systemd/system/wlan-hack.service $RASPI_DEBIAN_ROOT/etc/systemd/system/
ln -s /etc/systemd/system/wlan-hack.service $RASPI_DEBIAN_ROOT/etc/systemd/system/multi-user.target.wants/wlan-hack.service

# Remove wpa_supplicant service because it is replaced by the wlan-hack service.
rm $RASPI_DEBIAN_ROOT/etc/systemd/system/multi-user.target.wants/wpa_supplicant.service
rm $RASPI_DEBIAN_ROOT/usr/lib/systemd/system/wpa_supplicant.service

# Remove systemd-timesync services, which are replaced by chrony.
rm $RASPI_DEBIAN_ROOT/usr/lib/systemd/system/systemd-time-wait-sync.service
rm $RASPI_DEBIAN_ROOT/etc/systemd/system/sysinit.target.wants/systemd-timesyncd.service
rm $RASPI_DEBIAN_ROOT/etc/systemd/system/dbus-org.freedesktop.timesync1.service
rm $RASPI_DEBIAN_ROOT/usr/lib/systemd/system/systemd-timesyncd.service

# Remove apt-daily services/timers.
rm $RASPI_DEBIAN_ROOT/etc/systemd/system/timers.target.wants/apt-daily.timer
rm $RASPI_DEBIAN_ROOT/etc/systemd/system/timers.target.wants/apt-daily-upgrade.timer
rm $RASPI_DEBIAN_ROOT/usr/lib/systemd/system/apt-daily.timer
rm $RASPI_DEBIAN_ROOT/usr/lib/systemd/system/apt-daily-upgrade.timer
rm $RASPI_DEBIAN_ROOT/usr/lib/systemd/system/apt-daily.service
rm $RASPI_DEBIAN_ROOT/usr/lib/systemd/system/apt-daily-upgrade.service

# Create wpa_supplicant.conf template in the boot partition.
echo "country=AT" > $RASPI_DEBIAN_ROOT/boot/firmware/wpa_supplicant.conf
echo "ctrl_interface=DIR=/var/run/wpa_supplicant GROUP=netdev" >> $RASPI_DEBIAN_ROOT/boot/firmware/wpa_supplicant.conf
echo "update_config=1" >> $RASPI_DEBIAN_ROOT/boot/firmware/wpa_supplicant.conf
echo "network={" >> $RASPI_DEBIAN_ROOT/boot/firmware/wpa_supplicant.conf
echo "    ssid=\"YourSSID\"" >> $RASPI_DEBIAN_ROOT/boot/firmware/wpa_supplicant.conf
echo "    psk=\"YourPassword\"" >> $RASPI_DEBIAN_ROOT/boot/firmware/wpa_supplicant.conf
echo "    key_mgmt=WPA-PSK" >> $RASPI_DEBIAN_ROOT/boot/firmware/wpa_supplicant.conf
echo "}" >> $RASPI_DEBIAN_ROOT/boot/firmware/wpa_supplicant.conf

# Install i2c-tools & nano.
chroot $RASPI_DEBIAN_ROOT apt-get -y --no-install-recommends install i2c-tools nano

# Clean up package manager cache to free up some space.
chroot $RASPI_DEBIAN_ROOT apt-get clean
chroot $RASPI_DEBIAN_ROOT rm -rf /var/lib/apt/lists

# Add alphabot user to i2c group.
chroot $RASPI_DEBIAN_ROOT usermod -aG i2c alphabot

# Add alphabot user to dialout (gpio) group.
chroot $RASPI_DEBIAN_ROOT usermod -aG dialout alphabot

# Add udev rule for GPIO sysfs that ensures, that members of the dialout group can access GPIO pins.
echo "SUBSYSTEM==\"bcm2835-gpiomem\", KERNEL==\"gpiomem\", GROUP=\"dialout\", MODE=\"0660\"" > $RASPI_DEBIAN_ROOT/etc/udev/rules.d/60-rpi.gpio-common.rules
echo "SUBSYSTEM==\"gpio\", KERNEL==\"gpiochip*\", ACTION==\"add\", PROGRAM=\"/bin/sh -c 'chgrp dialout /sys/class/gpio/export /sys/class/gpio/unexport'\"" >> $RASPI_DEBIAN_ROOT/etc/udev/rules.d/60-rpi.gpio-common.rules

# Setup dotnet.
wget -nc https://download.visualstudio.microsoft.com/download/pr/c1f15b51-5e8c-4e6c-a803-241790159af3/b5cbcc59f67089d760e0ed4a714c47ed/dotnet-sdk-5.0.202-linux-arm64.tar.gz
mkdir -p $RASPI_DEBIAN_ROOT/opt/dotnet/
tar -xf dotnet-sdk-5.0.202-linux-arm64.tar.gz -C $RASPI_DEBIAN_ROOT/opt/dotnet/
ln -s /opt/dotnet/dotnet $RASPI_DEBIAN_ROOT/usr/local/bin
echo 'export DOTNET_ROOT=/opt/dotnet' >> $RASPI_DEBIAN_ROOT/etc/profile
echo 'export DOTNET_SYSTEM_GLOBALIZATION_INVARIANT=1' >> $RASPI_DEBIAN_ROOT/etc/profile
echo 'export DOTNET_CLI_TELEMETRY_OPTOUT=1' >> $RASPI_DEBIAN_ROOT/etc/profile

cd /root
find Alphabot.Net -type f -not -path '*/bin/*' -not -path '*/obj/*' -not -path '*/.vs/*' -print | cpio -pd $RASPI_DEBIAN_ROOT/home/alphabot
chroot $RASPI_DEBIAN_ROOT chown -R alphabot /home/alphabot/Alphabot.Net
chroot $RASPI_DEBIAN_ROOT chgrp -R 1000 /home/alphabot/Alphabot.Net
chroot $RASPI_DEBIAN_ROOT mount -t proc proc /proc
install -m 644 -o root -g root /root/build-debian/rootfs/etc/systemd/system/alphabot-dotnet.service $RASPI_DEBIAN_ROOT/etc/systemd/system/
ln -s /etc/systemd/system/alphabot-dotnet.service $RASPI_DEBIAN_ROOT/etc/systemd/system/basic.target.requires/alphabot-dotnet.service

# Setup kernel command line.
echo "console=ttyS1,115200 console=tty1 loglevel=8 root=LABEL=RASPIROOT rw elevator=deadline fsck.repair=yes memtest=1 net.ifnames=0 rootwait" > $RASPI_DEBIAN_ROOT/boot/firmware/cmdline.txt

# Clear files that will be auto-generated upon first boot.
rm $RASPI_DEBIAN_ROOT/etc/resolv.conf
chroot $RASPI_DEBIAN_ROOT rm -f /etc/machine-id /var/lib/dbus/machine-id
