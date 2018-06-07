using System;
using System.IO;
using System.Security.Cryptography;

namespace GitSharp.Hash {
	public class ContentHasher {
		public static HashKey hash(string content)
		{
			byte[] bytes = new byte[content.Length];
			System.Text.ASCIIEncoding.ASCII.GetBytes(content, 0, content.Length, bytes, 0);
			
            SHA1 sha = SHA1.Create();
            byte[] hash = sha.ComputeHash(bytes);
			return new HashKey(hash);
		}

		public static HashKey hashFileContent(string fileName)
		{
			string content;
			StreamReader reader = null;
			try {
				reader = new StreamReader(fileName);
				content = reader.ReadToEnd();
			}
			catch (IOException e) {
				throw new Exception($"Error when reading file {fileName}", e);
			}
			finally {
				if (reader != null) {
					reader.Close();
				}
			}

			return hash(content);
		}
	}
}