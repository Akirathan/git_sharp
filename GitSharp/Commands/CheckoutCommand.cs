using System;

namespace GitSharp.Commands {
	internal class CheckoutCommand : Command {
		public override void Process()
		{
			throw new System.NotImplementedException();
		}

		private void PrintHelp()
		{
			Console.WriteLine("usage: \"checkout <branch>\"");
		}
	}
}