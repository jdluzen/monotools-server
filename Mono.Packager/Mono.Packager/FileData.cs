using System;
using System.ComponentModel;
using System.Xml;
using System.Xml.Serialization;

namespace Mono.Packager
{
	public class FileData
	{
		[XmlAttribute ("name")]
		public string Name { get; set; }

		[XmlAttribute ("source")]
#if WINDOWS
		[Editor (typeof (System.Windows.Forms.Design.FileNameEditor), typeof (System.Drawing.Design.UITypeEditor))]
#endif		
		public string Source { get; set; }

		[XmlAttribute ("startup")]
		public bool StartupExecutable { get; set; }
		
		[XmlAttribute ("writeable")]
		public bool Writeable { get; set; }
		
		public FileData ()
		{
		}

		public FileData (string name, string source)
		{
			Name = name;
			Source = source;
		}

		public override string ToString ()
		{
			return Name;
		}
	}
}
