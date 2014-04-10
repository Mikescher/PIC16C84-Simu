
namespace PICSimulator.Model.Commands
{
	/// <summary>
	/// The power-down status bit, PD is
	/// cleared. Time-out status bit, TO is
	/// set. Watchdog Timer and its prescaler
	/// are cleared.
	/// The processor is put into SLEEP
	/// mode with the oscillator stopped. See
	/// Section 14.8 for more details.
	/// </summary>
	class PICCommand_SLEEP : PICCommand
	{
		public const string COMMANDCODE = "00 0000 0110 0011";

		public PICCommand_SLEEP(string sct, uint scl, uint pos, uint cmd)
			: base(sct, scl, pos, cmd)
		{

		}

		public override void Execute(PICController controller)
		{
			PICWatchDogTimer wdt = controller.GetWatchDog();

			wdt.Reset();

			if (controller.GetUnbankedRegisterBit(PICMemory.ADDR_OPTION, PICMemory.OPTION_BIT_PSA))
			{
				controller.SetUnbankedRegisterBit(PICMemory.ADDR_OPTION, PICMemory.OPTION_BIT_PS0, false);
				controller.SetUnbankedRegisterBit(PICMemory.ADDR_OPTION, PICMemory.OPTION_BIT_PS1, false);
				controller.SetUnbankedRegisterBit(PICMemory.ADDR_OPTION, PICMemory.OPTION_BIT_PS2, false);
			}

			controller.SetUnbankedRegisterBit(PICMemory.ADDR_STATUS, PICMemory.STATUS_BIT_TO, true);
			controller.SetUnbankedRegisterBit(PICMemory.ADDR_STATUS, PICMemory.STATUS_BIT_PD, false);

			controller.StartSleep();
		}

		public override string GetCommandCodeFormat()
		{
			return COMMANDCODE;
		}

		public override uint GetCycleCount(PICController controller)
		{
			return 1;
		}
	}
}
