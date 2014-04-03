using PICSimulator.Model.Commands;
using PICSimulator.Model.Events;
using System.Collections.Concurrent;
using System.Threading;

namespace PICSimulator.Model
{
	class PICController
	{
		public const uint ADDR_PC = 0x02;

		private Thread thread;

		public PICControllerMode Mode { get; private set; } // Set to true while running - false when program ended (NOT WHEN PAUSED)

		private PICCommand[] CommandList;

		public ConcurrentQueue<PICEvent> Outgoing_Events = new ConcurrentQueue<PICEvent>();
		public ConcurrentQueue<PICEvent> Incoming_Events = new ConcurrentQueue<PICEvent>();

		private uint[] register = new uint[0xFF];

		private uint register_W = 0x00;

		public PICController(PICCommand[] cmds)
		{
			Mode = PICControllerMode.WAITING;

			CommandList = cmds;
		}

		private void run()
		{
			hardResetRegister();

			while (Mode != PICControllerMode.FINISHED)
			{
				if (Mode == PICControllerMode.PAUSED)
				{
					Thread.Sleep(0); // Release Control

					continue;
				}

				if (register[ADDR_PC] >= CommandList.Length) // PC > Commandcount
				{
					Mode = PICControllerMode.FINISHED;
					continue;
				}

				//################
				//#    FETCH     #
				//################

				PICCommand cmd = CommandList[register[ADDR_PC]];

				//################
				//# INCREMENT PC #
				//################

				SetRegisterWithEvent(ADDR_PC, register[ADDR_PC] + 1);

				//################
				//#   EXECUTE    #
				//################

				cmd.Execute(this);



				/* DEBUG */
				Thread.Sleep(1750);
			}
		}

		public void SetRegisterWithEvent(uint p, uint n)
		{
			register[p] = n % 0xFF; //TODO Interrupt @ Overflow ... oder so ?

			Outgoing_Events.Enqueue(new RegisterChangedEvent() { RegisterPos = p, NewValue = n });
		}

		public uint GetRegister(uint p)
		{
			return register[p];
		}

		public void SetWRegisterWithEvent(uint n)
		{
			register_W = n % 0xFF; //TODO Interrupt @ Overflow ... oder so ?

			//Outgoing_Events.Enqueue(new RegisterChangedEvent() { RegisterPos = p, NewValue = n }); //TODO Register W Changed Event
		}

		public uint GetWRegister()
		{
			return register_W;
		}

		private void hardResetRegister()
		{
			for (uint i = 0; i < 0xFF; i++)
			{
				SetRegisterWithEvent(i, 0x00);
			}

			SetRegisterWithEvent(0x03, 0x18);
			SetRegisterWithEvent(0x81, 0xFF);
			SetRegisterWithEvent(0x85, 0x1F);
			SetRegisterWithEvent(0x86, 0xFF);
		}

		public void Start()
		{
			thread = new Thread(new ThreadStart(run));

			Mode = PICControllerMode.RUNNING;

			thread.Start();
		}
	}
}
