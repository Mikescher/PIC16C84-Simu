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
		public const uint ADDR_INDF = 0x00;			// TODO Add indirect Adressing with INDF and FSR (Kap 4.5)
		public const uint ADDR_TMR0 = 0x01;
		public const uint ADDR_PCL = 0x02;
		public const uint ADDR_STATUS = 0x03;
		public const uint ADDR_FSR = 0x04;
		public const uint ADDR_PORT_A = 0x05;
		public const uint ADDR_PORT_B = 0x06;
		public const uint ADDR_PCLATH = 0x0A;
		public const uint ADDR_INTCON = 0x0B;

		public const uint ADDR_OPTION = 0x81;
		public const uint ADDR_TRIS_A = 0x85;
		public const uint ADDR_TRIS_B = 0x86;
		public const uint ADDR_EECON1 = 0x88;
		public const uint ADDR_EECON2 = 0x89;

		public const uint STATUS_BIT_IRP = 7;		// Unused in PIC16C84
		public const uint STATUS_BIT_RP0 = 5;		// Register Bank Selection Bit //TODO Bank Select
		public const uint STATUS_BIT_TO = 4;		// Time Out Bit
		public const uint STATUS_BIT_PD = 3;		// Power Down Bit
		public const uint STATUS_BIT_Z = 2;			// Zero Bit
		public const uint STATUS_BIT_DC = 1;		// Digit Carry Bit
		public const uint STATUS_BIT_C = 0;			// Carry Bit

		public const uint OPTION_BIT_RBPU = 7;		// PORT-B Pull-Up Enable Bit
		public const uint OPTION_BIT_INTEDG = 6;	// Interrupt Edge Select Bit
		public const uint OPTION_BIT_T0CS = 5;		// TMR0 Clock Source Select Bit
		public const uint OPTION_BIT_T0SE = 4;		// TMR0 Source Edge Select Bit
		public const uint OPTION_BIT_PSA = 3;		// Prescaler Alignment Bit
		public const uint OPTION_BIT_PS2 = 2;		// Prescaler Rate Select Bit [2]
		public const uint OPTION_BIT_PS1 = 1;		// Prescaler Rate Select Bit [1]
		public const uint OPTION_BIT_PS0 = 0;		// Prescaler Rate Select Bit [0]

		public const uint INTCON_BIT_GIE = 7;		// Global Interrupt Enable Bit
		public const uint INTCON_BIT_EEIE = 6;		// EE Write Complete Interrupt Enable Bit
		public const uint INTCON_BIT_T0IE = 5;		// TMR0 Overflow Interrupt Enable Bit
		public const uint INTCON_BIT_INTE = 4;		// RB0/INT Interrupt Bit
		public const uint INTCON_BIT_RBIE = 3;		// RB Port Change Interrupt Enable Bit
		public const uint INTCON_BIT_T0IF = 2;		// TMR0 Overflow Interrupt Flag Bit
		public const uint INTCON_BIT_INTF = 1;		// RB0/INT Interrupt Flag Bit
		public const uint INTCON_BIT_RBIF = 0;		// RB Port Change Interrupt Flag Bit

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
		private uint pc_cache; // For ThreadSafe PC Access

		private PICCommand[] CommandList;

		//public ConcurrentQueue<PICEvent> Outgoing_Events = new ConcurrentQueue<PICEvent>();
		public ConcurrentQueue<PICEvent> Incoming_Events = new ConcurrentQueue<PICEvent>();

		private uint[] register = new uint[0xFF];
		private uint register_W = 0x00;
		private CircularStack CallStack = new CircularStack();

		private uint Cycles = 0; // Passed Controller Cycles

		private PICTimer Tmr0;
		private PICInterruptLogic Interrupt;

		public PICController(PICCommand[] cmds, PICControllerSpeed s)
		{
			Tmr0 = new PICTimer();
			Interrupt = new PICInterruptLogic(this);

			Mode = PICControllerMode.WAITING;
			SimulationSpeed = s;

			CommandList = cmds;
			breakpoints = new bool[cmds.Length];
		}

		#region Running

		private void run()
		{
			Cycles = 0;
			HardResetRegister();

			ResetStack();
			ResetInterrupts();

			SetPC_13Bit(0);

			while (Mode != PICControllerMode.FINISHED)
			{
				//################
				//#     MISC     #
				//################

				Frequency.Inc();

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

				SetPC_13Bit(GetPC() + 1);

				//################
				//#   EXECUTE    #
				//################

				cmd.Execute(this);
				Cycles += cmd.GetCycleCount(this);

				//################
				//#   AFTERMATH  #
				//################

				Interrupt.Update();
				Tmr0.Update(this);
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

				SetRegister(ce.Position, ce.Value);
			}
			else
			{
				throw new ArgumentException(e.ToString());
			}
		}

		public void SetRegister(uint p, uint n, bool forceEvent = false)
		{
			n %= 0x100; // Just 4 Safety

			if (GetRegister(p) != n || forceEvent)
			{
				register[p] = n;
			}

			uint? link;

			if ((link = GetLinkedRegister(p)) != null)
			{
				if (register[link.Value] != n)
				{
					SetRegister(link.Value, n);  // NO FORCE !!
				}
			}
		}

		public void SetRegisterBit(uint p, uint bitpos, bool newVal)
		{
			SetRegister(p, BinaryHelper.SetBit(GetRegister(p), bitpos, newVal));
		}

		public bool GetRegisterBit(uint p, uint bitpos)
		{
			return BinaryHelper.GetBit(GetRegister(p), bitpos);
		}

		public uint GetRegister(uint p)
		{
			return register[p];
		}

		public void SetWRegister(uint n, bool forceEvent = false)
		{
			n %= 0x100; // Just 4 Safety

			if (register_W != n || forceEvent)
			{
				register_W = n;
			}

		}

		public uint GetWRegister()
		{
			return register_W;
		}

		private void HardResetRegister()
		{
			for (uint i = 0; i < 0xFF; i++)
			{
				SetRegister(i, 0x00);
			}

			SetRegister(ADDR_STATUS, 0x18);
			SetRegister(ADDR_OPTION, 0xFF);
			SetRegister(ADDR_TRIS_A, 0x1F);
			SetRegister(ADDR_TRIS_B, 0xFF);
		}

		private void SoftResetRegister()
		{
			SetRegister(ADDR_PCL, 0x00);
			SetRegister(ADDR_PCLATH, 0x00);
			SetRegister(ADDR_INTCON, (GetRegister(ADDR_INTCON) & 0x01));
			SetRegister(ADDR_OPTION, 0xFF);
			SetRegister(ADDR_TRIS_A, 0x1F);
			SetRegister(ADDR_TRIS_B, 0xFF);
			SetRegister(ADDR_EECON1, (GetRegister(ADDR_EECON1) & 0x08));
		}

		private void ResetStack()
		{
			CallStack = new CircularStack();
		}

		private void ResetInterrupts()
		{
			Interrupt.Reset();
		}

		public uint GetPC()
		{
			pc_cache = (uint)((GetRegister(ADDR_PCLATH) & ~0x1F) << 8) | GetRegister(ADDR_PCL);
			return pc_cache;
		}

		public void SetPC_13Bit(uint value)
		{
			uint Low = value & 0xFF;
			uint High = (value >> 8) & 0x1F;

			SetRegister(ADDR_PCL, Low);
			SetRegister(ADDR_PCLATH, High);
		}

		public void SetPC_11Bit(uint value)
		{
			value |= (GetRegister(ADDR_PCLATH) & 0x18) << 8;

			SetPC_13Bit(value);
		}

		public void PushCallStack(uint v)
		{
			CallStack.Push(v);
		}

		public uint PopCallStack()
		{
			return CallStack.Pop();
		}

		public void DoInterrupt(PICInterruptType Type)
		{
			Interrupt.AddInterrupt(Type);
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

		public void Pause()
		{
			Incoming_Events.Enqueue(new ChangePICModeEvent() { Value = PICControllerMode.PAUSED });
		}

		#endregion

		#region Helper

		public uint GetThreadSafePC()
		{
			return pc_cache;
		}

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
				SetRegister(i, GetRegister(i), true);
			}

			SetWRegister(GetWRegister(), true);
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

		public Stack<uint> GetThreadSafeCallStack()
		{
			return CallStack.getAsNativeStack();
		}

		#endregion
	}
}
