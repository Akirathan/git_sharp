using System.IO;
using System.Text;

namespace GitSharp.Objects {
	internal class Blob : GitObject {
		private const string BlobFileType = "blob";
		private string content;

		public static Blob ParseFromString(string content)
		{
			StringReader reader = new StringReader(content);
			if (reader.ReadLine() != BlobFileType) {
				return null;
			}
			return new Blob(reader.ReadToEnd());
		}

		public static string CreateBlobFileContent(Blob blob)
		{
			StringBuilder contentBuilder = new StringBuilder();
			contentBuilder.AppendLine(BlobFileType);
			contentBuilder.Append(blob.Content);
			return contentBuilder.ToString();
		}
		
		public Blob(string content)
		{
			this.content = content;
		}

		public string Content {
			get { return content; }
		}
	}
}