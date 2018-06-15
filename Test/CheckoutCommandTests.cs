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

		// Creates new branch, and does one commit in this branch.
		// Tests whether the branch points to correct commit key.
		[Fact]
		public void CommitInNewBranch()
		{
			Init();
			
			CreateFile("z.txt", "");
			Program.Main(new string[]{"add", "z.txt"});
			Program.Main(new string[]{"commit", "Initial commit"});
			Program.Main(new string[]{"branch", "branch-1"});
			Program.Main(new string[]{"checkout", "branch-1"});
			
			// Create a commit in branch-1
			CreateFile("a.txt", "branch-1 content");
			Program.Main(new string[]{"add", "a.txt"});
			Program.Main(new string[]{"commit", "branch-1 commit"});

			Assert.Equal(
				ReferenceDatabase.GetHead().GetCommitKey(),
				ReferenceDatabase.GetBranch("branch-1").GetCommitKey()
			);

			Assert.Equal(
				ReferenceDatabase.GetBranch("branch-1").LoadCommit().Message.Trim(),
				"branch-1 commit"
			);
		}
		
		// Creates two branches and one commit in each branch.
		// Those commits contains same file a with different content.
		[Fact]
		public void TwoBranchesOverwriteFileTest()
		{
			Init();
			
			CreateFile("z.txt", "");
			Program.Main(new string[]{"add", "z.txt"});
			Program.Main(new string[]{"commit", "Initial commit"});
			Program.Main(new string[]{"branch", "branch-1"});
			Program.Main(new string[]{"checkout", "branch-1"});
			
			// Create a commit in branch-1
			CreateFile("a.txt", "branch-1 content");
			Program.Main(new string[]{"add", "a.txt"});
			Program.Main(new string[]{"commit", "branch-1 commit"});
			Program.Main(new string[]{"checkout", "master"});
			
			// Create a commit in master
			Assert.False(System.IO.File.Exists("a.txt"));
			CreateFile("a.txt", "master content");
			Program.Main(new string[]{"add", "a.txt"});
			Program.Main(new string[]{"commit", "master commit"});
			
			Program.Main(new string[]{"checkout", "branch-1"});
			
			Assert.Equal(ReadFile("a.txt"), "branch-1 content");
		}

		/// When tree which we want to checkout contains a file that is marked
		/// as modified or staged in index, checkout should fail.
		/// Otherwise local changes would be overwritten by checkout.
		[Fact]
		public void TwoBranchesCheckoutFailWithDirtyWdirTest()
		{
			Init();
			
			CreateFile("z.txt", "");
			Program.Main(new string[]{"add", "z.txt"});
			Program.Main(new string[]{"commit", "Initial commit"});
			Program.Main(new string[]{"branch", "branch-1"});
			Program.Main(new string[]{"checkout", "branch-1"});
			
			// Create a commit in branch-1
			CreateFile("a.txt", "branch-1 content");
			Program.Main(new string[]{"add", "a.txt"});
			Program.Main(new string[]{"commit", "branch-1 commit"});
			Program.Main(new string[]{"checkout", "master"});
			
			// Create a commit in master
			CreateFile("a.txt", "master content");
			Program.Main(new string[]{"add", "a.txt"});
			Program.Main(new string[]{"commit", "master commit"});

			// Make working directory dirty
			ModifyFile("a.txt", "blabla");
			// Checkout should fail - checkout tree contains same file
			// that we recently modified.
			Program.Main(new string[]{"checkout", "branch-1"});
			
			Assert.Equal(ReadFile("a.txt"), "blabla");
		}

		/// When checking out, local modifications should remain unchanged.
		[Fact]
		public void TwoBranchesModifiedFileShouldRemainInWdirTest()
		{
			Init();
			
			CreateFile("z.txt", "");
			Program.Main(new string[]{"add", "z.txt"});
			Program.Main(new string[]{"commit", "Initial commit"});
			Program.Main(new string[]{"branch", "branch-1"});
			Program.Main(new string[]{"checkout", "branch-1"});
			
			// Create a commit in branch-1
			CreateFile("b.txt", "branch-1 content");
			Program.Main(new string[]{"add", "b.txt"});
			Program.Main(new string[]{"commit", "branch-1 commit"});
			Program.Main(new string[]{"checkout", "master"});
			
			// Create a commit in master
			CreateFile("a.txt", "master content");
			Program.Main(new string[]{"add", "a.txt"});
			Program.Main(new string[]{"commit", "master commit"});

			// Make working directory dirty
			ModifyFile("a.txt", "blabla");
			Program.Main(new string[]{"checkout", "branch-1"});
			
			// Local modification should remain unchanged.
			Assert.Equal(ReadFile("a.txt"), "blabla");
			Assert.Equal(ReadFile("b.txt"), "branch-1 content");
			
			Assert.True(Index.ContainsFile(new RelativePath("a.txt")));
			Assert.True(Index.ContainsFile(new RelativePath("b.txt")));
			Assert.True(Index.IsModified(new RelativePath("a.txt")));
			Assert.True(Index.IsCommited(new RelativePath("b.txt")));
		}

		/// Staged files should remain staged after checkout.
		/// But only if they are not contained in checkout tree (note that this
		/// case is covered in above test methods).
		[Fact]
		public void TwoBranchesStagedFileShouldRemainInIndexTest()
		{
			Init();
			
			CreateFile("z.txt", "");
			Program.Main(new string[]{"add", "z.txt"});
			Program.Main(new string[]{"commit", "Initial commit"});
			Program.Main(new string[]{"branch", "branch-1"});
			Program.Main(new string[]{"checkout", "branch-1"});
			
			// Create a commit in branch-1
			CreateFile("b.txt", "branch-1 content");
			Program.Main(new string[]{"add", "b.txt"});
			Program.Main(new string[]{"commit", "branch-1 commit"});
			Program.Main(new string[]{"checkout", "master"});
			
			// Create a commit in master
			CreateFile("a.txt", "master content");
			Program.Main(new string[]{"add", "a.txt"});
			Program.Main(new string[]{"commit", "master commit"});

			// Stage a.txt
			ModifyFile("a.txt", "blabla");
			Program.Main(new string[]{"add", "a.txt"});
			
			Program.Main(new string[]{"checkout", "branch-1"});
			
			// Local modification should remain unchanged.
			Assert.Equal(ReadFile("a.txt"), "blabla");
			Assert.Equal(ReadFile("b.txt"), "branch-1 content");
			
			Assert.True(Index.ContainsFile(new RelativePath("a.txt")));
			Assert.True(Index.ContainsFile(new RelativePath("b.txt")));
			Assert.True(Index.IsStaged(new RelativePath("a.txt")));
			Assert.True(Index.IsCommited(new RelativePath("b.txt")));
		}
		
		private void Init()
		{
			if (Directory.Exists(".git_sharp")) {
                Directory.Delete(".git_sharp", recursive: true);
			}
			new InitCommand().Process();
		}

		private string ReadFile(string fileName)
		{
			string fileContent;
			using (StreamReader reader = new StreamReader(fileName)) {
				fileContent = reader.ReadToEnd();
			}
			return fileContent;
		}

		private void ModifyFile(string fileName, string content)
		{
			CreateFile(fileName, content);
		}
		
		private void CreateFile(string fileName, string content)
		{
			using (StreamWriter writer = new StreamWriter(fileName)) {
				writer.Write(content);
			}
		}
	}
}