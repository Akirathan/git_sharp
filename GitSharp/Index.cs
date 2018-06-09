using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using GitSharp.Commands;
using GitSharp.Objects;

namespace GitSharp {
	/// <summary>
	/// Index represents a database that contains (tracked) files as rows and their corresponding
	/// working directory version, stage area version and repository version as columns.
	/// Versions of files are saved as HashKeys ie. references to Blob objects.
	/// 
	/// Some of the columns may be null ie. 0 (for example when a file was never commited)
	/// 
	/// File is considered as being tracked after first call of 'git add' on that file.
	/// So after this call, that file is added into Index.
	/// 
	/// </summary>
	internal static class Index {
        public static readonly string IndexPath
	        = Traverser.GitRootDirName + Path.DirectorySeparatorChar + "index";

		private const string RemovedFileKey = "-";
		private static Dictionary<string, Entry> _entries = new Dictionary<string, Entry>();
		
		static Index()
		{
			Serializer.ReadFromFile();
		}
		
		public static void Dispose()
		{
			Serializer.WriteToFile();
		}
		
		/// <summary>
		/// Index should be updated just once during runtime for performance reasons.
		/// Note that calling Update() more than once is considered an error.
		/// </summary>
		public static bool Updated { get; private set; }

		/// <summary>
		/// Returns key to blob generated from the given file in working directory version.
		/// Note that returned checksum was generated not only from the content of the
		/// file, but also from some header.
		/// </summary>
		/// <param name="fileName"></param>
		/// <returns>
		/// Key representing pointer to blob generated from the file.
		/// </returns>
		public static string GetFileBlobKey(string fileName)
		{
			return GetWdirFileContentKey(fileName);
		}

		/// <summary>
		/// Returns all the files that are in the index ie. all tracked files.
		/// </summary>
		/// <returns></returns>
		public static IEnumerable<string> GetAllTrackedFiles()
		{
			return _entries.Keys;
		}
		
		public static IEnumerable<string> GetStagedFiles()
		{
			return null; // TODO:
		}

		public static bool IsModified(string fileName)
		{
			return GetWdirFileContentKey(fileName) != GetStageFileContentKey(fileName) &&
			       GetWdirFileContentKey(fileName) != Entry.KeyNullValue;
		}
		
		public static bool IsStaged(string fileName)
		{
			return GetWdirFileContentKey(fileName) == GetStageFileContentKey(fileName) &&
			       GetStageFileContentKey(fileName) != GetRepoFileContentKey(fileName) &&
			       GetWdirFileContentKey(fileName) != Entry.KeyNullValue &&
			       GetStageFileContentKey(fileName) != Entry.KeyNullValue;
		}

		public static bool IsCommited(string fileName)
		{
			return GetWdirFileContentKey(fileName) == GetStageFileContentKey(fileName) &&
			       GetStageFileContentKey(fileName) == GetRepoFileContentKey(fileName) &&
			       GetWdirFileContentKey(fileName) != Entry.KeyNullValue &&
			       GetStageFileContentKey(fileName) != Entry.KeyNullValue &&
			       GetRepoFileContentKey(fileName) != Entry.KeyNullValue;
		}
		
		public static File.StatusType ResolveFileStatus(string fileName)
		{
			if (!ContainsFile(fileName)) {
				return File.StatusType.Untracked;
			}

            if (IsCommited(fileName)) {
                return File.StatusType.Commited;
            }
            if (IsStaged(fileName)) {
                return File.StatusType.Staged;
            }
			if (IsModified(fileName)) {
				return File.StatusType.Modified;
			}
			
			if (ContainsFile(fileName) && !System.IO.File.Exists(fileName)) {
				return File.StatusType.Deleted;
			}
			
			return File.StatusType.Ignored;
		}

		/// <summary>
		/// When index contains file, the file is considered to be tracked.
		/// </summary>
		/// <param name="fileName"></param>
		/// <returns></returns>
		public static bool ContainsFile(string fileName)
		{
			return _entries.ContainsKey(fileName);
		}
		
		public static void StageFile(string fileName)
		{
			SetStageFileContentKey(fileName, GetWdirFileContentKey(fileName));
		}

		public static void CommitFile(string fileName)
		{
			SetRepoFileContentKey(fileName, GetStageFileContentKey(fileName));
		}

		/// <summary>
		/// Adds given file into the Index ie. starts tracking this file.
		/// </summary>
		/// <param name="fileName"></param>
		public static void StartTrackingFile(string fileName)
		{
			Debug.Assert(!_entries.ContainsKey(fileName), "File can be added into index just once");
			
			Entry entry = new Entry(fileName);
			_entries.Add(fileName, entry);
		}
		
		/// <summary>
		/// Updates wdir column for all files.
		/// Note that this is necessary in 'git status' call.
		/// </summary>
		public static void Update()
		{
			Debug.Assert(Updated == false, "Update should be called just once.");
			foreach (Entry entry in _entries.Values) {
				UpdateEntryIfNecessary(entry);
			}
			Updated = true;
		}
		
		/// <summary>
		/// Updates wdir content version of given file
		/// </summary>
		/// <param name="fileName"></param>
		public static void UpdateFileInWdir(string fileName)
		{
			UpdateEntryIfNecessary(_entries[fileName]);
		}
		
		private static void UpdateEntryIfNecessary(Entry entry)
		{
			if (!System.IO.File.Exists(entry.FileName)) {
				entry.WdirKey = RemovedFileKey;
				return;
			}
			
			Blob blob = new Blob(entry.FileName);
			string oldKey = entry.WdirKey;
			string newKey = blob.Checksum.ToString();
			if (oldKey != newKey) {
				entry.WdirKey = newKey;
			}
		}
		
		private static ICollection<Entry> GetEntries()
		{
			return _entries.Values;
		}

		private static void AddEntry(Entry entry)
		{
			if (_entries.ContainsKey(entry.FileName)) {
				throw new Exception("index format error: file already specified in index");
			}
			
			_entries.Add(entry.FileName, entry);
		}
		
		private static string GetWdirFileContentKey(string fileName)
		{
			Debug.Assert(_entries.ContainsKey(fileName), "file has to be in index");
			string wdirContentKey = _entries[fileName].WdirKey;
			Debug.Assert(wdirContentKey != Entry.KeyNullValue, "wdir file content must be first set");
			return wdirContentKey;
		}
		
		private static string GetStageFileContentKey(string fileName)
		{
			Debug.Assert(_entries.ContainsKey(fileName), "file has to be in index");
			return _entries[fileName].StageKey;
		}
		
		/// <summary>
		/// 
		/// </summary>
		/// <param name="fileName"></param>
		/// <returns>
		/// null when file was never commited.
		/// </returns>
		private static string GetRepoFileContentKey(string fileName)
		{
			Debug.Assert(_entries.ContainsKey(fileName), "file has to be in index");
			return _entries[fileName].RepoKey;
		}
		
		private static void SetStageFileContentKey(string fileName, string key)
		{
			Debug.Assert(_entries.ContainsKey(fileName), "file has to be in index");
			_entries[fileName].StageKey = key;
		}
		
		private static void SetRepoFileContentKey(string fileName, string key)
		{
			Debug.Assert(_entries.ContainsKey(fileName), "file has to be in index");
			_entries[fileName].RepoKey = key;
		}
		
		private static void SetWdirFileContentKey(string fileName, string key)
		{
			Debug.Assert(_entries.ContainsKey(fileName), "file has to be in index");
			_entries[fileName].WdirKey = key;
		}

		private class Entry {
			public const string KeyNullValue = "0";
			
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

			public Entry(string fileName) : this(fileName, KeyNullValue, KeyNullValue, KeyNullValue)
			{
			}

			public Entry(string fileName, string wdirKey, string stageKey, string repoKey)
			{
				FileName = fileName;
				WdirKey = wdirKey;
				StageKey = stageKey;
				RepoKey = repoKey;
			}
			
			public string FileName { get; set; }
			public string WdirKey { get; set; }
			public string StageKey { get; set; }
			public string RepoKey { get; set; }
		}

		private static class Serializer {
			
			/// <summary>
			/// Fills all the entries of Index.
			/// </summary>
			public static void ReadFromFile()
			{
				if (InitCommand.IsInitializing) {
					return;
				}
				
				using (StreamReader reader = new StreamReader(IndexPath)) {
					for (string line = reader.ReadLine(); line != null; line = reader.ReadLine()) {
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