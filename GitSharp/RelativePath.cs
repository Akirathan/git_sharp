namespace GitSharp {
	internal class RelativePath {
		/// <summary>
		/// 
		/// </summary>
		/// <param name="path">
		/// arbitrary path - absolute, relative to anything, ...
		/// </param>
		public RelativePath(string path)
		{
			
		}

		string GetRelativeToGitRoot()
		{
			string gitRootAbsolute = Traverser.GetRootDirPath();
		}

		string GetAbsolutePath()
		{
			return null;
		}
	}
}