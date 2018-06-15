using GitSharp.Hash;

namespace GitSharp.Objects {
	/// <summary>
	/// Represents git object that can be retrieved from <see cref="ObjectDatabase"/>.
	/// </summary>
	internal interface IGitObject {
		/// <summary>
		/// Returns a key that is used for retrieving this object from <see cref="ObjectDatabase"/>.
		/// </summary>
		/// <returns></returns>
		HashKey GetChecksum();
	}
}