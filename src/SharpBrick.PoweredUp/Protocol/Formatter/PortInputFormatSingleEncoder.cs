using System;
using SharpBrick.PoweredUp.Protocol.Messages;

namespace SharpBrick.PoweredUp.Protocol.Formatter
{
    // spec chapter: 3.23
    public class PortInputFormatSingleEncoder : IMessageContentEncoder
    {
        public ushort CalculateContentLength(CommonMessageHeader message)
            => throw new NotImplementedException();

        public CommonMessageHeader Decode(in Span<byte> data)
            => new PortInputFormatSingleMessage()
            {
                PortId = data[0],
                Mode = data[1],
                DeltaInterval = BitConverter.ToUInt32(data.Slice(2, 4)),
                NotificationEnabled = (data[6] == 0x01),
            };

        public void Encode(CommonMessageHeader message, in Span<byte> data)
            => throw new NotImplementedException();
    }
}