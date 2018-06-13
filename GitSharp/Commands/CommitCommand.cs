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
			
			List<string> stagedFiles = Index.GetStagedFiles();
			if (stagedFiles.Count == 0) {
				PrintNoStagedFilesError();
				return;
			}

			TreeBuilder treeBuilder = TreeBuilder.CreateRootTreeBuilder();
			
			foreach (string stagedFile in stagedFiles) {
				Blob blob = GetStagedFileBlob(stagedFile);
				treeBuilder.AddBlobToTreeHierarchy(blob);
				Index.CommitFile(new RelativePath(stagedFile));
			}

			Commit commit = CreateCommit(treeBuilder);
			StoreCommitAndAdvanceHeadBranch(commit, treeBuilder);
		}

		private Commit CreateCommit(TreeBuilder treeBuilder)
		{
			Commit commit = new Commit(
				parentKey: ReferenceDatabase.GetHead().GetCommitKey(),
				treeKey: treeBuilder.GetChecksum(),
				message: _message
			);
			return commit;
		}

		private void StoreCommitAndAdvanceHeadBranch(Commit commit, TreeBuilder treeBuilder)
		{
			Branch currentBranch = ReferenceDatabase.GetHead();
			HashKey newCommitKey = ObjectDatabase.StoreCommitWithTreeHierarchy(commit, treeBuilder);
			currentBranch.SetCommitKey(newCommitKey);
		}

		private Blob GetStagedFileBlob(string stagedFileName)
		{
            string stagedFileKey = Index.GetStageFileContentKey(new RelativePath(stagedFileName));
            return ObjectDatabase.RetrieveBlob(HashKey.ParseFromString(stagedFileKey));
		}

		private void PrintHelp()
		{
			Console.WriteLine("usage: \"git commit <message>\"");
		}
		
		private void PrintNoStagedFilesError()
		{
			Console.WriteLine("No staged files for commit");
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