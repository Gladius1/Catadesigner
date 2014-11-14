using System;
using System.Collections.Generic;
using System.IO;
using Gtk;

namespace Building_Designer
{
	// this class handles all the things assotiated with the treeview and the liststore
	public class myListStore
	{
		TreeView myTreeview = new TreeView();

		// the constructor just initializes the treeview
		public myListStore (Gtk.Container window)
		{
			myTreeview = addTreeview (window);
		}
		// returns the treeview so it can be attached to the liststore
		public TreeView returnTV ()
		{
			return (myTreeview);
		}
		// adds a new column;; option(s) could eventually be a list of options, right now only "text" and "pixbuf"
		public TreeViewColumn addTVColumn(string title, string option, int position)
		{
			TreeViewColumn tVColumn = new TreeViewColumn();
			tVColumn.Title = title;
			Gtk.CellRendererText cRText;
			Gtk.CellRendererPixbuf cRPixbuf;
			//check wether the option is "text" or "pixbuf" and initializes the apropriate CellRenderer
			if (option == "text") {
				cRText = new Gtk.CellRendererText ();
				tVColumn.PackStart (cRText, true);
				tVColumn.AddAttribute (cRText, option, position);
			}
			if (option == "pixbuf") {
				cRPixbuf = new Gtk.CellRendererPixbuf ();
				tVColumn.PackStart (cRPixbuf, true);
				tVColumn.AddAttribute (cRPixbuf, option, position);
			}
			// append the column to the treeview
			myTreeview.AppendColumn(tVColumn);
			return (tVColumn);
		}

		// just adds the treeview to the apropriate container/window
		public TreeView addTreeview (Gtk.Container window)
		{
			window.Add (myTreeview);
			return (myTreeview);
		}
	}
}

