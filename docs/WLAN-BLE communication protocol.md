# 1. WLAN – bitbasierendes Protokoll

<table border="1" cellspacing="0" cellpadding="0">
    <tbody>
        <tr>
            <td width="377" colspan="3" valign="top">
                <p align="center">
                    <strong>Steuerungsbyte</strong>
                </p>
            </td>
            <td width="152" rowspan="2" valign="top">
                <p>
                    <strong>Weitere Byte</strong>
                </p>
            </td>
            <td width="72" rowspan="2" valign="top">
                <p>
                    <strong>Gesamtgröße Byte</strong>
                </p>
            </td>
        </tr>
        <tr>
            <td width="110" valign="top">
                <p>
                    <strong>Kategorie</strong>
                </p>
            </td>
            <td width="116" valign="top">
                <p>
                    <strong>Unterkategorie</strong>
                </p>
            </td>
            <td width="152" valign="top">
                <p>
                    <strong>Beschreibung</strong>
                </p>
            </td>
        </tr>
        <tr>
            <td width="110" rowspan="3" valign="top">
                <p>
                    00 / Ansteuerung
                </p>
            </td>
            <td width="116" valign="top">
                <p>
                    00 / Steer &amp; Speed
                </p>
            </td>
            <td width="152" valign="top">
                <p>
                    0000
                </p>
            </td>
            <td width="152" valign="top">
                <p>
                    Anzahl der Steps sbyte (1B) + Speed in sbyte (1B)
                </p>
            </td>
            <td width="72" valign="top">
                <p>
                    3
                </p>
            </td>
        </tr>
        <tr>
            <td width="116" rowspan="2" valign="top">
                <p>
                    01 / Calibrate
                </p>
            </td>
            <td width="152" valign="top">
                <p>
                    0000 / Steering
                </p>
            </td>
            <td width="152" valign="top">
            </td>
            <td width="72" rowspan="2" valign="top">
                <p>
                    1
                </p>
            </td>
        </tr>
        <tr>
            <td width="152" valign="top">
                <p>
                    1111 / Compass
                </p>
            </td>
            <td width="152" valign="top">
            </td>
        </tr>
        <tr>
            <td width="110" rowspan="5" valign="top">
                <p>
                    01 / Anforderung (durch Client)
                </p>
            </td>
            <td width="116" valign="top">
                <p>
                    00 / Distanzsensor
                </p>
            </td>
            <td width="152" valign="top">
                <p>
                    000 + Grad des Sensors Bit 8 (MSB)*
                </p>
            </td>
            <td width="152" valign="top">
                <p>
                    Grad des Sensors Bit 0-7 (1B)
                </p>
            </td>
            <td width="72" valign="top">
                <p>
                    2
                </p>
            </td>
        </tr>
        <tr>
            <td width="116" valign="top">
                <p>
                    01 / Kartengröße
                </p>
            </td>
            <td width="152" valign="top">
                <p>
                    0000
                </p>
            </td>
            <td width="152" valign="top">
            </td>
            <td width="72" valign="top">
                <p>
                    1
                </p>
            </td>
        </tr>
        <tr>
            <td width="116" rowspan="2" valign="top">
                <p>
                    10 / Positionierung &amp; Path Finding
                </p>
            </td>
            <td width="152" valign="top">
                <p>
                    0000 / Positionierung
                </p>
            </td>
            <td width="152" rowspan="2" valign="top">
            </td>
            <td width="72" rowspan="2" valign="top">
                <p>
                    1
                </p>
            </td>
        </tr>
        <tr>
            <td width="152" valign="top">
                <p>
                    1111 / Path Finding
                </p>
            </td>
        </tr>
        <tr>
            <td width="116" valign="top">
                <p>
                    11 / Kompass
                </p>
            </td>
            <td width="152" valign="top">
                <p>
                    0000
                </p>
            </td>
            <td width="152" valign="top">
            </td>
            <td width="72" valign="top">
                <p>
                    1
                </p>
            </td>
        </tr>
        <tr>
            <td width="110" rowspan="5" valign="top">
                <p>
                    10 / Antwort (von Alphabot)
                </p>
            </td>
            <td width="116" valign="top">
                <p>
                    00 / Distanzsensor
                </p>
            </td>
            <td width="152" valign="top">
                <p>
                    000 + Grad des Sensors Bit 8 (MSB)*
                </p>
            </td>
            <td width="152" valign="top">
                <p>
                    Grad des Sensors Bit 0-7 (1B) + Distanz (1B)
                </p>
            </td>
            <td width="72" valign="top">
                <p>
                    3
                </p>
            </td>
        </tr>
        <tr>
            <td width="116" valign="top">
                <p>
                    01 / Kartengröße
                </p>
            </td>
            <td width="152" valign="top">
                <p>
                    0000
                </p>
            </td>
            <td width="152" valign="top">
                <p>
                    X und Y Länge (4B)
                </p>
            </td>
            <td width="72" valign="top">
                <p>
                    5
                </p>
            </td>
        </tr>
        <tr>
            <td width="116" rowspan="2" valign="top">
                <p>
                    10 / Positionierung &amp; Path Finding
                </p>
            </td>
            <td width="152" valign="top">
                <p>
                    0000 / Positionierung
                </p>
            </td>
            <td width="152" valign="top">
                <p>
                    X und Y Koordinaten (4B)
                </p>
            </td>
            <td width="72" valign="top">
                <p>
                    5
                </p>
            </td>
        </tr>
        <tr>
            <td width="152" valign="top">
                <p>
                    1111 / Path Finding
                </p>
            </td>
            <td width="152" valign="top">
                <p>
                    Path Finding Daten (max. 20B)
                </p>
            </td>
            <td width="72" valign="top">
                <p>
                    21
                </p>
            </td>
        </tr>
        <tr>
            <td width="116" valign="top">
                <p>
                    11 / Kompass
                </p>
            </td>
            <td width="152" valign="top">
                <p>
                    0000
                </p>
            </td>
            <td width="152" valign="top">
                <p>
                    Kompassdaten (2B)
                </p>
            </td>
            <td width="72" valign="top">
                <p>
                    3
                </p>
            </td>
        </tr>
        <tr>
            <td width="110" rowspan="9" valign="top">
                <p>
                    11 / Sonstiges
                </p>
            </td>
            <td width="116" valign="top">
                <p>
                    00 / Toggle
                </p>
            </td>
            <td width="152" valign="top">
                <p>
                    0000
                </p>
            </td>
            <td width="152" valign="top">
                <p>
                    Siehe Toggle-Bitfeld (2B)
                </p>
            </td>
            <td width="72" valign="top">
                <p>
                    3
                </p>
            </td>
        </tr>
        <tr>
            <td width="116" rowspan="4" valign="top">
                <p>
                    01 / Obstacle
                </p>
            </td>
            <td width="152" valign="top">
                <p>
                    0000 / Add
                </p>
            </td>
            <td width="152" valign="top">
                <p>
                    X, Y Koordinaten (4B) + Breite und Höhe (2B)
                </p>
            </td>
            <td width="72" valign="top">
                <p>
                    7
                </p>
            </td>
        </tr>
        <tr>
            <td width="152" valign="top">
                <p>
                    1100 / Remove one by ID
                </p>
            </td>
            <td width="152" valign="top">
                <p>
                    ID (1B)
                </p>
            </td>
            <td width="72" valign="top">
                <p>
                    2
                </p>
            </td>
        </tr>
        <tr>
            <td width="152" valign="top">
                <p>
                    1101 / Remove one by Coordinates
                </p>
            </td>
            <td width="152" valign="top">
                <p>
                    X, Y Koordinaten (4B)
                </p>
            </td>
            <td width="72" valign="top">
                <p>
                    5
                </p>
            </td>
        </tr>
        <tr>
            <td width="152" valign="top">
                <p>
                    1111 / Remove all
                </p>
            </td>
            <td width="152" valign="top">
            </td>
            <td width="72" valign="top">
                <p>
                    1
                </p>
            </td>
        </tr>
        <tr>
            <td width="116" rowspan="2" valign="top">
                <p>
                    10 / Ping
                </p>
            </td>
            <td width="152" valign="top">
                <p>
                    0000 / sendet Client
                </p>
            </td>
            <td width="152" rowspan="2" valign="top">
            </td>
            <td width="72" rowspan="2" valign="top">
                <p>
                    1
                </p>
            </td>
        </tr>
        <tr>
            <td width="152" valign="top">
                <p>
                    1111 / sendet Alphabot
                </p>
            </td>
        </tr>
        <tr>
            <td width="116" rowspan="2" valign="top">
                <p>
                    11 / Sonstiges
                </p>
            </td>
            <td width="152" valign="top">
                <p>
                    0001 / Configure positioning anchors location
                </p>
            </td>
            <td width="152" valign="top">
                <p>
                    Anchor ID (1B) + X, Y Koordinaten (4B)
                </p>
            </td>
            <td width="72" valign="top">
                <p>
                    6
                </p>
            </td>
        </tr>
        <tr>
            <td width="152" valign="top">
                <p>
                    0010 / Set target position for navigation
                </p>
            </td>
            <td width="152" valign="top">
                <p>
                    X, Y Koordinaten der Position (4B)
                </p>
            </td>
            <td width="72" valign="top">
                <p>
                    5
                </p>
            </td>
        </tr>
    </tbody>
</table>
*Da für die Darstellung von 360° 9 Bit benötigt werden ist das MSB (Bit 8)
noch im Steuerungsbyte enthalten. Die restlichen 8 Bit werden im nächsten
Byte gesendet.

# 2. BLE Protokoll
<table border="1" cellspacing="0" cellpadding="0">
    <tbody>
        <tr>
            <td width="98" rowspan="2" valign="top">
                <p align="center">
                    UUID
                </p>
            </td>
            <td width="158" rowspan="2" valign="top">
                <p align="center">
                    Name
                </p>
            </td>
            <td width="244" colspan="3" valign="top">
                <p align="center">
                    Beschreibung
                </p>
            </td>
            <td width="49" rowspan="2" valign="top">
                <p align="center">
                    Größe in Byte
                </p>
            </td>
            <td width="51" rowspan="2" valign="top">
                <p align="center">
                    <u>R</u>
                    ead/
                </p>
                <p align="center">
                    <u>W</u>
                    rite/
                </p>
                <p align="center">
                    <u>N</u>
                    otify
                </p>
            </td>
        </tr>
        <tr>
            <td width="95" valign="top">
                <p align="center">
                    1. Byte
                </p>
            </td>
            <td width="83" valign="top">
                <p align="center">
                    2. Byte
                </p>
            </td>
            <td width="66" valign="top">
                <p align="center">
                    Weitere Byte
                </p>
            </td>
        </tr>
        <tr>
            <td width="98" valign="top">
                <p>
                    a04295f7-eaa8-4536-b3a4-4e7ae4d72dc2
                </p>
            </td>
            <td width="158" valign="top">
                <p>
                    BLE_CHAR_DRIVE_STEER
                </p>
            </td>
            <td width="95" valign="top">
                <p>
                    Lenkung Steps in sbyte (negativ für links, positiv für
                    rechts)
                </p>
            </td>
            <td width="83" valign="top">
                <p>
                    Speed in sbyte (negativ für rückwärts, positiv für
                    vorwärts)
                </p>
            </td>
            <td width="66" valign="top">
            </td>
            <td width="49" valign="top">
                <p>
                    2
                </p>
            </td>
            <td width="51" valign="top">
                <p>
                    RW
                </p>
            </td>
        </tr>
        <tr>
            <td width="98" valign="top">
                <p>
                    4c999381-35e2-4af4-8443-ee8b9fe56ba0
                </p>
            </td>
            <td width="158" valign="top">
                <p>
                    BLE_CHAR_SENSOR
                </p>
            </td>
            <td width="178" colspan="2" valign="top">
                <p>
                    Siehe Sensor-Bitfeld (3.2.)
                </p>
            </td>
            <td width="66" valign="top">
            </td>
            <td width="49" valign="top">
                <p>
                    5
                </p>
            </td>
            <td width="51" valign="top">
                <p>
                    RWN
                </p>
            </td>
        </tr>
        <tr>
            <td width="98" valign="top">
                <p>
                    fce001d4-864a-48f4-9c95-de928f1da07b
                </p>
            </td>
            <td width="158" valign="top">
                <p>
                    BLE_CHAR_TOGGLE
                </p>
            </td>
            <td width="178" colspan="2" valign="top">
                <p>
                    Siehe Toggle-Bitfeld (3.1.)
                </p>
            </td>
            <td width="66" valign="top">
            </td>
            <td width="49" valign="top">
                <p>
                    2
                </p>
            </td>
            <td width="51" valign="top">
                <p>
                    RWN
                </p>
            </td>
        </tr>
        <tr>
            <td width="98" rowspan="3" valign="top">
                <p>
                    60db37c7-afeb-4d40-bb17-a19a07d6fc95
                </p>
            </td>
            <td width="158" rowspan="3" valign="top">
                <p>
                    BLE_CHAR_OBSTACLE
                </p>
            </td>
            <td width="95" valign="top">
                <p>
                    1 für Add
                </p>
            </td>
            <td width="149" colspan="2" valign="top">
                <p>
                    Koordinaten (4B), Breite und Höhe (2B)
                </p>
            </td>
            <td width="49" rowspan="3" valign="top">
                <p>
                    6
                </p>
            </td>
            <td width="51" rowspan="3" valign="top">
                <p>
                    W
                </p>
            </td>
        </tr>
        <tr>
            <td width="95" valign="top">
                <p>
                    2 für Remove one
                </p>
            </td>
            <td width="149" colspan="2" valign="top">
                <p>
                    Koordinaten (4B), ID (1B)
                </p>
            </td>
        </tr>
        <tr>
            <td width="95" valign="top">
                <p>
                    3 für Remove all
                </p>
            </td>
            <td width="83" valign="top">
            </td>
            <td width="66" valign="top">
            </td>
        </tr>
        <tr>
            <td width="98" valign="top">
                <p>
                    117ad3a5-b257-4465-abd4-7dc12a4cf77d
                </p>
            </td>
            <td width="158" valign="top">
                <p>
                    BLE_CHAR_PINGCLIENT
                </p>
            </td>
            <td width="95" valign="top">
            </td>
            <td width="83" valign="top">
            </td>
            <td width="66" valign="top">
            </td>
            <td width="49" valign="top">
                <p>
                    1
                </p>
            </td>
            <td width="51" valign="top">
                <p>
                    RN
                </p>
            </td>
        </tr>
        <tr>
            <td width="98" valign="top">
                <p>
                    d39e8d54-8019-46c8-a977-db13871bac59
                </p>
            </td>
            <td width="158" valign="top">
                <p>
                    BLE_CHAR_CALIBRATE
                </p>
            </td>
            <td width="95" valign="top">
                <p>
                    0 für calibrate abgeschlossen
                </p>
                <p>
                    1 für calibrate steering Anforderung
                </p>
                <p>
                    2 für calibrate compass direction Anforderung
                </p>
            </td>
            <td width="83" valign="top">
            </td>
            <td width="66" valign="top">
            </td>
            <td width="49" valign="top">
                <p>
                    1
                </p>
            </td>
            <td width="51" valign="top">
                <p>
                    RWN
                </p>
            </td>
        </tr>
        <tr>
            <td width="98" valign="top">
                <p>
                    f56f0a15-52ae-4ad5-bfe1-557eed983618
                </p>
            </td>
            <td width="158" valign="top">
                <p>
                    BLE_CHAR_TARGET_NAVI
                </p>
            </td>
            <td width="178" colspan="2" valign="top">
                <p>
                    X Koordinate
                </p>
            </td>
            <td width="66" valign="top">
                <p>
                    Y Koordinate (2B)
                </p>
            </td>
            <td width="49" valign="top">
                <p>
                    4
                </p>
            </td>
            <td width="51" valign="top">
                <p>
                    RW
                </p>
            </td>
        </tr>
        <tr>
            <td width="98" valign="top">
                <p>
                    8dad4c9a-1a1c-4a42-a522-ded592f4ed99
                </p>
            </td>
            <td width="158" valign="top">
                <p>
                    BLE_CHAR_PATH_FINDING
                </p>
            </td>
            <td width="244" colspan="3" valign="top">
                <p>
                    Path Finding Daten
                </p>
            </td>
            <td width="49" valign="top">
                <p>
                    20
                </p>
            </td>
            <td width="51" valign="top">
                <p>
                    R
                </p>
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

<table border="1" cellspacing="0" cellpadding="0" align="left" width="613">
    <tbody>
        <tr>
            <td width="78" valign="top">
                <p>
                    Bit 7 (MSB)
                </p>
            </td>
            <td width="76" valign="top">
                <p>
                    Bit 6
                </p>
            </td>
            <td width="76" valign="top">
                <p>
                    Bit 5
                </p>
            </td>
            <td width="76" valign="top">
                <p>
                    Bit 4
                </p>
            </td>
            <td width="76" valign="top">
                <p>
                    Bit 3
                </p>
            </td>
            <td width="76" valign="top">
                <p>
                    Bit 2
                </p>
            </td>
            <td width="76" valign="top">
                <p>
                    Bit 1
                </p>
            </td>
            <td width="76" valign="top">
                <p>
                    Bit 0 (LSB)
                </p>
            </td>
        </tr>
        <tr>
            <td width="78" valign="top">
                <p>
                    Compass calibration
                </p>
            </td>
            <td width="76" valign="top">
                <p>
                    Invite
                </p>
            </td>
            <td width="76" valign="top">
                <p>
                    Positionierung
                </p>
            </td>
            <td width="76" valign="top">
                <p>
                    Collision Avoidance
                </p>
            </td>
            <td width="76" valign="top">
                <p>
                    Navigation-Mode
                </p>
            </td>
            <td width="76" valign="top">
                <p>
                    Explore-Mode
                </p>
            </td>
            <td width="76" valign="top">
                <p>
                    unbelegt
                </p>
            </td>
            <td width="76" valign="top">
                <p>
                    unbelegt
                </p>
            </td>
        </tr>
    </tbody>
</table>

### 3.1.2. Bitfeld – 2. Byte (Logging)
<table border="1" cellspacing="0" cellpadding="0">
    <tbody>
        <tr>
            <td width="75" valign="top">
                <p>
                    Bit 7 (MSB)
                </p>
            </td>
            <td width="75" valign="top">
                <p>
                    Bit 6
                </p>
            </td>
            <td width="75" valign="top">
                <p>
                    Bit 5
                </p>
            </td>
            <td width="75" valign="top">
                <p>
                    Bit 4
                </p>
            </td>
            <td width="75" valign="top">
                <p>
                    Bit 3
                </p>
            </td>
            <td width="75" valign="top">
                <p>
                    Bit 2
                </p>
            </td>
            <td width="75" valign="top">
                <p>
                    Bit 1
                </p>
            </td>
            <td width="75" valign="top">
                <p>
                    Bit 0 (LSB)
                </p>
            </td>
        </tr>
        <tr>
            <td width="75" valign="top">
                <p>
                    Position
                </p>
            </td>
            <td width="75" valign="top">
                <p>
                    Obstacle distance
                </p>
            </td>
            <td width="75" valign="top">
                <p>
                    Pathfinder path
                </p>
            </td>
            <td width="75" valign="top">
                <p>
                    Compass direction
                </p>
            </td>
            <td width="75" valign="top">
                <p>
                    Anchor distances
                </p>
            </td>
            <td width="75" valign="top">
                <p>
                    Wheel speed
                </p>
            </td>
            <td width="75" valign="top">
                <p>
                    Accelerometer
                </p>
            </td>
            <td width="75" valign="top">
                <p>
                    Gyroscope
                </p>
            </td>
        </tr>
    </tbody>
</table>
## 3.2. Sensor – Bitfeld

Das Sensor Bitfeld kommt nur im BLE Protokoll zur Anwendung. Durch den MSB
wird angezeigt ob es eine Anforderung durch den Client ist (0) oder eine
Antwort des Alphabots (1).

<table border="1" cellspacing="0" cellpadding="0" align="left">
    <tbody>
        <tr>
            <td width="150" valign="top">
                <p>
                    Bit 7 (MSB)
                </p>
            </td>
            <td width="150" valign="top">
                <p>
                    Bit 4-6
                </p>
            </td>
            <td width="150" valign="top">
                <p>
                    Bit 0-3 (LSB)
                </p>
            </td>
            <td width="150" valign="top">
                <p>
                    Weitere Byte
                </p>
            </td>
        </tr>
        <tr>
            <td width="150" rowspan="5" valign="top">
                <p>
                    0 für Anforderung /
                </p>
                <p>
                    1 für Antwort
                </p>
            </td>
            <td width="150" valign="top">
                <p>
                    000 / Distanzsensor
                </p>
            </td>
            <td width="150" valign="top">
                <p>
                    000 + Grad des Sensors Bit 8 (MSB)*
                </p>
            </td>
            <td width="150" valign="top">
                <p>
                    Grad des Sensors Bit 0-7 (1B) + Distanz (1B)
                </p>
            </td>
        </tr>
        <tr>
            <td width="150" valign="top">
                <p>
                    001 / Kompass
                </p>
            </td>
            <td width="150" valign="top">
                <p>
                    0000
                </p>
            </td>
            <td width="150" valign="top">
                <p>
                    Kompassdaten (2B)
                </p>
            </td>
        </tr>
        <tr>
            <td width="150" valign="top">
                <p>
                    010 / Kartengröße
                </p>
            </td>
            <td width="150" valign="top">
                <p>
                    0000
                </p>
            </td>
            <td width="150" valign="top">
                <p>
                    Länge und Breite (2B)
                </p>
            </td>
        </tr>
        <tr>
            <td width="150" valign="top">
                <p>
                    011 / Positionierung
                </p>
            </td>
            <td width="150" valign="top">
                <p>
                    0000
                </p>
            </td>
            <td width="150" valign="top">
                <p>
                    Koordinaten (4B)
                </p>
            </td>
        </tr>
        <tr>
            <td width="150" valign="top">
                <p>
                    100 / Pos. Anchors
                </p>
            </td>
            <td width="150" valign="top">
                <p>
                    ID des Anchors
                </p>
            </td>
            <td width="150" valign="top">
                <p>
                    Koordinaten (4B)
                </p>
            </td>
        </tr>
    </tbody>
</table>



*Da für die Darstellung von 360° 9 Bit benötigt werden ist das MSB (Bit 8)
noch im Steuerungsbyte enthalten. Die restlichen 8 Bit werden im nächsten
Byte gesendet.

