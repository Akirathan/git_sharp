namespace GitSharp {
	internal static class FileTracker {
		public static bool IsModified(File file)
		{
			return false;
		}

		public static bool IsStaged(File file)
		{
			return false;
		}

		/// <summary>
		/// Adds given file into staging area.
		/// Also creates blob object for that file (this means that snapshot of current
		/// version of file is taken).
		/// </summary>
		/// <param name="file"></param>
		public static void StageFile(File file)
		{
			
		}

		/// <summary>
		/// Returns all modified Files.
		/// This method is important for 'git status'.
		/// </summary>
		/// <returns></returns>
		public static File[] GetModifiedFiles()
		{
			return null;
		}

		public static File[] GetUntrackedFiles()
		{
			return null;
		}

		public static File[] GetStagedFiles()
		{
			return null;
		}
	}
}