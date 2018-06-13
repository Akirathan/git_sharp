﻿using System;
using GitSharp.Hash;
using GitSharp.Reference;

namespace GitSharp.Commands {
	internal class BranchCommand : Command {
		private string _branchName;
		private string[] _args;
		
		public BranchCommand(string[] args)
		{
			_args = args;
		}
		
		public override void Process()
		{
			HashKey currentCommitKey = ReferenceDatabase.GetHead().GetCommitKey();
			Branch branch = ReferenceDatabase.CreateBranch(_branchName, currentCommitKey);
			ReferenceDatabase.SetHead(branch);
			
			PrintSuccess();
		}

		private void PrintHelp()
		{
			Console.WriteLine("usage: \"branch <name>\"");
		}

		private void PrintSuccess()
		{
			Console.WriteLine($"Switched to branch '{_branchName}'");
		}

		private bool ProcessArgs()
		{
			if (_args.Length != 1) {
				return false;
			}
			_branchName = _args[0];
			return true;
		}
	}
}