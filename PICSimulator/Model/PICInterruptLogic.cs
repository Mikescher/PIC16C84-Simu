using System.Collections.Generic;

namespace PICSimulator.Model
{
	class PICInterruptLogic
	{
		private const uint INTERRUPT_DELAY = 2;
		private const uint INTERRUPT_SERVICE_ADDRESS = 0x04;

		private class PICInterrupt
		{
			public uint Delay;
			public PICInterruptType Type;
		}

		private PICController controller;

		private List<PICInterrupt> Queue = new List<PICInterrupt>(); //TODO WHAT HAPPENS WITH MORER THAN 1 INTERRUPT ??????

		public PICInterruptLogic(PICController c)
		{
			this.controller = c;
		}

		private bool isEnabled()
		{
			return controller.GetRegisterBit(PICController.ADDR_INTCON, PICController.INTCON_BIT_GIE);
		}

		private bool isEnabled(PICInterruptType t)
		{
			switch (t)
			{
				case PICInterruptType.PIT_RB0INT:
					return isEnabled() && controller.GetRegisterBit(PICController.ADDR_INTCON, PICController.INTCON_BIT_INTE);
				case PICInterruptType.PIT_TIMER:
					return isEnabled() && controller.GetRegisterBit(PICController.ADDR_INTCON, PICController.INTCON_BIT_T0IE);
				case PICInterruptType.PIT_PORTB:
					return isEnabled() && controller.GetRegisterBit(PICController.ADDR_INTCON, PICController.INTCON_BIT_RBIE);
				case PICInterruptType.PIT_EEPROM:
					return isEnabled() && controller.GetRegisterBit(PICController.ADDR_INTCON, PICController.INTCON_BIT_EEIE);
				default:
					return false;
			}
		}

		public bool AddInterrupt(PICInterruptType t)
		{
			if (!isEnabled(t))
				return false;

			Queue.Add(new PICInterrupt() { Type = t, Delay = INTERRUPT_DELAY });

			return true;
		}

		public void Update()
		{
			Queue.ForEach(p => p.Delay--);

			for (int i = Queue.Count - 1; i >= 0; i--)
			{
				if (isEnabled() && Queue[i].Delay <= 0)
				{
					PICInterruptType Type = Queue[i].Type;
					Queue.RemoveAt(i);

					DoInterrupt(Type);

					return;
				}
			}
		}

		private void DoInterrupt(PICInterruptType Type)
		{
			// Set Flags
			switch (Type)
			{
				case PICInterruptType.PIT_RB0INT:
					controller.SetRegisterBit(PICController.ADDR_INTCON, PICController.INTCON_BIT_INTF, true);
					break;
				case PICInterruptType.PIT_TIMER:
					controller.SetRegisterBit(PICController.ADDR_INTCON, PICController.INTCON_BIT_T0IF, true);
					break;
				case PICInterruptType.PIT_PORTB:
					controller.SetRegisterBit(PICController.ADDR_INTCON, PICController.INTCON_BIT_RBIF, true);
					break;
				case PICInterruptType.PIT_EEPROM:
					// No Flag for EEPROM....
					break;
			}

			controller.SetRegisterBit(PICController.ADDR_INTCON, PICController.INTCON_BIT_GIE, false);

			controller.PushCallStack(controller.GetPC());
			controller.SetPC_11Bit(INTERRUPT_SERVICE_ADDRESS);
		}

		public void Reset()
		{
			Queue.Clear();
		}
	}
}
