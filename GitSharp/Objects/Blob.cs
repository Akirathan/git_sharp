using System;
using System.IO;
using System.Text;
using GitSharp.Hash;
using GitSharp.Reference;

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
			string filePath = ParseFirstLine(reader.ReadLine());
			if (filePath == null) {
				return null;
			}

			Blob blob = new Blob();
			blob.FilePath = filePath;
			blob.FileName = GetFileNameFromFilePath(filePath);
			blob.FileContent = reader.ReadToEnd();
			blob._blobContent = blob.CreateBlobFileContent();
			blob._checksum = ContentHasher.HashContent(blob._blobContent);
			return blob;
		}

		private static string GetFileNameFromFilePath(string filePath)
		{
			string[] pathItems = filePath.Split(Path.DirectorySeparatorChar);
			return pathItems[pathItems.Length - 1];
		}
		
		/// Returns file path
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
		
		private Blob() {}

		public string FileContent { get; private set; }
		
		/// path to file that is relative to git root directory
		public string FilePath { get; private set; }
		
		public string FileName { get; private set; }

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
			if (!CheckCheckoutPreconditions()) {
				return false;
			}

			using (StreamWriter writer = new StreamWriter(FileName)) {
				writer.Write(FileContent);
			}
			UpdateIndexAfterCheckout();
			return true;
		}

		private void UpdateIndexAfterCheckout()
		{
			RelativePath filePath = new RelativePath(FilePath);

			if (!Index.ContainsFile(filePath)) {
				Index.StartTrackingFile(filePath);
			}
            Index.SetWdirFileContentKey(filePath, _checksum.ToString());
			Index.SetStageFileContentKey(filePath, _checksum.ToString());
			Index.SetRepoFileContentKey(filePath, _checksum.ToString());
		}

		private bool CheckCheckoutPreconditions()
		{
			RelativePath filePath = new RelativePath(FilePath);
			
			if (Index.ContainsFile(filePath)) {
				if (Index.IsStaged(filePath) || Index.IsModified(filePath)) {
					return false;
				}
				else if (Index.IsCommited(filePath)) {
					return true;
				}
			}
			return true;
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