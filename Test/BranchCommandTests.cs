using System.IO;
using GitSharp;
using GitSharp.Commands;
using GitSharp.Objects;
using GitSharp.Reference;
using Xunit;

namespace Test {
	public class BranchCommandTests {
		
		[Fact]
		public void CreateOneBranchAfterCommitTest()
		{
			Init();
			
			CreateFile("a.txt", "a content");
			Program.Main(new string[]{"add", "a.txt"});
			Program.Main(new string[]{"commit", "First commit"});
			Program.Main(new string[]{"branch", "new-branch"});
			
			Branch headBranch = ReferenceDatabase.GetHead();
			Assert.Equal(headBranch.Name, "master");
			Branch newBranch = ReferenceDatabase.GetBranch("new-branch");
			Assert.Equal(headBranch.GetCommitKey(), newBranch.GetCommitKey());
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