using System;
using System.IO;
using SQLite;
using MessageBoard.iOS;

[assembly: Xamarin.Forms.Dependency (typeof(ISQLite_iOS))]

namespace MessageBoard.iOS
{
	public class ISQLite_iOS : ISQLite
	{
		public ISQLite_iOS()
		{
		}
		public SQLite.SQLiteConnection GetConnection()
		{
			var sqliteFilename = "MessageBoard.db3";
			string documentsPath = Environment.GetFolderPath(Environment.SpecialFolder.Personal); // Documents folder
			string libraryPath = Path.Combine(documentsPath, "..", "Library"); // Library folder
			var path = Path.Combine(libraryPath, sqliteFilename);
			// Create the connection
			var conn = new SQLite.SQLiteConnection(path);
			// Return the database connection
			return conn;
		}
	}
}
