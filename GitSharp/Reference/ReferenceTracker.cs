using GitSharp.Hash;

namespace GitSharp.Reference {
	internal static class ReferenceTracker {
		static ReferenceTracker()
		{
			
		}
		
		public static string CurrentBranchName { get; private set; }
		
		public static HashKey GetCurrentCommitKey()
		{
			return null;
		}
	}
}