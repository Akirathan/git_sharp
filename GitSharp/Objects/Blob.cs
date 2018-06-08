using System;
using System.IO;
using System.Text;
using GitSharp.Hash;

namespace GitSharp.Objects {
	/// <summary>
	/// Represents file blob object.
	/// File blob object contains file header and file content.
	/// Checksum method gives key which is used for storing and retrieving this blob
	/// object from ObjectDatabase.
	/// </summary>
	internal class Blob : GitObject, IEquatable<Blob> {
		private const string BlobFileType = "blob";

		public static Blob ParseFromString(string content)
		{
			StringReader reader = new StringReader(content);
			if (reader.ReadLine() != BlobFileType) {
				return null;
			}
			return new Blob(reader.ReadToEnd());
		}
		
		public Blob(string fileName)
		{
			FileName = fileName;
			using (StreamReader reader = new StreamReader(fileName)) {
				FileContent = reader.ReadToEnd();
			}
			BlobContent = CreateBlobFileContent();
			Checksum = ContentHasher.HashContent(BlobContent);
		}

		public string FileContent { get; }
		
		public string FileName { get; }
		
		/// <summary>
		/// Returns a string representing text of blob file.
		/// Contains filename alongside with filecontent
		/// </summary>
		public string BlobContent { get; }

		public HashKey Checksum { get; }

		public bool Equals(Blob other)
		{
			if (other == null) return false;
			if (this == other) return true;
			return string.Equals(FileContent, other.FileContent);
		}

		private string CreateBlobFileContent()
		{
			StringBuilder contentBuilder = new StringBuilder();
			contentBuilder.AppendLine(BlobFileType);
			contentBuilder.Append(FileContent);
			return contentBuilder.ToString();
		}
	}
}