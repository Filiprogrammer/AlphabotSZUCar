# 1. WLAN – bitbasierendes Protokoll

<table>
    <tbody>
        <tr>
            <td  colspan="3" >
                <b>Steuerungsbyte</b>
            </td>
            <td  rowspan="2" >
                <b>Weitere Byte</b>
            </td>
            <td  rowspan="2" >
                <b>Gesamtgröße Byte</b>
            </td>
        </tr>
        <tr>
            <td>
                <b>Kategorie</b>
            </td>
            <td>
                <b>Unterkategorie</b>
            </td>
            <td>
                <b>Beschreibung</b>
            </td>
        </tr>
        <tr>
            <td  rowspan="3" >
                    00 / Ansteuerung
            </td>
            <td>
                    00 / Steer &amp; Speed
            </td>
            <td>
                    0000
            </td>
            <td>
                    Anzahl der Steps sbyte (1B) + Speed in sbyte (1B)
            </td>
            <td>
                    3
            </td>
        </tr>
        <tr>
            <td  rowspan="2" >
                    01 / Calibrate
            </td>
            <td>
                    0000 / Steering
            </td>
            <td>
            </td>
            <td  rowspan="2" >
                    1
            </td>
        </tr>
        <tr>
            <td>
                    1111 / Compass
            </td>
            <td>
            </td>
        </tr>
        <tr>
            <td  rowspan="5" >
                    01 / Anforderung (durch Client)
            </td>
            <td>
                    00 / Distanzsensor
            </td>
            <td>
                    000 + Grad des Sensors Bit 8 (MSB)*
            </td>
            <td>
                    Grad des Sensors Bit 0-7 (1B)
            </td>
            <td>
                    2
            </td>
        </tr>
        <tr>
            <td>
                    01 / Kartengröße
            </td>
            <td>
                    0000
            </td>
            <td>
            </td>
            <td>
                    1
            </td>
        </tr>
        <tr>
            <td  rowspan="2" >
                    10 / Positionierung &amp; Path Finding
            </td>
            <td>
                    0000 / Positionierung
            </td>
            <td  rowspan="2" >
            </td>
            <td  rowspan="2" >
                    1
            </td>
        </tr>
        <tr>
            <td>
                    1111 / Path Finding
            </td>
        </tr>
        <tr>
            <td>
                    11 / Kompass
            </td>
            <td>
                    0000
            </td>
            <td>
            </td>
            <td>
                    1
            </td>
        </tr>
        <tr>
            <td  rowspan="5" >
                    10 / Antwort (von Alphabot)
            </td>
            <td>
                    00 / Distanzsensor
            </td>
            <td>
                    000 + Grad des Sensors Bit 8 (MSB)*
            </td>
            <td>
                    Grad des Sensors Bit 0-7 (1B) + Distanz (1B)
            </td>
            <td>
                    3
            </td>
        </tr>
        <tr>
            <td>
                    01 / Kartengröße
            </td>
            <td>
                    0000
            </td>
            <td>
                    X und Y Länge (4B)
            </td>
            <td>
                    5
            </td>
        </tr>
        <tr>
            <td  rowspan="2" >
                    10 / Positionierung &amp; Path Finding
            </td>
            <td>
                    0000 / Positionierung
            </td>
            <td>
                    X und Y Koordinaten (4B)
            </td>
            <td>
                    5
            </td>
        </tr>
        <tr>
            <td>
                    1111 / Path Finding
            </td>
            <td>
                    Path Finding Daten (max. 20B)
            </td>
            <td>
                    21
            </td>
        </tr>
        <tr>
            <td>
                    11 / Kompass
            </td>
            <td>
                    0000
            </td>
            <td>
                    Kompassdaten (2B)
            </td>
            <td>
                    3
            </td>
        </tr>
        <tr>
            <td  rowspan="10" >
                    11 / Sonstiges
            </td>
            <td>
                    00 / Toggle
            </td>
            <td>
                    0000
            </td>
            <td>
                    Siehe Toggle-Bitfeld (2B)
            </td>
            <td>
                    3
            </td>
        </tr>
        <tr>
            <td  rowspan="5" >
                    01 / Obstacle
            </td>
            <td>
                    0000 / Add
            </td>
            <td>
                    X, Y Koordinaten (4B) + Breite und Höhe (2B)
            </td>
            <td>
                    7
            </td>
        </tr>
        <tr>
            <td>
                    0010 / New obstacle registered (Alphabot to Client)
            </td>
            <td>
                    ID (1B) + X, Y Koordinaten (4B) + Breite und Höhe (2B)
            </td>
            <td>
                    8
            </td>
        </tr>
        <tr>
            <td>
                    1100 / Remove one by ID
            </td>
            <td>
                    ID (1B)
            </td>
            <td>
                    2
            </td>
        </tr>
        <tr>
            <td>
                    1101 / Remove one by Coordinates
            </td>
            <td>
                    X, Y Koordinaten (4B)
            </td>
            <td>
                    5
            </td>
        </tr>
        <tr>
            <td>
                    1111 / Remove all
            </td>
            <td>
            </td>
            <td>
                    1
            </td>
        </tr>
        <tr>
            <td  rowspan="2" >
                    10 / Ping
            </td>
            <td>
                    0000 / sendet Client
            </td>
            <td  rowspan="2" >
            </td>
            <td  rowspan="2" >
                    1
            </td>
        </tr>
        <tr>
            <td>
                    1111 / sendet Alphabot
            </td>
        </tr>
        <tr>
            <td  rowspan="2" >
                    11 / Sonstiges
            </td>
            <td>
                    0001 / Configure positioning anchors location
            </td>
            <td>
                    Anchor ID (1B) + X, Y Koordinaten (4B)
            </td>
            <td>
                    6
            </td>
        </tr>
        <tr>
            <td>
                    0010 / Set target position for navigation
            </td>
            <td>
                    X, Y Koordinaten der Position (4B)
            </td>
            <td>
                    5
            </td>
        </tr>
    </tbody>
</table>

*Da für die Darstellung von 360° 9 Bit benötigt werden ist das MSB (Bit 8)
noch im Steuerungsbyte enthalten. Die restlichen 8 Bit werden im nächsten
Byte gesendet.

# 2. BLE Protokoll


<table>
    <tbody>
        <tr>
            <td  rowspan="2" >
                
                    UUID
            </td>
            <td  rowspan="2" >
                
                    Name
            </td>
            <td  colspan="3" >
                
                    Beschreibung
            </td>
            <td  rowspan="2" >
                
                    Größe in Byte
            </td>
            <td  rowspan="2" >
                
                    <u>R</u>
                    ead/
                
                    <u>W</u>
                    rite/
                
                    <u>N</u>
                    otify
            </td>
        </tr>
        <tr>
            <td>
                
                    1. Byte
            </td>
            <td>
                
                    2. Byte
            </td>
            <td>
                
                    Weitere Byte
            </td>
        </tr>
        <tr>
            <td>
                    a04295f7-eaa8-4536-b3a4-4e7ae4d72dc2
            </td>
            <td>
                    BLE_CHAR_DRIVE_STEER
            </td>
            <td>
                    Lenkung Steps in sbyte (negativ für links, positiv für
                    rechts)
            </td>
            <td>
                    Speed in sbyte (negativ für rückwärts, positiv für
                    vorwärts)
            </td>
            <td>
            </td>
            <td>
                    2
            </td>
            <td>
                    RW
            </td>
        </tr>
        <tr>
            <td>
                    4c999381-35e2-4af4-8443-ee8b9fe56ba0
            </td>
            <td>
                    BLE_CHAR_SENSOR
            </td>
            <td  colspan="2" >
                    Siehe Sensor-Bitfeld (3.2.)
            </td>
            <td>
            </td>
            <td>
                    5
            </td>
            <td>
                    RWN
            </td>
        </tr>
        <tr>
            <td>
                    fce001d4-864a-48f4-9c95-de928f1da07b
            </td>
            <td>
                    BLE_CHAR_TOGGLE
            </td>
            <td  colspan="2" >
                    Siehe Toggle-Bitfeld (3.1.)
            </td>
            <td>
            </td>
            <td>
                    2
            </td>
            <td>
                    RWN
            </td>
        </tr>
        <tr>
            <td  rowspan="4" >
                    60db37c7-afeb-4d40-bb17-a19a07d6fc95
            </td>
            <td  rowspan="4" >
                    BLE_CHAR_OBSTACLE
            </td>
            <td>
                    1 für Add
            </td>
            <td  colspan="2" >
                    Koordinaten (4B), Breite und Höhe (2B)
            </td>
            <td  rowspan="4" >
                    7
            </td>
            <td  rowspan="4" >
                    RWN
            </td>
        </tr>
        <tr>
            <td>
                    2 für Registered
            </td>
            <td  colspan="2" >
                    ID (1B), Koordinaten (4B), Breite und Höhe (2B)
            </td>
        </tr>
        <tr>
            <td>
                    3 für Remove one
            </td>
            <td  colspan="2" >
                    Koordinaten (4B), ID (1B)
            </td>
        </tr>
        <tr>
            <td>
                    4 für Remove all
            </td>
            <td>
            </td>
            <td>
            </td>
        </tr>
        <tr>
            <td>
                    117ad3a5-b257-4465-abd4-7dc12a4cf77d
            </td>
            <td>
                    BLE_CHAR_PINGCLIENT
            </td>
            <td>
            </td>
            <td>
            </td>
            <td>
            </td>
            <td>
                    1
            </td>
            <td>
                    RN
            </td>
        </tr>
        <tr>
            <td>
                    d39e8d54-8019-46c8-a977-db13871bac59
            </td>
            <td>
                    BLE_CHAR_CALIBRATE
            </td>
            <td>
                    0 für calibrate abgeschlossen
                    1 für calibrate steering Anforderung
                    2 für calibrate compass direction Anforderung
            </td>
            <td>
            </td>
            <td>
            </td>
            <td>
                    1
            </td>
            <td>
                    RWN
            </td>
        </tr>
        <tr>
            <td>
                    f56f0a15-52ae-4ad5-bfe1-557eed983618
            </td>
            <td>
                    BLE_CHAR_TARGET_NAVI
            </td>
            <td  colspan="2" >
                    X Koordinate
            </td>
            <td>
                    Y Koordinate (2B)
            </td>
            <td>
                    4
            </td>
            <td>
                    RW
            </td>
        </tr>
        <tr>
            <td>
                    8dad4c9a-1a1c-4a42-a522-ded592f4ed99
            </td>
            <td>
                    BLE_CHAR_PATH_FINDING
            </td>
            <td  colspan="3" >
                    Path Finding Daten
            </td>
            <td>
                    20
            </td>
            <td>
                    R
            </td>
        </tr>
    </tbody>
</table>


# 3. Bitfelder

## 3.1. Toggle - Bitfeld

Im WLAN Protokoll wird mit der Bit-Folge 1100 0000 anschließend ein 2 Byte
großes Bitfeld mitgesendet, im BLE Protokoll ist es eine Characteristic
welche 2 Byte groß ist. Dieses Bitfeld ermöglicht es verschiedene
Einstellungen ein- bzw. ausschalten zu lassen. Wird eine 0 gesendet, ist
die jeweilige Einstellung zu deaktivieren, wird eine 1 gesendet soll es
aktiviert werden. Im 1. Byte werden allgemeine Einstellungen gesendet. 
Das 2. Byte ändert bestimmte Logging Optionen.


### 3.1.1. Bitfeld – 1. Byte (Einstellungen)


<table border="1" cellspacing="0" cellpadding="0" align="left" >
    <tbody>
        <tr>
            <td>
                    Bit 7 (MSB)
            </td>
            <td>
                    Bit 6
            </td>
            <td>
                    Bit 5
            </td>
            <td>
                    Bit 4
            </td>
            <td>
                    Bit 3
            </td>
            <td>
                    Bit 2
            </td>
            <td>
                    Bit 1
            </td>
            <td>
                    Bit 0 (LSB)
            </td>
        </tr>
        <tr>
            <td>
                    Compass calibration
            </td>
            <td>
                    Invite
            </td>
            <td>
                    Positionierung
            </td>
            <td>
                    Collision Avoidance
            </td>
            <td>
                    Navigation-Mode
            </td>
            <td>
                    Explore-Mode
            </td>
            <td>
                    unbelegt
            </td>
            <td>
                    unbelegt
            </td>
        </tr>
    </tbody>
</table>


### 3.1.2. Bitfeld – 2. Byte (Logging)

<table>
    <tbody>
        <tr>
            <td>
                    Bit 7 (MSB)
            </td>
            <td>
                    Bit 6
            </td>
            <td>
                    Bit 5
            </td>
            <td>
                    Bit 4
            </td>
            <td>
                    Bit 3
            </td>
            <td>
                    Bit 2
            </td>
            <td>
                    Bit 1
            </td>
            <td>
                    Bit 0 (LSB)
            </td>
        </tr>
        <tr>
            <td>
                    Position
            </td>
            <td>
                    Obstacle distance
            </td>
            <td>
                    Pathfinder path
            </td>
            <td>
                    Compass direction
            </td>
            <td>
                    Anchor distances
            </td>
            <td>
                    Wheel speed
            </td>
            <td>
                    Accelerometer
            </td>
            <td>
                    Gyroscope
            </td>
        </tr>
    </tbody>
</table>

## 3.2. Sensor – Bitfeld

Das Sensor Bitfeld kommt nur im BLE Protokoll zur Anwendung. Durch den MSB
wird angezeigt ob es eine Anforderung durch den Client ist (0) oder eine
Antwort des Alphabots (1).

<table>
    <tbody>
        <tr>
            <td>
                    Bit 7 (MSB)
            </td>
            <td>
                    Bit 4-6
            </td>
            <td>
                    Bit 0-3 (LSB)
            </td>
            <td>
                    Weitere Byte
            </td>
        </tr>
        <tr>
            <td  rowspan="5" >
                    0 für Anforderung /
                    1 für Antwort
            </td>
            <td>
                    000 / Distanzsensor
            </td>
            <td>
                    000 + Grad des Sensors Bit 8 (MSB)*
            </td>
            <td>
                    Grad des Sensors Bit 0-7 (1B) + Distanz (1B)
            </td>
        </tr>
        <tr>
            <td>
                    001 / Kompass
            </td>
            <td>
                    0000
            </td>
            <td>
                    Kompassdaten (2B)
            </td>
        </tr>
        <tr>
            <td>
                    010 / Kartengröße
            </td>
            <td>
                    0000
            </td>
            <td>
                    Länge und Breite (2B)
            </td>
        </tr>
        <tr>
            <td>
                    011 / Positionierung
            </td>
            <td>
                    0000
            </td>
            <td>
                    Koordinaten (4B)
            </td>
        </tr>
        <tr>
            <td>
                    100 / Pos. Anchors
            </td>
            <td>
                    ID des Anchors
            </td>
            <td>
                    Koordinaten (4B)
            </td>
        </tr>
    </tbody>
</table>

*Da für die Darstellung von 360° 9 Bit benötigt werden ist das MSB (Bit 8)
noch im Steuerungsbyte enthalten. Die restlichen 8 Bit werden im nächsten
Byte gesendet.