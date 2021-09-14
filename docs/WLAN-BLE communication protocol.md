# 1. WLAN – bitbasierendes Protokoll

<table border="1" cellspacing="0" cellpadding="0">
    <tbody>
        <tr>
            <td width="377" colspan="3" valign="top">
                    <strong>Steuerungsbyte</strong>
            </td>
            <td width="152" rowspan="2" valign="top">
                    <strong>Weitere Byte</strong>
            </td>
            <td width="72" rowspan="2" valign="top">
                    <strong>Gesamtgröße Byte</strong>
            </td>
        </tr>
        <tr>
            <td width="110" valign="top">
                    <strong>Kategorie</strong>
            </td>
            <td width="116" valign="top">
                    <strong>Unterkategorie</strong>
            </td>
            <td width="152" valign="top">
                    <strong>Beschreibung</strong>
            </td>
        </tr>
        <tr>
            <td width="110" rowspan="3" valign="top">
                    00 / Ansteuerung
            </td>
            <td width="116" valign="top">
                    00 / Steer &amp; Speed
            </td>
            <td width="152" valign="top">
                    0000
            </td>
            <td width="152" valign="top">
                    Anzahl der Steps sbyte (1B) + Speed in sbyte (1B)
            </td>
            <td width="72" valign="top">
                    3
            </td>
        </tr>
        <tr>
            <td width="116" rowspan="2" valign="top">
                    01 / Calibrate
            </td>
            <td width="152" valign="top">
                    0000 / Steering
            </td>
            <td width="152" valign="top">
            </td>
            <td width="72" rowspan="2" valign="top">
                    1
            </td>
        </tr>
        <tr>
            <td width="152" valign="top">
                    1111 / Compass
            </td>
            <td width="152" valign="top">
            </td>
        </tr>
        <tr>
            <td width="110" rowspan="5" valign="top">
                    01 / Anforderung (durch Client)
            </td>
            <td width="116" valign="top">
                    00 / Distanzsensor
            </td>
            <td width="152" valign="top">
                    000 + Grad des Sensors Bit 8 (MSB)*
            </td>
            <td width="152" valign="top">
                    Grad des Sensors Bit 0-7 (1B)
            </td>
            <td width="72" valign="top">
                    2
            </td>
        </tr>
        <tr>
            <td width="116" valign="top">
                    01 / Kartengröße
            </td>
            <td width="152" valign="top">
                    0000
            </td>
            <td width="152" valign="top">
            </td>
            <td width="72" valign="top">
                    1
            </td>
        </tr>
        <tr>
            <td width="116" rowspan="2" valign="top">
                    10 / Positionierung &amp; Path Finding
            </td>
            <td width="152" valign="top">
                    0000 / Positionierung
            </td>
            <td width="152" rowspan="2" valign="top">
            </td>
            <td width="72" rowspan="2" valign="top">
                    1
            </td>
        </tr>
        <tr>
            <td width="152" valign="top">
                    1111 / Path Finding
            </td>
        </tr>
        <tr>
            <td width="116" valign="top">
                    11 / Kompass
            </td>
            <td width="152" valign="top">
                    0000
            </td>
            <td width="152" valign="top">
            </td>
            <td width="72" valign="top">
                    1
            </td>
        </tr>
        <tr>
            <td width="110" rowspan="5" valign="top">
                    10 / Antwort (von Alphabot)
            </td>
            <td width="116" valign="top">
                    00 / Distanzsensor
            </td>
            <td width="152" valign="top">
                    000 + Grad des Sensors Bit 8 (MSB)*
            </td>
            <td width="152" valign="top">
                    Grad des Sensors Bit 0-7 (1B) + Distanz (1B)
            </td>
            <td width="72" valign="top">
                    3
            </td>
        </tr>
        <tr>
            <td width="116" valign="top">
                    01 / Kartengröße
            </td>
            <td width="152" valign="top">
                    0000
            </td>
            <td width="152" valign="top">
                    X und Y Länge (4B)
            </td>
            <td width="72" valign="top">
                    5
            </td>
        </tr>
        <tr>
            <td width="116" rowspan="2" valign="top">
                    10 / Positionierung &amp; Path Finding
            </td>
            <td width="152" valign="top">
                    0000 / Positionierung
            </td>
            <td width="152" valign="top">
                    X und Y Koordinaten (4B)
            </td>
            <td width="72" valign="top">
                    5
            </td>
        </tr>
        <tr>
            <td width="152" valign="top">
                    1111 / Path Finding
            </td>
            <td width="152" valign="top">
                    Path Finding Daten (max. 20B)
            </td>
            <td width="72" valign="top">
                    21
            </td>
        </tr>
        <tr>
            <td width="116" valign="top">
                    11 / Kompass
            </td>
            <td width="152" valign="top">
                    0000
            </td>
            <td width="152" valign="top">
                    Kompassdaten (2B)
            </td>
            <td width="72" valign="top">
                    3
            </td>
        </tr>
        <tr>
            <td width="110" rowspan="9" valign="top">
                    11 / Sonstiges
            </td>
            <td width="116" valign="top">
                    00 / Toggle
            </td>
            <td width="152" valign="top">
                    0000
            </td>
            <td width="152" valign="top">
                    Siehe Toggle-Bitfeld (2B)
            </td>
            <td width="72" valign="top">
                    3
            </td>
        </tr>
        <tr>
            <td width="116" rowspan="4" valign="top">
                    01 / Obstacle
            </td>
            <td width="152" valign="top">
                    0000 / Add
            </td>
            <td width="152" valign="top">
                    X, Y Koordinaten (4B) + Breite und Höhe (2B)
            </td>
            <td width="72" valign="top">
                    7
            </td>
        </tr>
        <tr>
            <td width="152" valign="top">
                    1100 / Remove one by ID
            </td>
            <td width="152" valign="top">
                    ID (1B)
            </td>
            <td width="72" valign="top">
                    2
            </td>
        </tr>
        <tr>
            <td width="152" valign="top">
                    1101 / Remove one by Coordinates
            </td>
            <td width="152" valign="top">
                    X, Y Koordinaten (4B)
            </td>
            <td width="72" valign="top">
                    5
            </td>
        </tr>
        <tr>
            <td width="152" valign="top">
                    1111 / Remove all
            </td>
            <td width="152" valign="top">
            </td>
            <td width="72" valign="top">
                    1
            </td>
        </tr>
        <tr>
            <td width="116" rowspan="2" valign="top">
                    10 / Ping
            </td>
            <td width="152" valign="top">
                    0000 / sendet Client
            </td>
            <td width="152" rowspan="2" valign="top">
            </td>
            <td width="72" rowspan="2" valign="top">
                    1
            </td>
        </tr>
        <tr>
            <td width="152" valign="top">
                    1111 / sendet Alphabot
            </td>
        </tr>
        <tr>
            <td width="116" rowspan="2" valign="top">
                    11 / Sonstiges
            </td>
            <td width="152" valign="top">
                    0001 / Configure positioning anchors location
            </td>
            <td width="152" valign="top">
                    Anchor ID (1B) + X, Y Koordinaten (4B)
            </td>
            <td width="72" valign="top">
                    6
            </td>
        </tr>
        <tr>
            <td width="152" valign="top">
                    0010 / Set target position for navigation
            </td>
            <td width="152" valign="top">
                    X, Y Koordinaten der Position (4B)
            </td>
            <td width="72" valign="top">
                    5
            </td>
        </tr>
    </tbody>
</table>

*Da für die Darstellung von 360° 9 Bit benötigt werden ist das MSB (Bit 8)
noch im Steuerungsbyte enthalten. Die restlichen 8 Bit werden im nächsten
Byte gesendet.

# 2. BLE Protokoll

<br>
<table border="1" cellspacing="0" cellpadding="0">
    <tbody>
        <tr>
            <td width="98" rowspan="2" valign="top">
                <p align="center">
                    UUID
            </td>
            <td width="158" rowspan="2" valign="top">
                <p align="center">
                    Name
            </td>
            <td width="244" colspan="3" valign="top">
                <p align="center">
                    Beschreibung
            </td>
            <td width="49" rowspan="2" valign="top">
                <p align="center">
                    Größe in Byte
            </td>
            <td width="51" rowspan="2" valign="top">
                <p align="center">
                    <u>R</u>
                    ead/
                <p align="center">
                    <u>W</u>
                    rite/
                <p align="center">
                    <u>N</u>
                    otify
            </td>
        </tr>
        <tr>
            <td width="95" valign="top">
                <p align="center">
                    1. Byte
            </td>
            <td width="83" valign="top">
                <p align="center">
                    2. Byte
            </td>
            <td width="66" valign="top">
                <p align="center">
                    Weitere Byte
            </td>
        </tr>
        <tr>
            <td width="98" valign="top">
                    a04295f7-eaa8-4536-b3a4-4e7ae4d72dc2
            </td>
            <td width="158" valign="top">
                    BLE_CHAR_DRIVE_STEER
            </td>
            <td width="95" valign="top">
                    Lenkung Steps in sbyte (negativ für links, positiv für
                    rechts)
            </td>
            <td width="83" valign="top">
                    Speed in sbyte (negativ für rückwärts, positiv für
                    vorwärts)
            </td>
            <td width="66" valign="top">
            </td>
            <td width="49" valign="top">
                    2
            </td>
            <td width="51" valign="top">
                    RW
            </td>
        </tr>
        <tr>
            <td width="98" valign="top">
                    4c999381-35e2-4af4-8443-ee8b9fe56ba0
            </td>
            <td width="158" valign="top">
                    BLE_CHAR_SENSOR
            </td>
            <td width="178" colspan="2" valign="top">
                    Siehe Sensor-Bitfeld (3.2.)
            </td>
            <td width="66" valign="top">
            </td>
            <td width="49" valign="top">
                    5
            </td>
            <td width="51" valign="top">
                    RWN
            </td>
        </tr>
        <tr>
            <td width="98" valign="top">
                    fce001d4-864a-48f4-9c95-de928f1da07b
            </td>
            <td width="158" valign="top">
                    BLE_CHAR_TOGGLE
            </td>
            <td width="178" colspan="2" valign="top">
                    Siehe Toggle-Bitfeld (3.1.)
            </td>
            <td width="66" valign="top">
            </td>
            <td width="49" valign="top">
                    2
            </td>
            <td width="51" valign="top">
                    RWN
            </td>
        </tr>
        <tr>
            <td width="98" rowspan="3" valign="top">
                    60db37c7-afeb-4d40-bb17-a19a07d6fc95
            </td>
            <td width="158" rowspan="3" valign="top">
                    BLE_CHAR_OBSTACLE
            </td>
            <td width="95" valign="top">
                    1 für Add
            </td>
            <td width="149" colspan="2" valign="top">
                    Koordinaten (4B), Breite und Höhe (2B)
            </td>
            <td width="49" rowspan="3" valign="top">
                    6
            </td>
            <td width="51" rowspan="3" valign="top">
                    W
            </td>
        </tr>
        <tr>
            <td width="95" valign="top">
                    2 für Remove one
            </td>
            <td width="149" colspan="2" valign="top">
                    Koordinaten (4B), ID (1B)
            </td>
        </tr>
        <tr>
            <td width="95" valign="top">
                    3 für Remove all
            </td>
            <td width="83" valign="top">
            </td>
            <td width="66" valign="top">
            </td>
        </tr>
        <tr>
            <td width="98" valign="top">
                    117ad3a5-b257-4465-abd4-7dc12a4cf77d
            </td>
            <td width="158" valign="top">
                    BLE_CHAR_PINGCLIENT
            </td>
            <td width="95" valign="top">
            </td>
            <td width="83" valign="top">
            </td>
            <td width="66" valign="top">
            </td>
            <td width="49" valign="top">
                    1
            </td>
            <td width="51" valign="top">
                    RN
            </td>
        </tr>
        <tr>
            <td width="98" valign="top">
                    d39e8d54-8019-46c8-a977-db13871bac59
            </td>
            <td width="158" valign="top">
                    BLE_CHAR_CALIBRATE
            </td>
            <td width="95" valign="top">
                    0 für calibrate abgeschlossen
                    1 für calibrate steering Anforderung
                    2 für calibrate compass direction Anforderung
            </td>
            <td width="83" valign="top">
            </td>
            <td width="66" valign="top">
            </td>
            <td width="49" valign="top">
                    1
            </td>
            <td width="51" valign="top">
                    RWN
            </td>
        </tr>
        <tr>
            <td width="98" valign="top">
                    f56f0a15-52ae-4ad5-bfe1-557eed983618
            </td>
            <td width="158" valign="top">
                    BLE_CHAR_TARGET_NAVI
            </td>
            <td width="178" colspan="2" valign="top">
                    X Koordinate
            </td>
            <td width="66" valign="top">
                    Y Koordinate (2B)
            </td>
            <td width="49" valign="top">
                    4
            </td>
            <td width="51" valign="top">
                    RW
            </td>
        </tr>
        <tr>
            <td width="98" valign="top">
                    8dad4c9a-1a1c-4a42-a522-ded592f4ed99
            </td>
            <td width="158" valign="top">
                    BLE_CHAR_PATH_FINDING
            </td>
            <td width="244" colspan="3" valign="top">
                    Path Finding Daten
            </td>
            <td width="49" valign="top">
                    20
            </td>
            <td width="51" valign="top">
                    R
            </td>
        </tr>
    </tbody>
</table>
<br>

# 3. Bitfelder

## 3.1. Toggle - Bitfeld

Im WLAN Protokoll wird mit der Bit-Folge 1100 0000 anschließend ein 2 Byte
großes Bitfeld mitgesendet, im BLE Protokoll ist es eine Characteristic
welche 2 Byte groß ist. Dieses Bitfeld ermöglicht es verschiedene
Einstellungen ein- bzw. ausschalten zu lassen. Wird eine 0 gesendet, ist
die jeweilige Einstellung zu deaktivieren, wird eine 1 gesendet soll es
aktiviert werden. Im 1. Byte werden allgemeine Einstellungen gesendet. 
Das 2. Byte ändert bestimmte Logging Optionen.
<br>

### 3.1.1. Bitfeld – 1. Byte (Einstellungen)
<br>

<table border="1" cellspacing="0" cellpadding="0" align="left" width="613">
    <tbody>
        <tr>
            <td width="78" valign="top">
                    Bit 7 (MSB)
            </td>
            <td width="76" valign="top">
                    Bit 6
            </td>
            <td width="76" valign="top">
                    Bit 5
            </td>
            <td width="76" valign="top">
                    Bit 4
            </td>
            <td width="76" valign="top">
                    Bit 3
            </td>
            <td width="76" valign="top">
                    Bit 2
            </td>
            <td width="76" valign="top">
                    Bit 1
            </td>
            <td width="76" valign="top">
                    Bit 0 (LSB)
            </td>
        </tr>
        <tr>
            <td width="78" valign="top">
                    Compass calibration
            </td>
            <td width="76" valign="top">
                    Invite
            </td>
            <td width="76" valign="top">
                    Positionierung
            </td>
            <td width="76" valign="top">
                    Collision Avoidance
            </td>
            <td width="76" valign="top">
                    Navigation-Mode
            </td>
            <td width="76" valign="top">
                    Explore-Mode
            </td>
            <td width="76" valign="top">
                    unbelegt
            </td>
            <td width="76" valign="top">
                    unbelegt
            </td>
        </tr>
    </tbody>
</table>
<br><br><br><br>

### 3.1.2. Bitfeld – 2. Byte (Logging)

<table border="1" cellspacing="0" cellpadding="0">
    <tbody>
        <tr>
            <td width="75" valign="top">
                    Bit 7 (MSB)
            </td>
            <td width="75" valign="top">
                    Bit 6
            </td>
            <td width="75" valign="top">
                    Bit 5
            </td>
            <td width="75" valign="top">
                    Bit 4
            </td>
            <td width="75" valign="top">
                    Bit 3
            </td>
            <td width="75" valign="top">
                    Bit 2
            </td>
            <td width="75" valign="top">
                    Bit 1
            </td>
            <td width="75" valign="top">
                    Bit 0 (LSB)
            </td>
        </tr>
        <tr>
            <td width="75" valign="top">
                    Position
            </td>
            <td width="75" valign="top">
                    Obstacle distance
            </td>
            <td width="75" valign="top">
                    Pathfinder path
            </td>
            <td width="75" valign="top">
                    Compass direction
            </td>
            <td width="75" valign="top">
                    Anchor distances
            </td>
            <td width="75" valign="top">
                    Wheel speed
            </td>
            <td width="75" valign="top">
                    Accelerometer
            </td>
            <td width="75" valign="top">
                    Gyroscope
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
                    Bit 7 (MSB)
            </td>
            <td width="150" valign="top">
                    Bit 4-6
            </td>
            <td width="150" valign="top">
                    Bit 0-3 (LSB)
            </td>
            <td width="150" valign="top">
                    Weitere Byte
            </td>
        </tr>
        <tr>
            <td width="150" rowspan="5" valign="top">
                    0 für Anforderung /
                    1 für Antwort
            </td>
            <td width="150" valign="top">
                    000 / Distanzsensor
            </td>
            <td width="150" valign="top">
                    000 + Grad des Sensors Bit 8 (MSB)*
            </td>
            <td width="150" valign="top">
                    Grad des Sensors Bit 0-7 (1B) + Distanz (1B)
            </td>
        </tr>
        <tr>
            <td width="150" valign="top">
                    001 / Kompass
            </td>
            <td width="150" valign="top">
                    0000
            </td>
            <td width="150" valign="top">
                    Kompassdaten (2B)
            </td>
        </tr>
        <tr>
            <td width="150" valign="top">
                    010 / Kartengröße
            </td>
            <td width="150" valign="top">
                    0000
            </td>
            <td width="150" valign="top">
                    Länge und Breite (2B)
            </td>
        </tr>
        <tr>
            <td width="150" valign="top">
                    011 / Positionierung
            </td>
            <td width="150" valign="top">
                    0000
            </td>
            <td width="150" valign="top">
                    Koordinaten (4B)
            </td>
        </tr>
        <tr>
            <td width="150" valign="top">
                    100 / Pos. Anchors
            </td>
            <td width="150" valign="top">
                    ID des Anchors
            </td>
            <td width="150" valign="top">
                    Koordinaten (4B)
            </td>
        </tr>
    </tbody>
</table>
<br><br><br><br><br><br><br><br><br><br><br>
*Da für die Darstellung von 360° 9 Bit benötigt werden ist das MSB (Bit 8)
noch im Steuerungsbyte enthalten. Die restlichen 8 Bit werden im nächsten
Byte gesendet.