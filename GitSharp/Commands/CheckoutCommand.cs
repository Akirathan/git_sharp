using System;
using GitSharp.Reference;

namespace GitSharp.Commands {
	/// <summary>
	/// Represents "git checkout [branch_name]" command with no options.
	/// </summary>
	internal class CheckoutCommand : Command {
		private string[] _args;
		private Branch _branch;
		
		public CheckoutCommand(string[] args)
		{
			_args = args;
		}
		
		public override void Process()
		{
			if (!ProcessArgs()) {
				return;
			}

			Index.Update();

			if (!_branch.LoadCommit().Checkout()) {
				PrintError();
				return;
			}
			
			ReferenceDatabase.SetHead(_branch);
			PrintSuccess();
		}

		private void PrintHelp()
		{
			Console.WriteLine("usage: \"checkout <branch>\"");
		}

		private void PrintError()
		{
			Console.WriteLine("Some of local changes would be modified by the checkout, exiting ...");
		}

		private void PrintSuccess()
		{
			Console.WriteLine($"{_branch.Name} checked out");
		}

		private bool ProcessArgs()
		{
			if (_args.Length != 1) {
				PrintHelp();
				return false;
			}

			_branch = ReferenceDatabase.GetBranch(_args[0]);
			if (_branch == null) {
				Console.WriteLine($"{_args[0]} does not specify any branch name");
				return false;
			}

			return true;
		}
	}
}