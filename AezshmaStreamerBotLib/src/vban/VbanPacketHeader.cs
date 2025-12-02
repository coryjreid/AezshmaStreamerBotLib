using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AezshmaStreamerBotLib.vban {
    public class VbanPacketHeader {
        public const int SizeBytes = 28;
        private const int StreamNameSizeBytes = 16;
        private readonly byte[] _prefix = Encoding.UTF8.GetBytes("VBAN");
        private readonly byte _sampleRateOrSubProtocol;
        private readonly byte _samplesPerFrame;
        private readonly byte _channelCount;
        private readonly byte _dataFormatOrCodec;
        private readonly byte[] _streamName;
        private readonly int _frameCount;

        private VbanPacketHeader(
            byte sampleRateOrSubProtocol,
            byte samplesPerFrame,
            byte channelCount,
            byte dataFormatOrCodec,
            byte[] streamName,
            int frameCount) {
            _sampleRateOrSubProtocol = sampleRateOrSubProtocol;
            _samplesPerFrame = samplesPerFrame;
            _channelCount = channelCount;
            _dataFormatOrCodec = dataFormatOrCodec;
            _streamName = streamName;
            _frameCount = frameCount;
        }

        public byte[] ToBytes() {
            List<byte> data = new List<byte>(SizeBytes);
            data.AddRange(_prefix);
            data.Add(_sampleRateOrSubProtocol);
            data.Add(_samplesPerFrame);
            data.Add(_channelCount);
            data.Add(_dataFormatOrCodec);
            data.AddRange(_streamName.ToList());
            for (int i = 0; i < StreamNameSizeBytes - _streamName.Length; i++) {
                data.Add(0);
            }

            data.AddRange(BitConverter.GetBytes(_frameCount));

            return data.ToArray();
        }

        public static Builder CreateBuilder() {
            return new Builder();
        }

        public static VbanPacketHeader FromBytes(byte[] data) {
            if (data == null || data.Length < SizeBytes) {
                throw new ArgumentException($"Data must be at least {SizeBytes} bytes");
            }

            // Verify VBAN prefix
            byte[] prefix = new byte[4];
            Array.Copy(data, 0, prefix, 0, 4);
            if (Encoding.UTF8.GetString(prefix) != "VBAN") {
                throw new ArgumentException("Invalid VBAN packet - missing VBAN prefix");
            }

            // Parse header fields
            byte sampleRateOrSubProtocol = data[4];
            byte samplesPerFrame = data[5];
            byte channelCount = data[6];
            byte dataFormatOrCodec = data[7];

            // Extract stream name (16 bytes starting at offset 8)
            byte[] streamName = new byte[StreamNameSizeBytes];
            Array.Copy(data, 8, streamName, 0, StreamNameSizeBytes);

            // Trim null bytes from stream name
            int nameLength = Array.IndexOf(streamName, (byte)0);
            if (nameLength >= 0) {
                Array.Resize(ref streamName, nameLength);
            }

            // Parse frame count (4 bytes at offset 24)
            int frameCount = BitConverter.ToInt32(data, 24);

            return new VbanPacketHeader(
                sampleRateOrSubProtocol,
                samplesPerFrame,
                channelCount,
                dataFormatOrCodec,
                streamName,
                frameCount);
        }

        public class Builder {
            private byte _sampleRateOrSubProtocol;
            private byte _samplesPerFrame;
            private byte _channelCount;
            private byte _dataFormatOrCodec;
            private byte[] _streamName = new byte[StreamNameSizeBytes];
            private int _frameCount;

            public Builder WithSampleRateOrSubProtocol(byte sampleRateOrSubProtocol) {
                _sampleRateOrSubProtocol = sampleRateOrSubProtocol;
                return this;
            }

            public Builder WithSamplesPerFrame(byte samplesPerFrame) {
                _samplesPerFrame = samplesPerFrame;
                return this;
            }

            public Builder WithChannelCount(byte channelCount) {
                _channelCount = channelCount;
                return this;
            }

            public Builder WithDataFormatOrCodec(byte dataFormatOrCodec) {
                _dataFormatOrCodec = dataFormatOrCodec;
                return this;
            }

            public Builder WithStreamName(string streamName) {
                if (streamName.Length > StreamNameSizeBytes) {
                    throw new ArgumentException(
                        $"Length of streamName cannot exceed {StreamNameSizeBytes} (current length: {streamName.Length})");
                }

                _streamName = Encoding.UTF8.GetBytes(streamName);
                return this;
            }

            public Builder WithFrameCount(int frameCount) {
                _frameCount = frameCount;
                return this;
            }

            public VbanPacketHeader Build() {
                return new VbanPacketHeader(
                    _sampleRateOrSubProtocol,
                    _samplesPerFrame,
                    _channelCount,
                    _dataFormatOrCodec,
                    _streamName,
                    _frameCount);
            }
        }
    }
}