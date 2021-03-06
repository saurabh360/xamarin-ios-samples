﻿using System;

using WatchKit;
using Foundation;
using System.Collections.Generic;
using System.IO;
using SQLite;
using System.Linq;

namespace WatchTodoExtension
{
	public partial class InterfaceController : WKInterfaceController
	{
		public InterfaceController (IntPtr handle) : base (handle)
		{
		}

		List<TodoItem> data = new List<TodoItem>();
		public TodoItemDatabase Database { get; set; }

		public override void Awake (NSObject context)
		{
			base.Awake (context);

			// Configure interface objects here.
			Console.WriteLine ("{0} awake with context", this);

			var FileManager = new NSFileManager ();
			var appGroupContainer = FileManager.GetContainerUrl ("group.com.conceptdevelopment.WatchTodo");
			var appGroupContainerPath = appGroupContainer.Path;
			Console.WriteLine ("agcpath: " + appGroupContainerPath);


			var sqliteFilename = "TodoSQLite.db3";
			var path = Path.Combine(appGroupContainerPath, sqliteFilename);
			var conn = new SQLiteConnection (path);

			Database = new TodoItemDatabase(conn);
		}

		public override NSObject GetContextForSegue (string segueIdentifier, WKInterfaceTable table, nint rowIndex)
		{
			if (segueIdentifier == "showTodoItem") {
				var nst = data [(int)rowIndex];
				return nst.As();
			}
			return null;
		}


		public override void WillActivate ()
		{
			// This method is called when the watch view controller is about to be visible to the user.
			Console.WriteLine ("{0} will activate", this);

			// reload each view
			data = Database.GetItems ().ToList();


			TodoTable.SetNumberOfRows ((nint)data.Count, "todoRow");

			for (var i = 0; i < data.Count; i++) {

				var elementRow = (TodoRowController)TodoTable.GetRowController (i);

				elementRow.Name.SetText (data [i].Name);

				Console.WriteLine (elementRow.Name + " set " + data [i].Name);
				if (data [i].Done)
					elementRow.DoneImage.SetImage ("done");
				else
					elementRow.DoneImage.SetImage ("notdone");
			}
		}

		public override void DidDeactivate ()
		{
			// This method is called when the watch view controller is no longer visible to the user.
			Console.WriteLine ("{0} did deactivate", this);
		}
	}
}

