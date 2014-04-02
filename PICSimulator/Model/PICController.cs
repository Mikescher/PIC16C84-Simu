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

		public bool isRunning { get; private set; } // Set to true while running - false when program ended (NOT WHEN PAUSED)

		public bool isPaused { get; private set; }

		private List<PICCommand> CommandList;

		public ConcurrentQueue<PICEvent> Outgoing_Events = new ConcurrentQueue<PICEvent>();
		public ConcurrentQueue<PICEvent> Incoming_Events = new ConcurrentQueue<PICEvent>();

		private uint[] register = new uint[0xFF];

		public PICController(List<PICCommand> cmds)
		{
			isRunning = false;
			CommandList = cmds;
		}

		private void run()
		{
			hardResetRegister();

			while (isRunning)
			{
				if (!isPaused)
				{
					Thread.Sleep(0); // Release Control

					continue;
				}

				//Do Smth
			}
		}

		private void setRegisterWithEvent(uint p, uint n)
		{
			register[p] = n;

			Outgoing_Events.Enqueue(new RegisterChangedEvent() { RegisterPos = p, NewValue = n });
		}

		private void hardResetRegister()
		{
			for (uint i = 0; i < 0xFF; i++)
			{
				setRegisterWithEvent(i, 0x00);
			}

			setRegisterWithEvent(0x03, 0x18);
			setRegisterWithEvent(0x81, 0xFF);
			setRegisterWithEvent(0x85, 0x1F);
			setRegisterWithEvent(0x86, 0xFF);
		}

		public void Start()
		{
			thread = new Thread(new ThreadStart(run));

			thread.Start();

			isRunning = true;
		}
	}
}
