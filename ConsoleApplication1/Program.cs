// ABB Redundant Network Routing Protocol (RNRP)
// Copyright 2017 Thomas Jäger
// https://github.com/thoj
// License: https://tldrlegal.com/license/bsd-3-clause-license-(revised)

using System;
using System.Net;
using System.Net.Sockets;
using System.IO;

namespace ConsoleApplication1
{
    class RnrpPacket {
        public int Version { get; }
        public int Length { get; }
        public int Node { get; }
        public int Network { get; }
        public int Path { get; }
        public byte[] Unknown { get; }
        public int Counter { get; }

        public RnrpPacket(Byte[] data)
        {
            MemoryStream memoryStream = new MemoryStream(data, 0, data.Length);
            BinaryReader br = new BinaryReader(memoryStream); // LittleEndian? Really.
            Version = br.ReadByte() * 256 + br.ReadByte();
            Length = br.ReadByte() * 256 + br.ReadByte();
            Node = br.ReadByte() * 256 + br.ReadByte();
            Network = br.ReadByte();
            Path = br.ReadByte();
            Unknown = br.ReadBytes(66);
            Counter = br.ReadByte() * 256 + br.ReadByte();
        }
        public override string ToString()
        {
            return string.Format("Version:{0} Length:{1} Node:{2} Network:{3} Path:{4} Counter:{5}", Version, Length, Node, Network, Path, Counter);
        }
    }

    class Program
    {
        private static void Main(string[] args)
        {
            UdpClient client = new UdpClient();

            client.ExclusiveAddressUse = false;
            IPEndPoint localEp = new IPEndPoint(IPAddress.Any, 2423);

            client.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
            client.ExclusiveAddressUse = false;

            client.Client.Bind(localEp);
            //TODO: Only join groups based on interface addresses. (Boring)
            for (int i = 0; i <= 255; i++) { 
                IPAddress multicastaddress = IPAddress.Parse(string.Format("239.239.239.{0}", i));
                client.JoinMulticastGroup(multicastaddress);
            }
            while (true)
            {
                Byte[] data = client.Receive(ref localEp);
                var r = new RnrpPacket(data);
                Console.WriteLine("{0} {1}", localEp.Address, r.ToString());
            }
        }
    }
}
