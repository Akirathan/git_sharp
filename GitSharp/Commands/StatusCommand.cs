using System;
using System.Collections.Generic;
using GitSharp.Hash;
using GitSharp.Objects;

namespace GitSharp.Commands {
	internal class StatusCommand : Command {
		public static File.StatusType ResolveFileStatus(string fileName)
		{
			if (!Index.ContainsFile(fileName)) {
				return File.StatusType.Untracked;
			}

            if (Index.IsCommited(fileName)) {
                return File.StatusType.Commited;
            }
            if (Index.IsStaged(fileName)) {
                return File.StatusType.Staged;
            }
			if (Index.IsModified(fileName)) {
				return File.StatusType.Modified;
			}
			
			if (Index.ContainsFile(fileName) && !System.IO.File.Exists(fileName)) {
				return File.StatusType.Deleted;
			}
			
			return File.StatusType.Ignored;
		}
		
        /// <summary>
        /// Traverses all files from working directory and all files from index and
        /// updates the index if file is modified (or deleted).
        /// Finally prints status of all the files.
        /// </summary>
		public override void Process()
		{
            Index.Update();
			
			List<string> untrackedFiles = new List<string>();
			List<string> modifiedFiles = new List<string>();
			List<string> stagedFiles = new List<string>();
			List<string> deletedFiles = new List<string>();
			
			IEnumerable<string> wdirFiles = Traverser.GetAllWdirFiles();
			IEnumerable<string> trackedFiles = Index.GetAllTrackedFiles();
			
			ISet<string> allFiles = new HashSet<string>(wdirFiles);
			allFiles.UnionWith(trackedFiles);
			
			foreach (string file in allFiles) {
				
				switch (ResolveFileStatus(file)) {
					case File.StatusType.Untracked:
						untrackedFiles.Add(file);
						break;
					case File.StatusType.Modified:
						modifiedFiles.Add(file);
						break;
					case File.StatusType.Staged:
						stagedFiles.Add(file);
						break;
                    case File.StatusType.Deleted:
						deletedFiles.Add(file);
	                    break;
					case File.StatusType.Commited:
					case File.StatusType.Ignored:
						break;
					default:
						throw new ArgumentOutOfRangeException();
				}
			}
			
			PrintModifiedAndDeletedFiles(modifiedFiles, deletedFiles);
			PrintStagedFiles(stagedFiles);
			PrintUntrackedFiles(untrackedFiles);
		}
		
		private void PrintModifiedAndDeletedFiles(IList<string> modifiedFiles, IList<string> deletedFiles)
		{
			if (modifiedFiles.Count == 0 && deletedFiles.Count == 0) {
				Console.WriteLine("Working tree clean - no modified files");
			}
			else {
                Console.WriteLine("Changes not staged for commit (modified files): ");
                Console.WriteLine("  (use \"git add <file>\" to update what will be commited)");
                foreach (string modifiedFile in modifiedFiles) {
                    Console.WriteLine($"    modified: {modifiedFile}");
                }
				foreach (string deletedFile in deletedFiles) {
                    Console.WriteLine($"    deleted: {deletedFile}");
				}
			}
		}

		private void PrintStagedFiles(IList<string> stagedFiles)
		{
			if (stagedFiles.Count == 0) {
				Console.WriteLine("no changes added to commit (no staged files)");
			}
			else {
				Console.WriteLine("Changes to be commited (staged files):");
				foreach (string stagedFile in stagedFiles) {
					Console.WriteLine($"    {stagedFile}");
				}
			}
		}

		private void PrintUntrackedFiles(IList<string> untrackedFiles)
		{
			if (untrackedFiles.Count == 0) {
				return;
			}
			
			Console.WriteLine("Untracked files:");
			Console.WriteLine("  (use \"git add <file>\" to include what will be commited)");
			foreach (string untrackedFile in untrackedFiles) {
				Console.WriteLine($"    {untrackedFile}");
			}
		}
	}
}