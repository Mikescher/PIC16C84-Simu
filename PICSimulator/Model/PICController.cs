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
	public sealed class PICController
	{
		public FrequencyCounter Frequency = new FrequencyCounter(); // Only to see the Performance
		public uint EmulatedFrequency = 4000000; // In Hz

		private Thread thread;

		public PICControllerMode Mode { get; private set; } // Set to true while running - false when program ended (NOT WHEN PAUSED)
		public PICControllerSpeed SimulationSpeed;
		private bool[] breakpoints;
		private uint pc_cache; // For ThreadSafe PC Access

		private PICCommand[] CommandList;

		public ConcurrentQueue<PICEvent> Incoming_Events = new ConcurrentQueue<PICEvent>();

		private PICMemory Memory;
		private uint register_W = 0x00;
		private CircularStack CallStack = new CircularStack();

		private uint Cycles = 0; // Passed Controller Cycles

		private PICTimer Tmr0;
		private PICInterruptLogic Interrupt;

		public PICController(PICCommand[] cmds, PICControllerSpeed s)
		{
			Tmr0 = new PICTimer();
			Interrupt = new PICInterruptLogic(this);
			Memory = new PICMemory(Tmr0, Interrupt);

			Mode = PICControllerMode.WAITING;
			SimulationSpeed = s;

			CommandList = cmds;
			breakpoints = new bool[cmds.Length];
		}

		#region Running

		private void run()
		{
			Cycles = 0;
			Memory.HardResetRegister();

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

		public uint GetRegister(uint p)
		{
			return Memory.GetRegister(p);
		}

		public void SetRegister(uint p, uint n)
		{
			Memory.SetRegister(p, n);
		}

		public void SetRegisterBit(uint p, uint bitpos, bool newVal)
		{
			Memory.SetRegisterBit(p, bitpos, newVal);
		}

		public bool GetRegisterBit(uint p, uint bitpos)
		{
			return Memory.GetRegisterBit(p, bitpos);
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

		private void ResetStack()
		{
			CallStack = new CircularStack();
		}

		private void ResetInterrupts()
		{
			Interrupt.Reset();
		}

		public uint GetPC() // TODO FIX PC --> Move 2 Memory
		{
			pc_cache = (uint)((GetRegister(PICMemory.ADDR_PCLATH) & ~0x1F) << 8) | GetRegister(PICMemory.ADDR_PCL);
			return pc_cache;
		}

		public void SetPC_13Bit(uint value)
		{
			uint Low = value & 0xFF;
			uint High = (value >> 8) & 0x1F;

			SetRegister(PICMemory.ADDR_PCL, Low);
			SetRegister(PICMemory.ADDR_PCLATH, High);
		}

		public void SetPC_11Bit(uint value)
		{
			value |= (GetRegister(PICMemory.ADDR_PCLATH) & 0x18) << 8;

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
				SetRegister(i, GetRegister(i));
			}

			SetWRegister(GetWRegister());
		}

		public uint GetRunTime() // in us
		{
			return (uint)(Cycles / (EmulatedFrequency / 1000000.0));
		}

		public Stack<uint> GetThreadSafeCallStack()
		{
			return CallStack.getAsNativeStack();
		}

		#endregion
	}
}
