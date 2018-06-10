using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using GitSharp.Hash;

namespace GitSharp.Objects {
	internal class Tree : GitObject {
		private const string TreeFileType = "tree";
		private readonly IDictionary<string, Tree> _subTrees;
		private readonly IDictionary<string, Blob> _blobs;
		private readonly string _treeObjectFileContent;
		private readonly HashKey _checksum;

		/// <summary>
		/// Parses tree from given string. Suppose that string represents content
		/// of a file.
		/// </summary>
		/// <param name="content"></param>
		/// <returns>null if parsing fails</returns>
		public static Tree ParseFromString(string content)
		{
			StringReader reader = new StringReader(content);

			string dirName = ParseFirstLine(reader.ReadLine());
			if (dirName == null) {
				return null;
			}

			Dictionary<string, Blob> blobs = new Dictionary<string, Blob>();
			Dictionary<string, Tree> subTrees = new Dictionary<string, Tree>();
			string line = reader.ReadLine();
			
			while (line != null) {
                string[] lineItems = line.Split(new char[] {' '}, StringSplitOptions.RemoveEmptyEntries);
				
                if (IsTreeEntry(lineItems)) {
	                Tree tree = ParseTreeEntry(lineItems);
                    subTrees.Add(tree.DirName, tree);
                }
                else if (IsBlobEntry(lineItems)) {
	                Blob blob = ParseBlobEntry(lineItems);
	                blobs.Add(blob.FileName, blob);
                }

				line = reader.ReadLine();
			}
			
			return new Tree(dirName, blobs, subTrees);
		}
		
		/// May return null
		private static string ParseFirstLine(string firstLine)
		{
			if (firstLine == null) {
				return null;
			}

			if (firstLine.Split(new char[] {' '})[0] != TreeFileType) {
				return null;
			}
			return firstLine.Split(new char[] {' '})[1];
		}

		private static bool IsTreeEntry(string[] lineItems)
		{
			return lineItems[0] == "tree";
		}
		
		private static bool IsBlobEntry(string[] lineItems)
		{
			return lineItems[0] == "blob";
		}

		/// May return null
		private static Tree ParseTreeEntry(string[] lineItems)
		{
            if (lineItems.Length != 3) {
	            return null;
            }

			string key = lineItems[1];
			return ObjectDatabase.RetrieveTree(HashKey.ParseFromString(key));
		}

		/// May return null
		private static Blob ParseBlobEntry(string[] lineItems)
		{
            if (lineItems.Length != 3) {
	            return null;
            }
			string key = lineItems[1];
			return ObjectDatabase.RetrieveBlob(HashKey.ParseFromString(key));
		}
		
		public Tree(string dirName, IDictionary<string, Blob> blobs, IDictionary<string, Tree> subTrees)
		{
			DirName = dirName;
			
			if (blobs == null) {
				_blobs = new Dictionary<string, Blob>();
			}
			else {
                _blobs = blobs;
			}

			if (subTrees == null) {
				_subTrees = new Dictionary<string, Tree>();
			}
			else {
                _subTrees = subTrees;
			}

			_treeObjectFileContent = CreateTreeFileContent();
			_checksum = ContentHasher.HashContent(_treeObjectFileContent);
		}
		
		public string DirName { get; }
		
		public override string GetGitObjectFileContent()
		{
			return _treeObjectFileContent;
		}

		public override HashKey GetChecksum()
		{
			return _checksum;
		}
		
		public void Checkout()
		{
			
		}

		private string CreateTreeFileContent()
		{
			StringBuilder contentBuilder = new StringBuilder();
			
			contentBuilder.AppendLine(TreeFileType + " " + DirName);
            foreach (Blob blobEntry in _blobs.Values) {
                contentBuilder.AppendLine(blobEntry.ToString());
            }
            foreach (Tree treeEntry in _subTrees.Values) {
                contentBuilder.AppendLine(treeEntry.ToString());
            }

			return contentBuilder.ToString();
		}
	}
}