﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.Net.Sockets;

namespace GameServer {
    class Server {
        public static int MaxPlayers { get; private set; }
        public static int Port { get; private set; }
        public static Dictionary<int, Client> clients = new Dictionary<int, Client>();
        public delegate void PacketHandler(int _fromClient, Packet _packet);
        public static Dictionary<int, PacketHandler> packetHandlers;

        private static TcpListener tcpListener;
        private static UdpClient udpListener;

        public static void Start(int _maxPlayers, int _port) {
            MaxPlayers = _maxPlayers;
            Port = _port;

            Console.WriteLine("Starting server...");
            InitalizeServerData();

            tcpListener = new TcpListener(IPAddress.Any, Port);
            tcpListener.Start();
            tcpListener.BeginAcceptTcpClient(new AsyncCallback(TCPConnectCallBack), null);

            udpListener = new UdpClient(Port);
            udpListener.BeginReceive(UDPReceiveCallback, null);

            Console.WriteLine($"Server started on {Port}.");
        }

        private static void TCPConnectCallBack(IAsyncResult _result) {
            TcpClient _client = tcpListener.EndAcceptTcpClient(_result);
            tcpListener.BeginAcceptTcpClient(new AsyncCallback(TCPConnectCallBack), null);

            Console.WriteLine($"Incoming connection from {_client.Client.RemoteEndPoint}...");

            for (int i = 1; i <= MaxPlayers; i++) {
                if (clients[i].tcp.socket == null) {
                    clients[i].tcp.Connect(_client);
                    //Console.WriteLine($"Connected from {_client.Client.RemoteEndPoint}");
                    return;
                }
            }

            Console.WriteLine($"{_client.Client.RemoteEndPoint} failed to connect: Server full");
        }

        private static void UDPReceiveCallback(IAsyncResult _result) {
            try {
                IPEndPoint _clientEndPoint = new IPEndPoint(IPAddress.Any, 0);
                byte[] _data = udpListener.EndReceive(_result, ref _clientEndPoint);
                udpListener.BeginReceive(UDPReceiveCallback, null);

                if (_data.Length < 4) {
                    Console.WriteLine("Packet have less than 4 bytes");
                    return;
                }

                using (Packet _packet = new Packet(_data)) {
                    int _clientId = _packet.ReadInt();

                    if (_clientId == 0) {
                        return;
                    }

                    if (clients[_clientId].udp.endPoint == null) {
                        clients[_clientId].udp.Connect(_clientEndPoint);
                        return;
                    }

                    if (clients[_clientId].udp.endPoint.ToString() == _clientEndPoint.ToString()) {
                        clients[_clientId].udp.HandleData(_packet);
                    }
                }
            } catch (Exception _ex){
                Console.WriteLine($"Error receiving UDP data: {_ex}");
            }
        }

        public static void SendUDPData(IPEndPoint _clientEndPoint, Packet _packet) {
            try {
                if (_clientEndPoint != null) {
                    udpListener.BeginSend(_packet.ToArray(), _packet.Length(), _clientEndPoint, null, null);
                }
            } catch (Exception _ex) {
                Console.WriteLine($"Error sendind data to {_clientEndPoint} via UDP {_ex}");
            }
        }

        private static void InitalizeServerData() {
            for (int i = 1; i <= MaxPlayers; i++) {
                clients.Add(i, new Client(i));
            }

            packetHandlers = new Dictionary<int, PacketHandler>() {
                {(int)ClientPackets.welcomeReceived, ServerHandler.WelcomeReceived},
                {(int)ClientPackets.udpTestReceived, ServerHandler.UDPTestReceived}
            };
            Console.WriteLine("Initialized packets.");
        }

    }
}