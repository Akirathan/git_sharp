using System.IO;
using GitSharp;
using GitSharp.Commands;
using GitSharp.Reference;
using GitSharp.Objects;
using Xunit;

namespace Test {
	public class CheckoutCommandTests {
		
		[Fact]
		public void TwoCommitsCheckoutToFirstCommitTest()
		{
			Init();
			
			CreateFile("a.txt", "a content");
			Program.Main(new string[]{"add", "a.txt"});
			Program.Main(new string[]{"commit", "First commit"});
			Program.Main(new string[]{"branch", "branch-1"});
			
			CreateFile("a.txt", "modified content");
			Program.Main(new string[]{"add", "a.txt"});
			Program.Main(new string[]{"commit", "Second commit"});
			Program.Main(new string[]{"branch", "branch-2"});
			
			Program.Main(new string[]{"checkout", "branch-1"});

			Tree headTree = ReferenceDatabase.GetHead().LoadCommit().LoadTree();
			Blob aFileBlob = headTree.FindAndLoadBlob("a.txt");
			Assert.Equal(aFileBlob.FileContent, "a content");
		}
		
		[Fact]
		public void TwoCommitsCheckoutToLastBranchTest()
		{
			Init();
			
			CreateFile("a.txt", "a content");
			Program.Main(new string[]{"add", "a.txt"});
			Program.Main(new string[]{"commit", "First commit"});
			Program.Main(new string[]{"branch", "branch-1"});
			
			CreateFile("a.txt", "modified content");
			Program.Main(new string[]{"add", "a.txt"});
			Program.Main(new string[]{"commit", "Second commit"});
			Program.Main(new string[]{"branch", "branch-2"});
			
			Program.Main(new string[]{"checkout", "branch-2"});

			Tree headTree = ReferenceDatabase.GetHead().LoadCommit().LoadTree();
			Blob aFileBlob = headTree.FindAndLoadBlob("a.txt");
			Assert.Equal(aFileBlob.FileContent, "modified content");
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