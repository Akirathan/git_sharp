using System;
using System.Collections.Generic;
using System.IO;
using GitSharp.Hash;

namespace GitSharp.Reference {
	internal static class ReferenceDatabase {
		private static readonly string HeadFilePath =
			Traverser.GetRootDirPath() + Path.DirectorySeparatorChar +
			Traverser.GitRootDirName + Path.DirectorySeparatorChar + "HEAD";

		private static readonly string RefsDirPath =
			Traverser.GetRootDirPath() + Path.DirectorySeparatorChar +
			Traverser.GitRootDirName + Path.DirectorySeparatorChar + "refs";

		private static readonly string BranchDirPath =
			RefsDirPath + Path.DirectorySeparatorChar + "heads";
		
		private static readonly IDictionary<string, Branch> _trackedBranches = new Dictionary<string, Branch>();

		private static Branch _head;

		/// <summary>
		/// Initializes whole directory structure necessary for this ReferenceDatabase
		/// to work.
		/// Called once from "git init".
		/// </summary>
		public static void Init()
		{
			Directory.CreateDirectory(BranchDirPath);
			InitHeadFile();
		}

		/// <summary>
		/// Saves all modified (or created) branches.
		/// Also saves HEAD.
		/// </summary>
		public static void Dispose()
		{
			foreach (Branch branch in _trackedBranches.Values) {
				if (branch.IsModified && BranchPointsToCommit(branch)) {
					SaveBranch(branch);
				}
			}

			if (_head != null && _head.IsModified) {
				SaveHead();
			}
		}

		/// <summary>
		/// Returns current head branch.
		/// If repository was recently initialized, there is just implicit master branch.
		/// </summary>
		/// <returns></returns>
		public static Branch GetHead()
		{
			string headBranchName = ReadFile(HeadFilePath);
			if (headBranchName == "0" && _head == null) {
                _head = CreateBranch("master", null);
			}
			else if (_head == null) {
				_head = GetBranch(headBranchName);
			}
			return _head;
		}

		public static void SetHead(Branch branch)
		{
			_head = branch;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="branchName"></param>
		/// <returns>null when no such branch exists</returns>
		public static Branch GetBranch(string branchName)
		{
			if (!BranchExists(branchName)) {
				return null;
			}
			
			if (!_trackedBranches.ContainsKey(branchName)) {
				Branch branch = ReadBranchFromFile(branchName);
				_trackedBranches.Add(branchName, branch);
			}
			return _trackedBranches[branchName];
		}

		/// Returns true if the given branch points to a commit.
		/// Note that implicitly-created master branch points to a commit once
		/// first commit is created.
		private static bool BranchPointsToCommit(Branch branch)
		{
			return branch.GetCommitKey() != null;
		}

		private static bool BranchExists(string branchName)
		{
			return System.IO.File.Exists(BranchDirPath + Path.DirectorySeparatorChar + branchName);
		}

		public static IEnumerable<Branch> GetAllBranches()
		{
			throw new NotImplementedException();
		}

		public static Branch CreateBranch(string name, HashKey commitKey)
		{
			Branch branch = new Branch(name, commitKey);
			_trackedBranches.Add(name, branch);
			return branch;
		}

		private static void InitHeadFile()
		{
			FileStream fileStream = System.IO.File.Create(HeadFilePath);
			using (StreamWriter writer = new StreamWriter(fileStream)) {
				writer.Write("0");
			}
		}
		
		private static Branch ReadBranchFromFile(string branchName)
		{
            string branchFileContent = ReadFile(BranchDirPath + Path.DirectorySeparatorChar + branchName);
			return Branch.ParseFromString(branchName, branchFileContent);
		}
		
		private static void SaveHead()
		{
			using (StreamWriter writer = new StreamWriter(HeadFilePath)) {
				writer.Write(_head.Name);
			}
		}

		private static void SaveBranch(Branch branch)
		{
			using (StreamWriter writer =
				new StreamWriter(BranchDirPath + Path.DirectorySeparatorChar + branch.Name))
			{
				writer.Write(GetBranchFileContent(branch));
			}
		}

		private static string GetBranchFileContent(Branch branch)
		{
			return branch.GetCommitKey().ToString();
		}

		private static string ReadFile(string fileName)
		{
			string content;
			using (StreamReader reader = new StreamReader(fileName)) {
				content = reader.ReadToEnd();
			}
			return content;
		}
	}
}