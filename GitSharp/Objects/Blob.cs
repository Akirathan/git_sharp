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
		private readonly string _blobFileContent;

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
			_blobFileContent = CreateBlobFileContent();
			Checksum = ContentHasher.HashContent(_blobFileContent);
		}

		public string FileContent { get; }
		
		public string FileName { get; }

		public HashKey Checksum { get; }

		public bool Equals(Blob other)
		{
			if (other == null) return false;
			if (this == other) return true;
			return string.Equals(FileContent, other.FileContent);
		}

		public void WriteToFile(string fileName)
		{
			using (StreamWriter writer = new StreamWriter(fileName)) {
				writer.Write(_blobFileContent);
			}
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