using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using GitSharp.Hash;

namespace GitSharp.Objects {
	internal class Tree : GitObject {
		private const string TreeFileType = "tree";
		private List<TreeEntry> _treeEntries;
		private List<BlobEntry> _blobEntries;

		/// <summary>
		/// Parses tree from given string. Suppose that string represents content
		/// of a file.
		/// </summary>
		/// <param name="content"></param>
		/// <returns>null if parsing fails</returns>
		public static Tree ParseFromString(string content)
		{
			StringReader reader = new StringReader(content);

			if (reader.ReadLine() != TreeFileType) {
				return null;
			}

			List<BlobEntry> blobEntries = new List<BlobEntry>();
			List<TreeEntry> treeEntries = new List<TreeEntry>();
			string line = reader.ReadLine();
			
			while (line != null) {
                string[] lineItems = line.Split(new char[] {' '}, StringSplitOptions.RemoveEmptyEntries);
				
                if (TreeEntry.IsTreeEntry(lineItems)) {
                    TreeEntry treeEntry;
                    if (!TreeEntry.ParseFromStringLineItems(lineItems, out treeEntry)) {
                        return null;
                    }
                    treeEntries.Add(treeEntry);
                }
                else if (BlobEntry.IsBlobEntry(lineItems)) {
	                BlobEntry blobEntry;
	                if (!BlobEntry.ParseFromStringLineItems(lineItems, out blobEntry)) {
		                return null;
	                }
	                blobEntries.Add(blobEntry);
                }

				line = reader.ReadLine();
			}
			
			return new Tree(blobEntries, treeEntries);
		}

		public static string CreateTreeFileContent(Tree tree)
		{
			StringBuilder contentBuilder = new StringBuilder();
			
			contentBuilder.AppendLine(TreeFileType);
			foreach (BlobEntry blobEntry in tree.GetBlobEntries()) {
				contentBuilder.AppendLine(blobEntry.ToString());
			}
			foreach (TreeEntry treeEntry in tree.GetTreeEntries()) {
				contentBuilder.AppendLine(treeEntry.ToString());
			}

			return contentBuilder.ToString();
		}
		
		public Tree(List<BlobEntry> blobEntries, List<TreeEntry> treeEntries)
		{
			_blobEntries = blobEntries;
			_treeEntries = treeEntries;
		}

		public List<BlobEntry> GetBlobEntries()
		{
			return _blobEntries;
		}

		public List<TreeEntry> GetTreeEntries()
		{
			return _treeEntries;
		}

		public struct BlobEntry {
			// TODO: FileMode
			public HashKey Key;

			public string FileName;
			public Blob Blob;

			public static bool IsBlobEntry(string[] lineItems)
			{
				return lineItems[0] == "blob";
			}

			public static bool ParseFromStringLineItems(string[] lineItems, out BlobEntry blobEntry)
			{
				blobEntry = new BlobEntry();
				if (lineItems.Length != 3) {
					return false;
				}
				blobEntry.Key = HashKey.ParseFromString(lineItems[1]);
				blobEntry.FileName = lineItems[2];
				return true;
			}
			
			public override string ToString()
			{
				return "blob " + Key.GetStringRepresentation() + " " + FileName;
			}
		}

		public struct TreeEntry {
			// TODO: FileMode
			public HashKey Key;
			public string DirectoryName;
			public Tree Tree;

			public static bool IsTreeEntry(string[] lineItems)
			{
				return lineItems[0] == "tree";
			}
			
			/// <summary>
			/// 
			/// </summary>
			/// <param name="lineItems"></param>
			/// <param name="treeEntry"></param>
			/// <returns>
			/// false if parsing fails.
			/// </returns>
			public static bool ParseFromStringLineItems(string[] lineItems, out TreeEntry treeEntry)
			{
				treeEntry = new TreeEntry();
				if (lineItems.Length != 3) {
					return false;
				}
				treeEntry.Key = HashKey.ParseFromString(lineItems[1]);
				treeEntry.DirectoryName = lineItems[2];
				return true;
			}
			
			public override string ToString()
			{
				return "tree " + Key.GetStringRepresentation() + " " + DirectoryName;
			}
		}
	}
}