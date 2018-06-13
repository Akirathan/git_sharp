using System;
using GitSharp.Commands;
using GitSharp.Reference;

namespace GitSharp {
	internal class Program {
		public static void Main(string[] args)
		{
			GitMain(args);
		}

		private static void GitMain(string[] args)
		{
			Command command = Cli.ParseCommand(args);
			if (command == null) {
				PrintHelp();
				Environment.Exit(1);
			}

			if (!(command is InitCommand) && !IsRepositoryInitialized()) {
				Console.Error.WriteLine("No git repository initialized, exiting...");
				Environment.Exit(1);
			}
			
			command.Process();
			
			Index.Dispose();
			ReferenceDatabase.Dispose();
		}

		private static void PrintHelp()
		{
			Console.WriteLine("Unrecognized command.");
			Console.WriteLine("usage: \"git (init|status|add|log)\"");
		}

		private static bool IsRepositoryInitialized()
		{
			return Traverser.GetRootDirPath() != null;
		}
	}
}