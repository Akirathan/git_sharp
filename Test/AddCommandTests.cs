using System;
using System.IO;
using Xunit;
using GitSharp.Commands;

namespace Test {
	public class AddCommandTests {
		[Fact]
		public void Test1()
		{
			CreateFile("a.txt", "Nazdar");
			AddCommand addCommand = new AddCommand(new string[] {"a.txt"});
		}

		private void CreateFile(string fileName, string content)
		{
			FileStream fileStream = File.Create(fileName);
			using (StreamWriter writer = new StreamWriter(fileStream)) {
				writer.Write(content);
			}
		}
	}
}