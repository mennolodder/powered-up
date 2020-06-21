using System;
using System.Threading.Tasks;
using SharpBrick.PoweredUp.Protocol;
using SharpBrick.PoweredUp.Protocol.Messages;

namespace SharpBrick.PoweredUp
{
    public abstract class BasicMotor : Device
    {
        public byte ModeIndexPower { get; protected set; } = 0;

        public sbyte Power { get; private set; } = 0;
        public sbyte PowerPct { get; private set; } = 0;
        public IObservable<Value<sbyte>> PowerObservable { get; }

        public BasicMotor()
        { }

        public BasicMotor(IPoweredUpProtocol protocol, byte hubId, byte portId)
            : base(protocol, hubId, portId)
        {
            PowerObservable = CreateSinglePortModeValueObservable<sbyte>(ModeIndexPower);

            ObserveOnLocalProperty(PowerObservable, v => Power = v.SI, v => PowerPct = v.Pct);
        }

        /// <summary>
        /// Starts the motor with full speed at the given power level.
        /// </summary>
        /// <param name="power">
        /// - Power levels in percentage: 1 - 100 (CW), -1 - -100 (CCW)
        /// - Stop Motor (floating): 0 
        /// - Stop Motor (breaking): 127
        /// </param>
        /// <returns>An awaitable Task.</returns>
        public async Task StartPowerAsync(sbyte power)
        {
            AssertValidPower(power, nameof(power));
            AssertIsConnected();

            await _protocol.SendMessageAsync(new PortOutputCommandStartPowerMessage()
            {
                HubId = _hubId,
                PortId = _portId,
                StartupInformation = PortOutputCommandStartupInformation.ExecuteImmediately,
                CompletionInformation = PortOutputCommandCompletionInformation.CommandFeedback,
                Power = power,
            });
        }

        /// <summary>
        /// Stops the motor (brake; no movement afterwards)
        /// </summary>
        /// <returns></returns>
        public Task StopByBrakeAsync()
            => StartPowerAsync((sbyte)SpecialSpeed.Brake);

        /// <summary>
        /// Stops the motor (float; freely floating by inertia)
        /// </summary>
        /// <returns></returns>
        public Task StopByFloatAsync()
            => StartPowerAsync((sbyte)SpecialSpeed.Float);

        /// <summary>
        /// Start the pair on motors with full speed on given power values.
        /// </summary>
        /// <param name="powerOnMotor1">
        /// - Power levels in percentage: 1 - 100 (CW), -1 - -100 (CCW)
        /// - Stop Motor (floating): 0 
        /// - Stop Motor (breaking): 127
        /// </param>
        /// <param name="powerOnMotor2">
        /// - Power levels in percentage: 1 - 100 (CW), -1 - -100 (CCW)
        /// - Stop Motor (floating): 0 
        /// - Stop Motor (breaking): 127
        /// </param>
        /// <returns></returns>
        public async Task StartPowerAsync(sbyte powerOnMotor1, sbyte powerOnMotor2)
        {
            AssertValidPower(powerOnMotor1, nameof(powerOnMotor1));
            AssertValidPower(powerOnMotor2, nameof(powerOnMotor2));
            AssertIsConnected();
            AssertIsVirtualPort();

            await _protocol.SendMessageAsync(new PortOutputCommandStartPower2Message()
            {
                HubId = _hubId,
                PortId = _portId,
                StartupInformation = PortOutputCommandStartupInformation.ExecuteImmediately,
                CompletionInformation = PortOutputCommandCompletionInformation.CommandFeedback,
                Power1 = powerOnMotor1,
                Power2 = powerOnMotor2,
            });
        }

        protected void AssertValidPower(sbyte power, string argumentName)
        {
            if (
                  power < -100 ||
                  (power > 100 && power != 127)
              )
            {
                throw new ArgumentOutOfRangeException(argumentName);
            }
        }
    }
}