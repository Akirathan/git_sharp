namespace GitSharp.Objects {
	internal class Blob : GitObject {
		private string content;
		
		public Blob(string content)
		{
			this.content = content;
		}

		public string Content {
			get { return content; }
		}
	}
}