namespace GitSharp {
    /// <summary>
    /// Represents enumeration of states that each file in working directory may
    /// be in.
    /// For checking state of specific file, call <see cref="Index.ResolveFileStatus"/>
    /// </summary>
    public enum FileStatus {
        Untracked,
        Modified,
        Staged,
        Commited,
        Deleted,
        Ignored
    }
}