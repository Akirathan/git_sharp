﻿using System;
using System.Collections.Generic;
using GitSharp.Hash;
using GitSharp.Objects;

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
				string stagedFileKey = Index.GetStageFileContentKey(stagedFile);
				Blob blob = ObjectDatabase.RetrieveBlob(HashKey.ParseFromString(stagedFileKey));
				treeBuilder.AddBlob(blob);
			}

			Tree tree = treeBuilder.CreateImmutableTree();
			ObjectDatabase.Store(tree);
			
			// TODO: create and save commit object
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