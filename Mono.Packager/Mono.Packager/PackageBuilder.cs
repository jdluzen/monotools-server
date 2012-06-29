//
// MonoPackager
//
// Author:
//   Everaldo Canuto <ecanuto@novell.com>
//
// Copyright (C) 2009 Novell, Inc (http://www.novell.com)
//

// TODO: Change all "/" to system dir separator.

using System;
using System.IO;
using System.Text;
using System.Diagnostics;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;

using Mono.TextTemplating;
using Microsoft.VisualStudio.TextTemplating;

namespace Mono.Packager
{
	public class PackageBuilder : MarshalByRefObject, ITextTemplatingEngineHost
	{
		private string output_file;
		
		#region Contructors and destructors
		
		public PackageBuilder (PackageProperties properties)
		{
			if (properties == null)
				throw new ArgumentNullException ("properties");

			this.properties = properties;
		}
		
		public PackageBuilder (PackageProperties properties, string root) : this (properties)
		{
			this.Root = root;
		}
		
		#endregion // Contructors and destructors
		
		#region Private methods

		private CompilerError AddError (string error)
		{
			CompilerError err = new CompilerError ();
			err.ErrorText = error;
			Errors.Add (err);
			
			return err;
		}
		
		private bool ProcessTemplate (string input_file, string output_file)
		{
			if (String.IsNullOrEmpty (input_file))
				throw new ArgumentNullException ("inputFile");
			
			if (String.IsNullOrEmpty (output_file))
				throw new ArgumentNullException ("outputFile");
			
			errors.Clear ();
			this.output_file = output_file;
			this.template_file = input_file;
			
			Engine engine = new Engine ();

			if (!Path.IsPathRooted (template_file)) {
				string base_dir = Path.GetDirectoryName (System.Reflection.Assembly.GetExecutingAssembly ().Location);
				template_file = Path.Combine (base_dir, template_file);
			}

			string content;
			try {
				content = File.ReadAllText (template_file);
			} catch (IOException ex) {
				AddError ("Could not read input file '" + template_file + "'");
				return false;
			}
			
			string output = engine.ProcessTemplate (content, this);
			
			try {
				if (!errors.HasErrors)
					File.WriteAllText (output_file, output);
					// TODO: When we use enconding UTF8 rpmbuild later fail!
					//File.WriteAllText (output_file, output, file_encoding);
			} catch (IOException ex) {
				AddError ("Could not write output file '" + output_file + "'");
			}
			
			return !errors.HasErrors;
		}

		private void BuildRpms ()
		{
			// rpmbuild seems to want this directory, even if its never used
			if (!Directory.Exists (Path.Combine (root, "BUILD")))
				Directory.CreateDirectory (Path.Combine (root, "BUILD"));
			
			string rpmfile = String.Format ("{0}-{1}-{2}.noarch.rpm", properties.ShortName, properties.Version, properties.Release);
			string rpmpath = String.Format ("{0}/{1}", root, rpmfile);

			string rpmbuild_args = 
				String.Format (
					"--define '_topdir {0}' " +
					"--define '_rpmdir {0}' " +
					"--define '_specdir {0}' " +
					"-bb {0}/{1}.spec", root, Path.GetFileNameWithoutExtension ( properties.ShortName));
			
			// build...
			Process process = new Process ();
			process.StartInfo.CreateNoWindow = true;
			process.StartInfo.FileName = "rpmbuild";
			process.StartInfo.Arguments = rpmbuild_args;
			process.StartInfo.UseShellExecute = false;
			process.StartInfo.RedirectStandardOutput = true;
			process.Start ();
			process.WaitForExit ();

			// Save output buffer to log file
			string logfile = Path.Combine (root, "package.log");
			File.WriteAllText (logfile, process.StandardOutput.ReadToEnd ());
			
			// Delete previous rpm file if exists
			if (File.Exists (rpmpath))
				File.Delete (rpmpath);
			
			// Move rpm to build root folder
			string noarch = String.Format ("{0}/noarch/{1}", root, rpmfile);
			if (File.Exists (noarch))
				File.Move (noarch, rpmpath);

			// Delete 'noarch' folder
			if (Directory.Exists (Path.Combine (root, "noarch")))
				Directory.Delete (String.Format ("{0}/noarch", root), true);
		}
		
		#endregion // Private methods

		#region Public interface

		public bool Build ()
		{
			if (properties.CreateDesktopFile && !string.IsNullOrEmpty (properties.DesktopIcon)) {
				properties.DesktopIcon = Path.Combine (root, Path.Combine ("BUILD", Path.GetFileName (properties.DesktopIcon.Replace ('\\', '/'))));

				// If the user gave us an .ico, convert it to a .png
				if (Path.GetExtension (properties.DesktopIcon) == ".ico") {
					Icon ico = new Icon (properties.DesktopIcon, 48, 48);
					Bitmap png = new Bitmap (ico.Width, ico.Height);
					
					using (Graphics g = Graphics.FromImage (png))
						g.DrawIcon (ico, new Rectangle (Point.Empty, png.Size));
					
					string new_icon = string.Format ("{0}.png", properties.DesktopIcon.Substring (0, properties.DesktopIcon.Length - 4));
					png.Save (new_icon, ImageFormat.Png);
					
					properties.DesktopIcon = new_icon;
				}
			}		
			
			// Create wrapper script
			if ((properties.Type == PackageType.Winexe) || (properties.Type ==  PackageType.Exe)) {
				this.ProcessTemplate (
						"templates/wrapper.tt", 
						Path.Combine (root, properties.ShortName));
			
				if (this.Errors.HasErrors)
					return false;
			}

			// Create desktop file
			if (properties.CreateDesktopFile) {
				this.ProcessTemplate (
						"templates/desktop.tt",
						Path.Combine (root, Path.ChangeExtension (properties.ShortName, ".desktop")));
	
				if (this.Errors.HasErrors)
					return false;
			}
			
			// Create spec file
			string template = properties.Type.ToString().ToLower();
			
			this.ProcessTemplate (
					Path.Combine ("templates", Path.ChangeExtension (template, ".spec.tt")),
					Path.Combine (root, Path.ChangeExtension (properties.ShortName, ".spec")));
	
			if (this.Errors.HasErrors)
				return false;
			
			BuildRpms ();
			
			return true;
		}

		#endregion // Public interface
		
		#region Properties

		private PackageProperties properties;

		public PackageProperties Properties {
			get { return this.properties; }
		}

		private CompilerErrorCollection errors = new CompilerErrorCollection ();
		
		public CompilerErrorCollection Errors {
			get { return errors; }
		}


		private string root;
		
		public string Root {
			get { return this.root; }
			set { this.root = value; }
		}

		#endregion // Properties
		
		#region Explicit ITextTemplatingEngineHost implementation
		
		public virtual object GetHostOption (string optionName)
		{
			return null;
		}
		
		public virtual AppDomain ProvideTemplatingAppDomain (string content)
		{
			return null;
		}
		
		public bool LoadIncludeText (string filename, out string content, out string location)
		{
			content = System.String.Empty;
			location = System.String.Empty;
		
			if (File.Exists (filename)) {
				content = File.ReadAllText (filename);
				return true;
			} else {
				AddError ("Could not read included file '" + location + "'");
				return false;
			}
		}
		
		public void LogErrors (CompilerErrorCollection errors)
		{
			this.errors.AddRange (errors);
		}
		
		public string ResolveAssemblyReference (string assembly_reference)
		{
			return assembly_reference;
		}
		
		public Type ResolveDirectiveProcessor (string processor_name)
		{
			throw new NotImplementedException ();
		}
		
		public string ResolveParameterValue (string id, string processor, string parameter)
		{
			throw new NotImplementedException ();
		}
		
		public string ResolvePath (string path)
		{
			throw new NotImplementedException ();
		}
		
		public void SetFileExtension (string extension)
		{
			output_file = Path.ChangeExtension (output_file, extension);
		}
		
		public void SetOutputEncoding (System.Text.Encoding encoding, bool from_output)
		{
			this.file_encoding = encoding;
		}

		private string template_file;

		public string TemplateFile {
			get { return template_file; }
		}
		
        private Encoding file_encoding = Encoding.UTF8;

        public Encoding FileEncoding {
            get { return file_encoding; }
        }

		public IList<string> StandardAssemblyReferences {
			get {
				return new string[] {
					typeof (PackageBuilder).Assembly.Location,
					typeof (TextTransformation).Assembly.Location,
					typeof (Path).Assembly.Location
				};
			}
		}
		
		public IList<string> StandardImports {
			get {
				return new string[] {
					typeof (string).Namespace,
					typeof (PackageBuilder).Namespace,
					typeof (TextTransformation).Namespace,
					typeof (Path).Namespace
				};
			}
		}

		#endregion

	}
}
