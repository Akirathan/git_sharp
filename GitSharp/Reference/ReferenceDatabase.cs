using System;
using System.Collections.Generic;
using System.IO;
using GitSharp.Hash;

namespace GitSharp.Reference {
	internal static class ReferenceDatabase {
		private static readonly string HeadFilePath =
			Traverser.GitRootDirName + Path.DirectorySeparatorChar + "HEAD";

		private static readonly string RefsDirPath =
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
				if (branch.IsModified) {
					// TODO: save branch.
				}
			}

			if (_head != null && _head.IsModified) {
				SaveBranch(_head);
			}
		}

		/// <summary>
		/// Returns current head branch.
		/// If repository was recently initialized, there is just implicit master branch.
		/// </summary>
		/// <returns></returns>
		public static Branch GetHead()
		{
			string headContent = ReadFile(HeadFilePath);
			if (headContent == "0" && _head == null) {
                _head = CreateBranch("master", null);
			}
			else if (_head == null) {
				_head = GetBranch(headContent);
			}
			return _head;
		}

		public static void SetHead(Branch branch)
		{
			_head = branch;
		}

		public static Branch GetBranch(string branchName)
		{
			if (!_trackedBranches.ContainsKey(branchName)) {
				Branch branch = ReadBranchFromFile(branchName);
				_trackedBranches.Add(branchName, branch);
			}
			return _trackedBranches[branchName];
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
			return Branch.ParseFromString(branchFileContent);
		}
		
		private static void SaveBranch(Branch branch)
		{
			using (StreamWriter writer =
				new StreamWriter(BranchDirPath + Path.DirectorySeparatorChar + branch.Name))
			{
				writer.Write(branch.GetCommitKey().ToString());
			}
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