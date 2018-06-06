using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;

namespace GitSharp {
	/// <summary>
	/// Index represents a database that contains (tracked) files as rows and their corresponding
	/// working directory version, stage area version and repository version as columns.
	/// Versions of files are saved as HashKeys ie. references to Blob objects.
	/// </summary>
	internal static class Index {
		private static Dictionary<string, Entry> _entries = new Dictionary<string, Entry>();
		
		static Index()
		{
			Serializer.ReadFromFile();
		}
		
		/// <summary>
		/// Index should be updated just once during runtime for performance reasons.
		/// Note that calling Update() more than once is considered an error.
		/// </summary>
		public static bool Updated { get; private set; }
		
		/// <summary>
		/// Returns content of file in working directory version.
		/// </summary>
		/// <param name="fileName"></param>
		/// <returns>Key representing pointer to content of the file</returns>
		public static string GetWdirFileContentKey(string fileName)
		{
			return null;
		}

		public static string GetStageFileContentKey(string fileName)
		{
			return null;
		}
		
		public static string GetRepoFileContentKey(string fileName)
		{
			return null;
		}
		
		/// <summary>
		/// Sets version of the content of given file.
		/// </summary>
		/// <param name="fileName"></param>
		/// <param name="key">pointer to content of the file</param>
		public static void SetWdirFileContentKey(string fileName, string key)
		{
			
		}
		
		public static void SetStageFileContentKey(string fileName, string key)
		{
			
		}
		
		public static void SetRepoFileContentKey(string fileName, string key)
		{
			
		}

		/// <summary>
		/// Updates wdir column for all files.
		/// Note that this is necessary in 'git status' call.
		/// </summary>
		public static void Update()
		{
			Debug.Assert(Updated == false, "Update should be called just once.");
			// ...
			Updated = true;
		}
		
		private static ICollection<Entry> GetEntries()
		{
			return _entries.Values;
		}

		private static void AddEntry(Entry entry)
		{
			
		}

		private class Entry {
			public static Entry ParseFromLine(string line)
			{
				string[] lineItems = line.Split(new char[] {' '}, StringSplitOptions.RemoveEmptyEntries);
				if (lineItems.Length != 4) {
					return null;
				}
				return new Entry(lineItems[0], lineItems[1], lineItems[2], lineItems[3]);
			}

			public static string PrintToLine(Entry entry)
			{
				StringBuilder lineBuilder = new StringBuilder();
				lineBuilder.Append(entry.FileName);
				lineBuilder.Append(" ");
				lineBuilder.Append(entry.WdirKey);
				lineBuilder.Append(" ");
				lineBuilder.Append(entry.StageKey);
				lineBuilder.Append(" ");
				lineBuilder.Append(entry.RepoKey);
				lineBuilder.Append(" ");
				return lineBuilder.ToString();
			}
			
			public Entry(string fileName, string wdirKey, string stageKey, string repoKey)
			{
				FileName = fileName;
				WdirKey = wdirKey;
				StageKey = stageKey;
				RepoKey = repoKey;
			}
			
			public string FileName { get; }
			public string WdirKey { get; }
			public string StageKey { get; }
			public string RepoKey { get; }
		}

		private static class Serializer {
			private const string IndexPath = "index"; // TODO: ROOT_PATH 
			
			/// <summary>
			/// Fills all the entries of Index.
			/// </summary>
			public static void ReadFromFile()
			{
				using (StreamReader reader = new StreamReader(IndexPath)) {
					string line = "";
					while (line != null) {
						line = reader.ReadLine();
						Entry entry = Entry.ParseFromLine(line);
						AddEntry(entry);
					}
				}
			}

			public static void WriteToFile()
			{
				using (StreamWriter writer = new StreamWriter(IndexPath)) {
					ICollection<Entry> entries = GetEntries();
					foreach (Entry entry in entries) {
						writer.WriteLine(Entry.PrintToLine(entry));
					}
				}
			}
		}
	}
}