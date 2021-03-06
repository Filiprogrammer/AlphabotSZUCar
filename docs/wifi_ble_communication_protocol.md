# 1. WiFi – bit based protocol

All byte sequences (int16, int64...) correspond to the "little-endian" standard.

## 1.1. Packet header

The packet header consists of 1 byte. All following bytes are the payload of the packet.
The last 3 bits (MSB) of the packet header define the protocol version. Currently the only version is 000.

### 1.1.1. Example packet

| Bit 7-5 (MSB) <br> [Version 0] | Bit 4-0 (LSB) <br> [Packet ID 0x01] |
|--------------------------------|-------------------------------------|
| 0 0 0                          | 0 0 0 0 1                           |

## 1.2. Requests (Client to Alphabot)

### Speed & Steer (Packet ID: 0x01)

| Field Name | Field Type | Notes                                                                                                            |
|------------|------------|------------------------------------------------------------------------------------------------------------------|
| Speed      | int8       | The desired speed (negative values indicate backward driving, positive values indicate forward driving)          |
| Steer      | int8       | The desired steering direction (negative values indicate left steering, positive values indicate right steering) |

### Calibrate steering (Packet ID: 0x03)

No further data

### Calibrate compass (Packet ID: 0x04)

| Field Name       | Field Type | Notes                                                                                                                                                  |
|------------------|------------|--------------------------------------------------------------------------------------------------------------------------------------------------------|
| Calibration type | uint8      | 0x01 for automatic compass calibration; 0x02 to start manual compass calibration; 0x03 to end manual compass calibration; 0x04 to set compass offset |

### Toggle request (Packet ID: 0x05)

see [3.1. Toggle bit field](#31-toggle---bit-field)

### Ping request (Packet ID: 0x06)

| Field Name | Field Type | Notes                                                       |
|------------|------------|-------------------------------------------------------------|
| Timestamp  | int64      | The Unix epoch time in milliseconds when the packet is sent |

### Configure positioning anchors location (Packet ID: 0x07)

| Field Name | Field Type | Notes                           |
|------------|------------|---------------------------------|
| AnchorID   | uint8      | The ID of the anchor            |
| Position X | int16      | The x coordinate in centimetres |
| Position Y | int16      | The y coordinate in centimetres |

### Set target position for navigation (Packet ID: 0x08)

| Field Name | Field Type | Notes                           |
|------------|------------|---------------------------------|
| Position X | int16      | The x coordinate in centimetres |
| Position Y | int16      | The y coordinate in centimetres |

### Add obstacle (Packet ID: 0x09)

| Field Name | Field Type | Notes                           |
|------------|------------|---------------------------------|
| Position X | int16      | The x coordinate in centimetres |
| Position Y | int16      | The y coordinate in centimetres |
| Width      | uint16     | The width in centimetres        |
| Height     | uint16     | The height in centimetres       |

### Remove one obstacle by ID (Packet ID: 0x0A)

| Field Name | Field Type | Notes                           |
|------------|------------|---------------------------------|
| ID         | uint16     | ID of the obstacle              |

### Remove obstacle by position (Packet ID: 0x0B)

If there are multiple objects in the same position, all of them will be removed.

| Field Name | Field Type | Notes                           |
|------------|------------|---------------------------------|
| Position X | int16      | The x coordinate in centimetres |
| Position Y | int16      | The y coordinate in centimetres |

### Remove all obstacles (Packet ID: 0x0C)

All obstacles (manually added and discovered by the Alphabot) will be removed.

No further data

## 1.3. Responses (Alphabot to client)

### Distance sensor response (Packet ID: 0x01)

Degree 0 represents the direction, in which the Alphabot is headed towards.
Degree 90 is the right direction.

| Field Name | Field Type | Notes                            |
|------------|------------|----------------------------------|
| Degree     | int16      | The degree of the sensor (0-359) |
| Distance   | uint16     | The distance in centimetres      |

### Positioning response (Packet ID: 0x02)

| Field Name | Field Type | Notes                           |
|------------|------------|---------------------------------|
| Position X | int16      | The x coordinate in centimetres |
| Position Y | int16      | The y coordinate in centimetres |

### Path finding response (Packet ID: 0x03)

| Field Name      | Field Type                | Notes                                                       |
|-----------------|---------------------------|-------------------------------------------------------------|
| X-start         | int8                      | X coordinate of the start position in decimeters            |
| Y-start         | int8                      | Y coordinate of the start position in decimeters            |
| Path steps data | Path steps data structure | see [Path steps data structure](#33-Path-steps-data-structure) |

### Compass response (Packet ID: 0x04)

Degree 0 represents the direction, in which the Alphabot is headed towards.
Degree 90 is the right direction.

| Field Name | Field Type | Notes                       |
|------------|------------|-----------------------------|
| Degree     | int16      | The measured degree (0-359) |

### Ping response (Packet ID: 0x05)

| Field Name | Field Type | Notes                                                        |
|------------|------------|--------------------------------------------------------------|
| Timestamp  | int64      | The Unix epoch time in milliseconds when the packet was sent |

### New obstacle registered (Packet ID: 0x06)

| Field Name | Field Type | Notes                           |
|------------|------------|---------------------------------|
| Position X | int16      | The x coordinate in centimetres |
| Position Y | int16      | The y coordinate in centimetres |
| Width      | uint16     | The width in centimetres        |
| Height     | uint16     | The height in centimetres       |
| ID         | uint16     | The ID of the obstacle          |

### Toggle response (Packet ID: 0x07)

The toggle settings get automatically sent upon a new client connection, but they will also be sent if
the Alphabot changes a setting by itself.

see [3.1. Toggle bit field](#31-toggle---bit-field)

### Anchor distance response (Packet ID: 0x08)

| Field Name | Field Type | Notes                                   |
|------------|------------|-----------------------------------------|
| Distance   | uint16     | The distance to anchor 0 in centimeters |
| Distance   | uint16     | The distance to anchor 1 in centimeters |
| Distance   | uint16     | The distance to anchor 2 in centimeters |

### Wheel speed response (Packet ID: 0x09)

| Field Name  | Field Type | Notes                                     |
|-------------|------------|-------------------------------------------|
| Speed left  | int8       | The speed of the left wheel in m/s * 100  |
| Speed right | int8       | The speed of the right wheel in m/s * 100 |

### Gyroscope response (Packet ID: 0x0A)

| Field Name | Field Type | Notes                                    |
|------------|------------|------------------------------------------|
| X-axis     | int16      | The speed of the x-axis in degree/s * 10 |
| Y-axis     | int16      | The speed of the y-axis in degree/s * 10 |
| Z-axis     | int16      | The speed of the z-axis in degree/s * 10 |

### Accelerometer response (Packet ID: 0x0B)

| Field Name | Field Type | Notes                                              |
|------------|------------|----------------------------------------------------|
| X-axis     | int16      | The acceleration of the x-axis in m/s&#xB2; * 1000 |
| Y-axis     | int16      | The acceleration of the y-axis in m/s&#xB2; * 1000 |
| Z-axis     | int16      | The acceleration of the z-axis in m/s&#xB2; * 1000 |

### Magnetometer response (Packet ID: 0x0C)

| Field Name | Field Type | Notes                                                   |
|------------|------------|---------------------------------------------------------|
| X-axis     | int16      | The magnetic flux density of the x-axis in &#xB5;T * 10 |
| Y-axis     | int16      | The magnetic flux density of the y-axis in &#xB5;T * 10 |
| Z-axis     | int16      | The magnetic flux density of the z-axis in &#xB5;T * 10 |

### Error message (Packet ID: 0x0D)

| Field Name     | Field Type         | Notes                                          |
|----------------|--------------------|------------------------------------------------|
| Error ID       | uint8              | The error ID, see [3.4.](#34-error-id-format)  |
| Packet header  | uint8              | The packet header that threw the error         |
| Payload        | uint8 array (0-20) | The payload of the packet that threw the error |

# 2. BLE Protocol

### BLE_CHAR_DRIVE_STEER (UUID: a04295f7-eaa8-4536-b3a4-4e7ae4d72dc2)

| Field Name | Field Type | Notes                                                                                                            |
|------------|------------|------------------------------------------------------------------------------------------------------------------|
| Speed      | int8       | The desired speed (negative values indicate backward driving, positive values forward driving)                   |
| Steer      | int8       | The desired steering direction (negative values indicate left steering, positive values indicate right steering) |

### BLE_CHAR_PINGCLIENT	(UUID: 117ad3a5-b257-4465-abd4-7dc12a4cf77d)

| Field Name | Field Type | Notes                                                       |
|------------|------------|-------------------------------------------------------------|
| Timestamp  | int64      | The Unix epoch time in milliseconds when the packet is sent |

### BLE_CHAR_TOGGLE (UUID: fce001d4-864a-48f4-9c95-de928f1da07b)

| Field Name                | Field Type | Notes                                                                     |
|---------------------------|------------|---------------------------------------------------------------------------|
| Settings toggle bit field | uint8      | see [3.1.1. Toggle bit field (Settings)](#311-bit-field--1-byte-settings) |
| Logging toggle bit field  | uint8      | see [3.1.2. Toggle bit field (Logging)](#312-bit-field--2-byte-logging)   |
| Timestamp                 | int64      | The Unix epoch time in milliseconds when the packet is sent               |

### BLE_CHAR_SENSOR (UUID: 4c999381-35e2-4af4-8443-ee8b9fe56ba0)

see [3.2. Sensor packets](#32-sensor--packets-ble-only)

### BLE_CHAR_TARGET_NAVI (UUID: f56f0a15-52ae-4ad5-bfe1-557eed983618)

| Field Name | Field Type | Notes                                                       |
|------------|------------|-------------------------------------------------------------|
| Position X | int16      | The x coordinate in centimetres                             |
| Position Y | int16      | The y coordinate in centimetres                             |
| Timestamp  | int64      | The Unix epoch time in milliseconds when the packet is sent |

### BLE_CHAR_CALIBRATE (UUID: d39e8d54-8019-46c8-a977-db13871bac59)

| Field Name       | Field Type  | Notes                                                                                                                                                                            |
|------------------|-------------|----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------|
| Calibration Type | int8        | 0x00 to calibrate steering; 0x01 for automatic compass calibration; 0x02 to start manual compass calibration; 0x03 to end manual compass calibration; 0x04 to set compass offset |
| Timestamp        | int64       | The Unix epoch time in milliseconds when the packet is sent                                                                                                                      |

### BLE_CHAR_ADD_OBSTACLE (UUID: 60db37c7-afeb-4d40-bb17-a19a07d6fc95)

At first position, width and height is sent by the client, the Alphabot will answer with the data and the ID.

| Field Name | Field Type | Notes                                                       |
|------------|------------|-------------------------------------------------------------|
| Timestamp  | int64      | The Unix epoch time in milliseconds when the packet is sent |
| Position X | int16      | The x coordinate in centimetres                             |
| Position Y | int16      | The y coordinate in centimetres                             |
| Width      | uint16     | The width in centimetres                                    |
| Height     | uint16     | The height in centimetres                                   |
| ID         | uint16     | The ID of the obstacle (gets set by the Alphabot)           |

### BLE_CHAR_REMOVE_OBSTACLE (UUID: 6d43e0df-682b-45ef-abb7-814ecf475771)

Obstacles can be removed either by ID or by position.
If the characteristic is 0 bytes long, ALL obstacles will be removed.
If the characteristic is 10 bytes long, the value is interpreted as the obstacle ID.
If the characteristic is 12 bytes long, the values are interpreted as the position.
The Alphabot will send the corresponding packet when the removal of obstacles is completed.
If there are multiple objects on the same position, all of them will be removed.

| Field Name | Field Type | Notes                                                       |
|------------|------------|-------------------------------------------------------------|
| Timestamp  | int64      | The Unix epoch time in milliseconds when the packet is sent |
| ID         | uint16     | The ID of the obstacle                                      |

#### OR

| Field Name | Field Type | Notes                                                       |
|------------|------------|-------------------------------------------------------------|
| Timestamp  | int64      | The Unix epoch time in milliseconds when the packet is sent |
| Position X | int16      | The x coordinate in centimetres                             |
| Position Y | int16      | The y coordinate in centimetres                             |

### BLE_CHAR_PATH_FINDING (UUID: 8dad4c9a-1a1c-4a42-a522-ded592f4ed99)

| Field Name      | Field Type                | Notes                                                          |
|-----------------|---------------------------|----------------------------------------------------------------|
| X-start         | int8                      | X coordinate of the start position in decimeters               |
| Y-start         | int8                      | Y coordinate of the start position in decimeters               |
| Path steps data | Path steps data structure | see [Path steps data structure](#33-Path-steps-data-structure) |

### BLE_CHAR_ANCHOR_LOCATIONS (UUID: 8a55dd30-463b-40f6-8f21-d68efcc386b2)

| Field Name | Field Type | Notes                                                       |
|------------|------------|-------------------------------------------------------------|
| Timestamp  | int64      | The Unix epoch time in milliseconds when the packet is sent |
| Position X | int16      | The x coordinate in centimetres of anchor 0                 |
| Position Y | int16      | The y coordinate in centimetres of anchor 0                 |
| Position X | int16      | The x coordinate in centimetres of anchor 1                 |
| Position Y | int16      | The y coordinate in centimetres of anchor 1                 |
| Position X | int16      | The x coordinate in centimetres of anchor 2                 |
| Position Y | int16      | The y coordinate in centimetres of anchor 2                 |

### BLE_CHAR_ANCHORS_DISTANCES (UUID: 254492a2-9324-469b-b1e2-4d4590972c35)

| Field Name | Field Type | Notes                                   |
|------------|------------|-----------------------------------------|
| Distance   | uint16     | The distance to anchor 0 in centimetres |
| Distance   | uint16     | The distance to anchor 1 in centimetres |
| Distance   | uint16     | The distance to anchor 2 in centimetres |

### BLE_CHAR_IMU (UUID: 93758afa-ce6f-4670-9562-ce92bda84d49)

| Field Name | Field Type | Notes                                                                                                                                   |
|------------|------------|-----------------------------------------------------------------------------------------------------------------------------------------|
| Type       | int8       | Indicates whether the packet is a gyroscope-package (value 0), an accelerometer-package (value 1), or an magnetometer-package (value 2) |
| x-Axis     | int16      | The value of the x-Axis                                                                                                                 |
| y-Axis     | int16      | The value of the y-Axis                                                                                                                 |
| z-Axis     | int16      | The value of the z-Axis                                                                                                                 |

### BLE_CHAR_WHEEL_SPEED (UUID: 8efafa16-15de-461f-bde1-493261201e2b)

| Field Name  | Field Type | Notes                                     |
|-------------|------------|-------------------------------------------|
| Speed left  | int8       | The speed of the left wheel in m/s * 100  |
| Speed right | int8       | The speed of the right wheel in m/s * 100 |

### BLE_CHAR_ERROR (UUID: dc458f08-ea3e-4fe1-adb3-25c840be081a)

If the original payload was 19 or 20 bytes long, the last one or two bytes won't be shown.

| Field Name     | Field Type         | Notes                                                              |
|----------------|--------------------|--------------------------------------------------------------------|
| Error ID       | uint8              | The error ID, see [3.4.](#34-error-id-format)                      |
| BLE CHAR ID    | uint8              | The first byte of the BLE characteristic UUID that threw the error |
| Payload        | uint8 array (0-18) | The payload of the packet that threw the error                     |

# 3. Bit fields

## 3.1. Toggle - Bit field

In the WiFi protocol, the packet 0x05 is followed by a 2-byte bit field.
In the BLE protocol it is a characteristic which is 10 bytes in size, because the timestamp (8 bytes) is sent right after the actual bit field (2 bytes)
This bit field allows various settings to be switched on or off. If a 0 is sent,
that setting is going to be deactivated, if a 1 is sent it is going to be activated.
General settings are sent in the 1st byte. 
The 2nd byte changes certain logging options.

### 3.1.1. Bit field – 1. Byte (Settings)

| Bit 7 (MSB) | Bit 6       | Bit 5               | Bit 4           | Bit 3        | Bit 2    | Bit 1    | Bit 0 (LSB) |
|-------------|-------------|---------------------|-----------------|--------------|----------|----------|-------------|
| Invite      | Positioning | Collision Avoidance | Navigation-Mode | Explore-Mode | not used | not used | not used    |

### 3.1.2. Bit field – 2. Byte (Logging)

| Bit 7 (MSB) | Bit 6             | Bit 5           | Bit 4             | Bit 3            | Bit 2       | Bit 1                     | Bit 0 (LSB) |
|-------------|-------------------|-----------------|-------------------|------------------|-------------|---------------------------|-------------|
| Positioning | Obstacle distance | Pathfinder path | Compass direction | Anchor distances | Wheel speed | Inertial Measurement Unit | not used    |

## 3.2. Sensor – Packets (BLE only)

The first two bytes describe the sensor types that will follow. Then the payload of these packets will follow. There can be multiple sensor responses sent at a time. The following responses can be sent in one packet:

### Example bit stream

In the first byte 0b01 is sent twice, because two distance sensor packets will follow, the following bits in the first two bytes are filled with 0. Byte 2 and 3 represent the payload of the first distance sensor data. Byte 4 and 5 represent the payload of the second distance sensor data.

| Byte 0   | Byte 1   | Byte 2   | Byte 3   | Byte 4   | Byte 5   |
|----------|----------|----------|----------|----------|----------|
| 00000101 | 00000000 | 00000101 | 10000000 | 01100100 | 10000001 |

### Distance sensor response (Sensor type: 0b01)

Degree 0 represents the direction, in which the Alphabot is headed towards.
Degree 90 is the right direction.

| Field Name | Field Type | Notes                                         |
|------------|------------|-----------------------------------------------|
| Degree     | uint8      | The degree of the sensor divided by 2 (0-179) |
| Distance   | uint8      | The distance in centimetres divided by 2      |

### Positioning response (Sensor type: 0b10)

The position will be split into 3 bytes, 12 bit per coordinate

| Field Name | Field Type | Notes                           |
|------------|------------|---------------------------------|
| Position X | int12      | The x coordinate in centimetres |
| Position Y | int12      | The y coordinate in centimetres |

### Compass response (Sensor type: 0b11)

Degree 0 represents the direction, in which the Alphabot is headed towards.
Degree 90 is the right direction.

| Field Name | Field Type | Notes                       |
|------------|------------|-----------------------------|
| Degree     | int16      | The measured degree (0-359) |

## 3.3. Path steps data structure

The Path steps data structure contains a list of steps in certain directions which together form a path.
The length of the data structure can be anywhere between 1 to 18 bytes long.
The first 6 bits of the first byte contain the number of the following path steps.
Note that bit 0 refers to the least significant bit.
The path steps are stored such that several entries are within a single byte and sometimes an entry overlaps between multiple bytes.
The first entry is stored in bits 6 and 7 of the first byte and in bit 0 of the second byte.
Bits 1 to 3 of the second byte are the second entry, bits 4 to 6 are the third entry, and so on.
With 3 bits per path step there are 8 possible values. The format of a path step is described in [Path step format](#331-Path-step-format)

### 3.3.1. Path step format

| Encoded value | Direction  |
|---------------|------------|
| 0             | left-up    |
| 1             | left       |
| 2             | left-down  |
| 3             | up         |
| 4             | right-down |
| 5             | down       |
| 6             | right-up   |
| 7             | right      |

### 3.3.2. Example bit stream

The following example shows a possible bit stream of the path finding data. In the [example encoded in bytes](#333-example-encoded-in-bytes) all steps are explained explicitly.

<span style="color:#f53d3d">001000</span>
| <span style="color:#f5993d">001</span>
| <span style="color:#f5f53d">010</span>
| <span style="color:#99f53d">101</span>
| <span style="color:#3df53d">101</span>
| <span style="color:#3df599">100</span>
| <span style="color:#3df5f5">100</span>
| <span style="color:#3d99f5">111</span>
| <span style="color:#3d3df5">111</span>
| 00

### 3.3.3. Example encoded in bytes

<span style="color:#f5993d">01</span><span style="color:#f53d3d">001000</span>
<span style="color:#3df53d">1</span><span style="color:#99f53d">101</span><span style="color:#f5f53d">010</span><span style="color:#f5993d">0</span>
<span style="color:#3df5f5">100</span><span style="color:#3df599">100</span><span style="color:#3df53d">10</span>
00<span style="color:#3d3df5">111</span><span style="color:#3d99f5">111</span>

| Binary value of step                      | Step direction           |
|-------------------------------------------|--------------------------|
| <span style="color:#f53d3d">001000</span> | Number of path steps = 8 |
| <span style="color:#f5993d">001</span>    | left                     |
| <span style="color:#f5f53d">010</span>    | left-down                |
| <span style="color:#99f53d">101</span>    | down                     |
| <span style="color:#3df53d">101</span>    | down                     |
| <span style="color:#3df599">100</span>    | right-down               |
| <span style="color:#3df5f5">100</span>    | right-down               |
| <span style="color:#3d99f5">111</span>    | right                    |
| <span style="color:#3d3df5">111</span>    | right                    |

## 3.4. Error id format

| Encoded value | Error                             |
|---------------|-----------------------------------|
| 0x00          | unknown error                     |
| 0x01          | unknown protocol                  |
| 0x02          | known but not supported protocol  |
| 0x03          | unknown packet ID                 |
| 0x04          | wrong payload                     |
