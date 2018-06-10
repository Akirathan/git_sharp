﻿using System;
using System.Collections.Generic;
using GitSharp.Hash;
using GitSharp.Objects;
using GitSharp.Reference;

namespace GitSharp.Commands {
	internal class CommitCommand : Command {
		private string[] _args;
		private string _message;
		
		public CommitCommand(string[] args)
		{
			_args = args;
		}
		
		public override void Process()
		{
			if (!ProcessOptions(_args)) {
				PrintHelp();
				return;
			}

			TreeBuilder treeBuilder = new TreeBuilder();
			
			IEnumerable<string> stagedFiles = Index.GetStagedFiles();
			foreach (string stagedFile in stagedFiles) {
				Blob blob = GetStagedFileBlob(stagedFile);
				treeBuilder.AddBlobToTreeHierarchy(blob);
			}

			Tree tree = treeBuilder.CreateImmutableTree();
			Commit commit = CreateCommit(tree);
			StoreCommitAndAdvanceHeadBranch(commit);
		}

		private Commit CreateCommit(Tree tree)
		{
			Commit commit = new Commit(
				parentKey: ReferenceDatabase.GetHead().GetCommitKey(),
				treeKey: tree.GetChecksum(),
				message: _message
			);
			return commit;
		}

		private void StoreCommitAndAdvanceHeadBranch(Commit commit)
		{
			Branch currentBranch = ReferenceDatabase.GetHead();
			HashKey newCommitKey = ObjectDatabase.StoreCommitWithTreeHierarchy(commit);
			currentBranch.SetCommitKey(newCommitKey);
		}

		private Blob GetStagedFileBlob(string stagedFileName)
		{
            string stagedFileKey = Index.GetStageFileContentKey(stagedFileName);
            return ObjectDatabase.RetrieveBlob(HashKey.ParseFromString(stagedFileKey));
		}

		private void PrintHelp()
		{
			Console.WriteLine("usage: \"git commit <message>\"");
		}
		
		private bool ProcessOptions(string[] args)
		{
			if (args.Length != 1) {
				return false;
			}
			_message = args[0];
			return true;
		}
	}
}