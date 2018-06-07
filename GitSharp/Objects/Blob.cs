using System;
using System.IO;
using System.Text;

namespace GitSharp.Objects {
	internal class Blob : GitObject, IEquatable<Blob> {
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

		public bool Equals(Blob other)
		{
			if (ReferenceEquals(null, other)) return false;
			if (ReferenceEquals(this, other)) return true;
			return string.Equals(content, other.content);
		}

		public override bool Equals(object obj)
		{
			if (ReferenceEquals(null, obj)) return false;
			if (ReferenceEquals(this, obj)) return true;
			if (obj.GetType() != typeof(Blob)) return false;
			return Equals((Blob) obj);
		}

		public override int GetHashCode()
		{
			return content.GetHashCode();
		}
	}
}