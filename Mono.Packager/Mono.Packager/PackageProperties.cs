using System;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Serialization;
using System.IO;

namespace Mono.Packager
{
	[XmlRoot ("package")]
	public class PackageProperties
	{
		[XmlElementAttribute ("name")]
		public string Name { get; set; }
		[XmlElementAttribute ("shortname")]
		public string ShortName { get; set; }
		[XmlElementAttribute ("summary")]
		public string Summary { get; set; }
		[XmlElementAttribute ("version")]
		public string Version { get; set; }
		[XmlElementAttribute ("url")]
		public string Url { get; set; }
		[XmlElementAttribute ("license")]
		public string License { get; set; }
		[XmlElementAttribute ("type")]
		public PackageType Type { get; set; }
		[XmlElementAttribute ("description")]
		public string Description { get; set; }
		[XmlElementAttribute ("createdesktopfile")]
		public bool CreateDesktopFile { get; set; }
		[XmlElementAttribute ("icon")]
		public string DesktopIcon { get; set; }
		[XmlElementAttribute ("categories")]
		public string Categories { get; set; }
		[XmlElementAttribute ("group")]
		public string Group { get; set; }

		[XmlArray ("dependencies"), XmlArrayItem ("dependency", typeof (string))]
		public List<string> Dependencies { get; set; }
		[XmlArray ("provides"), XmlArrayItem ("provide", typeof (string))]
		public List<string> Provides { get; set; }

		[XmlElement ("filetree")]
		public DirectoryData FileTree { get; set; }
		
		[XmlElementAttribute ("autogeneraterelease")]
		public bool AutoGenerateRelease { get; set; }
		[XmlElementAttribute ("release")]
		public string Release { get; set; }

		[XmlElementAttribute ("preinstallscript")]
		public string PreInstallScript { get; set; }
		[XmlElementAttribute ("postinstallscript")]
		public string PostInstallScript { get; set; }
		[XmlElementAttribute ("preuninstallscript")]
		public string PreUninstallScript { get; set; }
		[XmlElementAttribute ("postuninstallscript")]
		public string PostUninstallScript { get; set; }

		[XmlElement ("aspnetvirtualpath")]
		public string AspNetVirtualPath { get; set; }
		[XmlElement ("useiomap")]
		public bool UseMonoIomap { get; set; }
		[XmlElement ("minimummono")]
		public string MinimumMonoVersion { get; set; }

		[XmlElement ("leaveonserver")]
		public bool LeavePackageOnServer { get; set; }
		[XmlElement ("serverlocation")]
		public string ServerLocation { get; set; }
		[XmlElement("targetplatform")]
		public string TargetPlatform { get; set; }
		
		[XmlIgnoreAttribute ()]
		public string BaseDirectory { get; set; }
		
		public PackageProperties ()
		{
			Dependencies = new List<string> ();
			Provides = new List<string> ();
			FileTree = new DirectoryData ();
			
			Type = PackageType.Exe;

			Version = "1.0";
			Release = "1";
			License = "Proprietary";
			Group = "System/Packages";
			Categories = "GNOME";
			
			UseMonoIomap = true;
		}
		
		#region Public Methods
		public static PackageProperties Deserialize (string filename)
		{
			PackageProperties properties;

			XmlSerializer serializer = new XmlSerializer (typeof (PackageProperties));

			using (TextReader textreader = new StreamReader (filename))
				properties = (PackageProperties)serializer.Deserialize (textreader);

			properties.BaseDirectory = Path.GetDirectoryName (filename);

			return properties;
		}

		public void Serialize (string filename)
		{
			XmlSerializer serializer = new XmlSerializer (typeof (PackageProperties));
			
			using (TextWriter textwriter = new StreamWriter (filename))
				serializer.Serialize (textwriter, this);
		}

		public string WordWrap (string text, int length)
		{
			if (text.Length == 0)
				return "";

			string result = "";
			string current_line = "";

			string[] words = text.Replace ("\n", "\n ").Split (' ');

			foreach (string word in words) {
				if (current_line.EndsWith ("\n")) {
					result += current_line;
					current_line = "";
				} else if ((current_line.Length + word.Length) > length) {
					result += current_line + "\n";
					current_line = "";
				}

				if (current_line.Length > 0)
					current_line += " " + word;
				else
					current_line = word;
			}

			if (current_line.Length > 0)
				result += current_line;

			return result;
		}
		#endregion
		
		#region Public Properties
		[XmlIgnore]
		public List<string> Folders {
			get {
				List<string> output = new List<string> ();
				
				GetAllDirectories (FileTree, string.Empty, output, true);
				GetAllDirectories (FileTree, string.Empty, output, false);
				
				return output;
			}
		}

		[XmlIgnore]
		public List<string> WritableFolders {
			get {
				List<string> output = new List<string> ();
				
				GetAllDirectories (FileTree, string.Empty, output, true);
				
				return output;
			}
		}
	
		[XmlIgnore]
		public List<string> NonWritableFolders {
			get {
				List<string> output = new List<string> ();
				
				GetAllDirectories (FileTree, string.Empty, output, false);
				
				return output;
			}
		}
		
		[XmlIgnore]
		public List<string> Files {
			get {
				List<string> output = new List<string> ();

				GetAllFiles (FileTree, string.Empty, output, true);
				GetAllFiles (FileTree, string.Empty, output, false);
				
				return output;
			}
		}

		[XmlIgnore]
		public List<string> WritableFiles {
			get {
				List<string> output = new List<string> ();

				GetAllFiles (FileTree, string.Empty, output, true);
				
				return output;
			}
		}
		
		[XmlIgnore]
		public List<string> NonWritableFiles {
			get {
				List<string> output = new List<string> ();

				GetAllFiles (FileTree, string.Empty, output, false);
				
				return output;
			}
		}
		
		[XmlIgnore]
		public string StartUpExe {
			get { return FindStartupExecutable (FileTree); }	
		}
		#endregion
		
		#region Private Methods
		private void GetAllDirectories (DirectoryData directory, string path, List<string> output, bool writable)
		{
			// Add to the path
			if (directory.Name != "Application Root") {
				path += Path.DirectorySeparatorChar + directory.Name;
				path = path.TrimStart (Path.DirectorySeparatorChar);
			}

			// Add this directory
			if (path.Trim ().Length > 0 && (directory.Writable == writable))
				output.Add (path);
			
			// Add child directories
			foreach (DirectoryData d in directory.Directories)
				GetAllDirectories (d, path, output, writable);
		}
		
		private void GetAllFiles (DirectoryData directory, string path, List<string> output, bool writable)
		{
			// Add to the path
			if (directory.Name != "Application Root") {
				path += Path.DirectorySeparatorChar + directory.Name;
				path = path.TrimStart (Path.DirectorySeparatorChar);
			}

			// Add child files
			foreach (FileData file in directory.Files)
				if (file.Writeable == writable)
					output.Add (Path.Combine (path, file.Name));

			// Add child directories
			foreach (DirectoryData d in directory.Directories)
				GetAllFiles (d, path, output, writable);
		}

		private string FindStartupExecutable (DirectoryData directory)
		{
			foreach (FileData f in directory.Files)
				if (f.StartupExecutable)
					return f.Name;

			foreach (DirectoryData d in directory.Directories) {
				string exe = FindStartupExecutable (d);
				
				if (!string.IsNullOrEmpty (exe))
					return exe;
			}

			return string.Empty;
		}

		#endregion
	}
}
