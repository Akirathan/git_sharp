using System.Collections.Generic;
using System.Diagnostics;

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

		private static void AddEntry(Entry entry)
		{
			
		}

		private class Entry {
			public string FileName { get; private set; }
			public string WdirKey { get; private set; }
			public string StageKey { get; private set; }
			public string RepoKey { get; private set; }
		}

		private static class Serializer {
			public static void ReadFromFile(string fileName)
			{
				
			}
		}
	}
}