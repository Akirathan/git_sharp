using System.Diagnostics;
using GitSharp.Hash;

namespace GitSharp {
	/// <summary>
	/// Index represents a database that contains (tracked) files as rows and their corresponding
	/// working directory version, stage area version and repository version as columns.
	/// Versions of files are saved as HashKeys ie. references to Blob objects.
	/// </summary>
	internal static class Index {
		/// <summary>
		/// Index should be updated just once during runtime for performance reasons.
		/// Note that calling Update() more than once is considered an error.
		/// </summary>
		public static bool Updated { get; private set; }
		
		/// <summary>
		/// Returns content of file in working directory version.
		/// </summary>
		/// <param name="file"></param>
		/// <returns>Key representing pointer to content of the file</returns>
		public static HashKey GetWdirFileContentKey(File file)
		{
			return null;
		}

		public static HashKey GetStageFileContentKey(File file)
		{
			return null;
		}
		
		public static HashKey GetRepoFileContentKey(File file)
		{
			return null;
		}
		
		/// <summary>
		/// Sets version of the content of given file.
		/// </summary>
		/// <param name="file"></param>
		/// <param name="key">pointer to content of the file</param>
		public static void SetWdirFileContentKey(File file, HashKey key)
		{
			
		}
		
		public static void SetStageFileContentKey(File file, HashKey key)
		{
			
		}
		
		public static void SetRepoFileContentKey(File file, HashKey key)
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
	}
}