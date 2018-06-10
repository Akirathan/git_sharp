using System.Collections.Generic;
using System.Text;
using GitSharp.Hash;

namespace GitSharp.Objects {
	internal static class TreePrinter {
		private const string TreeFileType = "tree";
		
		public static string PrintToString(string dirName, IDictionary<string, HashKey> blobs,
			IDictionary<string, HashKey> subTrees)
		{
			StringBuilder contentBuilder = new StringBuilder();
			
			contentBuilder.AppendLine(TreeFileType + " " + dirName);
			
            foreach (KeyValuePair<string, HashKey> pair in blobs) {
	            HashKey key = pair.Value;
	            string fileName = pair.Key;
                contentBuilder.AppendLine("blob " + key.ToString() + " " + fileName);
            }
			
            foreach (KeyValuePair<string, HashKey> pair in subTrees) {
	            HashKey key = pair.Value;
	            string subTreeDirName = pair.Key;
                contentBuilder.AppendLine("tree " + key.ToString() + " " + subTreeDirName);
            }

			return contentBuilder.ToString();
		}
	}
}