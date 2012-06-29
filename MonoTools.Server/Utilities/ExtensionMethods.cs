using System;
using System.Net;
using System.IO;
using System.Xml;
using System.Text;

namespace MonoTools.Server
{
	public static class ExtensionMethods
	{
		public static void WriteString (this HttpListenerContext context, string s)
		{
			using (StreamWriter sw = new StreamWriter (context.Response.OutputStream))
				sw.Write (s);

			context.Response.OutputStream.Close ();
		}

		public static void ReturnSuccess (this HttpListenerContext context)
		{
			WriteString (context, "<response type='success' />");
		}

		public static void ReturnException (this HttpListenerContext context, Exception ex)
		{
			using (StringWriter sw = new StringWriter ()) {
				using (XmlTextWriter xw = new XmlTextWriter (sw)) {
					xw.WriteStartElement ("response");
					xw.WriteAttributeString ("type", "exception");

					Utilities.WriteExceptionToXml (xw, ex);

					xw.WriteEndElement ();
				}

				WriteString (context, sw.ToString ());
			}
		}

		public static string GetRequestData (this HttpListenerContext context)
		{
			string data;

			using (StreamReader sr = new StreamReader (context.Request.InputStream))
				data = sr.ReadToEnd ();

			return data;
		}

		public static XmlDocument GetRequestDataAsXml (this HttpListenerContext context)
		{
			string data = GetRequestData (context);

			XmlDocument doc = new XmlDocument ();
			doc.LoadXml (data);

			return doc;
		}

		public static void SavePostedFile (this HttpListenerContext context, string filename)
		{
			string boundary = context.Request.ContentType.Substring (context.Request.ContentType.IndexOf ("=") + 1);
			Stream stream = context.Request.InputStream;
			Encoding encoding = context.Request.ContentEncoding;
			long contentLength = context.Request.ContentLength64;


			if (!Directory.Exists (Path.GetDirectoryName (filename)))
				Directory.CreateDirectory (Path.GetDirectoryName (filename));

			using (BinaryWriter sw = new BinaryWriter (new FileStream (filename, FileMode.Create))) {
				using (BinaryReader sr = new BinaryReader (stream, encoding)) {
					int len;
					long file_length = contentLength;

					byte[] buffer = new byte[2048];

					// Read the first block, find where our file actually starts,
					// and write that part to our file
					len = sr.Read (buffer, 0, buffer.Length);

					int crlflen;
					int beg = FindBeginningOfFile (buffer, out crlflen);

					// Find the actual length of our file
					file_length -= beg;
					file_length -= boundary.Length;
					file_length -= 2 + (3 * crlflen);	// <CR><LF><boundary>--<CR><LF><CR><LF>

					// Write the first part of the file
					if (len - beg <= file_length) {
						sw.Write (buffer, beg, len - beg);
						file_length -= (len - beg);
					} else {
						// We already have the whole file
						sw.Write (buffer, beg, (int)file_length);

						// Drain the rest of the stream
						int l1 = sr.Read (buffer, 0, buffer.Length);
						return;
					}

					bool done = false;

					// Write the rest of the file
					while ((len = sr.Read (buffer, 0, buffer.Length)) > 0) {
						// If we are draining the stream, ignore writing
						if (done)
							continue;

						// Buffer isn't the end of the file, write it all
						if (file_length >= len) {
							sw.Write (buffer, 0, len);
							file_length -= len;
						} else {
							// Write the last piece of the file
							sw.Write (buffer, 0, (int)file_length);
							done = true;
						}
					}
				}
			}}

		private static int FindBeginningOfFile (byte[] buffer, out int crlflen)
		{
			int len = LengthOfString (buffer, 0, out crlflen);

			string s1 = Encoding.UTF8.GetString (buffer, 0, len);

			int len2 = LengthOfString (buffer, len, out crlflen);

			string s2 = Encoding.UTF8.GetString (buffer, len, len2 - len);
			int len3 = LengthOfString (buffer, len2, out crlflen);

			string s3 = Encoding.UTF8.GetString (buffer, len2, len3 - len2);
			int len4 = LengthOfString (buffer, len3, out crlflen);

			string s4 = Encoding.UTF8.GetString (buffer, len3, len4 - len3);

			return len4;
		}

		private static int LengthOfString (byte[] buffer, int start, out int crlflen)
		{
			crlflen = -1;

			for (int i = start; i < buffer.Length; i++) {
				int current = (int)buffer[i];

				if (current != 13 && current != 10)
					continue;

				if (current == 10 || (int)buffer[i + 1] != 10) {
					crlflen = 1;
					return i + 1;
				}

				crlflen = 2;
				return i + 2;
			}

			return -1;
		}

		public static void ReturnFile (this HttpListenerContext context, string filename)
		{
			HttpListenerResponse response = context.Response;
			Stream stream = response.OutputStream;

			byte[] file_bytes = new byte[2048];

			using (MemoryStream ms = new MemoryStream ()) {
				using (BinaryReader br = new BinaryReader (File.Open (filename, FileMode.Open))) {
					int len;

					while ((len = br.Read (file_bytes, 0, file_bytes.Length)) > 0)
						ms.Write (file_bytes, 0, len);
				}

				response.ContentLength64 = ms.Length;
				
				ms.WriteTo (stream);
			}
		}
	}
}
