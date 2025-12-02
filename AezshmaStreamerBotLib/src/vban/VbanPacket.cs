using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AezshmaStreamerBotLib.vban {
    public abstract class VbanPacket {
        private const int MaxDataSizeBytes = 1436;
        private readonly VbanPacketHeader _header;
        private readonly byte[] _data;

        protected VbanPacket(VbanPacketHeader header, byte[] data) {
            if (data == null) {
                throw new ArgumentException("data cannot be null");
            }

            if (data.Length > MaxDataSizeBytes) {
                throw new ArgumentException($"data length must be <= {MaxDataSizeBytes}");
            }

            _header = header ?? throw new ArgumentException("header cannot be null");
            _data = new byte[data.Length];
            Array.Copy(data, _data, data.Length);
        }

        public byte[] ToBytes() {
            byte[] header = _header.ToBytes();
            byte[] result = new byte[header.Length + _data.Length];
            Array.Copy(header, 0, result, 0, header.Length);
            Array.Copy(_data, 0, result, header.Length, _data.Length);
            return result;
        }
        
        public static VbanPacket FromBytes(byte[] data) {
            VbanPacketHeader header = VbanPacketHeader.FromBytes(data);
            byte[] payload = data.Skip(VbanPacketHeader.SizeBytes).ToArray();
            return new VbanTextCommand(header, payload);
        }
    }
}