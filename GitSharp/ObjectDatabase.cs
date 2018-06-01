using System;
using System.IO;
using System.Net.Mime;
using System.Text;
using GitSharp.Hash;
using GitSharp.Objects;

namespace GitSharp {
	internal class ObjectDatabase {
		private const string DefaultPath = ".git_sharp/objects";

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

			StreamWriter writer = null;
			try {
				FileStream fileStream = File.Create(DefaultPath + "/" + keyStr);
				writer = new StreamWriter(fileStream);
				writer.Write(blobFileContent);
			}
			catch (Exception e) {
				throw new Exception("Cannot create or write blob file", e);
			}
			finally {
				if (writer != null) {
					writer.Close();
				}
			}

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
			if (!File.Exists(key.GetStringRepresentation())) {
				return null;
			}
		}
		
		private string CreateBlobFileContent(Blob blob)
		{
			StringBuilder contentBuilder = new StringBuilder();
			contentBuilder.AppendLine("blob");
			contentBuilder.Append(blob.Content);
			return contentBuilder.ToString();
		}
	}
}