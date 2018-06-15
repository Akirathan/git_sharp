using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Mime;
using System.Text;
using GitSharp.Hash;
using GitSharp.Objects;

namespace GitSharp {
	internal static class ObjectDatabase {
		public static readonly string DefaultPath =
			Traverser.GetRootDirPath() + Path.DirectorySeparatorChar +
			Traverser.GitRootDirName + Path.DirectorySeparatorChar + "objects";

		/// <summary>
		/// 
		/// Note that it is not incorrect to call this method with same object more
		/// than once.
		/// </summary>
		/// <param name="gitObject"></param>
		/// <returns></returns>
		public static HashKey Store(IStorableGitObject gitObject)
		{
			HashKey key = gitObject.GetChecksum();
			if (FileObjectExists(key)) {
				return key;
			}
			
			string fileContent = gitObject.GetGitObjectFileContent();
			WriteObjectContentToFile(fileContent, key.ToString());
			return key;
		}

		/// <summary>
		/// Stores given commit, its root tree and all the subtrees of the root tree
		/// in the ObjectDatabase.
		/// This is useful for creating commit objects.
		/// </summary>
		/// Note that it is supposed that all the blob objects are already stored.
		/// <param name="commit"></param>
		/// <returns> HashKey to commit </returns>
		public static HashKey StoreCommitWithTreeHierarchy(Commit commit, TreeBuilder treeBuilder)
		{
			List<Blob> allBlobs = new List<Blob>();
			List<TreeBuilder> allTrees = new List<TreeBuilder>();
			treeBuilder.GetAllBlobsAndSubTrees(allBlobs, allTrees);
			
			foreach (TreeBuilder tree in allTrees) {
				Store(tree);
			}
			
			return Store(commit);
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
		
		private static bool FileObjectExists(HashKey key)
		{
			return File.Exists(DefaultPath + Path.DirectorySeparatorChar + key.ToString());
		}

		private static void WriteObjectContentToFile(string content, string fileName)
		{
			StreamWriter writer = null;
			try {
				FileStream fileStream = File.Create(DefaultPath + Path.DirectorySeparatorChar + fileName);
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

		/// Returns null if reading fails.
		private static string ReadFileContent(string fileName)
		{
			string content;
			try {
				StreamReader reader = new StreamReader(DefaultPath + Path.DirectorySeparatorChar + fileName);
				content = reader.ReadToEnd();
			}
			catch (Exception) {
				return null;
			}

			return content;
		}
	}
}