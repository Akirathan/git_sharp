using System.Collections.Generic;
using GitSharp.Hash;

namespace GitSharp.Reference {
	internal static class ReferenceDatabase {
		private static readonly IDictionary<string, Branch> _branches = new Dictionary<string, Branch>();
		
		static ReferenceDatabase()
		{
			
		}

		/// <summary>
		/// Saves all modified (or created) branches.
		/// </summary>
		public static void Dispose()
		{
			foreach (Branch branch in _branches.Values) {
				if (branch.IsModified) {
					// TODO: save branch.
				}
			}
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
			Branch branch = new Branch(name, commitKey);
			_branches.Add(name, branch);
			return branch;
		}
	}
}