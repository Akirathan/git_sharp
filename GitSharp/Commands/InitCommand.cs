using System;
using System.IO;
using GitSharp.Reference;

namespace GitSharp.Commands {
	/// <summary>
	/// Represents "git init" command.
	/// </summary>
	/// Creates all the necessary files and directories.
	/// Note that initializing already initialized repository is an error.
	internal class InitCommand : Command {
		public static bool IsInitializing { get; private set; }
		
		public override void Process()
		{
			IsInitializing = true;
			string rootDirPath = Traverser.GetRootDirPath();
			if (rootDirPath != null) {
				PrintTryingToReinitializeRepo(rootDirPath);
				return;
			}
			
			CreateRootDir();
			CreateAllFilesAndDirectories();
			PrintRepoInitialized();
			IsInitializing = false;
		}

		private void PrintTryingToReinitializeRepo(string rootPath)
		{
			Console.WriteLine($"There is already git direcotry in {rootPath}");
			Console.WriteLine("  If you want to reinitialize, remove it first");
		}

		private void PrintRepoInitialized()
		{
			string repoAbsolutePath = Path.Combine(Directory.GetCurrentDirectory(), Traverser.GitRootDirName);
			Console.WriteLine($"Initialized git repository in {repoAbsolutePath}");
		}

		private void CreateRootDir()
		{
			CreateDirectory(Traverser.GitRootDirName);
		}
		
		private void CreateAllFilesAndDirectories()
		{
			CreateDirectory(ObjectDatabase.DefaultPath);
			CreateFile(Index.IndexPath);
			ReferenceDatabase.Init();
		}

		private void CreateDirectory(string dirName)
		{
			try {
				Directory.CreateDirectory(dirName);
			}
			catch (Exception e) {
				throw new Exception($"Cannot create directory {dirName}", e);
			}
		}

		private void CreateFile(string fileName)
		{
			try {
				File.Create(fileName).Close();
			}
			catch (Exception e) {
				throw new Exception($"Cannot create file {fileName}", e);
			}
		}
	}
}