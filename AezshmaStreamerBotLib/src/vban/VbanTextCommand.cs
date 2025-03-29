using System;
using System.Collections.Generic;
using System.Text;

namespace AezshmaStreamerBotLib.vban {
    /// <summary>
    /// A <c>VbanPacket</c> implementation for quickly creating VBAN-TEXT packets.
    /// </summary>
    public class VbanTextCommand : VbanPacket {
        public VbanTextCommand(string command) : base(
            VbanPacketHeader.builder()
                .WithSampleRateOrSubProtocol(Convert.ToByte(VbanSubProtocol.Text))
                .WithStreamName("Command1")
                .WithFrameCount(1)
                .WithDataFormatOrCodec(Convert.ToByte(VbanCommandFormat.Utf8))
                .Build(),
            new List<byte>(Encoding.UTF8.GetBytes(command))) {
        }
    }
}