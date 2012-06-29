using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Net;
using System.Xml;
using MonoTools.Client;

namespace MonoTools.Server
{
	public class RunWebSiteRemotelySession : BaseSession
	{
		protected bool is_started;
		protected IPAddress host;

		private XspOptions xsp_options;

		public RunWebSiteRemotelySession (NameValueCollection query) : base (query)
		{
			IsWeb = true;
		}

		public override void Start (HttpListenerContext context)
		{
			Logger.LogDebug ("Starting web site");

			XmlDocument options = context.GetRequestDataAsXml ();
			host = context.Request.LocalEndPoint.Address;

			var session = StartXspSession (options);

			bool success = session != null && session.Result != null && session.Result.Success;

			if (success) {
				Logger.LogDebug ("  xsp started");
				is_started = true;
				process = session.Process;
				context.WriteString (ToXml ());
			} else {
				Logger.LogDebug ("  Could not start xsp");
				context.ReturnException (new ApplicationException ("Could not start xsp"));
			}
		}

		protected virtual XspSession StartXspSession (XmlDocument options)
		{
			// See if VS sent us a XspOptions
			var xsp_node = options.SelectSingleNode ("/options/xsp-options");

			if (xsp_node != null)
				xsp_options = new XspOptions (xsp_node);
			else
				xsp_options = new XspOptions (
					host,
					MonoToolsConfigurationManager.ApplicationPortRangeStart,
					MonoToolsConfigurationManager.ApplicationPortRangeEnd);

			xsp_options.LocalDirectory = RemotePath;

			// Set debug = true and RemoteErrors = off in web.config
			bool custom_errors_off = false;

			if (xsp_options.CustomErrorsOff != null)
				custom_errors_off = (bool)xsp_options.CustomErrorsOff;

			if (options.DocumentElement["customerrorsoff"] != null)
				custom_errors_off = bool.Parse (options.DocumentElement["customerrorsoff"].InnerText);

			Utilities.ModifyWebConfig (RemotePath, true, custom_errors_off);

			// Get environment variables and arguments for Mono
			StringDictionary env_vars = new StringDictionary ();
			SetEnvironmentVariables (env_vars, options);

			List<string> mono_args = new List<string> ();
			AddMonoArguments (mono_args);

			if (options.DocumentElement["monoarguments"] != null)
				mono_args.Add (options.DocumentElement["monoarguments"].InnerText);

			// Fire up the session
			xsp = new XspSession (xsp_options, RemotePath, mono_args, env_vars);

			if (xsp.Start () == false)
				return null;

			WaitForXspPort ();

			return xsp;
		}

		// For !debugging, we want to wait for the xsp port in StartSession
		protected virtual void WaitForXspPort ()
		{
			xsp.WaitHandle.WaitOne ();
		}

		protected override void SerializeSession (XmlWriter writer)
		{
			if (is_started)
				xsp.Result.Serialize (writer);
		}

		protected virtual void AddMonoArguments (List<string> args)
		{
			args.Add ("--debug");
		}
	}
}
