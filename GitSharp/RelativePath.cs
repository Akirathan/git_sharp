using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text;

namespace GitSharp {
	/// <summary>
	/// It is not used to control user input - this is responsibility of Commands.
	/// It is used for purposes of ObjectDatabase and Index ie. for converting
	/// to absolute path so file operations can be done.
	/// </summary>
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
				string cwd = Directory.GetCurrentDirectory();
				if (PathsIntersects(cwd, _path)) {
					_absolutePath = IntersectedPathsJoin(cwd, _path);
				}
				else {
                    _absolutePath = cwd + Path.DirectorySeparatorChar + _path;
				}
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

		private bool PathsIntersects(string pathA, string pathB)
		{
			string[] pathAItems =
				pathA.Split(new char[] {Path.DirectorySeparatorChar}, StringSplitOptions.RemoveEmptyEntries);
			
			string[] pathBItems =
				pathB.Split(new char[] {Path.DirectorySeparatorChar}, StringSplitOptions.RemoveEmptyEntries);

			for (int i = 0; i < pathAItems.Length; i++) {
				if (pathAItems[i] == pathBItems[0]) {
					return true;
				}
			}
			return false;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="pathA">absolute path</param>
		/// <param name="pathB">relative path that intersects with pathA</param>
		/// <returns>joined absolute path</returns>
		private string IntersectedPathsJoin(string pathA, string pathB)
		{
			string[] pathAItems =
				pathA.Split(new char[] {Path.DirectorySeparatorChar}, StringSplitOptions.RemoveEmptyEntries);
			
			string[] pathBItems =
				pathB.Split(new char[] {Path.DirectorySeparatorChar}, StringSplitOptions.RemoveEmptyEntries);

			StringBuilder builder = new StringBuilder();
			builder.Append(Path.DirectorySeparatorChar);
			
			for (int i = 0; pathAItems[i] != pathBItems[0]; i++) {
				builder.Append(pathAItems[i]);
				builder.Append(Path.DirectorySeparatorChar);
			}

			foreach (string pathBItem in pathBItems) {
				builder.Append(pathBItem);
				builder.Append(Path.DirectorySeparatorChar);
			}

			// Remove last DirectorySeparatorChar
			builder.Remove(builder.Length - 1, 1);

			return builder.ToString();
		}
	}
}