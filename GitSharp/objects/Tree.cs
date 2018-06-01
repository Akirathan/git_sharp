using System.Collections.Generic;

namespace GitSharp.objects {
	public class Tree : GitObject {
        private List<Tree> _subTrees = new List<Tree>();
	}
}