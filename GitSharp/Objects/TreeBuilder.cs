using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using GitSharp.Hash;

namespace GitSharp.Objects {
	/// <summary>
	/// Is not immutable like Tree.
	/// Particular useful for creating tree objects
	/// TODO: add dirName
	/// </summary>
	internal class TreeBuilder : IStorableGitObject {
		private IDictionary<string, Blob> _blobs = new Dictionary<string, Blob>();
		private IDictionary<string, TreeBuilder> _subTrees = new Dictionary<string, TreeBuilder>();
		private string _dirName;
		private string _treeBuilderFileContent;

		public static TreeBuilder CreateRootTreeBuilder()
		{
			return new TreeBuilder(".");
		}
		
		private TreeBuilder(string dirName)
		{
			_dirName = dirName;
		}
		
		public string GetGitObjectFileContent()
		{
			throw new System.NotImplementedException();
		}

		public HashKey GetChecksum()
		{
			throw new System.NotImplementedException();
		}

		/// <summary>
		/// Adds blob into corresponding subtree.
		/// If the subtree (or subtrees) does not exist yet, it is created.
		/// </summary>
		/// <param name="blob"></param>
		public void AddBlobToTreeHierarchy(Blob blob)
		{
			TreeBuilder subTreeBuilder = FindOrCreateSubTree(GetAllDirParents(blob.FileName), 0);
			subTreeBuilder.AddBlob(blob);
		}
		
		public void GetAllBlobsAndSubTrees(List<Blob> allBlobs, List<TreeBuilder> allTrees)
		{
			GetAllSubElements(allBlobs, allTrees);
		}

		private void AddBlob(Blob blob)
		{
			_blobs.Add(blob.FileName, blob);
		}
		
		private string[] GetAllDirParents(string path)
		{
			string[] splittedPath = path.Split(new char[] {Path.DirectorySeparatorChar});
			string[] dirParents = new string[splittedPath.Length - 1];
			Array.Copy(sourceArray: splittedPath, sourceIndex: 0,
				destinationArray: dirParents, destinationIndex: 0,
				length: splittedPath.Length - 1
				);
			return dirParents;
		}

		/// dirNames may be empty.
		private TreeBuilder FindOrCreateSubTree(string[] dirNames, int i)
		{
			if (dirNames.Length == 0 || i == dirNames.Length - 1) {
				return this;
			}

			if (!_subTrees.ContainsKey(dirNames[i])) {
				_subTrees.Add(dirNames[i], new TreeBuilder(dirNames[i]));
			}

			return _subTrees[dirNames[i]].FindOrCreateSubTree(dirNames, i + 1);
		}

		private void GetAllSubElements(List<Blob> allBlobs, List<TreeBuilder> allTrees)
		{
			allBlobs.AddRange(_blobs.Values);
			allTrees.Add(this);
			foreach (TreeBuilder subTree in _subTrees.Values) {
				subTree.GetAllSubElements(allBlobs, allTrees);
			}
		}

		private string CreateTreeBuilderFileContent()
		{
			StringBuilder contentBuilder = new StringBuilder();
			
			contentBuilder.AppendLine(Tree.TreeFileType + " " + _dirName);
			
            foreach (KeyValuePair<string, Blob> pair in _blobs) {
	            HashKey key = pair.Value.GetChecksum();
	            string fileName = pair.Key;
                contentBuilder.AppendLine("blob " + key.ToString() + " " + fileName);
            }
			
            foreach (KeyValuePair<string, TreeBuilder> pair in _subTrees) {
	            HashKey key = pair.Value.GetChecksum();
	            string subTreeDirName = pair.Key;
                contentBuilder.AppendLine("tree " + key.ToString() + " " + subTreeDirName);
            }

			return contentBuilder.ToString();
		}
	}
}