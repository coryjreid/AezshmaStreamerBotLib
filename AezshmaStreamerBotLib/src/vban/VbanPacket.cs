using System;
using System.Collections.Generic;

namespace AezshmaStreamerBotLib.vban {
    public abstract class VbanPacket {
        private const int MaxDataSizeBytes = 1436;
        private readonly VbanPacketHeader _header;
        private readonly List<byte> _data = new List<byte>();

        protected VbanPacket(VbanPacketHeader header, List<byte> data) {
            if (data == null) {
                throw new ArgumentException("data cannot be null");
            }

            if (data.Count > MaxDataSizeBytes) {
                throw new ArgumentException($"data length must be <= {MaxDataSizeBytes}");
            }

            _header = header ?? throw new ArgumentException("header cannot be null");
            _data.AddRange(data);
        }

        public byte[] ToBytes() {
            List<byte> packet = new List<byte>();
            packet.AddRange(_header.ToBytes());
            packet.AddRange(_data);
            return packet.ToArray();
        }
    }
}