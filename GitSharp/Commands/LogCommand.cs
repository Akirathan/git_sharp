using System;
using System.Collections.Generic;
using GitSharp.Objects;
using GitSharp.Reference;

namespace GitSharp.Commands {
	/// <summary>
	/// Represents "git log" command.
	/// </summary>
	/// Prints all the commits in current branch history
	internal class LogCommand : Command {
		public override void Process()
		{
			Branch headBranch = ReferenceDatabase.GetHead();

			for (Commit commit = headBranch.LoadCommit(); commit != null; commit = commit.LoadParent()) {
				PrintCommit(commit);
			}
		}

		private void PrintCommit(Commit commit)
		{
			Console.WriteLine("==============================");
			Console.WriteLine("commit " + commit.GetChecksum().ToString());
			Console.WriteLine();
			Console.WriteLine(commit.Message);
			Console.WriteLine("Modified files:");
			PrintAllModifiedFiles(commit);
			Console.WriteLine("==============================");
		}

		private void PrintAllModifiedFiles(Commit commit)
		{
			Tree tree = commit.LoadTree();
			List<Blob> blobs = new List<Blob>();
			tree.LoadAndGetAllBlobs(blobs);
			foreach (Blob blob in blobs) {
				Console.WriteLine($"  {blob.FilePath}");
			}
		}
	}
}