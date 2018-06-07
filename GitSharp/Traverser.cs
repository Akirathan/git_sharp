using System.Collections.Generic;
using System.IO;

namespace GitSharp {
	public static class Traverser {
		private const string GitRootDirName = ".git_sharp";
		
		private static string _rootDirPath;

		/// <summary>
		/// Returns absolute path to the directory, where .git is stored.
		/// Note that this method allows git to be invokend in some subdirectory.
		/// </summary>
		/// <returns>
		/// null when there is no .git in all parents.
		/// </returns>
		public static string GetRootDirPath()
		{
			if (_rootDirPath != null) {
				return _rootDirPath;
			}

			string enumeratedDir = Directory.GetCurrentDirectory();
			while (!DirContainsGitDir(enumeratedDir)) {
				DirectoryInfo currDirInfo = Directory.GetParent(enumeratedDir);
				if (currDirInfo == null) {
					return null;
				}
				enumeratedDir = currDirInfo.FullName;
			}
			_rootDirPath = enumeratedDir;
			return enumeratedDir;
		}
		
		/// <summary>
		/// Returns all file relative names (tracked, untracked and ignored).
		/// Note that it is not necessary to return absolute paths.
		/// </summary>
		/// <returns></returns>
		public static IEnumerable<string> GetAllFiles()
		{
			List<string> files = new List<string>();
			
			files.AddRange(Directory.EnumerateFiles(GetRootDirPath()));
			
			IEnumerable<string> dirNames = Directory.EnumerateDirectories(GetRootDirPath());
			foreach (string dirName in dirNames) {
				if (dirName.Contains(GitRootDirName)) {
					continue;
				}
				IEnumerable<string> dirFileNames =
					Directory.EnumerateFiles(dirName, "*", SearchOption.AllDirectories);
				
				files.AddRange(dirFileNames);
			}
			return ConvertPathsToRelative(files);
		}

		private static bool DirContainsGitDir(string dirPath)
		{
			IEnumerable<string> directories = Directory.EnumerateDirectories(dirPath);
			foreach (string directory in directories) {
				if (directory.Contains(GitRootDirName)) {
					return true;
				}
			}
			return false;
		}

		private static IEnumerable<string> ConvertPathsToRelative(IEnumerable<string> paths)
		{
			List<string> list = new List<string>();
			
			foreach (string path in paths) {
				if (Path.IsPathRooted(path)) {
					list.Add(ConvertPathToRelative(path));
				}
				else {
					list.Add(path);
				}
			}
			return list;
		}

		private static string ConvertPathToRelative(string path)
		{
			return path.Substring(GetRootDirPath().Length + 1);
		}
	}
}