using System;
using System.IO;
using System.Collections.Generic;
using GitSharp.Hash;
using GitSharp.Objects;

namespace GitSharp.Commands {
	internal class AddCommand : Command {
		private string[] _args;
		private IList<RelativePath> _encounteredIgnoredFiles = new List<RelativePath>();
		private IList<RelativePath> _files = new List<RelativePath>();
		private ISet<Option> _options = new HashSet<Option>();

		private enum Option {
			Force
		}

		public static void PrintHelp()
		{
			Console.WriteLine("Usage: \"git add [-f] <file|directory>\"");
		}
		
		public AddCommand(string[] args)
		{
			_args = args;
		}
		
		public override void Process()
		{
			if (!ProcessArguments()) {
				return;
			}

			foreach (RelativePath file in _files) {
				AddFile(file);
			}
			
			PrintEncounteredIgnoredFilesWarning();
		}

		private bool ProcessArguments()
		{
			foreach (string arg in _args) {
				if (IsArgument(arg)) {
					if (IsFile(arg)) {
						_files.Add(new RelativePath(arg));
					}
					else if (IsDirectory(arg)) {
						if (!ProcessDirectoryArgument(arg)) {
							return false;
						}
					}
					else if (FileDeletedFromWdir(arg)) {
						_files.Add(new RelativePath(arg));
					}
					else {
						PrintNonExistingFileError(arg);
						return false;
					}
				}
				else if (IsOption(arg)) {
					if (!ProcessOptionArgument(arg)) {
						PrintHelp();
					}
				}
			}
			return true;
		}
		
		private bool ProcessOptionArgument(string option)
		{
			if (option == "-f") {
				_options.Add(Option.Force);
				return true;
			}
			return false;
		}

		private bool ProcessDirectoryArgument(string dirPath)
		{
			IEnumerable<string> files = Directory.EnumerateFiles(dirPath, "*", SearchOption.AllDirectories);
			foreach (string file in files) {
				if (IsFile(file)) {
					_files.Add(new RelativePath(file));
				}
				else {
					PrintNonExistingFileError(file);
					return false;
				}
			}
			return true;
		}
		
		private void PrintNonExistingFileError(string nonExistingFile)
		{
			Console.Error.WriteLine($"Following path does not exist: {nonExistingFile}");
			Console.Error.WriteLine("Exiting...");
		}
		
		private void PrintEncounteredIgnoredFilesWarning()
		{
			if (_encounteredIgnoredFiles.Count == 0) {
				return;
			}
			
			Console.WriteLine("Warning: following files are ignored:");
			foreach (RelativePath ignoredFilePath in _encounteredIgnoredFiles) {
				Console.WriteLine($"  {ignoredFilePath.GetRelativeToGitRoot()}");
			}
			Console.WriteLine("specify -f flag if you want to add them");
		}
		
		private bool IsArgument(string s)
		{
			return s[0] != '-';
		}
		
		private bool IsOption(string s)
		{
			return s[0] == '-';
		}

		private bool IsFile(string path)
		{
			return System.IO.File.Exists(path);
		}

		private bool IsDirectory(string path)
		{
			return Directory.Exists(path);
		}
		
		private bool FileDeletedFromWdir(string fileName)
		{
			return Index.ContainsFile(new RelativePath(fileName)) && !System.IO.File.Exists(fileName);
		}

		/// Supposes that fileName is existing file
		private void AddFile(RelativePath filePath)
		{
			if (!Index.ContainsFile(filePath)) {
				Index.StartTrackingFile(filePath);
			}
			
			Index.UpdateFileInWdir(filePath);
			FileStatus fileFileStatus = Index.ResolveFileStatus(filePath);
			
			switch (fileFileStatus) {
				case FileStatus.Untracked:
				case FileStatus.Modified:
					CreateAndStoreBlob(filePath);
					Index.StageFile(filePath);
					break;
				case FileStatus.Staged:
				case FileStatus.Commited:
					break;
                case FileStatus.Deleted:
	                Index.StageFile(filePath);
                    break;
				case FileStatus.Ignored:
					_encounteredIgnoredFiles.Add(filePath);
					break;
				default:
					throw new ArgumentOutOfRangeException();
			}
		}

		private HashKey CreateAndStoreBlob(RelativePath filePath)
		{
			Blob blob = new Blob(filePath);
			return ObjectDatabase.Store(blob);
		}
	}
}