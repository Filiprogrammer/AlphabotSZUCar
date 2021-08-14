PWD := $(shell pwd)
obj-m += bcm2835-gpiomem.o

all:
	make ARCH=arm64 -C $(KERNEL) M=$(PWD) modules

clean:
	make -C $(KERNEL) M=$(PWD) clean
