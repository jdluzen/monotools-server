using System;
using System.Net;
using System.Net.Sockets;
using Mono.WebServer;
using MonoTools.Client;

namespace MonoTools.WebServer
{
	public class MonoWebSource : XSPWebSource
	{
		public XspOptions Options {
			get; private set;
		}

		public IPAddress BindAddress {
			get; private set;
		}

		public IPEndPoint EndPoint {
			get; private set;
		}

		public MonoWebSource (XspOptions options)
			: base(IPAddress.Any, 0)
		{
			this.Options = options;

			IPAddress address;
			if (options.BindAddress.Equals (IPAddress.Any))
				address = Dns.GetHostAddresses (Dns.GetHostName ())[0];
			else
				address = options.BindAddress;
			BindAddress = address;
		}

		public override Socket CreateSocket ()
		{
			Socket socket;
			if (Options.UseRandomPort) {
				SetListenAddress (BindAddress, 0);
				socket = base.CreateSocket ();
				if (socket != null)
					EndPoint = (IPEndPoint)socket.LocalEndPoint;
				return socket;
			}

			if (Options.Port != null) {
				try {
					SetListenAddress (BindAddress, (int)Options.Port);
					socket = base.CreateSocket ();
					if (socket != null) {
						EndPoint = (IPEndPoint)socket.LocalEndPoint;
						return socket;
					}
				} catch {
					if (Options.BindRange == null)
						throw;
				}
			}

			if (Options.BindRange == null)
				return null;

			int port = Options.BindRange[0];
			while (true) {
				try {
					SetListenAddress (BindAddress, port);
					socket = base.CreateSocket ();
					if (socket != null) {
						EndPoint = (IPEndPoint)socket.LocalEndPoint;
						return socket;
					}
				} catch {
					if (++port >= Options.BindRange[1])
						throw;
				}
			}

			return null;
		}

	}
}
