using System;
using GitSharp.Commands;

namespace GitSharp {
	/// <summary>
	/// Represents command line input parser.
	/// </summary>
	internal static class Cli {
		private const string InitCommandName = "init";
		private const string StatusCommandName = "status";
		private const string AddCommandName = "add";
		private const string CommitCommandName = "commit";
		private const string LogCommandName = "log";
		private const string BranchCommandName = "branch";
		private const string CheckoutCommandName = "checkout";
		
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
				return new AddCommand(CutOutFirstArgument(args));
			}
			if (args[0] == CommitCommandName) {
				return new CommitCommand(CutOutFirstArgument(args));
			}
			if (args[0] == BranchCommandName) {
				return new BranchCommand(CutOutFirstArgument(args));
			}
			if (args[0] == CheckoutCommandName) {
				return new CheckoutCommand(CutOutFirstArgument(args));
			}
			return null;
		}

		private static string[] CutOutFirstArgument(string[] args)
		{
			string[] restOfArgs = new string[args.Length - 1];
			
			Array.Copy(sourceArray: args, sourceIndex: 1,
				destinationArray: restOfArgs, destinationIndex: 0,
				length: restOfArgs.Length);
			
			return restOfArgs;
		}
	}
}