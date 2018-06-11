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
			if (ReferenceEquals(null, other)) return false;
			if (ReferenceEquals(this, other)) return true;
			return string.Equals(_path, other._path);
		}

		public override bool Equals(object obj)
		{
			if (ReferenceEquals(null, obj)) return false;
			if (ReferenceEquals(this, obj)) return true;
			if (obj.GetType() != this.GetType()) return false;
			return Equals((RelativePath) obj);
		}

		public override int GetHashCode()
		{
			return _path.GetHashCode();
		}
	}
}