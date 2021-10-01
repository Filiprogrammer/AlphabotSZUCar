# 1. WiFi – bit based protocol

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

### Distance sensor request (Packet ID: 0x02)

If the degree value corresponds to no sensor, a "wrong payload error" will be sent.
Degree 0 represents the direction, in which the Alphabot is headed towards.
Degree 90 is the right direction.

| Field Name | Field Type | Notes                            |
|------------|------------|----------------------------------|
| Degree     | int16      | The degree of the sensor (0-359) |

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
| ID         | uint8      | ID of the obstacle              |

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
| ID         | uint8      | The ID of the obstacle          |
| Position X | int16      | The x coordinate in centimetres |
| Position Y | int16      | The y coordinate in centimetres |
| Width      | uint16     | The width in centimetres        |
| Height     | uint16     | The height in centimetres       |

### Toggle response (Packet ID: 0x07)

The toggle settings get automatically sent upon a new client connection, but they will also be sent if
the Alphabot changes a setting by itself.

see [3.1. Toggle bit field](#31-toggle---bit-field)

### Error message (Packet ID: 0x08)

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

see [3.1. Toggle bit field](#31-toggle---bit-field)

### BLE_CHAR_SENSOR (UUID: 4c999381-35e2-4af4-8443-ee8b9fe56ba0)

see [3.2. Sensor packets](#32-sensor--packets-ble-only)

### BLE_CHAR_TARGET_NAVI (UUID: f56f0a15-52ae-4ad5-bfe1-557eed983618)

| Field Name | Field Type | Notes                           |
|------------|------------|---------------------------------|
| Position X | int16      | The x coordinate in centimetres |
| Position Y | int16      | The y coordinate in centimetres |

### BLE_CHAR_CALIBRATE (UUID: d39e8d54-8019-46c8-a977-db13871bac59)

| Field Name       | Field Type  | Notes                                                                                                                                                    |
|------------------|-------------|----------------------------------------------------------------------------------------------------------------------------------------------------------|
| Calibration Type | int8        | 0x00 for calibration finished; 0x01 for steering; 0x02 for automatic compass calibration; 0x03 for manual compass calibration; 0x04 for compass offset |

### BLE_CHAR_ADD_OBSTACLE (UUID: 60db37c7-afeb-4d40-bb17-a19a07d6fc95)

At first position, width and height is sent by the client, the Alphabot will answer with the data and the ID.

| Field Name | Field Type | Notes                                             |
|------------|------------|---------------------------------------------------|
| Position X | int16      | The x coordinate in centimetres                   |
| Position Y | int16      | The y coordinate in centimetres                   |
| Width      | uint16     | The width in centimetres                          |
| Height     | uint16     | The height in centimetres                         |
| ID         | uint8      | The ID of the obstacle (gets set by the Alphabot) |

### BLE_CHAR_REMOVE_OBSTACLE (UUID: 6d43e0df-682b-45ef-abb7-814ecf475771)

Obstacles can be removed either by Position or by ID, if the Position is set, it will be used for deletion. 
If the Position is 0xFFFF, on x and y, the ID will be used for deletion. 
If the Position is 0xFFFF and the ID is 0xFF all obstacles will be removed.
The Alphabot will set all values to 0 when the deletion is finished. 
If there are multiple objects on the same position, all of them will be removed.

| Field Name | Field Type | Notes                                                                      |
|------------|------------|----------------------------------------------------------------------------|
| Position X | int16      | The x coordinate in centimetres                                            |
| Position Y | int16      | The y coordinate in centimetres                                            |
| ID         | uint8      | The ID of the obstacle; if the value is 0xFF all obstacles will be cleared |

### BLE_CHAR_PATH_FINDING (UUID: 8dad4c9a-1a1c-4a42-a522-ded592f4ed99)

| Field Name      | Field Type                | Notes                                                       |
|-----------------|---------------------------|-------------------------------------------------------------|
| X-start         | int8                      | X coordinate of the start position in decimeters            |
| Y-start         | int8                      | Y coordinate of the start position in decimeters            |
| Path steps data | Path steps data structure | see [Path steps data structure](#33-Path-steps-data-structure) |

### BLE_CHAR_ERROR (UUID: dc458f08-ea3e-4fe1-adb3-25c840be081a)

The BLE protocol won't reply with the characteristic that threw an error,
it will only show the payload that has wrong content. If the original payload
was 20 bytes long, the last byte won't be shown.

| Field Name     | Field Type         | Notes                                          |
|----------------|--------------------|------------------------------------------------|
| Error ID       | uint8              | The error ID, see [3.4.](#34-error-id-format)  |
| Payload        | uint8 array (0-19) | The payload of the packet that threw the error |

# 3. Bit fields

## 3.1. Toggle - Bit field

In the WiFi protocol, the packet 0x05 is followed by a 2-byte bit field.
in the BLE protocol it is a characteristic which is 2 bytes in size.
This bit field allows various settings to be switched on or off. If a 0 is sent,
that setting is going to be deactivated, if a 1 is sent it is going to be activated.
General settings are sent in the 1st byte. 
The 2nd byte changes certain logging options.

### 3.1.1. Bit field – 1. Byte (Settings)

| Bit 7 (MSB)         | Bit 6  | Bit 5       | Bit 4               | Bit 3           | Bit 2        | Bit 1    | Bit 0 (LSB) |
|---------------------|--------|-------------|---------------------|-----------------|--------------|----------|-------------|
| Compass calibration | Invite | Positioning | Collision Avoidance | Navigation-Mode | Explore-Mode | not used | not used    |

### 3.1.2. Bit field – 2. Byte (Logging)

| Bit 7 (MSB) | Bit 6             | Bit 5           | Bit 4             | Bit 3            | Bit 2       | Bit 1         | Bit 0 (LSB) |
|-------------|-------------------|-----------------|-------------------|------------------|-------------|---------------|-------------|
| Positioning | Obstacle distance | Pathfinder path | Compass direction | Anchor distances | Wheel speed | Accelerometer | Gyroscope   |

## 3.2. Sensor – Packets (BLE only)

The first two bytes describe the packet types that will follow. There can be multiple sensor responses sent at a time. The following responses can be sent in one packet:

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
| 0x03          | unknown packet id                 |
| 0x04          | wrong payload                     |
