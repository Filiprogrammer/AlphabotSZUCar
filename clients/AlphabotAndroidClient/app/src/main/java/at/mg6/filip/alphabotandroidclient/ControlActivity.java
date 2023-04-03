package at.mg6.filip.alphabotandroidclient;

import android.bluetooth.BluetoothGattCharacteristic;
import android.content.Context;
import android.content.SharedPreferences;
import android.hardware.Sensor;
import android.hardware.SensorEvent;
import android.hardware.SensorEventListener;
import android.hardware.SensorManager;
import android.os.Bundle;
import android.support.annotation.Nullable;
import android.view.View;
import android.widget.CompoundButton;
import android.widget.ProgressBar;
import android.widget.Switch;
import android.widget.TextView;
import android.widget.Toast;
import android.widget.Button;

import java.math.BigInteger;
import java.text.DecimalFormat;
import java.util.Timer;
import java.util.TimerTask;

public class ControlActivity extends ImmersiveActivity implements SensorEventListener, LPSConfigureDialog.LPSConfigureDialogListener, ObstacleConfigureDialog.ObstacleConfigureDialogListener, LPSView.LPSViewListener {
    TextView pingView;
    TextView kmphView;
    TextView mpsView;
    TextView lpsPosView;
    Button btnDisconnect;
    Switch swtchCollisionAvoid;
    Switch swtchPosition;
    Switch swtchNavigation;
    Switch swtchExplore;
    Switch swtchLogImu;
    Switch swtchLogWheelSpeed;
    Switch swtchLogAnchorDistances;
    Switch swtchLogCompassDirection;
    Switch swtchLogPathfinderPath;
    Switch swtchLogObstacleDistance;
    Switch swtchLogPosition;
    Button btnAccelerate;
    Button btnBrake;
    Button btnCalibrateSteering;
    Button btnCalibrateCompassDirection;
    Switch swtchInvite;
    Switch swtchCalibrateMagnetometer;
    Button btnAddObstacle;
    Button btnRemoveObstacle;
    ProgressBar speedBar;
    ProgressBar speedLeftBar;
    ProgressBar speedRightBar;
    LPSView lpsView;

    BluetoothLeServiceListener bleListener;

    SensorManager sensorManager;
    int carSpeed = 0;
    int carSpeed_prev = carSpeed;
    float carSteer = 0;
    float carSteer_prev = carSteer;

    boolean exploreMode = false;
    boolean navigationMode = false;
    boolean collisionAvoidance = true;
    boolean positioning = false;
    boolean invite = false;

    boolean loggingImu = false;
    boolean loggingWheelSpeed = false;
    boolean loggingAnchorDistances = false;
    boolean loggingCompassDirection = false;
    boolean loggingPathfinderPath = false;
    boolean loggingObstacleDistance = false;
    boolean loggingPositioning = false;

    BLECharacteristicSender toggleCharSender = new BLECharacteristicSender(BLEHandler.getInstance().bleClient, BLEHandler.getInstance().bleCharToggle);
    BLECharacteristicSender pathfindingTargetCharSender = new BLECharacteristicSender(BLEHandler.getInstance().bleClient, BLEHandler.getInstance().bleCharPathfindingTarget);
    BLECharacteristicSender calibrateCharSender = new BLECharacteristicSender(BLEHandler.getInstance().bleClient, BLEHandler.getInstance().bleCharCalibrate);
    BLECharacteristicSender addObstacleCharSender = new BLECharacteristicSender(BLEHandler.getInstance().bleClient, BLEHandler.getInstance().bleCharAddObstacle);
    BLECharacteristicSender removeObstacleCharSender = new BLECharacteristicSender(BLEHandler.getInstance().bleClient, BLEHandler.getInstance().bleCharRemoveObstacle);
    BLECharacteristicSender anchorLocationsCharSender = new BLECharacteristicSender(BLEHandler.getInstance().bleClient, BLEHandler.getInstance().bleCharAnchorLocations);

    Obstacle pendingObstacleToAdd = null;
    Obstacle pendingObstacleToRemove = null;

    @Override
    protected void onCreate(@Nullable Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_control);
        pingView = findViewById(R.id.pingView);
        kmphView = findViewById(R.id.kmphView);
        mpsView = findViewById(R.id.mpsView);
        lpsPosView = findViewById(R.id.lpsPosView);
        btnDisconnect = findViewById(R.id.btnDisconnect);
        swtchCollisionAvoid = findViewById(R.id.swtchCollisionAvoid);
        swtchPosition = findViewById(R.id.swtchPositioning);
        swtchNavigation = findViewById(R.id.swtchNavigation);
        swtchExplore = findViewById(R.id.swtchExplore);
        swtchLogImu = findViewById(R.id.swtchLogImu);
        swtchLogWheelSpeed = findViewById(R.id.swtchLogWheelSpeed);
        swtchLogAnchorDistances = findViewById(R.id.swtchLogAnchorDistances);
        swtchLogCompassDirection = findViewById(R.id.swtchLogCompassDirection);
        swtchLogPathfinderPath = findViewById(R.id.swtchLogPathfinderPath);
        swtchLogObstacleDistance = findViewById(R.id.swtchLogObstacleDistance);
        swtchLogPosition = findViewById(R.id.swtchLogPosition);
        btnAccelerate = findViewById(R.id.btnAccelerate);
        btnBrake = findViewById(R.id.btnBrake);
        btnCalibrateSteering = findViewById(R.id.btnCalibrateSteering);
        btnCalibrateCompassDirection = findViewById(R.id.btnCalibrateCompassDirection);
        swtchInvite = findViewById(R.id.swtchInvite);
        swtchCalibrateMagnetometer = findViewById(R.id.swtchCalibrateMagnetometer);
        btnAddObstacle = findViewById(R.id.btnAddObstacle);
        btnRemoveObstacle = findViewById(R.id.btnRemoveObstacle);
        speedBar = findViewById(R.id.speedBar);
        speedLeftBar = findViewById(R.id.speedLeftBar);
        speedRightBar = findViewById(R.id.speedRightBar);
        lpsView = findViewById(R.id.lps_canvas);
        setTitle("Alphabot Android Client");
        loadLPSAnchorsPreference();
        setupTasksAndListeners();
    }

    @Override
    public void onSensorChanged(SensorEvent sensorEvent) {
        if(sensorEvent.sensor.getType() == Sensor.TYPE_ACCELEROMETER)
            carSteer = sensorEvent.values[1] * 13;
    }

    @Override
    public void onAccuracyChanged(Sensor sensor, int i) {}

    private void setupTasksAndListeners() {
        sensorManager = (SensorManager) getSystemService(Context.SENSOR_SERVICE);
        sensorManager.registerListener(this, sensorManager.getDefaultSensor(Sensor.TYPE_ACCELEROMETER), SensorManager.SENSOR_DELAY_GAME);

        btnDisconnect.setOnClickListener(new View.OnClickListener() {
            @Override
            public void onClick(View view) {
                BLEHandler.getInstance().disconnect();
            }
        });

        btnCalibrateSteering.setOnClickListener(new View.OnClickListener() {
            @Override
            public void onClick(View view) {
                btnCalibrateSteering.setEnabled(false);
                byte[] vals = new byte[9];
                vals[0] = 0;
                writeTimestamp(vals, 1);
                calibrateCharSender.sendValue(vals);
            }
        });

        btnCalibrateCompassDirection.setOnClickListener(new View.OnClickListener() {
            @Override
            public void onClick(View view) {
                btnCalibrateCompassDirection.setEnabled(false);
                byte[] vals = new byte[9];
                vals[0] = 4;
                writeTimestamp(vals, 1);
                calibrateCharSender.sendValue(vals);
            }
        });

        swtchCalibrateMagnetometer.setOnCheckedChangeListener(new CompoundButton.OnCheckedChangeListener() {
            @Override
            public void onCheckedChanged(CompoundButton compoundButton, boolean b) {
                swtchCalibrateMagnetometer.setEnabled(false);
                byte[] vals = new byte[9];
                vals[0] = (byte)(b ? 2 : 3);
                writeTimestamp(vals, 1);
                calibrateCharSender.sendValue(vals);
            }
        });

        swtchInvite.setOnCheckedChangeListener(new CompoundButton.OnCheckedChangeListener() {
            @Override
            public void onCheckedChanged(CompoundButton compoundButton, boolean b) {
                swtchInvite.setEnabled(false);
                invite = b;
                sendToggleUpdate();
            }
        });

        final ControlActivity _this = this;

        btnAddObstacle.setOnClickListener(new View.OnClickListener() {
            @Override
            public void onClick(View view) {
                btnAddObstacle.setEnabled(false);
                ObstacleConfigureDialog obstacleConfigureDialog = new ObstacleConfigureDialog();
                Bundle args = new Bundle();
                args.putShort("x", (short)30);
                args.putShort("y", (short)0);
                args.putShort("width", (short)40);
                args.putShort("height", (short)50);
                obstacleConfigureDialog.setArguments(args);
                obstacleConfigureDialog.show(getSupportFragmentManager(), "obstacle_configure_dialog");
            }
        });

        btnRemoveObstacle.setOnClickListener(new View.OnClickListener() {
            @Override
            public void onClick(View view) {
                Obstacle selectedObstacle = lpsView.getSelectedObstacle();

                if (selectedObstacle != null) {
                    btnRemoveObstacle.setEnabled(false);

                    byte[] vals = new byte[10];
                    int id = selectedObstacle.getId();
                    vals[8] = (byte)id;
                    vals[9] = (byte)(id >> 8);
                    writeTimestamp(vals, 0);
                    removeObstacleCharSender.sendValue(vals);

                    pendingObstacleToRemove = selectedObstacle;
                    lpsView.invalidate();
                }
            }
        });

        swtchCollisionAvoid.setOnCheckedChangeListener(new CompoundButton.OnCheckedChangeListener() {
            @Override
            public void onCheckedChanged(CompoundButton compoundButton, boolean b) {
                swtchCollisionAvoid.setEnabled(false);
                collisionAvoidance = b;
                sendToggleUpdate();
            }
        });

        swtchPosition.setOnCheckedChangeListener(new CompoundButton.OnCheckedChangeListener() {
            @Override
            public void onCheckedChanged(CompoundButton compoundButton, final boolean b) {
                swtchPosition.setEnabled(false);

                if(b) {
                    LPSConfigureDialog lpsConfigureDialog = new LPSConfigureDialog();
                    Bundle args = new Bundle();
                    args.putShort("anchor0x", lpsView.anchor0x);
                    args.putShort("anchor0y", lpsView.anchor0y);
                    args.putShort("anchor1x", lpsView.anchor1x);
                    args.putShort("anchor1y", lpsView.anchor1y);
                    args.putShort("anchor2x", lpsView.anchor2x);
                    args.putShort("anchor2y", lpsView.anchor2y);
                    lpsConfigureDialog.setArguments(args);
                    lpsConfigureDialog.show(getSupportFragmentManager(), "lps_configure_dialog");
                } else {
                    // Disable positioning
                    positioning = false;
                    sendToggleUpdate();
                }
            }
        });

        swtchNavigation.setOnCheckedChangeListener(new CompoundButton.OnCheckedChangeListener() {
            @Override
            public void onCheckedChanged(CompoundButton compoundButton, boolean b) {
                swtchNavigation.setEnabled(false);
                navigationMode = b;
                sendToggleUpdate();
            }
        });

        swtchExplore.setOnCheckedChangeListener(new CompoundButton.OnCheckedChangeListener() {
            @Override
            public void onCheckedChanged(CompoundButton compoundButton, boolean b) {
                swtchExplore.setEnabled(false);
                exploreMode = b;
                sendToggleUpdate();
            }
        });

        swtchLogImu.setOnCheckedChangeListener(new CompoundButton.OnCheckedChangeListener() {
            @Override
            public void onCheckedChanged(CompoundButton compoundButton, boolean b) {
                swtchLogImu.setEnabled(false);
                loggingImu = b;
                sendToggleUpdate();
            }
        });

        swtchLogWheelSpeed.setOnCheckedChangeListener(new CompoundButton.OnCheckedChangeListener() {
            @Override
            public void onCheckedChanged(CompoundButton compoundButton, boolean b) {
                swtchLogWheelSpeed.setEnabled(false);
                loggingWheelSpeed = b;
                sendToggleUpdate();
            }
        });

        swtchLogAnchorDistances.setOnCheckedChangeListener(new CompoundButton.OnCheckedChangeListener() {
            @Override
            public void onCheckedChanged(CompoundButton compoundButton, boolean b) {
                swtchLogAnchorDistances.setEnabled(false);
                loggingAnchorDistances = b;
                sendToggleUpdate();
            }
        });

        swtchLogCompassDirection.setOnCheckedChangeListener(new CompoundButton.OnCheckedChangeListener() {
            @Override
            public void onCheckedChanged(CompoundButton compoundButton, boolean b) {
                swtchLogCompassDirection.setEnabled(false);
                loggingCompassDirection = b;
                sendToggleUpdate();
            }
        });

        swtchLogPathfinderPath.setOnCheckedChangeListener(new CompoundButton.OnCheckedChangeListener() {
            @Override
            public void onCheckedChanged(CompoundButton compoundButton, boolean b) {
                swtchLogPathfinderPath.setEnabled(false);
                loggingPathfinderPath = b;
                sendToggleUpdate();
            }
        });

        swtchLogObstacleDistance.setOnCheckedChangeListener(new CompoundButton.OnCheckedChangeListener() {
            @Override
            public void onCheckedChanged(CompoundButton compoundButton, boolean b) {
                swtchLogObstacleDistance.setEnabled(false);
                loggingObstacleDistance = b;
                sendToggleUpdate();
            }
        });

        swtchLogPosition.setOnCheckedChangeListener(new CompoundButton.OnCheckedChangeListener() {
            @Override
            public void onCheckedChanged(CompoundButton compoundButton, boolean b) {
                swtchLogPosition.setEnabled(false);
                loggingPositioning = b;
                sendToggleUpdate();
            }
        });

        btnAccelerate.setOnTouchListener(new HoldDownListener(new View.OnClickListener() {
            @Override
            public void onClick(View view) {
                if (carSpeed >= -64 && carSpeed < 64)
                    carSpeed = 64;

                carSpeed = Math.min(127, carSpeed + 10);
            }
        }));

        btnBrake.setOnTouchListener(new HoldDownListener(new View.OnClickListener() {
            @Override
            public void onClick(View view) {
                if(carSpeed > -64 && carSpeed <= 64)
                    carSpeed = -64;

                carSpeed = Math.max(-127, carSpeed - 10);
            }
        }));

        final Timer pingTimer = new Timer();
        final TimerTask pingTimerTask = new TimerTask() {
            @Override
            public void run() {
                BLEHandler bleHandler = BLEHandler.getInstance();
                bleHandler.bleCharPingclient.setValue("" + System.currentTimeMillis());
                bleHandler.bleClient.sendCharacteristic(bleHandler.bleCharPingclient);
            }
        };
        pingTimer.schedule(pingTimerTask, 100, 3000);

        final Timer physicsUpdateTimer = new Timer();
        final TimerTask physicsUpdateTimerTask = new TimerTask() {
            @Override
            public void run() {
                carSpeed /= 1.05;
                carSpeed -= Math.signum(carSpeed);
                BLEHandler bleHandler = BLEHandler.getInstance();

                if (bleHandler.bleClient != null) {
                    if(carSpeed_prev != 0 || carSpeed != 0 || Math.abs(carSteer - carSteer_prev) > 2.0) {
                        bleHandler.bleCharUpdateMotors.setValue(new byte[] { (byte)carSpeed, (byte)carSteer });
                        bleHandler.bleClient.sendCharacteristic(bleHandler.bleCharUpdateMotors);
                        runOnUiThread(new Runnable() {
                            @Override
                            public void run() {
                                speedBar.setProgress(Math.max(0, Math.abs(carSpeed) - 60));
                            }
                        });
                        carSteer_prev = carSteer;
                    }
                    carSpeed_prev = carSpeed;
                }
            }
        };
        physicsUpdateTimer.schedule(physicsUpdateTimerTask, 100, 100);

        bleListener = new BluetoothLeServiceListener() {
            @Override
            public void onCharacteristicChanged(final BluetoothGattCharacteristic characteristic) {
                System.out.println("Got notified: " + new BigInteger(1, characteristic.getValue()).toString(16));
                BLEHandler bleHandler = BLEHandler.getInstance();

                if (characteristic.getUuid().compareTo(bleHandler.CHAR_UUID_SENSOR) == 0) {
                    byte[] vals = characteristic.getValue();
                    byte[] sensorTypes = new byte[8];
                    byte sensorCount = 8;

                    for (byte i = 0; i < 8; ++i) {
                        byte sensorType = (byte) ((vals[i / 4] >> ((i % 4) * 2)) & 0x03);

                        if (sensorType == 0)
                            sensorCount = i;
                        else
                            sensorTypes[i] = sensorType;
                    }

                    byte offset = 2;

                    for (byte i = 0; i < sensorCount; ++i) {
                        switch (sensorTypes[i]) {
                            case 1:
                                // Distance sensor
                                int direction = (vals[offset] & 0xFF) * 2;
                                int distance = (vals[offset + 1] & 0xFF) * 2;
                                lpsView.updateObstacleSensorDistance(direction, distance, false);
                                offset += 2;
                                break;
                            case 2:
                                // Position
                                final int x = (vals[offset] & 0xFF) | ((vals[offset + 1] & 0x07) << 8) | (((vals[offset + 1] & 0x08) != 0) ? 0xFFFFF800 : 0);
                                final int y = ((vals[offset + 1] & 0xF0) >> 4) | (vals[offset + 2] << 4);
                                runOnUiThread(new Runnable() {
                                    @Override
                                    public void run() {
                                        lpsPosView.setText("X: " + x + "\nY: " + y);
                                    }
                                });
                                lpsView.updatePosition(x, y, false);
                                offset += 3;
                                break;
                            case 3:
                                // Compass
                                int compass_direction = (vals[offset] & 0xFF) | (vals[offset + 1] << 8);
                                lpsView.updateDirection(compass_direction, false);
                                offset += 2;
                                break;
                        }
                    }

                    if (sensorCount != 0) {
                        runOnUiThread(new Runnable() {
                            @Override
                            public void run() {
                                lpsView.invalidate();
                            }
                        });
                    }
                } else if (characteristic.getUuid().compareTo(bleHandler.CHAR_UUID_ADD_OBSTACLE) == 0) {
                    byte[] vals = characteristic.getValue();
                    short x = (short)((vals[8] & 0xFF) | (vals[9] << 8));
                    short y = (short)((vals[10] & 0xFF) | (vals[11] << 8));
                    int width = (vals[12] & 0xFF) | ((vals[13] & 0xFF) << 8);
                    int height = (vals[14] & 0xFF) | ((vals[15] & 0xFF) << 8);
                    int id = (vals[16] & 0xFF) | ((vals[17] & 0xFF) << 8);

                    Obstacle obstacle = lpsView.getObstacle(id);

                    if (obstacle == null) {
                        if (pendingObstacleToAdd != null) {
                            lpsView.removeObstacle(pendingObstacleToAdd);
                            pendingObstacleToAdd = null;
                            obstacle = new Obstacle(x, y, width, height, id);
                            lpsView.addObstacle(obstacle);

                            runOnUiThread(new Runnable() {
                                @Override
                                public void run() {
                                    lpsView.invalidate();
                                    btnAddObstacle.setEnabled(true);
                                }
                            });
                        } else {
                            obstacle = new Obstacle(x, y, width, height, id);
                            lpsView.addObstacle(obstacle);
                            runOnUiThread(new Runnable() {
                                @Override
                                public void run() {
                                    lpsView.invalidate();
                                }
                            });
                        }
                    } else {
                        obstacle.setX(x);
                        obstacle.setY(y);
                        obstacle.setWidth(width);
                        obstacle.setHeight(height);
                        runOnUiThread(new Runnable() {
                            @Override
                            public void run() {
                                lpsView.invalidate();
                            }
                        });
                    }
                } else if (characteristic.getUuid().compareTo(bleHandler.CHAR_UUID_REMOVE_OBSTACLE) == 0) {
                    byte[] vals = characteristic.getValue();

                    if (vals.length >= 12) {
                        short x = (short)(vals[8] | ((int)vals[9] << 8));
                        short y = (short)(vals[10] | ((int)vals[11] << 8));
                        lpsView.removeObstacle(x, y);
                        runOnUiThread(new Runnable() {
                            @Override
                            public void run() {
                                lpsView.invalidate();
                            }
                        });
                    } else if (vals.length >= 10) {
                        short id = (short)(vals[8] | ((int)vals[9] << 8));
                        lpsView.removeObstacle(id);
                        runOnUiThread(new Runnable() {
                            @Override
                            public void run() {
                                lpsView.invalidate();
                            }
                        });

                        if (pendingObstacleToRemove != null && pendingObstacleToRemove.getId() == id) {
                            pendingObstacleToRemove = null;
                            runOnUiThread(new Runnable() {
                                @Override
                                public void run() {
                                    btnRemoveObstacle.setEnabled(true);
                                }
                            });
                        }
                    }
                } else if (characteristic.getUuid().compareTo(bleHandler.CHAR_UUID_ANCHOR_DISTANCES) == 0) {
                    byte[] vals = characteristic.getValue();
                    short anchor0_distance = (short)(vals[0] | ((int)vals[1] << 8));
                    short anchor1_distance = (short)(vals[2] | ((int)vals[3] << 8));
                    short anchor2_distance = (short)(vals[4] | ((int)vals[5] << 8));
                    System.out.println("anchor0_distance: " + anchor0_distance + " anchor1_distance: " + anchor1_distance + " anchor2_distance: " + anchor2_distance);
                    // TODO: Show anchor distances in GUI
                } else if (characteristic.getUuid().compareTo(bleHandler.CHAR_UUID_IMU) == 0) {
                    byte[] vals = characteristic.getValue();
                    byte type = vals[0];
                    short xAxis = (short)(vals[1] | ((int)vals[2] << 8));
                    short yAxis = (short)(vals[3] | ((int)vals[4] << 8));
                    short zAxis = (short)(vals[5] | ((int)vals[6] << 8));
                    System.out.println("IMU type: " + type + " xAxis: " + xAxis + " yAxis: " + yAxis + " zAxis: " + zAxis);
                    // TODO: Show IMU values in GUI
                } else if (characteristic.getUuid().compareTo(bleHandler.CHAR_UUID_WHEEL_SPEED) == 0) {
                    byte[] vals = characteristic.getValue();
                    final byte speed_left = vals[0];
                    final byte speed_right = vals[1];
                    System.out.println("Wheel speed left: " + speed_left + " right: " + speed_right);
                    runOnUiThread(new Runnable() {
                        @Override
                        public void run() {
                            speedLeftBar.setProgress((Math.max(speed_left, 0) * 100) / 127);
                            speedLeftBar.setSecondaryProgress((Math.max(-speed_left, 0) * 100) / 127);
                            speedRightBar.setProgress((Math.max(speed_right, 0) * 100) / 127);
                            speedRightBar.setSecondaryProgress((Math.max(-speed_right, 0) * 100) / 127);
                            DecimalFormat decimalFormat = new DecimalFormat("#.##");
                            kmphView.setText(decimalFormat.format(((speed_left + speed_right) / 2.0) * 0.036) + " km/h");
                            mpsView.setText(decimalFormat.format(((speed_left + speed_right) / 2.0) * 0.01) + " m/s");
                        }
                    });
                } else if (characteristic.getUuid().compareTo(bleHandler.CHAR_UUID_ERROR) == 0) {
                    System.out.println("Received error");
                } else if (characteristic.getUuid().compareTo(bleHandler.CHAR_UUID_TOGGLE) == 0) {
                    byte[] vals = characteristic.getValue();
                    exploreMode = ((vals[0] >> 3) & 1) == 1;
                    navigationMode = ((vals[0] >> 4) & 1) == 1;
                    collisionAvoidance = ((vals[0] >> 5) & 1) == 1;
                    positioning = ((vals[0] >> 6) & 1) == 1;
                    invite = ((vals[0] >> 7) & 1) == 1;

                    loggingImu = ((vals[1] >> 1) & 1) == 1;
                    loggingWheelSpeed = ((vals[1] >> 2) & 1) == 1;
                    loggingAnchorDistances = ((vals[1] >> 3) & 1) == 1;
                    loggingCompassDirection = ((vals[1] >> 4) & 1) == 1;
                    loggingPathfinderPath = ((vals[1] >> 5) & 1) == 1;
                    loggingObstacleDistance = ((vals[1] >> 6) & 1) == 1;

                    runOnUiThread(new Runnable() {
                        @Override
                        public void run() {
                            swtchExplore.setChecked(exploreMode);
                            swtchNavigation.setChecked(navigationMode);
                            swtchCollisionAvoid.setChecked(collisionAvoidance);
                            swtchPosition.setChecked(positioning);
                            swtchInvite.setChecked(invite);
                            swtchLogImu.setChecked(loggingImu);
                            swtchLogWheelSpeed.setChecked(loggingWheelSpeed);
                            swtchLogAnchorDistances.setChecked(loggingAnchorDistances);
                            swtchLogCompassDirection.setChecked(loggingCompassDirection);
                            swtchLogPathfinderPath.setChecked(loggingPathfinderPath);
                            swtchLogObstacleDistance.setChecked(loggingObstacleDistance);
                        }
                    });
                } else if (characteristic.getUuid().compareTo(bleHandler.CHAR_UUID_PATHFINDING_PATH) == 0) {
                    runOnUiThread(new Runnable() {
                        @Override
                        public void run() {
                            byte[] vals = characteristic.getValue();
                            lpsView.updatePath(vals);
                        }
                    });
                }
            }

            @Override
            public void onCharacteristicWrite(BluetoothGattCharacteristic characteristic) {
                if (characteristic.getUuid().compareTo(BLEHandler.getInstance().CHAR_UUID_PINGCLIENT) == 0) {
                    final long t = System.currentTimeMillis() - Long.parseLong(new String(characteristic.getValue()));
                    runOnUiThread(new Runnable() {
                        @Override
                        public void run() {
                            pingView.setText("Ping: " + t + "ms");
                        }
                    });
                }
            }

            @Override
            public void onDisconnect() {
                runOnUiThread(new Runnable() {
                    @Override
                    public void run() {
                        Toast.makeText(ControlActivity.this, "Disconnected", Toast.LENGTH_SHORT).show();
                    }
                });
                pingTimer.cancel();
                pingTimerTask.cancel();
                physicsUpdateTimer.cancel();
                physicsUpdateTimerTask.cancel();

                BLEHandler bleHandler = BLEHandler.getInstance();
                bleHandler.bleClient.removeListener(bleListener);
                bleHandler.bleClient = null;
                bleHandler.bleService = null;
                bleHandler.bleCharPingclient = null;
                bleHandler.bleCharUpdateMotors = null;
                bleHandler.bleCharToggle = null;
                bleHandler.bleCharSensor = null;
                bleHandler.bleCharPathfindingTarget = null;
                bleHandler.bleCharCalibrate = null;
                bleHandler.bleCharAddObstacle = null;
                bleHandler.bleCharRemoveObstacle = null;
                bleHandler.bleCharPathfindingPath = null;
                bleHandler.bleCharAnchorLocations = null;
                bleHandler.bleCharAnchorDistances = null;
                bleHandler.bleCharImu = null;
                bleHandler.bleCharWheelSpeed = null;
                bleHandler.bleCharError = null;
                sensorManager.unregisterListener(_this);
                finish();
            }
        };
        BLEHandler.getInstance().bleClient.addListener(bleListener);

        toggleCharSender.addListener(new BLECharacteristicSender.BLECharacteristicSenderListener() {
            @Override
            public void onValueArrive(byte[] value) {
                if (value.length >= 2) {
                    final boolean tmpExploreMode = ((value[0] >> 3) & 1) == 1;
                    final boolean tmpNavigationMode = ((value[0] >> 4) & 1) == 1;
                    final boolean tmpCollisionAvoidance = ((value[0] >> 5) & 1) == 1;
                    final boolean tmpPositioning = ((value[0] >> 6) & 1) == 1;
                    final boolean tmpInvite = ((value[0] >> 7) & 1) == 1;

                    final boolean tmpLoggingImu = ((value[1] >> 1) & 1) == 1;
                    final boolean tmpLoggingWheelSpeed = ((value[1] >> 2) & 1) == 1;
                    final boolean tmpLoggingAnchorDistances = ((value[1] >> 3) & 1) == 1;
                    final boolean tmpLoggingCompassDirection = ((value[1] >> 4) & 1) == 1;
                    final boolean tmpLoggingPathfinderPath = ((value[1] >> 5) & 1) == 1;
                    final boolean tmpLoggingObstacleDistance = ((value[1] >> 6) & 1) == 1;
                    final boolean tmpLoggingPositioning = ((value[1] >> 7) & 1) == 1;

                    runOnUiThread(new Runnable() {
                        @Override
                        public void run() {
                            if (tmpExploreMode == exploreMode)
                                swtchExplore.setEnabled(true);

                            if (tmpNavigationMode == navigationMode)
                                swtchNavigation.setEnabled(true);

                            if (tmpCollisionAvoidance == collisionAvoidance)
                                swtchCollisionAvoid.setEnabled(true);

                            if (tmpPositioning == positioning)
                                swtchPosition.setEnabled(true);

                            if (tmpInvite == invite)
                                swtchInvite.setEnabled(true);

                            if (tmpLoggingImu == loggingImu)
                                swtchLogImu.setEnabled(true);

                            if (tmpLoggingWheelSpeed == loggingWheelSpeed)
                                swtchLogWheelSpeed.setEnabled(true);

                            if (tmpLoggingAnchorDistances == loggingAnchorDistances)
                                swtchLogAnchorDistances.setEnabled(true);

                            if (tmpLoggingCompassDirection == loggingCompassDirection)
                                swtchLogCompassDirection.setEnabled(true);

                            if (tmpLoggingPathfinderPath == loggingPathfinderPath)
                                swtchLogPathfinderPath.setEnabled(true);

                            if (tmpLoggingObstacleDistance == loggingObstacleDistance)
                                swtchLogObstacleDistance.setEnabled(true);

                            if (tmpLoggingPositioning == loggingPositioning)
                                swtchLogPosition.setEnabled(true);
                        }
                    });
                }
            }
        });

        calibrateCharSender.addListener(new BLECharacteristicSender.BLECharacteristicSenderListener() {
            @Override
            public void onValueArrive(byte[] value) {
                if (value.length >= 1) {
                    switch (value[0]) {
                        case 0:
                            runOnUiThread(new Runnable() {
                                @Override
                                public void run() {
                                    btnCalibrateSteering.setEnabled(true);
                                }
                            });
                            break;
                        case 1:
                            // TODO: Automatic compass calibration
                            break;
                        case 2:
                        case 3:
                            runOnUiThread(new Runnable() {
                                @Override
                                public void run() {
                                    swtchCalibrateMagnetometer.setEnabled(true);
                                }
                            });
                            break;
                        case 4:
                            runOnUiThread(new Runnable() {
                                @Override
                                public void run() {
                                    btnCalibrateCompassDirection.setEnabled(true);
                                }
                            });
                            break;
                    }
                }
            }
        });
    }

    @Override
    public void onBackPressed(){
        moveTaskToBack(true);
    }

    @Override
    public void onLPSConfigureDialogConfirm(short anchor0x, short anchor0y, short anchor1x, short anchor1y, short anchor2x, short anchor2y) {
        lpsView.anchor0x = anchor0x;
        lpsView.anchor0y = anchor0y;
        lpsView.anchor1x = anchor1x;
        lpsView.anchor1y = anchor1y;
        lpsView.anchor2x = anchor2x;
        lpsView.anchor2y = anchor2y;

        byte[] vals = new byte[20];
        writeTimestamp(vals, 0);
        vals[8] = (byte)lpsView.anchor0x;
        vals[9] = (byte)(lpsView.anchor0x >> 8);
        vals[10] = (byte)lpsView.anchor0y;
        vals[11] = (byte)(lpsView.anchor0y >> 8);
        vals[12] = (byte)lpsView.anchor1x;
        vals[13] = (byte)(lpsView.anchor1x >> 8);
        vals[14] = (byte)lpsView.anchor1y;
        vals[15] = (byte)(lpsView.anchor1y >> 8);
        vals[16] = (byte)lpsView.anchor2x;
        vals[17] = (byte)(lpsView.anchor2x >> 8);
        vals[18] = (byte)lpsView.anchor2y;
        vals[19] = (byte)(lpsView.anchor2y >> 8);
        anchorLocationsCharSender.sendValue(vals);

        positioning = true;
        sendToggleUpdate();

        saveLPSAnchorsPreference();

        enableImmersiveMode();
    }

    @Override
    public void onLPSConfigureDialogCancel() {
        enableImmersiveMode();
        swtchPosition.setEnabled(true);
        swtchPosition.setChecked(false);
    }

    @Override
    public void onObstacleConfigureDialogConfirm(short x, short y, short width, short height) {
        byte[] vals = new byte[17];
        vals[8] = (byte)x;
        vals[9] = (byte)(x >> 8);
        vals[10] = (byte)y;
        vals[11] = (byte)(y >> 8);
        vals[12] = (byte)width;
        vals[13] = (byte)(width >> 8);
        vals[14] = (byte)height;
        vals[15] = (byte)(height >> 8);
        writeTimestamp(vals, 0);
        addObstacleCharSender.sendValue(vals);

        pendingObstacleToAdd = new Obstacle(x, y, width, height);
        lpsView.addObstacle(pendingObstacleToAdd);
        lpsView.invalidate();

        enableImmersiveMode();
    }

    @Override
    public void onObstacleConfigureDialogCancel() {
        enableImmersiveMode();
        btnAddObstacle.setEnabled(true);
    }

    @Override
    public void onLPSSetTargetPosition(short x, short y) {
        byte[] vals = new byte[13];
        vals[0] = (byte)x;
        vals[1] = (byte)(x >> 8);
        vals[2] = (byte)y;
        vals[3] = (byte)(y >> 8);
        writeTimestamp(vals, 4);
        pathfindingTargetCharSender.sendValue(vals);
    }

    private void writeTimestamp(byte[] vals, int offset) {
        long currentTime = System.currentTimeMillis();

        for (byte i = 0; i < 8; ++i)
            vals[i + offset] = (byte)(currentTime >> (i * 8));
    }

    private void loadLPSAnchorsPreference() {
        SharedPreferences lpsAnchorsPref = getApplicationContext().getSharedPreferences("lps_anchors", 0);
        lpsView.anchor0x = (short) lpsAnchorsPref.getInt("0x", 0);
        lpsView.anchor0y = (short) lpsAnchorsPref.getInt("0y", 0);
        lpsView.anchor1x = (short) lpsAnchorsPref.getInt("1x", 100);
        lpsView.anchor1y = (short) lpsAnchorsPref.getInt("1y", 0);
        lpsView.anchor2x = (short) lpsAnchorsPref.getInt("2x", 0);
        lpsView.anchor2y = (short) lpsAnchorsPref.getInt("2y", 100);
    }

    private void saveLPSAnchorsPreference() {
        SharedPreferences lpsAnchorsPref = getApplicationContext().getSharedPreferences("lps_anchors", 0);
        SharedPreferences.Editor editor = lpsAnchorsPref.edit();
        editor.putInt("0x", lpsView.anchor0x);
        editor.putInt("0y", lpsView.anchor0y);
        editor.putInt("1x", lpsView.anchor1x);
        editor.putInt("1y", lpsView.anchor1y);
        editor.putInt("2x", lpsView.anchor2x);
        editor.putInt("2y", lpsView.anchor2y);
        editor.apply();
    }

    private void sendToggleUpdate() {
        byte[] vals = new byte[10];
        vals[0] = (byte) (((exploreMode ? 1 : 0) << 3) | ((navigationMode ? 1 : 0) << 4) |
                         ((collisionAvoidance ? 1 : 0) << 5) | ((positioning ? 1 : 0) << 6) |
                         ((invite ? 1 : 0) << 7));
        vals[1] = (byte) (((loggingImu ? 1 : 0) << 1) | ((loggingWheelSpeed ? 1 : 0) << 2) |
                         ((loggingAnchorDistances ? 1 : 0) << 3) | ((loggingCompassDirection ? 1 : 0) << 4) |
                         ((loggingPathfinderPath ? 1 : 0) << 5) | ((loggingObstacleDistance ? 1 : 0) << 6) |
                         ((loggingPositioning ? 1 : 0) << 7));
        writeTimestamp(vals, 2);
        toggleCharSender.sendValue(vals);
    }
}
