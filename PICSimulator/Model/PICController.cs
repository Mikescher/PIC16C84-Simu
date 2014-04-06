using PICSimulator.Helper;
using PICSimulator.Model.Commands;
using PICSimulator.Model.Events;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;

namespace PICSimulator.Model
{
	public class PICController
	{
		public const uint ADDR_INDF = 0x00;
		public const uint ADDR_PCL = 0x02;
		public const uint ADDR_STATUS = 0x03;
		public const uint ADDR_FSR = 0x04;
		public const uint ADDR_PORT_A = 0x05;
		public const uint ADDR_PORT_B = 0x06;
		public const uint ADDR_PCLATH = 0x0A;
		public const uint ADDR_INTCON = 0x0B;

		public const uint ADDR_TRIS_A = 0x85;
		public const uint ADDR_TRIS_B = 0x86;

		public const uint STATUS_BIT_IRP = 7;	// Unused in PIC16C84
		public const uint STATUS_BIT_RP1 = 6;	// Unused in PIC16C84
		public const uint STATUS_BIT_RP0 = 5;	// Register Bank Selection Bit
		public const uint STATUS_BIT_TO = 4;	// Time Out Bit
		public const uint STATUS_BIT_PD = 3;	// Power Down Bit
		public const uint STATUS_BIT_Z = 2;		// Zero Bit
		public const uint STATUS_BIT_DC = 1;	// Digit Carry Bit
		public const uint STATUS_BIT_C = 0;		// Carry Bit

		public static readonly List<Tuple<uint, uint>> Linked_Register = new List<Tuple<uint, uint>>() 
		{
			Tuple.Create(ADDR_INDF, ADDR_INDF + 0x80),
			Tuple.Create(ADDR_PCL, ADDR_PCL + 0x80),
			Tuple.Create(ADDR_STATUS, ADDR_STATUS + 0x80),
			Tuple.Create(ADDR_FSR, ADDR_FSR + 0x80),
			Tuple.Create(ADDR_PCLATH, ADDR_PCLATH + 0x80),
			Tuple.Create(ADDR_INTCON, ADDR_INTCON + 0x80),
		};

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
		private CircularStack CallStack = new CircularStack();

		private uint Cycles = 0; // Passed Controller Cycles

		public PICController(PICCommand[] cmds, PICControllerSpeed s)
		{
			Mode = PICControllerMode.WAITING;
			SimulationSpeed = s;

			CommandList = cmds;
			breakpoints = new bool[cmds.Length];
		}

		#region Running

		private void run()
		{
			Cycles = 0;
			hardResetRegister();
			ResetStack();

			SetPCWithEvent_13Bit(0);

			while (Mode != PICControllerMode.FINISHED)
			{
				//################
				//#     MISC     #
				//################

				Frequency.Inc();

				if (Outgoing_Events.Count > 128)
				{
					while (Outgoing_Events.Count > 0)
					{
						Thread.Sleep(0);
						Debug.WriteLine("Event Queque too full ... waiting to clear");
					}
					Thread.Sleep(10);
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
				HandleEvent(e);
			}
		}

		private void HandleEvent(PICEvent e)
		{
			Debug.WriteLine("[EVENT::FROM_VIEW] " + e);

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
			else if (e is ManuallyRegisterChangedEvent)
			{
				ManuallyRegisterChangedEvent ce = e as ManuallyRegisterChangedEvent;

				SetRegisterWithEvent(ce.Position, ce.Value);
			}
			else
			{
				throw new ArgumentException(e.ToString());
			}
		}

		public void SetRegisterWithEvent(uint p, uint n, bool forceEvent = false)
		{
			n %= 0xFF; // Just 4 Safety

			if (GetRegister(p) != n || forceEvent)
			{
				register[p] = n;
				Outgoing_Events.Enqueue(new RegisterChangedEvent() { Position = p, Value = n });
			}

			uint? link;

			if ((link = GetLinkedRegister(p)) != null)
			{
				if (register[link.Value] != n)
				{
					SetRegisterWithEvent(link.Value, n);  // NO FORCE !!
				}
			}
		}

		public void SetRegisterBitWithEvent(uint p, uint bitpos, bool newVal)
		{
			SetRegisterWithEvent(p, BinaryHelper.SetBit(GetRegister(p), bitpos, newVal));
		}

		public uint GetRegister(uint p)
		{
			return register[p];
		}

		public void SetWRegisterWithEvent(uint n, bool forceEvent = false)
		{
			n %= 0xFF; // Just 4 Safety

			if (register_W != n || forceEvent)
			{
				register_W = n;
				Outgoing_Events.Enqueue(new WRegisterChangedEvent() { Value = n });
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

		private void ResetStack()
		{
			CallStack = new CircularStack();
			Outgoing_Events.Enqueue(new StackResetEvent());
		}

		public uint GetPC()
		{
			return (uint)((GetRegister(ADDR_PCLATH) & ~0x1F) << 8) | GetRegister(ADDR_PCL);
		}

		public void SetPCWithEvent_13Bit(uint value)
		{
			uint Low = value & 0xFF;
			uint High = (value >> 8) & 0x1F;

			SetRegisterWithEvent(ADDR_PCL, Low);
			SetRegisterWithEvent(ADDR_PCLATH, High);

			Outgoing_Events.Enqueue(new PCChangedEvent() { Value = value });
		}

		public void SetPCWithEvent_11Bit(uint value)
		{
			value |= (GetRegister(ADDR_PCLATH) & 0x18) << 8;

			SetPCWithEvent_13Bit(value);
		}

		public void PushCallStack(uint v)
		{
			Outgoing_Events.Enqueue(new PushCallStackEvent() { Value = v });

			CallStack.Push(v);
		}

		public uint PopCallStack()
		{
			Outgoing_Events.Enqueue(new PopCallStackEvent());

			return CallStack.Pop();
		}

		#endregion

		#region Control

		public void Start()
		{
			thread = new Thread(new ThreadStart(run));

			Mode = PICControllerMode.RUNNING;

			thread.Start();
		}

		public void StartPaused()
		{
			thread = new Thread(new ThreadStart(run));

			Mode = PICControllerMode.PAUSED;

			thread.Start();
		}

		public void Stop()
		{
			Incoming_Events.Enqueue(new ChangePICModeEvent() { Value = PICControllerMode.FINISHED });
		}

		public void Continue()
		{
			Incoming_Events.Enqueue(new ChangePICModeEvent() { Value = PICControllerMode.CONTINUE });
		}

		public void Step()
		{
			Incoming_Events.Enqueue(new ChangePICModeEvent() { Value = PICControllerMode.SKIPONE });
		}

		#endregion

		#region Helper

		public long GetSCLineForPC(uint pc)
		{
			return pc < CommandList.Length ? CommandList[pc].SourceCodeLine : -1L;
		}

		public long GetPCLineForSCLine(int sc)
		{
			return (CommandList.Count(p => p.SourceCodeLine == sc) == 1) ? CommandList.Single(p => p.SourceCodeLine == sc).Position : -1L;
		}

		public string GetSourceCodeForPC(uint pc)
		{
			return pc < CommandList.Length ? CommandList[pc].SourceCodeText : "";
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

		public uint? GetLinkedRegister(uint r)
		{
			// One Expression to rule them all.
			return Linked_Register.Count(p => (p.Item1 == r || p.Item2 == r)) == 1 ? (Linked_Register.Where(p => (p.Item1 == r || p.Item2 == r)).Select(p => p.Item1 + p.Item2).Single() - r) : ((uint?)null);
		}

		#endregion
	}
}
