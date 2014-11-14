using System;
using System.Collections.Generic;
using System.IO;
using Gtk;
using Newtonsoft.Json;

namespace Building_Designer
{
	public class JsonClass
	{

		// This is the Class that the Jsondeserializer writes to. Each tile is an instance of this class
		public class Json_tile_class
		{
			public string id { get; set;}
			public int? fg { get; set;}
			public int? bg { get; set;}
			public bool rotates { get; set;}
			public Gdk.Pixbuf fg_image { get; set;}
			public Gdk.Pixbuf bg_image { get; set;}
			// maybe add a merged image eventually
			public bool multitile { get; set;}
			public List<Json_tile_class> additional_tiles;
		}

		//This class creates the lists of tile_info, tiles and the list of 32x32 images that the tileset reader writes to
		public class tile_info_nested
		{
			public List<Dictionary<string, int>> tile_info = new List<Dictionary<string, int>>(); // ToDo: is empty right now
			public List<Json_tile_class> tiles;
			public List<Gdk.Pixbuf> lstPixbuf = new List<Gdk.Pixbuf>();
		}

		// window is the scrolled window; should be named apropriately eventually
		public JsonClass (Gtk.Container window, Gtk.Image image, myListStore myListstore) 
		{
			Gdk.Pixbuf tileSet = image.Pixbuf, scaled = image.Pixbuf;


			int height = tileSet.Height, width = tileSet.Width;
			// a treestore so the tiles with subtiles can be expanded, eventually also categories that are expandable
			Gtk.TreeStore myListStore = new Gtk.TreeStore (typeof(string), typeof(int), typeof(string), typeof(Gdk.Pixbuf));

			tile_info_nested tile_info = new tile_info_nested (); // should have a better name eventually

			// should either be chosen by user or maybe the folder where the tileset resides
			StreamReader streamReader = new StreamReader (@"/home/gervinius/Pragramming/Mono/jsonTest/jsonTest/tile_config.json");

			tile_info_nested deserializedJsonFile = JsonConvert.DeserializeObject<tile_info_nested> (streamReader.ReadToEnd ());

			streamReader.Close ();

			int value, tileWidthHeight = 16;
			if (deserializedJsonFile.tile_info [0].TryGetValue ("height", out value)) {
				tileWidthHeight = value;
				Console.WriteLine (tileWidthHeight);
			}


			scaled = scaled.ScaleSimple (tileWidthHeight, tileWidthHeight, Gdk.InterpType.Bilinear); // temp solution; should look for a more elegant solution
			//this copies the list from the json object list to another list. Could be a better solution someday
			tile_info.tiles = new List<Json_tile_class> (deserializedJsonFile.tiles);

			//checks if tile has subtiles and adds them to a list
			for (int i = 0; i < tile_info.tiles.Count; i++) {
				if (tile_info.tiles [i].multitile == true) {
					//just copy the list with subtiles
					tile_info.tiles [i].additional_tiles = new List<Json_tile_class> (deserializedJsonFile.tiles[i].additional_tiles);
				}
			}

			//reads the tileset image and copies each individual tile to a pixbuf; should be a seperate method eventually
			for (int i = 0; i < height/tileWidthHeight; i++) {
				for (int h = 0; h < width/tileWidthHeight; h++) {
					tile_info.lstPixbuf.Add (scaled.Copy()); //hack to initialize a empty pixbuf at the correct index in the list, there may be a more performant solution
					tileSet.CopyArea(h * tileWidthHeight, i * tileWidthHeight,  tileWidthHeight, tileWidthHeight, 
					                 tile_info.lstPixbuf[i * 16 + h], 0, 0); //copy the pixels that reperesent one tile to the pixbuf at the lists index

				}
			}

			//checks whether the tile has a bg or fg image (or both)
			for (int i = 0; i < tile_info.tiles.Count; i ++) {


				Gdk.Pixbuf myListstoreImage = scaled.Copy(); // initialize a pixbuf with 32x32 pixels;; could be in the class eventually

				if (tile_info.tiles [i].fg != null && tile_info.tiles [i].fg >= 0) { // some persons have used the value -1 to mark a nonexisting fg- or bg-image
					tile_info.tiles[i].fg_image = tile_info.lstPixbuf[tile_info.tiles [i].fg.Value];
					myListstoreImage = tile_info.tiles [i].fg_image;
				}

				else if (tile_info.tiles [i].bg != null && tile_info.tiles [i].bg >= 0) {
					tile_info.tiles[i].bg_image = tile_info.lstPixbuf[tile_info.tiles [i].bg.Value];
					myListstoreImage = tile_info.tiles [i].bg_image;
				}

				else if (tile_info.tiles [i].fg != null && tile_info.tiles [i].fg >= 0 && tile_info.tiles [i].bg != null && tile_info.tiles [i].bg >= 0) {
					tile_info.tiles[i].fg_image = tile_info.lstPixbuf[tile_info.tiles [i].fg.Value];
					tile_info.tiles[i].bg_image = tile_info.lstPixbuf[tile_info.tiles [i].bg.Value];

					// this merges background and foreground
					tile_info.tiles [i].bg_image.Composite(myListstoreImage,0,0,tileWidthHeight,tileWidthHeight,0,0,1,1,Gdk.InterpType.Bilinear,1);
					tile_info.tiles [i].fg_image.Composite(myListstoreImage,0,0,tileWidthHeight,tileWidthHeight,0,0,1,1,Gdk.InterpType.Bilinear,1);
				}

				// if the tile object has additional tiles ...
				if (tile_info.tiles [i].multitile == true) {
					// create an expandable treeiter
					Gtk.TreeIter iter = myListStore.AppendValues (tile_info.tiles[i].id); // category name is the maintiles name, could be changed later
					//append the PARENT TILE to the treestore
					myListStore.AppendValues (iter, tile_info.tiles [i].id, 1, 
					                          "test1", myListstoreImage);

					// look through each subtile and add the foreground or background image to the liststore
					for (int x = 0; x < tile_info.tiles[i].additional_tiles.Count; x ++) {
						if (tile_info.tiles [i].additional_tiles [x].fg != null && tile_info.tiles [i].additional_tiles [x].fg >= 0) {
							tile_info.tiles [i].additional_tiles [x].fg_image = tile_info.lstPixbuf [tile_info.tiles [i].additional_tiles [x].fg.Value];
							myListStore.AppendValues (iter, tile_info.tiles [i].additional_tiles [x].id, "test", 
							                          "test1", tile_info.tiles [i].additional_tiles [x].fg_image);

						} else if (tile_info.tiles [i].additional_tiles [x].bg != null && tile_info.tiles [i].additional_tiles [x].bg >= 0) {
							tile_info.tiles [i].additional_tiles [x].bg_image = tile_info.lstPixbuf [tile_info.tiles [i].additional_tiles [x].bg.Value];
							myListStore.AppendValues (iter, tile_info.tiles [i].additional_tiles [x].id, "test", 
							                          "test1", tile_info.tiles [i].additional_tiles [x].bg_image);
							// ToDo:: merge fg and bg
						} else if (tile_info.tiles [i].additional_tiles [x].fg != null && tile_info.tiles [i].additional_tiles [x].fg >= 0 && tile_info.tiles [i].additional_tiles [x].bg != null && tile_info.tiles [i].additional_tiles [x].bg >= 0) {
							tile_info.tiles [i].additional_tiles [x].fg_image = tile_info.lstPixbuf [tile_info.tiles [i].additional_tiles [x].fg.Value];
							tile_info.tiles [i].additional_tiles [x].bg_image = tile_info.lstPixbuf [tile_info.tiles [i].additional_tiles [x].bg.Value];
							myListStore.AppendValues (tile_info.tiles [i].additional_tiles [x].id, "test", 
							                          "test1", tile_info.tiles [i].additional_tiles [x].fg_image);
						}


					}
				} else {
					// append the tile at index i to the liststore
					myListStore.AppendValues (tile_info.tiles [i].id, "test", 
					                          "test1", myListstoreImage);
				}

			}
			myListstore.returnTV().Model = myListStore;
		}
	}
}

