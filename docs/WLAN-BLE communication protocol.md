# 1. WLAN – bitbased protocol

## 1.1. Requests (Client to Alphabot)

### Speed & Steer (Packet ID: 0x01)

| Field Name | Field Type | Notes                                                                                                            |
|------------|------------|------------------------------------------------------------------------------------------------------------------|
| Speed      | int8       | The desired speed (negative values indicate backward driving, positive values forward driving)                   |
| Steer      | int8       | The desired steering direction (negative values indicate left steering, positive values indicate right steering) |

### Distance sensor request (Packet ID: 0x02)

| Field Name | Field Type  | Notes                    |
|------------|-------------|--------------------------|
| Degree     | int16       | The degree of the sensor |

### Calibrate steering (Packet ID: 0x03)

No further data

### Calibrate compass (Packet ID: 0x04)

No further data

### Toggle request (Packet ID: 0x05)

see [3.1. Toggle bit field](#3.1.)

### Ping request (Packet ID: 0x06)

| Field Name | Field Type  | Notes                              |
|------------|-------------|------------------------------------|
| Time       | int64       | The time when the packet gets sent |

### Configure positioning anchors location (Packet ID: 0x07)

| Field Name | Field Type  | Notes                           |
|------------|-------------|---------------------------------|
| AnchorID   | uint8       | The ID of the anchor            |
| Position X | int16       | The x coordinate in centimetres |
| Position Y | int16       | The y coordinate in centimetres |

### Set target position for navigation (Packet ID: 0x08)

| Field Name | Field Type  | Notes                           |
|------------|-------------|---------------------------------|
| Position X | int16       | The x coordinate in centimetres |
| Position Y | int16       | The y coordinate in centimetres |

### Add obstacle (Packet ID: 0x09)

| Field Name | Field Type  | Notes                           |
|------------|-------------|---------------------------------|
| Position X | int16       | The x coordinate in centimetres |
| Position Y | int16       | The y coordinate in centimetres |
| Width      | uint16      | The width in centimetres        |
| Height     | uint16      | The height in centimetres       |

### Remove one obstacle by ID (Packet ID: 0x0A)

| Field Name | Field Type  | Notes                           |
|------------|-------------|---------------------------------|
| ID         | uint8       | ID of the obstacle              |

### Remove one obstacle by Position (Packet ID: 0x0B)

| Field Name | Field Type  | Notes                           |
|------------|-------------|---------------------------------|
| Position X | int16       | The x coordinate in centimetres |
| Position Y | int16       | The y coordinate in centimetres |

### Remove all obstacles (Packet ID: 0x0C)

No further data

## 1.2. Responses (Alphabot to Client)

### Distance sensor response (Packet ID: 0x81)

| Field Name | Field Type  | Notes                       |
|------------|-------------|-----------------------------|
| Degree     | int16       | The degree of the sensor    |
| Distance   | uint16      | The distance in centimetres |

### Positioning response (Packet ID: 0x82)

| Field Name | Field Type | Notes                           |
|------------|------------|---------------------------------|
| Position X | int16      | The x coordinate in centimetres |
| Position Y | int16      | The y coordinate in centimetres |

### Path finding response (Packet ID: 0x83)

WIP

### Compass response (Packet ID: 0x84)

| Field Name | Field Type | Notes               |
|------------|------------|---------------------|
| Degree     | int16      | The measured degree |

### Ping response (Packet ID: 0x85)

| Field Name | Field Type  | Notes                             |
|------------|-------------|-----------------------------------|
| Time       | int64       | The time when the packet was sent |

### New obstacle registered (Packet ID: 0x86)

| Field Name | Field Type  | Notes                           |
|------------|-------------|---------------------------------|
| ID         | uint8       | The ID of the obstacle          |
| Position X | int16       | The x coordinate in centimetres |
| Position Y | int16       | The y coordinate in centimetres |
| Width      | uint16      | The width in centimetres        |
| Height     | uint16      | The height in centimetres       |


# 2. BLE Protocol


### BLE_CHAR_DRIVE_STEER (UUID: a04295f7-eaa8-4536-b3a4-4e7ae4d72dc2)

| Field Name | Field Type | Notes                                                                                                            |
|------------|------------|------------------------------------------------------------------------------------------------------------------|
| Speed      | int8       | The desired speed (negative values indicate backward driving, positive values forward driving)                   |
| Steer      | int8       | The desired steering direction (negative values indicate left steering, positive values indicate right steering) |

### BLE_CHAR_PINGCLIENT	(UUID: 117ad3a5-b257-4465-abd4-7dc12a4cf77d)

| Field Name | Field Type  | Notes                             |
|------------|-------------|-----------------------------------|
| Time       | int64       | The time when the packet was sent |

### BLE_CHAR_TOGGLE (UUID: fce001d4-864a-48f4-9c95-de928f1da07b)

see [3.1. Toggle bit field](#3.1.)

### BLE_CHAR_SENSOR (UUID: 4c999381-35e2-4af4-8443-ee8b9fe56ba0)

see [3.2. Sensor packets](#3.2.)

### BLE_CHAR_TARGET_NAVI (UUID: f56f0a15-52ae-4ad5-bfe1-557eed983618)

| Field Name | Field Type  | Notes                           |
|------------|-------------|---------------------------------|
| Position X | int16       | The x coordinate in centimetres |
| Position Y | int16       | The y coordinate in centimetres |

### BLE_CHAR_CALIBRATE (UUID: d39e8d54-8019-46c8-a977-db13871bac59)

| Field Name       | Field Type  | Notes                                                                                                                    |
|------------------|-------------|--------------------------------------------------------------------------------------------------------------------------|
| Calibration Type | int8        | 0x00 for calibration finished; 0x01 for steering; 0x02 for automatical compass calibration; 0x03 for manual compass calibration; 0x04 for compass offset|

### BLE_CHAR_ADD_OBSTACLE (UUID: 60db37c7-afeb-4d40-bb17-a19a07d6fc95)

At first Position, Width and Height is sent by the Client, the Alphabot will answer with the data and the ID.

| Field Name | Field Type  | Notes                                             |
|------------|-------------|---------------------------------------------------|
| Position X | int16       | The x coordinate in centimetres                   |
| Position Y | int16       | The y coordinate in centimetres                   |
| Width      | uint16      | The width in centimetres                          |
| Height     | uint16      | The height in centimetres                         |
| ID         | uint8       | The ID of the obstacle (gets set by the Alphabot) |

### BLE_CHAR_REMOVE_OBSTACLE (UUID: 6d43e0df-682b-45ef-abb7-814ecf475771)

Obstacles can be removed either by Position or by ID, if the Position is set, it will be used for deletion. If the Position is 0xFFFF, on x and y, the ID will be used for deletion. If the Position is 0xFFFF and the ID is 0xFF all obstacles will be removed. The Alphabot will set all values to 0 when the deletion is finished.

| Field Name | Field Type  | Notes                                                                      |
|------------|-------------|----------------------------------------------------------------------------|
| Position X | int16       | The x coordinate in centimetres                                            |
| Position Y | int16       | The y coordinate in centimetres                                            |
| ID         | uint8       | The ID of the obstacle; if the value is 0xFF all obstacles will be cleared |

### BLE_CHAR_PATH_FINDING (UUID: 8dad4c9a-1a1c-4a42-a522-ded592f4ed99)

WIP

# 3. Bitfields

## 3.1. Toggle - Bitfield

In the WLAN protocol, the packet 0x05 is followed by a 2-byte bit field.
in the BLE protocol it is a characteristic which is 2 bytes in size.
This bit field allows various settings to be switched on or off. If a 0 is sent,
that setting is going to be deactivated, if a 1 is sent it is going to be activated.
General settings are sent in the 1st byte. 
The 2nd byte changes certain logging options.

### 3.1.1. Bitfield – 1. Byte (Einstellungen)

| Bit 7 (MSB)         | Bit 6  | Bit 5       | Bit 4               | Bit 3           | Bit 2        | Bit 1    | Bit 0 (LSB) |
|---------------------|--------|-------------|---------------------|-----------------|--------------|----------|-------------|
| Compass calibration | Invite | Positioning | Collision Avoidance | Navigation-Mode | Explore-Mode | not used | not used    |

### 3.1.2. Bitfield – 2. Byte (Logging)

| Bit 7 (MSB) | Bit 6             | Bit 5           | Bit 4             | Bit 3            | Bit 2       | Bit 1         | Bit 0 (LSB) |
|-------------|-------------------|-----------------|-------------------|------------------|-------------|---------------|-------------|
| Positioning | Obstacle distance | Pathfinder path | Compass direction | Anchor distances | Wheel speed | Accelerometer | Gyroscope   |

## 3.2. Sensor – Packets (BLE only)

The first byte in the BLE characteristic describes which type of packet it is. The following bytes represent the data.

### Distance sensor response (BLE Packet ID: 0x01)

| Field Name | Field Type  | Notes                       |
|------------|-------------|-----------------------------|
| Degree     | int16       | The degree of the sensor    |
| Distance   | uint16      | The distance in centimetres |

### Positioning response (BLE Packet ID: 0x02)

| Field Name | Field Type | Notes                           |
|------------|------------|---------------------------------|
| Position X | int16      | The x coordinate in centimetres |
| Position Y | int16      | The y coordinate in centimetres |

### Compass response (BLE Packet ID: 0x03)

| Field Name | Field Type | Notes               |
|------------|------------|---------------------|
| Degree     | int16      | The measured degree |
