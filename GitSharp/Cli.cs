using System;
using GitSharp.Commands;

namespace GitSharp {
	internal static class Cli {
		private const string InitCommandName = "init";
		private const string StatusCommandName = "status";
		private const string AddCommandName = "add";
		
		public static Command ParseCommand(string[] args)
		{
			if (args[0] == InitCommandName) {
				return new InitCommand();
			}
			if (args[0] == StatusCommandName) {
				return new StatusCommand();
			}
			if (args[0] == AddCommandName) {
				string[] restOfArgs = new string[args.Length - 1];
				Array.Copy(args, 1, restOfArgs, 0, restOfArgs.Length);
				return new AddCommand(restOfArgs);
			}
		}
	}
}