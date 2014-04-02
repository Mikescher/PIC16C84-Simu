using PICSimulator.Model.Commands;
using PICSimulator.Model.Events;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;

namespace PICSimulator.Model
{
	class PICController
	{
		private Thread thread;

		public bool isRunning { get; private set; }

		private List<PICCommand> CommandList;

		public ConcurrentQueue<PICEvent> Outgoing_Events = new ConcurrentQueue<PICEvent>();
		public ConcurrentQueue<PICEvent> Incoming_Events = new ConcurrentQueue<PICEvent>();

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

					// Check for Incoming Events

					continue;
				}

				//Do Smth
			}
		}
	}
}
