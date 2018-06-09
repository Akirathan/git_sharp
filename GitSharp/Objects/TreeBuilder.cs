namespace GitSharp.Objects {
	/// <summary>
	/// Is not immutable like Tree.
	/// Particular useful for creating tree objects
	/// </summary>
	internal class TreeBuilder {
		public Tree CreateImmutableTree()
		{
			return null;
		}

		public void AddBlobToTreeHierarchy(Blob blob)
		{
			
		}
	}
}