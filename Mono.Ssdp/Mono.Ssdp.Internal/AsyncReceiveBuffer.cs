//
// AsyncReceiveBuffer.cs
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
    internal class AsyncReceiveBuffer
    {
        private const int length = 1024;
        private byte[] buffer = new byte[length];
        private int bytes_received;
        private SsdpSocket socket;
        
        // a field because it gets passed as ref; internal API anyway
        public EndPoint SenderEndPoint = new IPEndPoint (IPAddress.Any, Protocol.Port);
        
        public AsyncReceiveBuffer (SsdpSocket socket)
        {
            this.socket = socket;
        }
        
        public byte [] Buffer {
            get { return buffer; }
        }
        
        public int BytesReceived {
            get { return bytes_received; }
            set {
                byte[] buffer = this.buffer;
                for (int i = value; i < length; i++) {
                    buffer[i] = 0;
                }
                bytes_received = value;
            }
        }
        
        public SsdpSocket Socket {
            get { return socket; }
        }
        
        public IPEndPoint SenderIPEndPoint {
            get { return (IPEndPoint)SenderEndPoint; }
        }
    }
}
