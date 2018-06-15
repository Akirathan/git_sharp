namespace GitSharp.Objects {
	/// <summary>
	/// Represents git object that can be stored to <see cref="ObjectDatabase"/>.
	/// </summary>
	internal interface IStorableGitObject : IGitObject {
		/// <summary>
		/// Returns a text that will be saved in a git object file.
		/// </summary>
		/// This text will be parsed with <c>ParseFromString</c> methods that
		/// are in different objects.
		/// <returns></returns>
		string GetGitObjectFileContent();
	}
}