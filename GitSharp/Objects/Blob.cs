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
		private string _blobContent;
		private HashKey _checksum;

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
			_blobContent = CreateBlobFileContent();
			_checksum = ContentHasher.HashContent(_blobContent);
		}

		public string FileContent { get; }
		
		public string FileName { get; }

		public override string GetGitObjectFileContent()
		{
			return _blobContent;
		}

		public override HashKey GetChecksum()
		{
			return _checksum;
		}

		public bool Equals(Blob other)
		{
			if (other == null) return false;
			if (this == other) return true;
			return string.Equals(FileContent, other.FileContent);
		}

		private string CreateBlobFileContent()
		{
			StringBuilder contentBuilder = new StringBuilder();
			contentBuilder.AppendLine(BlobFileType + " " + FileName);
			contentBuilder.Append(FileContent);
			return contentBuilder.ToString();
		}
	}
}