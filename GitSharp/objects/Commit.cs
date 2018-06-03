using System;

namespace GitSharp.Objects {
	internal class Commit : GitObject {
        private Tree _tree;

        public Commit(Tree tree, Author author, DateTime dateTime)
        { }

        public Author Author {
	        get { return null; }
        }
	}
}