using System;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Serialization;

namespace Mono.Packager
{
	public class DirectoryData
	{
		[XmlAttribute ("name")]
		public string Name { get; set; }

		[XmlAttribute ("writable")]
		public bool Writable { get; set; }

		[XmlArray ("directories"), XmlArrayItem ("directory", typeof (DirectoryData))]
		public List<DirectoryData> Directories { get; set; }

		[XmlArray ("files"), XmlArrayItem ("file", typeof (FileData))]
		public List<FileData> Files { get; set; }

		public DirectoryData ()
		{
			Directories = new List<DirectoryData> ();
			Files = new List<FileData> ();
		}

		public DirectoryData (string name) : this ()
		{
			Name = name;
		}

		public DirectoryData (string name, bool writable) : this ()
		{
			Name = name;
			Writable = writable;
		}

	}
}
