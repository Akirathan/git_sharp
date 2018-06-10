using System;
using GitSharp.Hash;

namespace GitSharp.Objects {
	internal class Commit : GitObject {
        private Tree _tree;

        public Commit(HashKey parent, Tree tree, DateTime dateTime)
        { }
	}
}