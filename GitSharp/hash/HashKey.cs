namespace GitSharp.hash {
	public class HashKey {
		private byte[] content;
		
		public HashKey(byte[] hash)
		{
			content = hash;
		}

		public byte[] Content {
			get { return content; }
		}
	}
}