using System;
using GitSharp.Hash;
using GitSharp.Reference;

namespace GitSharp.Commands {
	/// <summary>
	/// Represents "git branch [branch_name]" command.
	/// </summary>
	/// Currently supports just creating of new branches.
	/// 
	/// TODO: Does not check whether given branch_name exists.
	internal class BranchCommand : Command {
		private string _branchName;
		private string[] _args;
		
		public BranchCommand(string[] args)
		{
			_args = args;
		}
		
		public override void Process()
		{
			if (!ProcessArgs()) {
				return;
			}
			
			HashKey currentCommitKey = ReferenceDatabase.GetHead().GetCommitKey();
			ReferenceDatabase.CreateBranch(_branchName, currentCommitKey);
			
			PrintSuccess();
		}

		private void PrintHelp()
		{
			Console.WriteLine("usage: \"branch <name>\"");
		}

		private void PrintSuccess()
		{
			Console.WriteLine($"Created branch '{_branchName}'");
		}

		private bool ProcessArgs()
		{
			if (_args.Length != 1) {
				PrintHelp();
				return false;
			}
			_branchName = _args[0];
			return true;
		}
	}
}