using System.Security.Cryptography;

namespace GitSharp.Hash {
	public class ContentHasher {
		/// <summary>
		/// Computes SHA-1 hash on <paramref name="content"/>
		/// </summary>
		/// <param name="content"> ASCII-encoded string </param>
		/// <returns></returns>
		public static HashKey HashContent(string content)
		{
			byte[] bytes = new byte[content.Length];
			System.Text.Encoding.ASCII.GetBytes(content, 0, content.Length, bytes, 0);
			
            SHA1 sha = SHA1.Create();
            byte[] hash = sha.ComputeHash(bytes);
			return new HashKey(hash);
		}
	}
}