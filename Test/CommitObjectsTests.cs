﻿using System;
using System.IO;
using GitSharp;
using GitSharp.Commands;
using GitSharp.Hash;
using GitSharp.Objects;
using Xunit;

namespace Test {
	public class CommitObjectsTests {

		public CommitObjectsTests()
		{
			if (Directory.Exists(".git_sharp")) {
                Directory.Delete(".git_sharp", recursive: true);
			}
			new InitCommand().Process();
		}
		
		[Fact]
		public void SimpleTest()
		{
			CreateFile("a.txt", "some content");
			CreateFile("b.txt", "other content");
			
			Blob blobA = new Blob(new RelativePath("a.txt"));
			Blob blobB = new Blob(new RelativePath("b.txt"));

			HashKey commitKey = CreateAndStoreSimpleCommit(new Blob[]{blobA, blobB});
			Commit commit = ObjectDatabase.RetrieveCommit(commitKey);

			Tree tree = commit.LoadTree();
			Blob retrievedBlobA = tree.FindAndLoadBlob("a.txt");
			Blob retrievedBlobB = tree.FindAndLoadBlob("b.txt");
			
			Assert.Equal(blobA, retrievedBlobA);
			Assert.Equal(blobB, retrievedBlobB);
		}

		[Fact]
		public void MoreDirectoriesTest()
		{
			CreateFile("a.txt", "some content");
			Directory.CreateDirectory("dir");
			CreateFile("dir/b.txt", "other content");
			
			Blob blobA = new Blob(new RelativePath("a.txt"));
			Blob blobB = new Blob(new RelativePath("dir/b.txt"));

			HashKey commitKey = CreateAndStoreSimpleCommit(new Blob[]{blobA, blobB});
			Commit commit = ObjectDatabase.RetrieveCommit(commitKey);

			Tree tree = commit.LoadTree();
			Blob retrievedBlobA = tree.FindAndLoadBlob("a.txt");
			Blob retrievedBlobB = tree.FindAndLoadBlob("dir/b.txt");
			
			Assert.Equal(blobA, retrievedBlobA);
			Assert.Equal(blobB, retrievedBlobB);
		}
		
		[Fact]
		public void SkipDirectoryTest()
		{
			CreateFile("a.txt", "some content");
			Directory.CreateDirectory("dir/subdir");
			CreateFile("dir/subdir/b.txt", "other content");
			
			Blob blobA = new Blob(new RelativePath("a.txt"));
			Blob blobB = new Blob(new RelativePath("dir/subdir/b.txt"));

			HashKey commitKey = CreateAndStoreSimpleCommit(new Blob[]{blobA, blobB});
			Commit commit = ObjectDatabase.RetrieveCommit(commitKey);

			Tree tree = commit.LoadTree();
			Blob retrievedBlobA = tree.FindAndLoadBlob("a.txt");
			Blob retrievedBlobB = tree.FindAndLoadBlob("dir/subdir/b.txt");
			
			Assert.Equal(blobA, retrievedBlobA);
			Assert.Equal(blobB, retrievedBlobB);
		}

		private HashKey CreateAndStoreSimpleCommit(Blob[] blobs)
		{
			HashKey[] blobKeys = new HashKey[blobs.Length];

			// blobs are normaly stored in AddCommand
			for (int i = 0; i < blobKeys.Length; i++) {
				blobKeys[i] = ObjectDatabase.Store(blobs[i]);
			}
			
			// this is done in CommitCommand
			TreeBuilder treeBuilder = TreeBuilder.CreateRootTreeBuilder();
			foreach (Blob blob in blobs) {
				treeBuilder.AddBlobToTreeHierarchy(blob);
			}
			
			Commit commit = new Commit(null, treeBuilder.GetChecksum(), "Some commit message");
			return ObjectDatabase.StoreCommitWithTreeHierarchy(commit, treeBuilder);
		}
		
		private void CreateFile(string fileName, string content)
		{
			using (StreamWriter writer = new StreamWriter(fileName)) {
				writer.Write(content);
			}
		}
	}
}