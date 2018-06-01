using System.Collections.Generic;

namespace GitSharp.Objects {
	public class Tree : GitObject {
        private List<Tree> _subTrees = new List<Tree>();
	}
}