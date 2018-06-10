using System.Collections.Generic;
using GitSharp.Hash;

namespace GitSharp.Reference {
	internal static class ReferenceDatabase {
		private static IDictionary<string, Branch> _branches;
		
		static ReferenceDatabase()
		{
			
		}

		public static void Dispose()
		{
			
		}

		public static Branch GetHead()
		{
			
		}

		public static void SetHead(Branch branch)
		{
			
		}

		public static Branch GetBranch(string branchName)
		{
			
		}
		
		public static IEnumerable<Branch> GetAllBranches()
		{
			
		}

		public static Branch CreateBranch(string name, HashKey commitKey)
		{
			
		}
	}
}