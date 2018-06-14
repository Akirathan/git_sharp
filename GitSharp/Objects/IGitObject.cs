using GitSharp.Hash;

namespace GitSharp.Objects {
	/// <summary>
	/// Represents git object that can be retrieved from ObjectDatabase
	/// </summary>
	internal interface IGitObject {
		HashKey GetChecksum();
	}
}