using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.Specialized;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Xml;
using Mono.Packager;

namespace MonoTools.Server
{
	public class CreateRpmSession : BaseSession
	{
		private string rpm_file = string.Empty;
		private Exception build_exception;
		private Exception copy_exception;

		public CreateRpmSession (NameValueCollection query) : base (query)
		{
		}

		public override void Start (HttpListenerContext context)
		{
			XmlDocument options = context.GetRequestDataAsXml ();

			string pkgxml = options.DocumentElement["executable"].InnerText;

			PackageProperties properties = PackageProperties.Deserialize (Path.Combine (RemotePath, pkgxml));

			if (properties.AutoGenerateRelease)
				properties.Release = DateTime.Now.ToString ("yyyyMMdd.HHmmss");

			PackageBuilder builder = new PackageBuilder (properties);
			builder.Root = RemotePath;

			bool result = builder.Build ();

			if (result) {
				// Build was successful
				string rpm = String.Format ("{0}-{1}-{2}.noarch.rpm", properties.ShortName, properties.Version, properties.Release);
				rpm_file = String.Format ("{0}/{1}", RemotePath, rpm);

				// See if we are supposed to leave a copy on the server
				if (properties.LeavePackageOnServer) {
					try {
						if (!Directory.Exists (properties.ServerLocation))
							Directory.CreateDirectory (properties.ServerLocation);

						File.Copy (rpm_file, Path.Combine (properties.ServerLocation, rpm), true);
					} catch (Exception ex) {
						copy_exception = ex;
					}
				}
			} else {
				build_exception = new Exception (builder.Errors[0].ErrorText);
			}

			HasTerminated = true;
			context.WriteString (ToXml ());
		}

		protected override void SerializeSession (XmlWriter writer)
		{
			// Add the results of the build
			if (HasTerminated) {
				if (build_exception != null) {
					writer.WriteElementString ("packageresult", "build exception");
					Utilities.WriteExceptionToXml (writer, build_exception);
				} else if (copy_exception != null) {
					writer.WriteElementString ("packageresult", "copy exception");
					writer.WriteElementString ("package", Path.GetFileName (rpm_file));
					Utilities.WriteExceptionToXml (writer, copy_exception);
				} else {
					writer.WriteElementString ("packageresult", "success");
					writer.WriteElementString ("package", Path.GetFileName (rpm_file));
				}
			}
		}

		public override void ProcessRequest (HttpListenerContext context)
		{
			string route = context.Request.RawUrl.ToLowerInvariant ();

			if (route.Contains ("?"))
				route = route.Substring (0, route.IndexOf ('?'));

			if (context.Request.HttpMethod == "GET" && route.EndsWith ("/package")) {
				GetPackage (context);
				return;
			}

			base.ProcessRequest (context);
		}

		private void GetPackage (HttpListenerContext context)
		{
			context.ReturnFile (rpm_file);
		}
	}
}
