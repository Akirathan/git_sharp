﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using GitSharp.Hash;

namespace GitSharp.Objects {
	internal class Tree : GitObject {
		private const string TreeFileType = "tree";
		private readonly IDictionary<string, TreeEntry> _subTrees = new Dictionary<string, TreeEntry>();
		private readonly IDictionary<string, BlobEntry> _blobs = new Dictionary<string, BlobEntry>();
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

			Dictionary<string, HashKey> blobs = new Dictionary<string, HashKey>();
			Dictionary<string, HashKey> subTrees = new Dictionary<string, HashKey>();
			string line = reader.ReadLine();
			
			while (line != null) {
                string[] lineItems = line.Split(new char[] {' '}, StringSplitOptions.RemoveEmptyEntries);
				
                if (IsTreeEntry(lineItems)) {
	                string subTreeDirName = lineItems[2];
	                HashKey subTreeKey = HashKey.ParseFromString(lineItems[1]);
                    subTrees.Add(subTreeDirName, subTreeKey);
                }
                else if (IsBlobEntry(lineItems)) {
	                string blobFileName = lineItems[2];
	                HashKey blobKey = HashKey.ParseFromString(lineItems[1]);
	                blobs.Add(blobFileName, blobKey);
                }
                else {
	                return null;
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

		public Tree(string dirName, IDictionary<string, HashKey> blobs, IDictionary<string, HashKey> subTrees)
		{
			InitBlobs(blobs);
			InitSubTrees(subTrees);
			
			DirName = dirName;
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
		
		/// <summary>
		/// 
		/// </summary>
		/// <param name="dirPath"></param>
		/// <returns>
		/// null when given dirPath does not exist in this Tree.
		/// </returns>
		public Tree FindAndLoadSubTree(string dirPath)
		{
			return FindAndLoadSubTree(GetAllDirParents(dirPath), 0);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="filePath"></param>
		/// <returns>
		/// null when given filePath does not exist in this Tree.
		/// </returns>
		public Blob FindAndLoadBlob(string filePath)
		{
			return FindAndLoadBlob(GetAllDirParents(filePath), 0);
		}

		private void InitBlobs(IDictionary<string, HashKey> blobs)
		{
			if (blobs == null) {
				return;
			}
			
			foreach (KeyValuePair<string, HashKey> pair in blobs) {
				string fileName = pair.Key;
				HashKey key = pair.Value;
				_blobs.Add(fileName, new BlobEntry(key));
			}
		}

		private void InitSubTrees(IDictionary<string, HashKey> trees)
		{
			if (trees == null) {
				return;
			}

			foreach (KeyValuePair<string, HashKey> pair in trees) {
				string dirName = pair.Key;
				HashKey key = pair.Value;
				_subTrees.Add(dirName, new TreeEntry(key));
			}
		}
		
		private string[] GetAllDirParents(string path)
		{
			return path.Split(new char[] {Path.DirectorySeparatorChar});
		}

		private Tree FindAndLoadSubTree(string[] dirHierarchy, int i)
		{
			if (dirHierarchy.Length == 1 || i == dirHierarchy.Length - 1) {
				return this;
			}

			if (!_subTrees.ContainsKey(dirHierarchy[i + 1])) {
				return null;
			}
			
			TreeEntry subTreeEntry = _subTrees[dirHierarchy[i + 1]];
			return subTreeEntry.LoadTree().FindAndLoadSubTree(dirHierarchy, i + 1);
		}

		private Blob FindAndLoadBlob(string[] dirHierarchy, int i)
		{
			if (dirHierarchy.Length == 1) {
				if (!_blobs.ContainsKey(dirHierarchy[0])) {
					return null;
				}
				return _blobs[dirHierarchy[0]].LoadBlob();
			}

			if (i == dirHierarchy.Length - 2) {
				if (!_blobs.ContainsKey(dirHierarchy[i + 1])) {
					return null;
				}
				return _blobs[dirHierarchy[i + 1]].LoadBlob();
			}

			TreeEntry subTreeEntry = _subTrees[dirHierarchy[i + 1]];
			return subTreeEntry.LoadTree().FindAndLoadBlob(dirHierarchy, i + 1);
		}
		
		private string CreateTreeFileContent()
		{
			StringBuilder contentBuilder = new StringBuilder();
			
			contentBuilder.AppendLine(TreeFileType + " " + DirName);
            foreach (BlobEntry blobEntry in _blobs.Values) {
                contentBuilder.AppendLine(blobEntry.Key.ToString());
            }
            foreach (TreeEntry treeEntry in _subTrees.Values) {
                contentBuilder.AppendLine(treeEntry.Key.ToString());
            }

			return contentBuilder.ToString();
		}

		private class TreeEntry {
			private Tree _tree;
			
			public TreeEntry(HashKey key)
			{
				Key = key;
			}
			
			public HashKey Key { get; }

			public Tree LoadTree()
			{
				if (_tree == null) {
                    _tree = ObjectDatabase.RetrieveTree(Key);
				}
				return _tree;
			}
		}

		private class BlobEntry {
			private Blob _blob;

			public BlobEntry(HashKey key)
			{
				Key = key;
			}
			
			public HashKey Key { get; }

			public Blob LoadBlob()
			{
				if (_blob == null) {
					_blob = ObjectDatabase.RetrieveBlob(Key);
				}
				return _blob;
			}
		}
	}
}
