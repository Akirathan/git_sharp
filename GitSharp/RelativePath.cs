using System;
using System.IO;

namespace GitSharp {
	internal class RelativePath : IEquatable<RelativePath> {
		private string _path;
		private string _absolutePath;
		private string _relativeToGitRootPath;
		
		/// <summary>
		/// 
		/// </summary>
		/// <param name="path">
		/// arbitrary path - absolute, relative to anything, ...
		/// </param>
		public RelativePath(string path)
		{
			_path = path;
			
			if (Path.IsPathRooted(_path)) {
				_absolutePath = _path;
			}
			else {
				_absolutePath = Directory.GetCurrentDirectory() + Path.DirectorySeparatorChar + _path;
			}
			
			string gitRootAbsolute = Traverser.GetRootDirPath();
			_relativeToGitRootPath = _absolutePath.Substring(gitRootAbsolute.Length + 1);
		}

		public string GetRelativeToGitRoot()
		{
			return _relativeToGitRootPath;
		}

		public string GetAbsolutePath()
		{
			return _absolutePath;
		}

		public bool Equals(RelativePath other)
		{
			return _path.Equals(other._path);
		}
	}
}