using GitSharp.Hash;

namespace GitSharp.Objects {
	/// <summary>
	/// Represents git object that can be retrieved from ObjectDatabase
	/// </summary>
	internal interface IGitObject {
		HashKey GetChecksum();
		/// <summary>
		/// Standard checkout as described in "git checkout branch"
		/// </summary>
		/// <returns>false if checkout preconditions were not met.</returns>
		bool Checkout();
	}
}