# Alphabot Assembly Guide

## Parts list

|                     Part                     |   Amount    |
|----------------------------------------------|-------------|
| TT DC Gearbox Motor                          |     2x      |
| DIN 912 M3x10 screw                          |     9x      |
| 28BYJ-48 stepper motor                       |     2x      |
| DIN 912 M3x5 screw                           |     2x      |
| Ball bearing 10x15x4                         |     4x      |
| Ball bearing 5x11x4                          |     4x      |
| Ball bearing 5x8x2.5                         |     2x      |
| DIN 912 M3x14 screw                          |     4x      |
| TF-Luna LiDAR sensor                         |     1x      |
| M2x6 screw                                   |     4x      |
| 3S 18650 battery holder case box (7.6x6x2cm) |     1x      |
| M2.5x5 screw                                 |     2x      |
| 18650 Lithium-Ion Battery 3.7V               |     3x      |
| DIN 912 M3x8 screw                           |     2x      |
| DIN 912 M3x6 screw                           |     2x      |
| L298N DC motor driver module                 |     1x      |
| M2 screw                                     |     4x      |
| M2 male-female hex spacer                    |     4x      |
| M2 nut                                       |     2x      |
| M2.5 screw                                   |     5x      |
| M2.5 male-female hex spacer                  |     4x      |
| ESP-WROOM-32 board                           |     1x      |
| Arduino Pro Mini 16MHz 5V                    |     4x      |
| 15mm*21mm 2 pins rocker switch               |     1x      |
| CZM5/2 terminal block                        |     1x      |
| 2.54mm female 12 pin header                  |     8x      |
| 2.54mm female 19 pin header                  |     2x      |
| 2.54mm any 6 pin header                      |     3x      |
| 2.54mm any 4 pin header                      |     1x      |
| S5B-XH-A connector header                    |     2x      |
| DIN 912 M3x5 screw                           |     6x      |
| wire                                         |     16x     |
| FC-03 infrared sensor                        |     1x      |

## Custom PCBs to produce

|          PCB           | Amount |
|------------------------|--------|
| pcb/dwm1000_adapter    |   4x   |
| pcb/alphabot_esp32_pcb |   1x   |

## Parts to 3D print

|                            Part                         | Amount | Material |
|---------------------------------------------------------|--------|----------|
| stl/dwm1000.anchor.adapter.board.mount.stl              |   3x   |   PLA    |
| stl/vehicle/base/alphabot.4Wheel.base.Top.esp32.stl     |   1x   |   PLA    |
| stl/vehicle/base/alphabot.4Wheel.base.suspension.V6.stl |   1x   |   PLA    |
| stl/vehicle/base/frontwheel.suspension.distance.stl     |   3x   |   PLA    |
| stl/vehicle/steering/steering.axle.stl                  |   1x   |   PLA    |
| stl/vehicle/steering/steering.stepper.pin.stl           |   1x   |   PLA    |
| stl/vehicle/steering/suspension.left.V4.stl             |   1x   |   PLA    |
| stl/vehicle/steering/suspension.right.V4.stl            |   1x   |   PLA    |
| stl/vehicle/wheel/rim.front.62mm.V2.stl                 |   2x   |   PLA    |
| stl/vehicle/wheel/rim.back.62mm.V2.stl                  |   2x   |   PLA    |
| stl/vehicle/wheel/rim.62mm.front.bearing.cage.V2.stl    |   2x   |   PLA    |
| stl/vehicle/wheel/rim.62mm.back.bearing.cage.V2.stl     |   2x   |   PLA    |
| stl/vehicle/wheel/rim.62mm.front.nut.stl                |   4x   |   PLA    |
| stl/vehicle/wheel/tire.base.62.78.stl                   |   4x   | Filaflex |
| stl/vehicle/wheel/rim.axle.back.stl                     |   2x   |   PLA    |
| stl/vehicle/addon/wingv4.stl                            |   1x   |   PLA    |
| stl/vehicle/addon/wing.holder.V4.hc-sr04.stl            |   1x   |   PLA    |
| stl/vehicle/addon/dwm1000.tag.adapter.board.mount.stl   |   1x   |   PLA    |
| stl/vehicle/addon/bumper.lidar.holder.stl               |   1x   |   PLA    |
| stl/vehicle/addon/bumper.lidar.front.stl                |   1x   |   PLA    |
| stl/vehicle/addon/wheel.encoder.stl                     |   1x   |   PLA    |

## PCB soldering

### DWM1000 Adapter

After ordering the PCB with assembly service, the board should look like this:

![positioning_module_assembly_1](positioning_module_assembly_1.png)

Now solder pin headers for the Arduino Pro Mini to the board as seen in the following picture.

![positioning_module_assembly_2](positioning_module_assembly_2.png)

Finally plug the Arduino Pro Mini into the DWM1000 Adapter.

![positioning_module_assembly_3](positioning_module_assembly_3.png)

### Alphabot ESP32 PCB

After ordering the PCB with assembly service, the board should look like this:

![T-U13W273319A-01](T-U13W273319A-01.jpg)

Now solder the pin headers for the ESP-WROOM-32, for the L298N motor driver, for the positioning module and for the TF-Luna LiDAR sensor to the board.

![alphabot_esp32_pcb_1](alphabot_esp32_pcb_1.png)

Also solder two S5B-XH-A connector headers for the stepper motors to the board as seen in the following picture.

![alphabot_esp32_pcb_2](alphabot_esp32_pcb_2.png)

Solder a CZM5/2 terminal block to the 12V power pins of the board.

![alphabot_esp32_pcb_3](alphabot_esp32_pcb_3.png)

Finally you can plug the ESP-WROOM-32 into the Alphabot ESP32 PCB.

![alphabot_esp32_pcb_4](alphabot_esp32_pcb_4.png)

## Assembly

### Alphabot car

Start with soldering the gear motors. Both contacts of the motor are to be soldered to a separate wire.

![20221114_232438](20221114_232438.jpg)

Once the gear motors are soldered, insert them on the bottom of the base plate (stl/vehicle/base/alphabot.4Wheel.base.Top.esp32.stl) of the Alphabot.

![image83](image83.png)

Now take the wheel encoder (stl/vehicle/addon/wheel.encoder.stl) and put it onto the hub of the motor as seen in the pictures below.

![image_wheel_encoder_1](image_wheel_encoder_1.png)

![image_wheel_encoder_2](image_wheel_encoder_2.png)

Next, pull a tire (stl/vehicle/wheel/tire.base.62.78.stl) onto a rear wheel. (stl/vehicle/wheel/rim.back.62mm.V2.stl)

![image84](image84.png)

This results in a ready rear wheel.

![image85](image85.png)

Attach a rear wheel to the gear motor on each side.

![image86](image86.png)

When mounting the rear wheel, first attach the axle (stl/vehicle/wheel/rim.axle.back.stl) to the motor hub.

![image87](image87.png)

For a stable position of the wheel axle, slide a 10x15x4 ball bearing around the wheel axle, so that the motor can still easily turn the wheel.

![image88](image88.png)

To prevent the ball bearing from falling out, screw a wheel axle cover (stl/vehicle/wheel/rim.62mm.back.bearing.cage.V2.stl) onto it.

![image89](image89.png)

Attach the rear wheel to the axle.

![image90](image90.png)

Screw the wheel to the axle with a wheel nut (stl/vehicle/wheel/rim.62mm.front.nut.stl) and a DIN 912 M3x14 screw so that the wheel is securely mounted on the axle.

![image91](image91.png)

Once both rear wheels are attached it should look like this:

![image92](image92.png)

Next, the base plate with the motors can be put aside for the time being. Screw three spacers (stl/vehicle/base/frontwheel.suspension.distance.stl) to the steering bracket (stl/vehicle/base/alphabot.4Wheel.base.suspension.V6.stl) using three DIN 912 M3x10 screws.

![image93](image93.png)

![image94](image94.png)

Now screw a 28BYJ-48 stepper motor to the front bumper (stl/vehicle/addon/bumper.lidar.front.stl) using two DIN 912 M3x10 screws.

![image95](image95.png)

Attach the LiDAR holder (stl/vehicle/addon/bumper.lidar.holder.stl) to the hub of the stepper motor.

![image96](image96.png)

In the next step, screw the steering bracket to the front bumper. Use four DIN 912 M3x5 screws for this.

![image97](image97.png)

![image98](image98.png)

In the next step, return to the base plate. Attach a stepper motor for steering to the base plate using two DIN 912 M3x5 screws.

![image99](image99.png)

![image100](image100.png)

Once the stepper motor has been screwed to the base plate, the steering extension (stl/vehicle/steering/steering.stepper.pin.stl) can be attached to the the stepper motor hub protruding from the bottom of the base plate.

![image101_102](image101_102.png)

Now insert the two 5x8x2.5 ball bearings on the steering bracket (stl/vehicle/steering/steering.axle.stl).

![image150](image150.png)

Attach the steering bracket to the steering extension with a DIN 912 M3x10 screw. Insert 5x11x4 ball bearings in the provided recesses and place the axles (stl/vehicle/steering/suspension.left.V4.stl & stl/vehicle/steering/suspension.right.V4.stl) on top.

![image103](image103.png)

![image104](image104.png)

Attach 5x11x4 ball bearings to the underside of the steering axles.

![image105](image105.png)

Now the two large parts can be screwed together. Screw the spacers to the base plate using three DIN 912 M3x10 screws. The ball bearings should fit into the provided recesses.

![image106](image106.png)

![image107](image107.png)

The next step is to mount the front wheels.

![image108](image108.png)

First slide the front bearing cage (stl/vehicle/wheel/rim.62mm.front.bearing.cage.V2.stl) containing a 10x15x4 ball bearing onto the front wheel axle.

![image109](image109.png)

Slide the front wheel (stl/vehicle/wheel/rim.front.62mm.V2.stl & stl/vehicle/wheel/tire.base.62.78.stl) onto the axle and screw the bearing cage onto the inside of the wheel.

![image110](image110.png)

Then screw the wheel to the axle with a wheel nut (stl/vehicle/wheel/rim.62mm.front.nut.stl) and a DIN 912 M3x14 screw so that it does not come loose while driving.

![image111](image111.png)

For obstacle detection, screw the TF-Luna LiDAR sensor to the dedicated holder at the front. Use M2x6 screws for that.

![image112](image112.png)

![image113](image113.png)

Insert a 15mm*21mm 2 pins rocker switch through the base plate at the designated location.

![rocker_switch](rocker_switch.png)

The battery case can be screwed to the underside of the base plate with two M2.5x5mm screws. Furthermore, interrupt a power line from the battery case with the rocker switch.

![20221116_101925](20221116_101925.jpg)

Mount the FC-03 infrared sensor around the encoder wheel with an M2.5 screw.

![20221116_102728](20221116_102728.jpg)

Screw four M2 hex spacers into the screw holes provided for the Alphabot ESP32 PCB and screw four M2.5 hex spacers into the screw holes provided for the L298N.

![hex_spacers](hex_spacers.png)

Up next, mount the rear spoiler to the rear of the base plate. Mount the wing holder (stl/vehicle/addon/wing.holder.V4.hc-sr04.stl) to the base plate with two DIN 912 M3x8 screws and mount the wing (stl/vehicle/addon/wingv4.stl) to the wing holder with two DIN 912 M3x6 screws.

![image115_116](image115_116.png)

For the positioning module, glue the holder for the tag (stl/vehicle/addon/dwm1000.tag.adapter.board.mount.stl) to the base plate.

![image117](image117.png)

Screw the L298N motor driver to the base plate and connect it to the drive motors and the power supply.

![20221116_113454](20221116_113454.jpg)

Now the heart of this Alphabot comes into play: the Alphabot ESP32 PCB. Attach the Alphabot ESP32 PCB with the ESP-WROOM-32 plugged in to the base plate using M2 male-female hex spacers and M2 screws.

![frame48](frame48.png)

Connect the Alphabot ESP32 PCB to the power supply through its 12V power connectors.

![frame49](frame49.png)

Connect the pins of the L298N motor driver to the Alphabot ESP32 PCB.

![frame50](frame50.png)

Next, insert the positioning module into the DWM1000 tag holder.

![frame52](frame52.png)

Connect the GND, VCC and TXO pins on the Arduino Pro Mini of the positioning module to the Alphabot ESP32 PCB.

![frame53](frame53.png)

Screw the positioning module on with two M2x6 screws and two M2 nuts.

![frame54_screws](frame54_screws.png)

Connect the steering stepper motor to the correspondingly labeled B5B-XH-AM connector header on the Alphabot ESP32 PCB.

![frame55](frame55.png)

Connect the stepper motor at the front to the other B5B-XH-AM connector header on the Alphabot ESP32 PCB.

![frame56](frame56.png)

Connect the TF-Luna LiDAR sensor at the front to the Alphabot ESP32 PCB.

![frame57](frame57.png)

Now that the Alphabot is successfully assembled, the software part can begin.

### Positioning Anchor

![DWM1000_ANCHOR_mount](DWM1000_ANCHOR_mount.png)

![positioning_anchor_mounted](positioning_anchor_mounted.png)
