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
		public const uint _ADDR_PCL = 0x02;			//TODO 02H or 82H ????
		public const uint ADDR_STATUS = 0x03;		//TODO Where is Status ?? 03H or 83H ??
		public const uint _ADDR_PCLATH = 0x0A;		//TODO 0AH or 0AH ???? [CONFUSION INTENSIFIES]

		public const uint ADDR_PORT_A = 0x05;
		public const uint ADDR_TRIS_A = 0x85;

		public const uint ADDR_PORT_B = 0x06;
		public const uint ADDR_TRIS_B = 0x86;

		public const uint STATUS_BIT_IRP = 7;	// Unused in PIC16C84
		public const uint STATUS_BIT_RP1 = 6;	// Unused in PIC16C84
		public const uint STATUS_BIT_RP0 = 5;	// Register Bank Selection Bit
		public const uint STATUS_BIT_TO = 4;	// Time Out Bit
		public const uint STATUS_BIT_PD = 3;	// Power Down Bit
		public const uint STATUS_BIT_Z = 2;		// Zero Bit
		public const uint STATUS_BIT_DC = 1;	// Digit Carry Bit
		public const uint STATUS_BIT_C = 0;		// Carry Bit

		public FrequencyCounter Frequency = new FrequencyCounter(); // Only to see the Performance
		public uint EmulatedFrequency = 4000000; // In Hz

		private Thread thread;

		public PICControllerMode Mode { get; private set; } // Set to true while running - false when program ended (NOT WHEN PAUSED)
		public PICControllerSpeed SimulationSpeed;
		private bool[] breakpoints;

		private PICCommand[] CommandList;

		public ConcurrentQueue<PICEvent> Outgoing_Events = new ConcurrentQueue<PICEvent>();
		public ConcurrentQueue<PICEvent> Incoming_Events = new ConcurrentQueue<PICEvent>();

		private uint[] register = new uint[0xFF];
		private uint register_W = 0x00;

		private uint Cycles = 0; // Passed Controller Cycles

		public PICController(PICCommand[] cmds, PICControllerSpeed s)
		{
			Mode = PICControllerMode.WAITING;
			SimulationSpeed = s;

			CommandList = cmds;
			breakpoints = new bool[cmds.Length];
		}

		private void run()
		{
			Cycles = 0;
			hardResetRegister();

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

				if (GetPC() >= CommandList.Length) // PC > Commandcount
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
					if (breakpoints[GetPC()])
					{
						Mode = PICControllerMode.PAUSED;
						continue; // Continue so the current Cmd is NOT executed
					}
				}

				//################
				//#    FETCH     #
				//################

				PICCommand cmd = CommandList[GetPC()];

				//################
				//# INCREMENT PC #
				//################

				UnreleasedSleep((int)SimulationSpeed);

				SetPCWithEvent_13Bit(GetPC() + 1);

				//################
				//#   EXECUTE    #
				//################

				cmd.Execute(this);
				Cycles += cmd.GetCycleCount(this);
			}

			Mode = PICControllerMode.WAITING;
		}

		private void UnreleasedSleep(int s)
		{
			if (s > 0)
				Thread.Sleep(s);
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

		public void SetRegisterWithEvent(uint p, uint n, bool forceEvent = false)
		{

			n %= 0xFF; // Just 4 Safety

			if (GetRegister(p) != n || forceEvent)
			{
				register[p] = n;
				Outgoing_Events.Enqueue(new RegisterChangedEvent() { RegisterPos = p, NewValue = n });
			}
		}

		public void SetRegisterBitWithEvent(uint p, uint bitpos, bool newVal)
		{
			SetRegisterWithEvent(p, BinaryHelper.SetBit(GetRegister(p), bitpos, newVal));
		}

		public uint GetRegister(uint p)
		{
			return GetRegister(p);
		}

		public void SetWRegisterWithEvent(uint n, bool forceEvent = false)
		{
			n %= 0xFF; // Just 4 Safety

			if (register_W != n || forceEvent)
			{
				register_W = n;
				Outgoing_Events.Enqueue(new WRegisterChangedEvent() { NewValue = n });
			}

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
			SetRegisterWithEvent(ADDR_TRIS_A, 0x1F);
			SetRegisterWithEvent(ADDR_TRIS_B, 0xFF);
		}

		public uint GetPC()
		{
			return (uint)((GetRegister(_ADDR_PCLATH) & ~0x1F) << 8) | GetRegister(_ADDR_PCL);
		}

		public void SetPCWithEvent_13Bit(uint value)
		{
			uint Low = value & 0xFF;
			uint High = (value >> 8) & 0x1F;

			SetRegisterWithEvent(_ADDR_PCL, Low);
			SetRegisterWithEvent(_ADDR_PCLATH, High);

			Outgoing_Events.Enqueue(new PCChangedEvent() { NewValue = value });
		}

		public void SetPCWithEvent_11Bit(uint value)
		{
			uint Low = value & 0xFF;
			uint High = (value >> 8) & 0x1F;

			High |= (GetRegister(_ADDR_PCLATH) & 0x18) << 8;

			SetRegisterWithEvent(_ADDR_PCL, Low);
			SetRegisterWithEvent(_ADDR_PCLATH, High);
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
				SetRegisterWithEvent(i, GetRegister(i), true);
			}

			SetWRegisterWithEvent(GetWRegister(), true);
		}

		public uint GetRunTime() // in us
		{
			return (uint)(Cycles / (EmulatedFrequency / 1000000.0));
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
