using System.Text;

namespace GitSharp.Hash {
	public class HashKey {
		private byte[] content;
		
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
		public string GetStringRepresentation()
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