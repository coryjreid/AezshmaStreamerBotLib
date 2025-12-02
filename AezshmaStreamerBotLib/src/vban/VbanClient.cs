using System;
using System.Net;
using System.Net.Sockets;

namespace AezshmaStreamerBotLib.vban {
    public class VbanClient : IDisposable {
        private readonly Socket _socket;
        private readonly IPEndPoint _endpoint;

        public VbanClient(string host, int port) {
            _socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            _socket.ReceiveTimeout = 5000; // 5 second timeout
            _endpoint = new IPEndPoint(IPAddress.Parse(host), port);
        }

        public byte[] SendCommand(VbanTextCommand command) {
            // Send
            byte[] packetBytes = command.ToBytes();
            _socket.SendTo(packetBytes, _endpoint);

            // Receive response
            byte[] receiveBuffer = new byte[1464];
            EndPoint remoteEndPoint = new IPEndPoint(IPAddress.Any, 0);
            int bytesReceived = _socket.ReceiveFrom(receiveBuffer, ref remoteEndPoint);

            byte[] response = new byte[bytesReceived];
            Array.Copy(receiveBuffer, response, bytesReceived);
            return response;
        }

        public void Dispose() {
            _socket?.Close();
            _socket?.Dispose();
        }
    }
}