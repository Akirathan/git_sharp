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
			HashKey key = ContentHasher.hash(blob.Content);
			string keyStr = key.GetStringRepresentation();
			string blobFileContent = CreateBlobFileContent(blob);
			WriteObjectContentToFile(blobFileContent, keyStr);
			return key;
		}

		public HashKey Store(Tree tree)
		{
			string treeFileContent = Tree.CreateTreeFileContent(tree);
			HashKey key = ContentHasher.hash(treeFileContent);
			WriteObjectContentToFile(treeFileContent, key.GetStringRepresentation());
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
			StreamReader reader = null;
			try {
				reader = new StreamReader(DefaultPath + "/" + key.GetStringRepresentation());
			}
			catch (FileNotFoundException) {
				return null;
			}

			if (reader.ReadLine() != BlobFileType) {
				// Object is not blob type.
				return null;
			}
			
			Blob blob = new Blob(reader.ReadToEnd());
			reader.Close();
			return blob;
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
		
		private string CreateBlobFileContent(Blob blob)
		{
			StringBuilder contentBuilder = new StringBuilder();
			contentBuilder.AppendLine(BlobFileType);
			contentBuilder.Append(blob.Content);
			return contentBuilder.ToString();
		}
	}
}