using System;
using System.IO;

namespace GitSharp.Commands {
	internal class InitCommand : Command {
		public override void Process()
		{
			string rootDirPath = Traverser.GetRootDirPath();
			if (rootDirPath != null) {
				PrintTryingToReinitializeRepo(rootDirPath);
				return;
			}
			
			CreateRootDir();
			CreateAllFilesDirectories();
			PrintRepoInitialized();
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
		
		private void CreateAllFilesDirectories()
		{
			CreateDirectory(ObjectDatabase.DefaultPath);
			CreateFile(Index.IndexPath);	
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
				System.IO.File.Create(fileName);
			}
			catch (Exception e) {
				throw new Exception($"Cannot create file {fileName}", e);
			}
		}
	}
}