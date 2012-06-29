using System;
using System.Reflection;
using Gtk;
using Gdk;

public class LogViewerDialog : Dialog
{
	private TextView view;

	public LogViewerDialog (TextBuffer buffer) : base ()
	{
		this.Title = Mono.Unix.Catalog.GetString ("MonoVS Log Viewer");
		this.Icon = new Pixbuf (Assembly.GetExecutingAssembly (), "monotools.png");
		this.SetDefaultSize (750, 350);
	
		view = new TextView (buffer);
		view.Editable = false;
		view.CursorVisible = true;
		view.Buffer.Changed += OnTextChanged;

		ScrolledWindow sw = new ScrolledWindow ();
		sw.HscrollbarPolicy = PolicyType.Automatic;
		sw.VscrollbarPolicy = PolicyType.Automatic;
		sw.ShadowType = ShadowType.In;
		sw.BorderWidth = 6;
		sw.Add (view);
		VBox.PackStart (sw);

		Button button_clear = new Gtk.Button();
		button_clear.CanDefault = true;
		button_clear.CanFocus = true;
		button_clear.Name = "buttonOk";
		button_clear.UseStock = true;
		button_clear.UseUnderline = true;
		button_clear.Label = "gtk-clear";
		button_clear.Clicked += new System.EventHandler (OnButtonClearClicked);
		ActionArea.Add (button_clear);
		
		Button button_close = new Gtk.Button();
		button_close.CanDefault = true;
		button_close.CanFocus = true;
		button_close.Name = "buttonOk";
		button_close.UseStock = true;
		button_close.UseUnderline = true;
		button_close.Label = "gtk-close";
		AddActionWidget (button_close, ResponseType.Close);

		this.ShowAll ();
	}

	protected virtual void OnButtonClearClicked (object sender, System.EventArgs e)
	{
		view.Buffer.Clear ();
	}

	void OnTextChanged (object sender, EventArgs args)
	{
		view.ScrollToMark (view.Buffer.InsertMark, 0.0, true, 0.0, 1.0);
	}
}