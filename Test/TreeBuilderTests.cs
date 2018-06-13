using System.Collections.Generic;
using System.IO;
using GitSharp;
using Xunit;
using GitSharp.Objects;
using GitSharp.Commands;
using GitSharp.Hash;

namespace Test {
	public class TreeBuilderTests {

		[Fact]
		public void SimpleTest()
		{
			Directory.Delete(".git_sharp", recursive: true);
			new InitCommand().Process();
			
			CreateFile("a.txt", "some content");
			Directory.CreateDirectory("dir");
			CreateFile("dir/b.txt", "other content");
			
			Blob blobA = CreateBlob("a.txt");
			Blob blobB = CreateBlob("dir/b.txt");
			
			TreeBuilder treeBuilder = TreeBuilder.CreateRootTreeBuilder();
			treeBuilder.AddBlobToTreeHierarchy(blobA);
			treeBuilder.AddBlobToTreeHierarchy(blobB);

			List<Blob> allBlobs = new List<Blob>();
			List<TreeBuilder> allTrees = new List<TreeBuilder>();
			treeBuilder.GetAllBlobsAndSubTrees(allBlobs, allTrees);
			
			Assert.Equal(allBlobs.Count, 2);
			Assert.Equal(allTrees.Count, 2);
		}
		
		[Fact]
		public void SkipDirTest()
		{
			Directory.Delete(".git_sharp", recursive: true);
			new InitCommand().Process();
			
			CreateFile("a.txt", "some content");
			Directory.CreateDirectory("dir/subdir");
			CreateFile("dir/subdir/b.txt", "other content");
			
			Blob blobA = CreateBlob("a.txt");
			Blob blobB = CreateBlob("dir/subdir/b.txt");
			
			TreeBuilder treeBuilder = TreeBuilder.CreateRootTreeBuilder();
			treeBuilder.AddBlobToTreeHierarchy(blobA);
			treeBuilder.AddBlobToTreeHierarchy(blobB);

			List<Blob> allBlobs = new List<Blob>();
			List<TreeBuilder> allTrees = new List<TreeBuilder>();
			treeBuilder.GetAllBlobsAndSubTrees(allBlobs, allTrees);
			
			Assert.Equal(allBlobs.Count, 2);
			Assert.Equal(allTrees.Count, 3);
		}

		/// Creates Blobs, TreeBuilder and store those into ObjectDatabase.
		/// Then retrieves Tree from the database and checks whether that
		/// tree corresponds to treebuilder.
		[Fact]
		public void SimpleStoreTest()
		{
			Directory.Delete(".git_sharp", recursive: true);
			new InitCommand().Process();
			
			CreateFile("a.txt", "some content");
			CreateFile("b.txt", "other content");
			
			Blob blobA = CreateBlob("a.txt");
			Blob blobB = CreateBlob("b.txt");

			HashKey keyBlobA = ObjectDatabase.Store(blobA);
			HashKey keyBlobB = ObjectDatabase.Store(blobB);
			
			TreeBuilder treeBuilder = TreeBuilder.CreateRootTreeBuilder();
			treeBuilder.AddBlobToTreeHierarchy(blobA);
			treeBuilder.AddBlobToTreeHierarchy(blobB);

			HashKey keyTreeBuilder = ObjectDatabase.Store(treeBuilder);

			Tree retrievedTree = ObjectDatabase.RetrieveTree(keyTreeBuilder);
			
			Assert.Equal(treeBuilder.GetChecksum(), retrievedTree.GetChecksum());
		}
		
		[Fact]
		public void Test1()
		{
			Directory.CreateDirectory("dir");
			CreateFile("dir/b.txt", "Nazdar Pepindo!");
			TreeBuilder treeBuilder = TreeBuilder.CreateRootTreeBuilder();
		}

		private void CreateFile(string fileName, string content)
		{
			using (StreamWriter writer = new StreamWriter(fileName)) {
				writer.Write(content);
			}
		}
		
		private Blob CreateBlob(string fileName)
		{
			return new Blob(new RelativePath(fileName));
		}
	}
}