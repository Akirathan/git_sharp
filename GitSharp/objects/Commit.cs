using System;

namespace GitSharp.objects {
	public class Commit : GitObject {
        private Tree _tree;

        public Commit(Tree tree, Author author, DateTime dateTime)
        { }

        public Author Author {
	        get { return null; }
        }
	}
}