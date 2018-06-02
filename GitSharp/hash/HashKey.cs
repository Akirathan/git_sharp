using System;
using System.Globalization;
using System.Text;

namespace GitSharp.Hash {
	public class HashKey {
		private byte[] content;

		/// <summary>
		/// Parses a hash key from given string
		/// </summary>
		/// <param name="content">hexadecimal number string</param>
		/// <returns>null if parsing fails</returns>
		public static HashKey ParseFromString(string content)
		{
			if (content.Length % 2 != 0) {
				return null;
			}
			byte[] bytes = new byte[content.Length / 2];
			int bytesIdx = 0;
			
			for (int i = 0; i < content.Length; i += 2) {
				byte b;
				if (!Byte.TryParse(content.Substring(i, 2), NumberStyles.HexNumber, null, out b)) {
					return null;
				}
				bytes[bytesIdx++] = b;
			}
			
			return new HashKey(bytes);
		}
		
		public HashKey(byte[] hash)
		{
			content = hash;
		}

		public byte[] Content {
			get { return content; }
		}

		/// <summary>
		/// Returns hexadecimal string representation (of length 40) of this HashKey.
		/// Note that Base64 encoding is not used due to possible conflicts.
		/// </summary>
		/// <returns> string representing hexadecimal format of this HashKey </returns>
		public override string ToString()
		{
			return GetHexRepresentation(content);
		}

		private string GetHexRepresentation(byte[] bytes)
		{
			StringBuilder sb = new StringBuilder(bytes.Length * 2);
			foreach (byte b in bytes) {
				sb.AppendFormat("{0:x2}", b);
			}
			return sb.ToString();
		}
	}
}