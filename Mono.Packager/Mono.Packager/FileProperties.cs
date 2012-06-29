using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using System.ComponentModel;
using System.IO;

namespace Mono.Packager
{
	public class FileProperties
	{
		[XmlElement ("name")]
		public string Name { get; set; }
		[XmlElement ("source")]
#if WINDOWS
		[Editor (typeof (System.Windows.Forms.Design.FileNameEditor), typeof (System.Drawing.Design.UITypeEditor))]
#endif		
		public string Source { get; set; }
		[XmlElement ("destination")]
		[Browsable (false)]
		public string Destination { get; set; }
		
		public FileProperties ()
		{
		}

		public FileProperties (string name, string source) : this (name, source, string.Empty)
		{
		}

		public FileProperties (string name, string source, string destination)
		{
			Name = name;
			Source = source;
			Destination = destination;
		}

		public string FinalDestination {
			get {
				if (Destination == "Application Root")
					return Name;
				
				return Path.Combine (Destination.Replace ('\\', '/').Substring ("Application Root\\".Length), Name);
			}
		}
		
		public override string ToString ()
		{
			return Name;
		}
	}
}
