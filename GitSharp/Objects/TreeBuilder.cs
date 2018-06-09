using System.Collections.Generic;
using System.IO;

namespace GitSharp.Objects {
	/// <summary>
	/// Is not immutable like Tree.
	/// Particular useful for creating tree objects
	/// </summary>
	internal class TreeBuilder {
		private IDictionary<string, Blob> _blobs = new Dictionary<string, Blob>();
		private IDictionary<string, TreeBuilder> _subTrees = new Dictionary<string, TreeBuilder>();
		
		public Tree CreateImmutableTree()
		{
			return null;
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

		private void AddBlob(Blob blob)
		{
			_blobs.Add(blob.FileName, blob);
		}
		
		private string[] GetAllDirParents(string path)
		{
			return path.Split(new char[] {Path.DirectorySeparatorChar});
		}

		/// dirNames may be empty.
		private TreeBuilder FindOrCreateSubTree(string[] dirNames, int i)
		{
			if (dirNames.Length == 0 || i == dirNames.Length - 1) {
				return this;
			}

			if (!_subTrees.ContainsKey(dirNames[i])) {
				_subTrees.Add(dirNames[i], new TreeBuilder());
			}

			return _subTrees[dirNames[i]].FindOrCreateSubTree(dirNames, i + 1);
		}
	}

}