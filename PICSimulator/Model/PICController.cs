using PICSimulator.Helper;
using PICSimulator.Model.Commands;
using PICSimulator.Model.Events;
using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Linq;
using System.Threading;

namespace PICSimulator.Model
{
	class PICController
	{
		public const uint ADDR_PC = 0x02;

		public FrequencyCounter Frequency = new FrequencyCounter();

		private Thread thread;

		public PICControllerMode Mode { get; private set; } // Set to true while running - false when program ended (NOT WHEN PAUSED)

		private PICCommand[] CommandList;

		public ConcurrentQueue<PICEvent> Outgoing_Events = new ConcurrentQueue<PICEvent>();
		public ConcurrentQueue<PICEvent> Incoming_Events = new ConcurrentQueue<PICEvent>();

		private uint[] register = new uint[0xFF];

		private bool[] breakpoints;

		private uint register_W = 0x00;

		public PICController(PICCommand[] cmds)
		{
			Mode = PICControllerMode.WAITING;

			CommandList = cmds;
			breakpoints = new bool[cmds.Length];
		}

		private void run()
		{
			hardResetRegister();
			//register[ADDR_PC] = 0;

			while (Mode != PICControllerMode.FINISHED)
			{
				//################
				//#     MISC     #
				//################

				Frequency.Inc();

				if (Outgoing_Events.Count > 128)
				{
					if (Outgoing_Events.Count > 0)
					{
						Thread.Sleep(0);
						Debug.WriteLine("Event Queque too full ... waiting to clear");
					}
				}

				if (register[ADDR_PC] >= CommandList.Length) // PC > Commandcount
				{
					Mode = PICControllerMode.FINISHED;
					continue;
				}

				HandleIncomingEvents();

				//################
				//#   CONTROL    #
				//################

				if (Mode == PICControllerMode.FINISHED)
				{
					continue;
				}

				if (Mode == PICControllerMode.PAUSED)
				{
					Thread.Sleep(0); // Release Control

					continue;
				}

				if (Mode == PICControllerMode.CONTINUE)
				{
					Mode = PICControllerMode.RUNNING;
				}
				else if (Mode == PICControllerMode.SKIPONE)
				{
					Mode = PICControllerMode.PAUSED;
				}
				else
				{
					if (breakpoints[register[ADDR_PC]])
					{
						Mode = PICControllerMode.PAUSED;
						continue; // Continue so the current Cmd is NOT executed
					}
				}

				//################
				//#    FETCH     #
				//################

				PICCommand cmd = CommandList[register[ADDR_PC]];

				//################
				//# INCREMENT PC #
				//################

				/* DEBUG */
				Thread.Sleep(150);
				/* DEBUG */
				SetRegisterWithEvent(ADDR_PC, register[ADDR_PC] + 1);

				//################
				//#   EXECUTE    #
				//################

				cmd.Execute(this);
			}

			Mode = PICControllerMode.WAITING;
		}

		private void HandleIncomingEvents()
		{
			PICEvent e;
			while (Incoming_Events.TryDequeue(out e))
			{
				if (e is BreakPointChangedEvent)
				{
					BreakPointChangedEvent ce = e as BreakPointChangedEvent;

					breakpoints[ce.Position] = ce.Value;
				}
				else if (e is ChangePICModeEvent)
				{
					ChangePICModeEvent ce = e as ChangePICModeEvent;

					Mode = ce.Value;
				}
				else
				{
					throw new ArgumentException(e.ToString());
				}
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

			Outgoing_Events.Enqueue(new WRegisterChangedEvent() { NewValue = n }); //TODO Register W Changed Event
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

		public void Stop()
		{
			Incoming_Events.Enqueue(new ChangePICModeEvent() { Value = PICControllerMode.FINISHED });
		}

		public long GetSCLineForPC(uint pc)
		{
			return pc < CommandList.Length ? CommandList[pc].SourceCodeLine : -1L;
		}

		public long GetPCLineForSCLine(int sc)
		{
			return (CommandList.Count(p => p.SourceCodeLine == sc) == 1) ? CommandList.Single(p => p.SourceCodeLine == sc).Position : -1L;
		}

		public void RaiseCompleteEventResetChain()
		{
			for (uint i = 0; i < 0xFF; i++)
			{
				SetRegisterWithEvent(i, register[i]);
			}

			SetWRegisterWithEvent(GetWRegister());
		}

		public void Continue()
		{
			Incoming_Events.Enqueue(new ChangePICModeEvent() { Value = PICControllerMode.CONTINUE });
		}

		public void Step()
		{
			Incoming_Events.Enqueue(new ChangePICModeEvent() { Value = PICControllerMode.SKIPONE });
		}
	}
}
