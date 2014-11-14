using System;
using System.Collections.Generic;
using System.IO;
using Gtk;
using Newtonsoft.Json;

namespace Building_Designer
{
	class MainClass
	{
		public static void Main (string[] args)
		{
			Application.Init ();
			MainWindow win = new MainWindow();
			win.ShowAll ();
			Application.Run ();

		}
#region MyRegion

		public void CreateTileGrid(Gtk.Container window, Gtk.Image image)
		{

			Gdk.Pixbuf tileSet = image.Pixbuf, scaled = image.Pixbuf; // gets the pixbuf of the tileset image, scaled is a temporary solution
			scaled = scaled.ScaleSimple (32, 32, Gdk.InterpType.Bilinear); // scaled should have the dimension of each tile

			myListStore myListstore = new myListStore (window);

			//adds columns to the Liststore
			myListstore.addTVColumn ("id", "text", 0);
			myListstore.addTVColumn ("number", "text", 1); //should be something more logic
			myListstore.addTVColumn ("stuff", "text", 2);  //should be something more logic than stuff
			myListstore.addTVColumn ("tile", "pixbuf", 3);

			new JsonClass (window, image, myListstore);

			window.ShowAll ();
		}
		#endregion

		public void DrawTiles()
		{
			
		}


		public void LoadTiles(string path, Gtk.Container window)
		{
			//loads the tileset and calls the CreateTileGrid function
			Gtk.Image image = new Gtk.Image(path);
			CreateTileGrid (window, image);


		}
	}
}
