using System;
using Gtk;

public partial class MainWindow: Gtk.Window
{	
	public MainWindow (): base (Gtk.WindowType.Toplevel)
	{
		Build ();
	}

	protected void OnDeleteEvent (object sender, DeleteEventArgs a)
	{
		Application.Quit ();
		a.RetVal = true;
	}




	protected void OnLoadTilesetButtonClicked (object sender, EventArgs e)
	{

		Gtk.FileChooserDialog fc=
		new Gtk.FileChooserDialog("Choose the file to open",
		                            this,
		                            FileChooserAction.Open,
		                            "Cancel",ResponseType.Cancel,
		                            "Open",ResponseType.Accept);

		if (fc.Run() == (int)ResponseType.Accept) 
		{
			System.IO.FileStream file=System.IO.File.OpenRead(fc.Filename);
			Building_Designer.MainClass mClass = new Building_Designer.MainClass ();
			mClass.LoadTiles(fc.Filename, TileWindow);
			//mClass.LoadTiles (fc.Filename, scroll);
			file.Close();
		}
		//Don't forget to call Destroy() or the FileChooserDialog window won't get closed.
		fc.Destroy();
	}
}
