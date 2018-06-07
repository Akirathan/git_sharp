using System;
using GitSharp.Hash;

namespace GitSharp.Objects {
	internal class Commit : GitObject {
        private Tree _tree;

        public Commit(HashKey parent, Tree tree, Author author, DateTime dateTime)
        { }

        public Author Author {
	        get { return null; }
        }
	}
}