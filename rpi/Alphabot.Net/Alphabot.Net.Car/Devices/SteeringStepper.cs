using System;
using System.Threading;
using System.Threading.Tasks;
using Alphabot.Net.Car.Contracts;
using Alphabot.Net.Car.Settings;
using Alphabot.Net.Car.Steering;
using Alphabot.Net.Shared.Contracts;
using Alphabot.Net.Shared.Logger;
using Iot.Device.Uln2003;

namespace Alphabot.Net.Car.Devices
{
    public class SteeringStepper
    {
        private readonly Prefs _prefs = Prefs.GetInstance();
        private readonly IServiceLogger _logger = ServiceLogger.GetInstance().Current;
        private readonly Uln2003 _uln2003;

        public int CurrentStepperPosition { get; set; } = 0;

        public SteeringStepper(int pin1, int pin2, int pin3, int pin4)
        {
            _uln2003 = new Uln2003(pin1, pin2, pin3, pin4);
            _uln2003.RPM = 6;
        }

        public short Rpm
        {
            get => _uln2003.RPM;
            set => _uln2003.RPM = value;
        }

        public async Task TurnAsync(int totalSteps, TurnDirection direction, CancellationToken token)
        {
            int currentSteps = 0;
            int steeringBlocks = 0;
            int finalSteps = 0;


            var minimumSteeringSteps = _prefs.DeviceSettings.MinimumSteeringSteps;

            _uln2003.Mode = StepperMode.FullStepSinglePhase;

            _logger.Log(LogLevel.Debug, "Stepper.TurnAsync", $"steps: {totalSteps}");
            
            try // for cancelling task
            {
                steeringBlocks = totalSteps / minimumSteeringSteps;
                while (currentSteps < totalSteps)
                {
                    // another steering block
                    if ((totalSteps - currentSteps) > minimumSteeringSteps)
                    {
                        if (direction == TurnDirection.Right)
                        {
                            _uln2003.Step(minimumSteeringSteps);
                            //currentSteps += minimumSteeringSteps;
                            SetCurrentStepperPosition(direction, minimumSteeringSteps);
                        }
                        else
                        {
                            _uln2003.Step(minimumSteeringSteps * -1);
                            //currentSteps += minimumSteeringSteps;
                            SetCurrentStepperPosition(direction, minimumSteeringSteps);
                        }

                        currentSteps += minimumSteeringSteps;
                    }
                    // rest of steering action
                    else
                    {
                        finalSteps = totalSteps % minimumSteeringSteps;
                        _logger.Log(LogLevel.Debug, "SteeringStepper.TurnAsync ",
                            $"calculated final steps: {finalSteps}");
                        if (direction == TurnDirection.Right)
                        {
                            _uln2003.Step(finalSteps);
                            currentSteps += finalSteps;
                            SetCurrentStepperPosition(direction, finalSteps);
                            _logger.Log(LogLevel.Debug, "SteeringStepper.TurnAsync",
                                $" final steps done - current position {CurrentStepperPosition}");
                        }
                        else
                        {
                            _uln2003.Step(finalSteps * -1);
                            currentSteps -= finalSteps;
                            SetCurrentStepperPosition(direction, finalSteps);
                            _logger.Log(LogLevel.Debug, "SteeringStepper.TurnAsync",
                                $" final steps done - current position {CurrentStepperPosition}");
                        }

                        currentSteps += (totalSteps - currentSteps);
                    }

                    await Task.Delay(1);

                    // cancel task if requested
                    if (token.IsCancellationRequested)
                    {
                        token.ThrowIfCancellationRequested();
                        _logger.Log(LogLevel.Debug,
                            "TurnAsync has been canceled at CurrentStepperPosition ++ :  " + CurrentStepperPosition);
                    }

                    if (currentSteps == totalSteps) // exit steppersequence
                        break;
                    _logger.Log(LogLevel.Debug,
                        "TurnAsync: Steps done " + currentSteps + " direction " + direction.ToString());
                    _logger.Log(LogLevel.Debug, "TurnAsync: CurrentStepperPosition:  " + CurrentStepperPosition);
                }
            }
            catch (OperationCanceledException opc)
            {
                _logger.Log(LogLevel.Debug, "TurnAsync has been canceled with Exception :  " + opc.Message);
            }
            finally
            {
                // set stepper pins low
                _uln2003.Stop();
                _logger.Log(LogLevel.Information, "SteeringStepper", "uln 2003 stopped");
            }
        }


        /// <summary>
        /// Stop the motor.
        /// </summary>
        public void Stop()
        {
            _uln2003.Stop();
        }

        private void SetCurrentStepperPosition(TurnDirection direction, int steps)
        {
            //ToDo: make DeviceSettings
            int maxStepps = 336; // 336
            if (direction == TurnDirection.Right)
            {
                CurrentStepperPosition += steps;
                if (CurrentStepperPosition > maxStepps)
                {
                    CurrentStepperPosition = maxStepps;
                }
            }
            else
            {
                CurrentStepperPosition -= steps;
                if (CurrentStepperPosition < (-1) * maxStepps)
                {
                    CurrentStepperPosition = (-1) * maxStepps;
                }
            }
        }
    }
}
