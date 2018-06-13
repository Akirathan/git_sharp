using System.IO;
using Xunit;
using GitSharp.Objects;
using GitSharp;
using GitSharp.Hash;
using GitSharp.Commands;

namespace Test {
	public class BlobTests {

		public BlobTests()
		{
			if (Directory.Exists(".git_sharp")) {
                Directory.Delete(".git_sharp", recursive: true);
			}
			new InitCommand().Process();
		}
		
		[Fact]
		public void SimpleTest()
		{
			CreateFile("a.txt", "a content");
			Blob blob = new Blob(new RelativePath("a.txt"));
			
			HashKey blobKey = ObjectDatabase.Store(blob);
			Blob retrievedBlob = ObjectDatabase.RetrieveBlob(blobKey);
			
			Assert.Equal(retrievedBlob, blob);
		}

		[Fact]
		public void ChangeCwdTest()
		{
			Directory.CreateDirectory("dir");
			CreateFile("dir/a.txt", "a content");
			
			Directory.SetCurrentDirectory("dir");
			
			Blob blob = new Blob(new RelativePath("a.txt"));
			
			HashKey blobKey = ObjectDatabase.Store(blob);
			Blob retrievedBlob = ObjectDatabase.RetrieveBlob(blobKey);
			
			Assert.Equal(retrievedBlob, blob);
		}
		
		private void CreateFile(string fileName, string content)
		{
			using (StreamWriter writer = new StreamWriter(fileName)) {
				writer.Write(content);
			}
		}
	}
}