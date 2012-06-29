using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Net;
using System.IO;
using System.Web;

namespace MonoTools.Client
{
	static class Connection
	{
		private static string server_url = "http://192.168.199.137:8805";
		//private static string server_url = "http://localhost:8805";

		public static void SetServer (string address, int port)
		{
			server_url = string.Format ("http://{0}:{1}", address, port);
		}

		public static Uri CreateUri (string relativeUrl)
		{
			return new Uri (server_url + relativeUrl);
		}

		public static XmlDocument HttpGet (string url)
		{
			WebClient wc = new WebClient ();
			url = server_url + url;

			string result = wc.DownloadString (url);

			XmlDocument doc = new XmlDocument ();
			doc.LoadXml (result);

			return doc;
		}

		public static XmlDocument HttpDelete (string url, string data)
		{
			WebClient wc = new WebClient ();
			url = server_url + url;

			byte[] buffer = Encoding.ASCII.GetBytes (data);
			byte[] response = wc.UploadData (url, "DELETE", buffer);

			string result = Encoding.ASCII.GetString (response);

			XmlDocument doc = new XmlDocument ();
			doc.LoadXml (result);

			return doc;
		}

		public static XmlDocument HttpPost (string url, string data)
		{
			WebClient wc = new WebClient ();
			url = server_url + url;

			byte[] buffer = Encoding.ASCII.GetBytes (data);
			byte[] response = wc.UploadData (url, buffer);

			string result = Encoding.ASCII.GetString (response);

			XmlDocument doc = new XmlDocument ();
			doc.LoadXml (result);

			return doc;
		}
		
		public static XmlDocument HttpPostFile (string url, string filename)
		{
			WebClient wc = new WebClient ();
			url = server_url + url;

			byte[] response = wc.UploadFile (url, filename);

			string result = Encoding.ASCII.GetString (response);

			XmlDocument doc = new XmlDocument ();
			doc.LoadXml (result);

			return doc;
		}
		public static XmlDocument HttpPost (string url, string[] paramName, string[] paramVal)
		{
			WebRequest req = WebRequest.Create (new Uri (server_url + url));

			req.Method = "POST";
			req.ContentLength = 0;

			if (paramName != null) {
				req.ContentType = "application/x-www-form-urlencoded";

				// Build a string with all the params, properly encoded.
				// We assume that the arrays paramName and paramVal are
				// of equal length:
				StringBuilder p = new StringBuilder ();
				for (int i = 0; i < paramName.Length; i++) {
					p.Append (paramName[i]);
					p.Append ("=");
					p.Append (HttpUtility.UrlEncode (paramVal[i]));
					p.Append ("&");
				}

				// Encode the parameters as form data:
				byte[] formData = UTF8Encoding.UTF8.GetBytes (p.ToString ());
				req.ContentLength = formData.Length;

				// Send the request:
				using (Stream post = req.GetRequestStream ())
					post.Write (formData, 0, formData.Length);
			}

			// Pick up the response:
			string result = null;

			using (WebResponse resp = req.GetResponse ())
			using (StreamReader reader = new StreamReader (resp.GetResponseStream ()))
				result = reader.ReadToEnd ();

			XmlDocument doc = new XmlDocument ();
			doc.LoadXml (result);

			return doc;
		}
	}
}
