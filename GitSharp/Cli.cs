using System;
using GitSharp.Commands;

namespace GitSharp {
	internal static class Cli {
		private const string InitCommandName = "init";
		private const string StatusCommandName = "status";
		private const string AddCommandName = "add";
		private const string CommitCommandName = "commit";
		private const string LogCommandName = "log";
		
		public static Command ParseCommand(string[] args)
		{
			if (args.Length == 0) {
				return null;
			}
			
			if (args[0] == InitCommandName) {
				return new InitCommand();
			}
			if (args[0] == StatusCommandName) {
				return new StatusCommand();
			}
			if (args[0] == LogCommandName) {
				return new LogCommand();
			}
			if (args[0] == AddCommandName) {
				string[] restOfArgs = new string[args.Length - 1];
				Array.Copy(args, 1, restOfArgs, 0, restOfArgs.Length);
				return new AddCommand(restOfArgs);
			}
			if (args[0] == CommitCommandName) {
				string[] restOfArgs = new string[args.Length - 1];
				Array.Copy(args, 1, restOfArgs, 0, restOfArgs.Length);
				return new CommitCommand(restOfArgs);
			}
			return null;
		}
	}
}