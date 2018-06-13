using System.IO;
using Xunit;
using GitSharp;
using GitSharp.Commands;
using GitSharp.Objects;
using GitSharp.Reference;

namespace Test {
	public class CommitCommandTests {

		[Fact]
		public void DoCommitCommandAndIntrospectFromBranch()
		{
			Init();
			
			CreateFile("a.txt", "a content");
			Program.Main(new string[]{"add", "a.txt"});
			Program.Main(new string[]{"commit", "Commit message"});
			
			RelativePath aFilePath = new RelativePath("a.txt");
			Assert.True(Index.IsCommited(aFilePath));
			
			Branch headBranch = ReferenceDatabase.GetHead();
			Commit commit = headBranch.LoadCommit();
			Tree tree = commit.LoadTree();
			Blob blobA = tree.FindAndLoadBlob("a.txt");
			
			Assert.Equal(blobA.FileName, "a.txt");
			Assert.Equal(blobA.FileContent, "a content");
		}

		[Fact]
		public void TwoCommits()
		{
			Init();
			
			CreateFile("a.txt", "a content");
			Program.Main(new string[]{"add", "a.txt"});
			Program.Main(new string[]{"commit", "First commit"});
			
			CreateFile("a.txt", "other content");
			Program.Main(new string[]{"add", "a.txt"});
			Program.Main(new string[]{"commit", "Second commit"});
			
			RelativePath aFilePath = new RelativePath("a.txt");
			Assert.True(Index.IsCommited(aFilePath));
			
			Branch headBranch = ReferenceDatabase.GetHead();
			Commit commit = headBranch.LoadCommit();
			Tree tree = commit.LoadTree();
			Blob blobA = tree.FindAndLoadBlob("a.txt");
			
			Assert.Equal(blobA.FileName, "a.txt");
			Assert.Equal(blobA.FileContent, "other content");
			
		}
		
		private void Init()
		{
			if (Directory.Exists(".git_sharp")) {
                Directory.Delete(".git_sharp", recursive: true);
			}
			new InitCommand().Process();
		}
		
		private void CreateFile(string fileName, string content)
		{
			using (StreamWriter writer = new StreamWriter(fileName)) {
				writer.Write(content);
			}
		}
	}
}