﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace KNXLib
{
    internal class KNXSenderTunneling : KNXSender
    {
        #region constructor
        internal KNXSenderTunneling(KNXConnectionTunneling connection, UdpClient udpClient, IPEndPoint remoteEndpoint)
            : base(connection)
        {
            this.RemoteEndpoint = remoteEndpoint;
            this.UdpClient = udpClient;
        }
        #endregion

        #region variables
        private IPEndPoint _remoteEndpoint;
        private IPEndPoint RemoteEndpoint
        {
            get
            {
                return this._remoteEndpoint;
            }
            set
            {
                this._remoteEndpoint = value;
            }
        }

        private UdpClient _udpClient;
        private UdpClient UdpClient
        {
            get
            {
                return this._udpClient;
            }
            set
            {
                this._udpClient = value;
            }
        }

        private KNXConnectionTunneling KNXConnectionTunneling
        {
            get
            {
                return (KNXConnectionTunneling) this.KNXConnection;
            }
            set
            {
                this.KNXConnection = value;
            }
        }
        #endregion

        #region send
        internal override void SendData(byte[] dgram)
        {
            UdpClient.Send(dgram, dgram.Length, RemoteEndpoint);
        }
        #endregion

        #region datagram processing
        internal override byte[] CreateDatagram(string destination_address, byte[] data)
        {
            int data_length = KNXHelper.GetDataLength(data);
            // HEADER
            byte[] dgram = new byte[10];
            dgram[00] = 0x06;
            dgram[01] = 0x10;
            dgram[02] = 0x04;
            dgram[03] = 0x20;
            byte[] total_length = BitConverter.GetBytes(data_length + 21);
            dgram[04] = total_length[1];
            dgram[05] = total_length[0];

            dgram[06] = 0x04;
            dgram[07] = this.KNXConnectionTunneling.ChannelId;
            dgram[08] = this.KNXConnectionTunneling.GenerateSequenceNumber();
            dgram[09] = 0x00;

            return base.CreateDatagram(destination_address, data, dgram);
        }
        #endregion
    }
}
