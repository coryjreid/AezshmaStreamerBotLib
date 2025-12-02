using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AezshmaStreamerBotLib.vban {
    /// <summary>
    /// A <c>VbanPacket</c> implementation for quickly creating VBAN-TEXT packets.
    /// </summary>
    public class VbanTextCommand : VbanPacket {
        public VbanTextCommand(string command) : base(
            VbanPacketHeader.CreateBuilder()
                .WithSampleRateOrSubProtocol(Convert.ToByte(VbanSubProtocol.Text))
                .WithStreamName("Command1")
                .WithFrameCount(1)
                .WithDataFormatOrCodec(Convert.ToByte(VbanCommandFormat.Utf8))
                .Build(),
            Encoding.UTF8.GetBytes(command)) {
        }

        // Constructor for parsing received packets
        public VbanTextCommand(VbanPacketHeader header, byte[] data) : base(header, data) {
        }

        // Helper to get the command text from a received packet
        public string GetCommandText() {
            return Encoding.UTF8.GetString(ToBytes().Skip(VbanPacketHeader.SizeBytes).ToArray());
        }
    }
}