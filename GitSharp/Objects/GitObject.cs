using GitSharp.Hash;

namespace GitSharp.Objects {
	internal abstract class GitObject {
		public abstract string GetGitObjectFileContent();

		public abstract HashKey GetChecksum();
	}
}