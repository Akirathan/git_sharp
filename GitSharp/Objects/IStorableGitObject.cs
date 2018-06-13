namespace GitSharp.Objects {
	/// <summary>
	/// Represents git object that can be stored to ObjectDatabase
	/// </summary>
	internal interface IStorableGitObject : IGitObject {
		string GetGitObjectFileContent();
	}
}