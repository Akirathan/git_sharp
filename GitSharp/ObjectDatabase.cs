using System;
using System.IO;
using System.Net.Mime;
using System.Text;
using GitSharp.Hash;
using GitSharp.Objects;

namespace GitSharp {
	internal class ObjectDatabase {
		private const string DefaultPath = ".git_sharp/objects";
		private const string BlobFileType = "blob";

		public ObjectDatabase()
		{
			try {
				Directory.CreateDirectory(DefaultPath);
			}
			catch (Exception e) {
				throw new Exception("Cannot create objects directory", e);
			}
		}

		public HashKey Store(Blob blob)
		{
			string blobFileContent = Blob.CreateBlobFileContent(blob);
			HashKey key = ContentHasher.hash(blobFileContent);
			WriteObjectContentToFile(blobFileContent, key.ToString());
			return key;
		}

		public HashKey Store(Tree tree)
		{
			string treeFileContent = Tree.CreateTreeFileContent(tree);
			HashKey key = ContentHasher.hash(treeFileContent);
			WriteObjectContentToFile(treeFileContent, key.ToString());
			return key;
		}

		/// <summary>
		/// Retrieves blob object from database.
		/// </summary>
		/// <param name="key"></param>
		/// <returns>
		/// null when no blob found
		/// </returns>
		public Blob RetrieveBlob(HashKey key)
		{
			string fileName = key.ToString();

			string fileContent = ReadFileContent(fileName);
			if (fileContent == null) {
				return null;
			}

			return Blob.ParseFromString(fileContent);
		}
		
		private void WriteObjectContentToFile(string content, string fileName)
		{
			StreamWriter writer = null;
			try {
				FileStream fileStream = File.Create(DefaultPath + "/" + fileName);
				writer = new StreamWriter(fileStream);
				writer.Write(content);
			}
			catch (Exception e) {
				throw new Exception("Cannot create or write blob file", e);
			}
			finally {
				if (writer != null) {
					writer.Close();
				}
			}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="fileName"></param>
		/// <returns>null if reading fails</returns>
		private string ReadFileContent(string fileName)
		{
			string content;
			try {
				StreamReader reader = new StreamReader(DefaultPath + "/" + fileName);
				content = reader.ReadToEnd();
			}
			catch (Exception) {
				// TODO: log
				return null;
			}

			return content;
		}
	}
}