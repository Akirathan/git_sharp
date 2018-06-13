using System;

namespace GitSharp.Commands {
	internal class BranchCommand : Command {
		public BranchCommand(string[] args)
		{
			
		}
		
		public override void Process()
		{
			throw new System.NotImplementedException();
		}

		private void PrintHelp()
		{
			Console.WriteLine("usage: \"branch <name>\"");
		}
	}
}