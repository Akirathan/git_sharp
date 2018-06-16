using System.IO;
using GitSharp.Commands;
using GitSharp;
using Xunit;

namespace Test {
	public class StatusCommandTests {
		
		[Fact]
		public void StatusBeforeFirstCommitTest()
		{
			Init();
			
			CreateFile("a.txt", "a content");
			Program.Main(new []{"status"});
			Assert.True(true);
		}

		[Fact]
		public void StatusAfterFirstStageTest()
		{
			Init();
			
			CreateFile("a.txt", "a content");
			Program.Main(new []{"add", "a.txt"});
			Program.Main(new []{"status"});
			Assert.True(true);
		}

		[Fact]
		public void StatusAfterFirstCommitTest()
		{
			Init();
			
			CreateFile("a.txt", "a content");
			Program.Main(new []{"add", "a.txt"});
			Program.Main(new []{"commit", "Initial commit"});
			Program.Main(new []{"status"});
			Assert.True(true);
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