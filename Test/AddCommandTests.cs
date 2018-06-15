using System;
using System.IO;
using Xunit;
using GitSharp.Commands;
using GitSharp;

namespace Test {
	public class AddCommandTests {

		public AddCommandTests()
		{
			if (Directory.Exists(".git_sharp")) {
                Directory.Delete(".git_sharp", recursive: true);
			}
			new InitCommand().Process();
		}
		
		[Fact]
		public void StageOneFileTest()
		{
			CreateFile("a.txt", "Nazdar");
			new AddCommand(new string[] {"a.txt"}).Process();
			
			RelativePath aFilePath = new RelativePath("a.txt");
			Assert.True(Index.IsStaged(aFilePath));
			Assert.False(Index.IsModified(aFilePath));
		}

		[Fact]
		public void StageFilesInSubDirTest()
		{
			CreateFile("a.txt", "a content");
			Directory.CreateDirectory("dir/subdir");
			CreateFile("dir/subdir/b.txt", "b content");
			
			new AddCommand(new string[] {"a.txt", "dir/subdir/b.txt"}).Process();
			RelativePath aFilePath = new RelativePath("a.txt");
			RelativePath bFilePath = new RelativePath("dir/subdir/b.txt");
			
			Assert.True(Index.IsStaged(aFilePath));
			Assert.True(Index.IsStaged(bFilePath));
			Assert.False(Index.IsModified(aFilePath));
			Assert.False(Index.IsModified(bFilePath));
		}

		private void CreateFile(string fileName, string content)
		{
			FileStream fileStream = System.IO.File.Create(fileName);
			using (StreamWriter writer = new StreamWriter(fileStream)) {
				writer.Write(content);
			}
		}
	}
}