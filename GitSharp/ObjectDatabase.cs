using System;
using System.IO;
using System.Net.Mime;
using System.Text;
using GitSharp.Hash;
using GitSharp.Objects;

namespace GitSharp {
	internal static class ObjectDatabase {
		public static readonly string DefaultPath =
			Traverser.GitRootDirName + Path.DirectorySeparatorChar + "objects";

		public static HashKey Store(IStorableGitObject gitObject)
		{
			string fileContent = gitObject.GetGitObjectFileContent();
			HashKey key = gitObject.GetChecksum();
			WriteObjectContentToFile(fileContent, key.ToString());
			return key;
		}

		/// <summary>
		/// Stores given commit, its root tree and all the subtrees of the root tree
		/// in the ObjectDatabase.
		/// This is useful for creating commit objects.
		/// </summary>
		/// <param name="commit"></param>
		/// <returns></returns>
		public static HashKey StoreCommitWithTreeHierarchy(Commit commit)
		{
			
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

		public static Commit RetrieveCommit(HashKey key)
		{
			string fileName = key.ToString();
			string fileContent = ReadFileContent(fileName);
			if (fileContent == null) {
				return null;
			}

			return Commit.ParseFromString(fileContent);
		}
		
		private static void WriteObjectContentToFile(string content, string fileName)
		{
			StreamWriter writer = null;
			try {
				FileStream fileStream = System.IO.File.Create(DefaultPath + Path.DirectorySeparatorChar + fileName);
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
				StreamReader reader = new StreamReader(DefaultPath + Path.DirectorySeparatorChar + fileName);
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