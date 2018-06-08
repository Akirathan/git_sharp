using System;
using System.IO;
using System.Collections.Generic;
using GitSharp.Hash;
using GitSharp.Objects;

namespace GitSharp.Commands {
	internal class AddCommand : Command {
		private string[] _args;
		private IList<string> _encounteredIgnoredFiles = new List<string>();
		private IList<string> _files = new List<string>();
		private ISet<Option> _options = new HashSet<Option>();

		private enum Option {
			Force
		}

		public static void PrintHelp()
		{
			
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

			foreach (string file in _files) {
				AddFile(file);
			}
			
			PrintEncounteredIgnoredFilesWarning();
		}

		private bool ProcessArguments()
		{
			foreach (string arg in _args) {
				if (IsArgument(arg)) {
					if (IsFile(arg)) {
						_files.Add(arg);
					}
					else if (IsDirectory(arg)) {
						if (!ProcessDirectoryArgument(arg)) {
							return false;
						}
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

		private bool IsOption(string s)
		{
			return s[0] == '-';
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
					_files.Add(file);
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
			foreach (string ignoredFile in _encounteredIgnoredFiles) {
				Console.WriteLine($"  {ignoredFile}");
			}
			Console.WriteLine("specify -f flag if you want to add them");
		}
		
		private bool IsArgument(string s)
		{
			return s[0] != '-';
		}

		private bool IsFile(string path)
		{
			return System.IO.File.Exists(path);
		}

		private bool IsDirectory(string path)
		{
			return Directory.Exists(path);
		}

		/// Supposes that fileName is existing file
		private void AddFile(string fileName)
		{
			File.StatusType fileStatus = StatusCommand.ResolveFileStatus(fileName);
			switch (fileStatus) {
				case File.StatusType.Untracked:
				case File.StatusType.Modified:
					HashKey key = CreateAndSaveBlob(fileName);
					Index.UpdateFileContentKey(fileName, key.ToString());
					Index.StageFile(fileName);
					break;
				case File.StatusType.Staged:
				case File.StatusType.Commited:
					break;
				case File.StatusType.Ignored:
					_encounteredIgnoredFiles.Add(fileName);
					break;
				default:
					throw new ArgumentOutOfRangeException();
			}
		}

		private HashKey CreateAndSaveBlob(string fileName)
		{
			HashKey key;
			using (StreamReader reader = new StreamReader(fileName)) {
				string content = reader.ReadToEnd();
				Blob blob = new Blob(content);
				key = ObjectDatabase.Store(blob);
			}
			return key;
		}
	}
}