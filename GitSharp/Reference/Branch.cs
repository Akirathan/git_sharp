using GitSharp.Hash;
using GitSharp.Objects;

namespace GitSharp.Reference {
	internal class Branch {
		public static Branch ParseFromString(string content)
		{
			return null;
		}

		public string Name { get; }
		public bool IsModified { get; private set; }
		private HashKey _commitKey;
		private Commit _commit;

		/// <summary>
		/// 
		/// </summary>
		/// <param name="name"></param>
		/// <param name="commitKey">may be null</param>
		public Branch(string name, HashKey commitKey)
		{
			Name = name;
			IsModified = false;
			_commitKey = commitKey;
		}

		public HashKey GetCommitKey()
		{
			return _commitKey;
		}

		public void SetCommitKey(HashKey commitKey)
		{
			_commitKey = commitKey;
			IsModified = true;
		}

		public Commit LoadCommit()
		{
			return null;
		}
	}
}