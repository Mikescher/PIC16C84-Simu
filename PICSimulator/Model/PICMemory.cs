using PICSimulator.Helper;
using System;
using System.Collections.Generic;

namespace PICSimulator.Model
{
	class PICMemory
	{
		public const uint ADDR_INDF = 0x00;
		public const uint ADDR_TMR0 = 0x01;
		public const uint ADDR_PCL = 0x02;
		public const uint ADDR_STATUS = 0x03;
		public const uint ADDR_FSR = 0x04;
		public const uint ADDR_PORT_A = 0x05;
		public const uint ADDR_PORT_B = 0x06;
		public const uint ADDR_UNIMPL_A = 0x07;
		public const uint ADDR_PCLATH = 0x0A;
		public const uint ADDR_INTCON = 0x0B;

		public const uint ADDR_OPTION = 0x81;
		public const uint ADDR_TRIS_A = 0x85;
		public const uint ADDR_TRIS_B = 0x86;
		public const uint ADDR_UNIMPL_B = 0x87;
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

		public delegate uint RegisterRead(uint Pos);
		public delegate void RegisterWrite(uint Pos, uint Value);

		private readonly Dictionary<uint, Tuple<RegisterRead, RegisterWrite>> SpecialRegisterEvents;

		private uint pc = 0;
		private uint[] register = new uint[0xFF];

		private PICTimer Timer;
		private PICInterruptLogic Interrupt;

		public PICMemory(PICTimer tmr, PICInterruptLogic il)
		{
			this.Timer = tmr;
			this.Interrupt = il;

			SpecialRegisterEvents = new Dictionary<uint, Tuple<RegisterRead, RegisterWrite>>() 
			{
				#region Linked Register && PC

				//##############################################################################
				{
					ADDR_PCL, 
					Tuple.Create<RegisterRead, RegisterWrite>(
						GetRegisterDirect, 
						(p, v) => { SetRegisterDirect(p, v); SetRegisterDirect(p+0x80, v); UpdatePC(v); })
				}, 
				{
					ADDR_PCL + 0x80, 
					Tuple.Create<RegisterRead, RegisterWrite>(
						GetRegisterDirect, 
						(p, v) => { SetRegisterDirect(p, v); SetRegisterDirect(p-0x80, v); UpdatePC(v); })
				}, 
				//##############################################################################
				{
					ADDR_STATUS, 
					Tuple.Create<RegisterRead, RegisterWrite>(
						GetRegisterDirect, 
						(p, v) => { SetRegisterDirect(p, v); SetRegisterDirect(p+0x80, v); })
				}, 
				{
					ADDR_STATUS + 0x80, 
					Tuple.Create<RegisterRead, RegisterWrite>(
						GetRegisterDirect, 
						(p, v) => { SetRegisterDirect(p, v); SetRegisterDirect(p-0x80, v); })
				}, 
				//##############################################################################
				{
					ADDR_FSR, 
					Tuple.Create<RegisterRead, RegisterWrite>(
						GetRegisterDirect, 
						(p, v) => { SetRegisterDirect(p, v); SetRegisterDirect(p+0x80, v); })
				}, 
				{
					ADDR_FSR + 0x80, 
					Tuple.Create<RegisterRead, RegisterWrite>(
						GetRegisterDirect, 
						(p, v) => { SetRegisterDirect(p, v); SetRegisterDirect(p-0x80, v); })
				}, 
				//##############################################################################
				{
					ADDR_PCLATH, 
					Tuple.Create<RegisterRead, RegisterWrite>(
						GetRegisterDirect, 
						(p, v) => { SetRegisterDirect(p, v); SetRegisterDirect(p+0x80, v); })
				}, 
				{
					ADDR_PCLATH + 0x80, 
					Tuple.Create<RegisterRead, RegisterWrite>(
						GetRegisterDirect, 
						(p, v) => { SetRegisterDirect(p, v); SetRegisterDirect(p-0x80, v); })
				}, 
				//##############################################################################
				{
					ADDR_INTCON, 
					Tuple.Create<RegisterRead, RegisterWrite>(
						GetRegisterDirect, 
						(p, v) => { SetRegisterDirect(p, v); SetRegisterDirect(p+0x80, v); })
				},
				{
					ADDR_INTCON + 0x80, 
					Tuple.Create<RegisterRead, RegisterWrite>(
						GetRegisterDirect, 
						(p, v) => { SetRegisterDirect(p, v); SetRegisterDirect(p-0x80, v); })
				},

				#endregion
			
				#region Unimplemented

				{
					ADDR_UNIMPL_A,	
					Tuple.Create<RegisterRead, RegisterWrite>(
						(p) => 0, 
						(p, v) => { /* NOP */ })
				}, 
				{
					ADDR_UNIMPL_B,	
					Tuple.Create<RegisterRead, RegisterWrite>(
						(p) => 0, 
						(p, v) => { /* NOP */ })
				}, 

				#endregion
			
				#region RB0/INT Interrupt + Port RB Interrupt

				{
					ADDR_PORT_B,	
					Tuple.Create<RegisterRead, RegisterWrite>(
						GetRegisterDirect, 
						(p, v) => { Do_Interrupt_ADDR_PORT_B(v); SetRegisterDirect(p, v); })
				}, 

				#endregion

				#region Indirect Addressing

				{
					ADDR_INDF,	
					Tuple.Create<RegisterRead, RegisterWrite>(
						(p) => 
						{
							return (GetRegister(ADDR_FSR) % 0x80 == 0) ? (0) : (GetRegister(GetRegister(ADDR_FSR)));
						}, 
						(p, v) => 
						{
							if (GetRegister(ADDR_FSR) % 0x80 != 0) 
								SetRegister(GetRegister(ADDR_FSR), v);
						})
				}, 

				{
					ADDR_INDF + 0x80,	
					Tuple.Create<RegisterRead, RegisterWrite>(
						(p) => 
						{
							return (GetRegister(ADDR_FSR) % 0x80 == 0) ? (0) : (GetRegister(GetRegister(ADDR_FSR)));
						}, 
						(p, v) => 
						{
							if (GetRegister(ADDR_FSR) % 0x80 != 0) 
								SetRegister(GetRegister(ADDR_FSR), v);
						})
				}, 

				#endregion
			};
		}

		#region Internal

		private void Do_Interrupt_ADDR_PORT_B(uint val)
		{
			uint changes = (register[ADDR_PORT_B] ^ val) & 0xFF;

			// RB0/INT

			if (BinaryHelper.GetBit(changes, 0))
			{
				if (BinaryHelper.GetBit(register[ADDR_OPTION], OPTION_BIT_INTEDG) && BinaryHelper.GetBit(val, 0)) // Rising Edge
				{
					Interrupt.AddInterrupt(PICInterruptType.PIT_RB0INT);
				}
				else if (!BinaryHelper.GetBit(register[ADDR_OPTION], OPTION_BIT_INTEDG) && !BinaryHelper.GetBit(val, 0)) // Falling Edge
				{
					Interrupt.AddInterrupt(PICInterruptType.PIT_RB0INT);
				}
			}

			// PORT RB

			if (BinaryHelper.GetBit(changes, 4) || BinaryHelper.GetBit(changes, 5) || BinaryHelper.GetBit(changes, 6) || BinaryHelper.GetBit(changes, 7))
			{
				Interrupt.AddInterrupt(PICInterruptType.PIT_PORTB);
			}
		}

		#endregion

		#region Getter/Setter

		public uint GetRegister(uint p)
		{
			if (SpecialRegisterEvents.ContainsKey(p))
			{
				return SpecialRegisterEvents[p].Item1(p);
			}
			else
			{
				return GetRegisterDirect(p);
			}
		}

		public void SetRegister(uint p, uint n)
		{
			if (SpecialRegisterEvents.ContainsKey(p))
			{
				SpecialRegisterEvents[p].Item2(p, n);
			}
			else
			{
				SetRegisterDirect(p, n);
			}
		}

		protected uint GetRegisterDirect(uint p)
		{
			return register[p];
		}

		protected void SetRegisterDirect(uint p, uint n)
		{
			n %= 0x100; // Just 4 Safety

			register[p] = n;
		}

		public void SetRegisterBit(uint p, uint bitpos, bool newVal)
		{
			SetRegister(p, BinaryHelper.SetBit(GetRegister(p), bitpos, newVal));
		}

		public bool GetRegisterBit(uint p, uint bitpos)
		{
			return BinaryHelper.GetBit(GetRegister(p), bitpos);
		}

		#endregion

		#region Helper

		public void HardResetRegister()
		{
			for (uint i = 0; i < 0xFF; i++)
			{
				SetRegister(i, 0x00);
			}

			SetRegister(ADDR_PCL, 0x00);
			SetRegister(ADDR_STATUS, 0x18);
			SetRegister(ADDR_PCLATH, 0x00);
			SetRegister(ADDR_INTCON, 0x00);

			SetRegister(ADDR_OPTION, 0xFF);
			SetRegister(ADDR_TRIS_A, 0x1F);
			SetRegister(ADDR_TRIS_B, 0xFF);

			SetRegister(ADDR_EECON1, 0x00);
			SetRegister(ADDR_EECON2, 0x00);
		}

		public void SoftResetRegister()
		{
			SetRegister(ADDR_PCL, 0x00);
			SetRegister(ADDR_PCLATH, 0x00);
			SetRegister(ADDR_INTCON, (GetRegister(ADDR_INTCON) & 0x01));
			SetRegister(ADDR_OPTION, 0xFF);
			SetRegister(ADDR_TRIS_A, 0x1F);
			SetRegister(ADDR_TRIS_B, 0xFF);
			SetRegister(ADDR_EECON1, (GetRegister(ADDR_EECON1) & 0x08));
		}

		#endregion

		#region Program Counter

		private void UpdatePC(uint value)
		{
			value &= 0xFF; // Only Low 8 Bit

			uint high = GetRegister(ADDR_PCLATH);
			high &= 0x1F; // Only Bit <0,1,2,3,4>
			high <<= 8;

			value = high | value;

			pc = value;
			SetRegisterDirect(ADDR_PCL, value & 0xFF);
			SetRegisterDirect(ADDR_PCL + 0x80, value & 0xFF);
		}

		public uint GetPC()
		{
			return pc;
		}

		public void SetPC(uint value)
		{
			pc = value;

			SetRegisterDirect(ADDR_PCL, value & 0xFF);
			SetRegisterDirect(ADDR_PCL + 0x80, value & 0xFF);
		}

		public void SetPC_11Bit(uint value)
		{
			value &= 0x7FF; // Only Low 11 Bit

			uint high = GetRegister(ADDR_PCLATH);
			high &= 0x18; // Only Bit <3,4>
			high <<= 8;

			value = high | value;

			pc = value;
			SetRegisterDirect(ADDR_PCL, value & 0xFF);
			SetRegisterDirect(ADDR_PCL + 0x80, value & 0xFF);
		}

		#endregion
	}
}
