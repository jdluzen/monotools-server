using Gtk;
using System;
using System.Globalization;
using System.Diagnostics;

public class LoggerListener : TraceListener
{
	LogViewerDialog dialog;
	TextBuffer buffer = new TextBuffer (new TextTagTable ());

	public TextBuffer Buffer {
		get { return buffer; }
	}

	public override void Write (string text)
	{
		// To prevent empty lines
		if ((text == null) || (text.Trim ().Trim ('\n').Length == 0))
			return;

		string timestamp = DateTime.Now.ToString (
			"hh:mm:ss:fffffff", CultureInfo.InvariantCulture);

		Gtk.Application.Invoke(
			delegate {
				TextIter end = buffer.EndIter;
				buffer.Insert (ref end, String.Format ("[{0}] {1}", timestamp, text));
				buffer.PlaceCursor (end);
			}
		);
	}

	public override void WriteLine (string text)
	{
		Write (text + "\n");
	}

	public void ShowDialog ()
	{
		if (dialog != null)
			return;

		dialog = new LogViewerDialog (buffer);
		dialog.Response += delegate (object o, ResponseArgs args) {
			dialog.Destroy ();
			dialog = null;
		};

		dialog.Show ();
	}
}
