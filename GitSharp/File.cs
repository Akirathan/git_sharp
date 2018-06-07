using System;

namespace GitSharp {
	/// <summary>
	/// Wrapper class for System.IO.File.
	/// This class is merely a wrapper for System.IO.File, it does not track
	/// its own status - this is done by FileTracker.
	/// </summary>
	internal class File : IEquatable<File> {
		public enum StatusType {
			Untracked,
			Modified,
			Staged,
			Commited,
			Ignored
		}

		public File()
		{
			Status = StatusType.Untracked;
		}
		
		public string Name { get; private set; }
		
		public StatusType Status { get; set; }

		public bool Equals(File otherFile)
		{
			if (otherFile == null) {
				return false;
			}
			return Name.Equals(otherFile.Name);
		}
	}
}