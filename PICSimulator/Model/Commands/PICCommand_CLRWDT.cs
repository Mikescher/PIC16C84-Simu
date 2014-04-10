
namespace PICSimulator.Model.Commands
{
	/// <summary>
	/// CLRWDT instruction resets the Watch-
	/// dog Timer. It also resets the prescaler
	/// of the WDT. Status bits TO and PD are
	/// set.
	/// </summary>

	class PICCommand_CLRWDT : PICCommand
	{
		public const string COMMANDCODE = "00 0000 0110 0100";

		public PICCommand_CLRWDT(string sct, uint scl, uint pos, uint cmd)
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
			controller.SetUnbankedRegisterBit(PICMemory.ADDR_STATUS, PICMemory.STATUS_BIT_PD, true);
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
