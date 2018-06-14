using System;
using System.IO;
using System.Text;
using GitSharp.Hash;

namespace GitSharp.Objects {
	/// <summary>
	/// root tree and parent commits are retrieved on demand.
	/// </summary>
	internal class Commit : IStorableGitObject {
		private const string CommitFileType = "commit";

		private const string NullParentKey = "0";
		
		// Lazyli-loaded root tree of this commit
        private Tree _tree;
		private Commit _parent;
		private string _commitFileContent;
		private HashKey _checksum;

		public static Commit ParseFromString(string fileContent)
		{
			StringReader reader = new StringReader(fileContent);
			if (reader.ReadLine() != CommitFileType) {
				return null;
			}
			HashKey parentKey = HashKey.ParseFromString(reader.ReadLine());
			HashKey treeKey = HashKey.ParseFromString(reader.ReadLine());
			
			reader.ReadLine();
			string message = reader.ReadToEnd();
			
			if (treeKey == null) {
				return null;
			}
			
			return new Commit(parentKey, treeKey, message);
		}

		public Commit(HashKey parentKey, HashKey treeKey, string message)
		{
			ParentKey = parentKey;
			TreeKey = treeKey;
			Message = message;
			_commitFileContent = CreateCommitFileContent();
			_checksum = ContentHasher.HashContent(_commitFileContent);
		}
		
		/// <summary>
		/// May be null
		/// </summary>
		public HashKey ParentKey { get;}
		
		public HashKey TreeKey { get; }
		
		public string Message { get; }

		public string GetGitObjectFileContent()
		{
			return _commitFileContent;
		}

		public HashKey GetChecksum()
		{
			return _checksum;
		}

		/// <summary>
		/// Standard checkout as described in "git checkout branch"
		/// </summary>
		/// <returns>false if checkout preconditions were not met.</returns>
		public bool Checkout()
		{
			return LoadTree().Checkout();
		}

		public Tree LoadTree()
		{
			if (_tree == null) {
                _tree = ObjectDatabase.RetrieveTree(TreeKey);
			}
			return _tree;
		}

		public Commit LoadParent()
		{
			if (ParentKey == null) {
				return null;
			}

			if (_parent == null) {
				_parent = ObjectDatabase.RetrieveCommit(ParentKey);
			}

			return _parent;
		}

		private string CreateCommitFileContent()
		{
			StringBuilder contentBuilder = new StringBuilder();
			contentBuilder.AppendLine(CommitFileType);
			if (ParentKey == null) {
				contentBuilder.AppendLine(NullParentKey);
			}
			else {
				contentBuilder.AppendLine(ParentKey.ToString());
			}
			contentBuilder.AppendLine(TreeKey.ToString());
			contentBuilder.AppendLine();
			contentBuilder.AppendLine(Message);
			return contentBuilder.ToString();
		}
	}
}