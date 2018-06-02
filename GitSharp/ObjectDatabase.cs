using System;
using System.IO;
using System.Net.Mime;
using System.Text;
using GitSharp.Hash;
using GitSharp.Objects;

namespace GitSharp {
	internal static class ObjectDatabase {
		private const string DefaultPath = ".git_sharp/objects";

		static ObjectDatabase()
		{
			CreateObjectsDirectory();
		}

		public static HashKey Store(Blob blob)
		{
			string blobFileContent = Blob.CreateBlobFileContent(blob);
			HashKey key = ContentHasher.hash(blobFileContent);
			WriteObjectContentToFile(blobFileContent, key.ToString());
			return key;
		}

		public static HashKey Store(Tree tree)
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
		public static Blob RetrieveBlob(HashKey key)
		{
			string fileName = key.ToString();

			string fileContent = ReadFileContent(fileName);
			if (fileContent == null) {
				return null;
			}

			return Blob.ParseFromString(fileContent);
		}

		/// <summary>
		/// Retrieves tree object from the database.
		/// </summary>
		/// <param name="key">key to use for searching in database</param>
		/// <returns>null when no tree was found</returns>
		public static Tree RetrieveTree(HashKey key)
		{
			string fileName = key.ToString();
			string fileContent = ReadFileContent(fileName);
			if (fileContent == null) {
				return null;
			}

			return Tree.ParseFromString(fileContent);
		}
		
		private static void WriteObjectContentToFile(string content, string fileName)
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
		private static string ReadFileContent(string fileName)
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

		private static void CreateObjectsDirectory()
		{
			if (Directory.Exists(DefaultPath)) {
				return;
			}
			
			try {
				Directory.CreateDirectory(DefaultPath);
			}
			catch (Exception e) {
				throw new Exception("Cannot create objects directory", e);
			}
		}
	}
}