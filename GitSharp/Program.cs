using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Text;
using GitSharp.Commands;
using GitSharp.Objects;
using GitSharp.Hash;

namespace GitSharp {
	internal class Program {
		public static void Main(string[] args)
		{
			GitMain();
		}

		private static void GitMain()
		{
			if (!IsRepositoryInitialized()) {
				Console.Error.WriteLine("No git repository initialized, exiting...");
				Environment.Exit(1);
			}
			
			//AddCommand addCommand = new AddCommand(new string[]{"prd.txt"});
			//addCommand.Process();
			File.StatusType status = StatusCommand.ResolveFileStatus("prd.txt");
			
			Index.Dispose();
		}

		private static bool IsRepositoryInitialized()
		{
			return Traverser.GetRootDirPath() != null;
		}

		private static bool TestBlobStore()
		{
			HashKey key = ObjectDatabase.Store(new Blob("bla"));
			Blob blob = ObjectDatabase.RetrieveBlob(key);
			return true;
		}

		private static bool TestTreeStore()
		{
			Blob blob = new Blob("content of a.txt");
			HashKey blobKey = ObjectDatabase.Store(blob);
			Tree.BlobEntry blobEntry = new Tree.BlobEntry(blobKey, "a.txt");
			Tree tree = new Tree(new List<Tree.BlobEntry> {blobEntry}, null);

			HashKey treeKey = ObjectDatabase.Store(tree);
			Tree retrievedTree = ObjectDatabase.RetrieveTree(treeKey);
			
			return true;
		}
	}
}