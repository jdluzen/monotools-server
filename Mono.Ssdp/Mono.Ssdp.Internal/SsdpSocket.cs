//
// SsdpSocket.cs
//
// Author:
//   Aaron Bockover <abockover@novell.com>
//
// Copyright (C) 2008 Novell, Inc.
//
// Permission is hereby granted, free of charge, to any person obtaining
// a copy of this software and associated documentation files (the
// "Software"), to deal in the Software without restriction, including
// without limitation the rights to use, copy, modify, merge, publish,
// distribute, sublicense, and/or sell copies of the Software, and to
// permit persons to whom the Software is furnished to do so, subject to
// the following conditions:
//
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
// LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
// OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
// WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
//

using System;
using System.Net;
using System.Net.Sockets;

namespace Mono.Ssdp.Internal
{
    internal class SsdpSocket : Socket
    {
        private static IPEndPoint ssdp_endpoint = new IPEndPoint (Protocol.IPAddress, Protocol.Port);
        private bool multi_cast;
        
        public SsdpSocket () : this (true)
        {
        }
        
        public SsdpSocket (bool multicast) : base (AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp)
        {
            multi_cast = multicast;
            
            SetSocketOption (SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
            if (multicast) {
                SetSocketOption (SocketOptionLevel.Socket, SocketOptionName.Broadcast, true);
                SetSocketOption (SocketOptionLevel.IP, SocketOptionName.MulticastTimeToLive, Protocol.SocketTtl);
            } 
        }
        
        public IAsyncResult BeginSendTo (byte [] data, AsyncCallback callback)
        {
            return BeginSendTo (data, callback, ssdp_endpoint);
        }

        public IAsyncResult BeginSendTo (byte[] data, AsyncCallback callback, IPEndPoint endPoint)
        {
            return BeginSendTo (data, 0, data.Length, SocketFlags.None, endPoint, callback, this);
        }
        
        public IAsyncResult BeginReceiveFrom (AsyncReceiveBuffer buffer, AsyncCallback callback)
        {
            return base.BeginReceiveFrom (buffer.Buffer, 0, buffer.Buffer.Length, SocketFlags.None, 
                ref buffer.SenderEndPoint, callback, buffer);
        }
        
        public void Bind ()
        {
            if (multi_cast) {
                Bind (new IPEndPoint (IPAddress.Any, Protocol.Port));
                SetSocketOption (SocketOptionLevel.IP, SocketOptionName.AddMembership, new MulticastOption (Protocol.IPAddress));
            } else {
		Bind (ssdp_endpoint);
            }
       }
    }
}
