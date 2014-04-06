

namespace PICSimulator.Model.Commands
{
	class PICCommand_ADDWF : PICCommand
	{
		public const string COMMANDCODE = "00 0111 dfff ffff";

		public readonly uint Register;
		public readonly bool Target;

		public PICCommand_ADDWF(string sct, uint scl, uint pos, uint cmd)
			: base(sct, scl, pos, cmd)
		{
			Register = Parameter.GetParam('f').Value;
			Target = Parameter.GetBoolParam('d').Value;
		}

		private bool getAdditionDigitCarry(uint a, uint b)
		{
			a %= 0x10;
			b %= 0x10;

			return (a + b) > 0x10;
		}

		public override void Execute(PICController controller)
		{
			uint a = controller.GetRegister(Register);
			uint b = controller.GetWRegister();

			uint Result = a + b;
			bool dc = getAdditionDigitCarry(a, b);

			controller.SetRegisterBitWithEvent(PICController.ADDR_STATUS, PICController.STATUS_BIT_Z, Result == 0);
			controller.SetRegisterBitWithEvent(PICController.ADDR_STATUS, PICController.STATUS_BIT_DC, dc);
			controller.SetRegisterBitWithEvent(PICController.ADDR_STATUS, PICController.STATUS_BIT_C, Result > 0xFF);

			Result %= 0xFF;

			if (Target)
				controller.SetRegisterWithEvent(Register, Result);
			else
				controller.SetWRegisterWithEvent(Result);
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
