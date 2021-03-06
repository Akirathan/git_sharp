﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using GitSharp.Hash;
using GitSharp.Reference;

namespace GitSharp.Objects {
	/// <summary>
	/// Represents a git commit object.
	/// </summary>
	/// Root tree and parent commit may be loaded on demand with <see cref="LoadTree"/>
	/// or <see cref="LoadParent"/> methods.
	internal class Commit : IStorableGitObject {
		private const string CommitFileType = "commit";

		private const string NullParentKey = "0";
		
        private Tree _tree;
		private Commit _parent;
		private string _commitFileContent;
		private HashKey _checksum;

		public static Commit ParseFromString(string fileContent)
		{
			StringReader reader = new StringReader(fileContent);
			if (reader.ReadLine() != CommitFileType) {
				return null;
			}
			HashKey parentKey = HashKey.ParseFromString(reader.ReadLine());
			HashKey treeKey = HashKey.ParseFromString(reader.ReadLine());
			
			reader.ReadLine();
			string message = reader.ReadToEnd();
			
			if (treeKey == null) {
				return null;
			}
			
			return new Commit(parentKey, treeKey, message);
		}

		/// <param name="parentKey"> null if this is the initial commit </param>
		/// <param name="treeKey"></param>
		/// <param name="message"></param>
		public Commit(HashKey parentKey, HashKey treeKey, string message)
		{
			ParentKey = parentKey;
			TreeKey = treeKey;
			Message = message;
			_commitFileContent = CreateCommitFileContent();
			_checksum = ContentHasher.HashContent(_commitFileContent);
		}
		
		/// <summary>
		/// May be null
		/// </summary>
		public HashKey ParentKey { get;}
		
		public HashKey TreeKey { get; }
		
		public string Message { get; }

		public string GetGitObjectFileContent()
		{
			return _commitFileContent;
		}

		public HashKey GetChecksum()
		{
			return _checksum;
		}

		/// <summary>
		/// Standard checkout as described in "git checkout branch"
		/// </summary>
		/// <returns> false if checkout preconditions were not met. </returns>
		public bool Checkout()
		{
			if (IsHeadCommit()) {
				return true;
			}

			Tree tree = LoadTree();
			if (!tree.Checkout()) {
				return false;
			}

			ProcessRestOfIndexFilesAfterCheckout();
			return true;
		}

		public Tree LoadTree()
		{
			if (_tree == null) {
                _tree = ObjectDatabase.RetrieveTree(TreeKey);
			}
			return _tree;
		}

		public Commit LoadParent()
		{
			if (ParentKey == null) {
				return null;
			}

			if (_parent == null) {
				_parent = ObjectDatabase.RetrieveCommit(ParentKey);
			}

			return _parent;
		}
		
		private void ProcessRestOfIndexFilesAfterCheckout()
		{
			ISet<string> onlyIndexFiles = GetRestOfIndexFiles();
			foreach (string indexFile in onlyIndexFiles) {
				RelativePath indexFilePath = new RelativePath(indexFile);

				if (Index.IsCommited(indexFilePath)) {
					Index.RemoveFile(indexFilePath);
					File.Delete(indexFilePath.GetAbsolutePath());
				}
				else if (Index.IsStaged(indexFilePath)) {
					Index.SetRepoFileContentKey(indexFilePath, "0");
				}
				else if (Index.IsModified(indexFilePath)) {
					Index.SetStageFileContentKey(indexFilePath, "0");
					Index.SetRepoFileContentKey(indexFilePath, "0");
				}
			}
		}

		/// Returns all the files that are remaining in the Index and were not
		/// part of this (checkout) tree.
		private ISet<string> GetRestOfIndexFiles()
		{
			Tree tree = LoadTree();
			
			List<Blob> blobs = new List<Blob>();
			tree.LoadAndGetAllBlobs(blobs);
			List<string> treeFiles = new List<string>();
			foreach (Blob blob in blobs) {
				treeFiles.Add(blob.FilePath);
			}

			ISet<string> onlyIndexFiles = new HashSet<string>(Index.GetAllTrackedFiles());
			onlyIndexFiles.ExceptWith(treeFiles);
			return onlyIndexFiles;
		}

		/// Returns true if this Commit is in the HEAD.
		private bool IsHeadCommit()
		{
			HashKey headCommitKey = ReferenceDatabase.GetHead().GetCommitKey();
			return headCommitKey.Equals(_checksum);
		}

		private string CreateCommitFileContent()
		{
			StringBuilder contentBuilder = new StringBuilder();
			contentBuilder.AppendLine(CommitFileType);
			if (ParentKey == null) {
				contentBuilder.AppendLine(NullParentKey);
			}
			else {
				contentBuilder.AppendLine(ParentKey.ToString());
			}
			contentBuilder.AppendLine(TreeKey.ToString());
			contentBuilder.AppendLine();
			contentBuilder.AppendLine(Message);
			return contentBuilder.ToString();
		}
	}
}