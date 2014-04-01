
using PICSimulator.Model.Commands;
using System.Collections.Generic;
using System.Threading;
namespace PICSimulator.Model
{
	class PICController
	{
		private Thread thread;

		public bool isRunning { get; private set; }

		private List<PICCommand> CommandList;

		public PICController(List<PICCommand> cmds)
		{
			isRunning = false;
			CommandList = cmds;

			thread = new Thread(new ThreadStart(run));
		}

		private void run()
		{
			for (; ; ) // Endless
			{
				if (!isRunning)
				{
					Thread.Sleep(0); // Release Control
					continue;
				}

				//Do Smth
			}
		}
	}
}
