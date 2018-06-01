using System.Security.Cryptography;

namespace GitSharp.hash {
	public class ContentHasher {
		public static HashKey hash(string content)
		{
			byte[] bytes = new byte[content.Length];
			System.Text.ASCIIEncoding.ASCII.GetBytes(content, 0, content.Length, bytes, 0);
			
            SHA1 sha = SHA1.Create();
            byte[] hash = sha.ComputeHash(bytes);
			return new HashKey(hash);
		}
	}
}