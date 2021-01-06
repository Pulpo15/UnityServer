﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.Net.Sockets;

namespace GameServer {
    class Client {
        public static int dataBufferSize = 4096;

        public int id;
        public TCP tcp;

        public Client(int _clientId) {
            id = _clientId;
            tcp = new TCP(id);
        }

        public class TCP {
            public TcpClient socket;

            private readonly int id;
            private NetworkStream stream;
            private Packet receivedData;
            private byte[] receiveBuffer;

            public TCP(int _id) {
                id = _id;
            }

            public void Connect(TcpClient _socket) {
                socket = _socket;
                socket.ReceiveBufferSize = dataBufferSize;
                socket.SendBufferSize = dataBufferSize;

                stream = socket.GetStream();

                receivedData = new Packet();
                receiveBuffer = new byte[dataBufferSize];

                stream.BeginRead(receiveBuffer, 0, dataBufferSize, RecieveCallback, null);

                ServerSend.Welcome(id, "Welcome to server");
            }

            public void SendData(Packet _packet) {
                try {
                    if(socket != null) {
                        stream.BeginWrite(_packet.ToArray(), 0, _packet.Length(), null, null);
                    }
                } catch (Exception _ex) {
                    Console.WriteLine($"Error sending data to player {id} via TCP: {_ex}");
                }
            }

            private void RecieveCallback(IAsyncResult _result) {
                try {
                    int _byteLength = stream.EndRead(_result);
                    if (_byteLength <= 0) {
                        return;
                    }

                    byte[] _data = new byte[_byteLength];
                    Array.Copy(receiveBuffer, _data, _byteLength);

                    receivedData.Reset(HandleData(_data));
                    stream.BeginRead(receiveBuffer, 0, dataBufferSize, RecieveCallback, null);
                } catch (Exception _ex) {
                    Console.WriteLine($"Error recieving TCP data: {_ex}.");
                }
            }

            private bool HandleData(byte[] _data) {
                int _packetLength = 0;

                receivedData.SetBytes(_data);

                if (receivedData.UnreadLength() >= 4) {
                    _packetLength = receivedData.ReadInt();
                    if (_packetLength <= 0) {
                        return true;
                    }
                }

                while (_packetLength > 0 && _packetLength <= receivedData.UnreadLength()) {
                    byte[] _packetBytes = receivedData.ReadBytes(_packetLength);
                    ThreadManager.ExecuteOnMainThread(() => {
                        using (Packet _packet = new Packet(_packetBytes)) {
                            int _packetId = _packet.ReadInt();
                            Server.packetHandlers[_packetId](id, _packet);
                        }
                    });

                    _packetLength = 0;
                    if (receivedData.UnreadLength() >= 4) {
                        _packetLength = receivedData.ReadInt();
                        if (_packetLength <= 0) {
                            return true;
                        }
                    }
                }

                if (_packetLength <= 1) {
                    return true;
                }

                return false;
            }
        }
    }
    
}
