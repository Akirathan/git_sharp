using GitSharp.Hash;
using GitSharp.Objects;

namespace GitSharp.Reference {
	internal class Branch {
		public static Branch ParseFromString(string content)
		{
			return null;
		}

		public HashKey CommitKey { get; }
		public string Name { get; }
		private Commit _commit;

		public Branch(string name, HashKey commitKey)
		{
			
		}

		public Commit LoadCommit()
		{
			return null;
		}
	}
}