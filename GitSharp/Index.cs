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
	        = Traverser.GetRootDirPath() + Path.DirectorySeparatorChar + Traverser.GitRootDirName
	          + Path.DirectorySeparatorChar + "index";

		private const string RemovedFileKey = "-";
		private static Dictionary<RelativePath, Entry> _entries = new Dictionary<RelativePath, Entry>();
		
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
		public static string GetFileBlobKey(RelativePath filePath)
		{
			return GetWdirFileContentKey(filePath);
		}
		
		public static string GetStageFileContentKey(RelativePath filePath)
		{
			Debug.Assert(_entries.ContainsKey(filePath), "file has to be in index");
			return _entries[filePath].StageKey;
		}

		/// <summary>
		/// Returns all the files that are in the index ie. all tracked files.
		/// </summary>
		/// <returns></returns>
		public static IEnumerable<string> GetAllTrackedFiles()
		{
			List<string> fileNames = new List<string>();
			foreach (RelativePath path in _entries.Keys) {
				fileNames.Add(path.GetRelativeToGitRoot());
			}
			return fileNames;
		}
		
		public static List<string> GetStagedFiles()
		{
			return null; // TODO:
		}

		public static bool IsModified(RelativePath filePath)
		{
			return GetWdirFileContentKey(filePath) != GetStageFileContentKey(filePath) &&
			       GetWdirFileContentKey(filePath) != Entry.KeyNullValue;
		}
		
		public static bool IsStaged(RelativePath filePath)
		{
			return GetWdirFileContentKey(filePath) == GetStageFileContentKey(filePath) &&
			       GetStageFileContentKey(filePath) != GetRepoFileContentKey(filePath) &&
			       GetWdirFileContentKey(filePath) != Entry.KeyNullValue &&
			       GetStageFileContentKey(filePath) != Entry.KeyNullValue;
		}

		public static bool IsCommited(RelativePath filePath)
		{
			return GetWdirFileContentKey(filePath) == GetStageFileContentKey(filePath) &&
			       GetStageFileContentKey(filePath) == GetRepoFileContentKey(filePath) &&
			       GetWdirFileContentKey(filePath) != Entry.KeyNullValue &&
			       GetStageFileContentKey(filePath) != Entry.KeyNullValue &&
			       GetRepoFileContentKey(filePath) != Entry.KeyNullValue;
		}

		/// <summary>
		/// Supposes that index is updated (at least for given file)
		/// </summary>
		/// <param name="fileName"></param>
		/// <returns></returns>
		public static File.StatusType ResolveFileStatus(RelativePath filePath)
		{
			if (!ContainsFile(filePath)) {
				return File.StatusType.Untracked;
			}

            if (IsCommited(filePath)) {
                return File.StatusType.Commited;
            }
            if (IsStaged(filePath)) {
                return File.StatusType.Staged;
            }
			if (IsDeletedInWdir(filePath)) {
				return File.StatusType.Deleted;
			}
			if (IsModified(filePath)) {
				return File.StatusType.Modified;
			}
			
			return File.StatusType.Ignored;
		}

		/// <summary>
		/// When index contains file, the file is considered to be tracked.
		/// </summary>
		/// <param name="fileName"></param>
		/// <returns></returns>
		public static bool ContainsFile(RelativePath filePath)
		{
			return _entries.ContainsKey(filePath);
		}
		
		public static void StageFile(RelativePath filePath)
		{
			SetStageFileContentKey(filePath, GetWdirFileContentKey(filePath));
		}

		public static void CommitFile(RelativePath filePath)
		{
			SetRepoFileContentKey(filePath, GetStageFileContentKey(filePath));
		}

		/// <summary>
		/// Adds given file into the Index ie. starts tracking this file.
		/// </summary>
		/// <param name="fileName"></param>
		public static void StartTrackingFile(RelativePath filePath)
		{
			Debug.Assert(!_entries.ContainsKey(filePath), "File can be added into index just once");
			
			Entry entry = new Entry(filePath);
			_entries.Add(filePath, entry);
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
		public static void UpdateFileInWdir(RelativePath filePath)
		{
			Debug.Assert(_entries.ContainsKey(filePath));
			UpdateEntryIfNecessary(_entries[filePath]);
		}
		
		private static void UpdateEntryIfNecessary(Entry entry)
		{
			if (!System.IO.File.Exists(entry.FilePath.GetAbsolutePath())) {
				entry.WdirKey = RemovedFileKey;
				return;
			}
			
			Blob blob = new Blob(entry.FilePath);
			string oldKey = entry.WdirKey;
			string newKey = blob.GetChecksum().ToString();
			if (oldKey != newKey) {
				entry.WdirKey = newKey;
			}
		}
		
		private static bool IsDeletedInWdir(RelativePath filePath)
		{
			return _entries[filePath].WdirKey == RemovedFileKey;
		}
		
		private static ICollection<Entry> GetEntries()
		{
			return _entries.Values;
		}

		private static void AddEntry(Entry entry)
		{
			if (_entries.ContainsKey(entry.FilePath)) {
				throw new Exception("index format error: file already specified in index");
			}
			
			_entries.Add(entry.FilePath, entry);
		}
		
		private static string GetWdirFileContentKey(RelativePath filePath)
		{
			Debug.Assert(_entries.ContainsKey(filePath), "file has to be in index");
			string wdirContentKey = _entries[filePath].WdirKey;
			Debug.Assert(wdirContentKey != Entry.KeyNullValue, "wdir file content must be first set");
			return wdirContentKey;
		}
		
		/// <summary>
		/// 
		/// </summary>
		/// <param name="fileName"></param>
		/// <returns>
		/// null when file was never commited.
		/// </returns>
		private static string GetRepoFileContentKey(RelativePath filePath)
		{
			Debug.Assert(_entries.ContainsKey(filePath), "file has to be in index");
			return _entries[filePath].RepoKey;
		}
		
		private static void SetStageFileContentKey(RelativePath filePath, string key)
		{
			Debug.Assert(_entries.ContainsKey(filePath), "file has to be in index");
			_entries[filePath].StageKey = key;
		}
		
		private static void SetRepoFileContentKey(RelativePath filePath, string key)
		{
			Debug.Assert(_entries.ContainsKey(filePath), "file has to be in index");
			_entries[filePath].RepoKey = key;
		}
		
		private static void SetWdirFileContentKey(RelativePath filePath, string key)
		{
			Debug.Assert(_entries.ContainsKey(filePath), "file has to be in index");
			_entries[filePath].WdirKey = key;
		}

		private class Entry {
			public const string KeyNullValue = "0";
			
			public static Entry ParseFromLine(string line)
			{
				string[] lineItems = line.Split(new char[] {' '}, StringSplitOptions.RemoveEmptyEntries);
				if (lineItems.Length != 4) {
					return null;
				}
				return new Entry(new RelativePath(lineItems[0]), lineItems[1], lineItems[2], lineItems[3]);
			}

			public static string PrintToLine(Entry entry)
			{
				StringBuilder lineBuilder = new StringBuilder();
				lineBuilder.Append(entry.FilePath.GetRelativeToGitRoot());
				lineBuilder.Append(" ");
				lineBuilder.Append(entry.WdirKey);
				lineBuilder.Append(" ");
				lineBuilder.Append(entry.StageKey);
				lineBuilder.Append(" ");
				lineBuilder.Append(entry.RepoKey);
				lineBuilder.Append(" ");
				return lineBuilder.ToString();
			}

			public Entry(RelativePath filePath) : this(filePath, KeyNullValue, KeyNullValue, KeyNullValue)
			{
			}

			private Entry(RelativePath filePath, string wdirKey, string stageKey, string repoKey)
			{
				FilePath = filePath;
				WdirKey = wdirKey;
				StageKey = stageKey;
				RepoKey = repoKey;
			}
			
			public RelativePath FilePath { get; set; }
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