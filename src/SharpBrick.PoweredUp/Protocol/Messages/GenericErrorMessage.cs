﻿namespace SharpBrick.PoweredUp.Protocol.Messages
{
    // spec chapter: 3.9.1
    public class GenericErrorMessage : CommonMessageHeader
    {
        public byte CommandType { get; set; }
        public ErrorCodes ErrorCode { get; set; }
    }
}