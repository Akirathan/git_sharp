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
	internal class Blob : IStorableGitObject, IEquatable<Blob> {
		private const string BlobFileType = "blob";
		private string _blobContent;
		private HashKey _checksum;

		public static Blob ParseFromString(string content)
		{
			StringReader reader = new StringReader(content);
			string fileName = ParseFirstLine(reader.ReadLine());
			if (fileName == null) {
				return null;
			}
			return new Blob(new RelativePath(fileName));
		}
		
		private static string ParseFirstLine(string firstLine)
		{
			if (firstLine == null) {
				return null;
			}
			string[] lineItems = firstLine.Split(new char[] {' '});
			if (lineItems[0] != BlobFileType || lineItems.Length != 2) {
				return null;
			}
			return lineItems[1];
		}
		
		public Blob(RelativePath filePath)
		{
			FilePath = filePath.GetRelativeToGitRoot();
			FileName = filePath.GetFileName();
			using (StreamReader reader = new StreamReader(filePath.GetAbsolutePath())) {
				FileContent = reader.ReadToEnd();
			}
			_blobContent = CreateBlobFileContent();
			_checksum = ContentHasher.HashContent(_blobContent);
		}

		public string FileContent { get; }
		
		/// path to file that is relative to git root directory
		public string FilePath { get; }
		
		public string FileName { get; }

		public string GetGitObjectFileContent()
		{
			return _blobContent;
		}

		public HashKey GetChecksum()
		{
			return _checksum;
		}

		public bool Checkout()
		{
			
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
			contentBuilder.AppendLine(BlobFileType + " " + FilePath);
			contentBuilder.Append(FileContent);
			return contentBuilder.ToString();
		}
	}
}