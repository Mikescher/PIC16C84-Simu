
namespace PICSimulator.Model.Commands
{
	class PICCommand_ADDLW : PICCommand
	{
		public const string COMMANDCODE = "11 111x kkkk kkkk";

		public readonly uint Literal;

		public PICCommand_ADDLW(string sct, uint scl, uint pos, uint cmd)
			: base(sct, scl, pos, cmd)
		{
			Literal = Parameter.GetParam('k').Value;
		}


		private bool getAdditionDigitCarry(uint a, uint b)
		{
			a %= 0x10;
			b %= 0x10;

			return (a + b) > 0x10;
		}

		public override void Execute(PICController controller)
		{
			uint a = controller.GetWRegister();
			uint b = Literal;

			uint Result = a + b;
			bool dc = getAdditionDigitCarry(a, b);

			controller.SetRegisterBitWithEvent(PICController.ADDR_STATUS, PICController.STATUS_BIT_Z, Result == 0);
			controller.SetRegisterBitWithEvent(PICController.ADDR_STATUS, PICController.STATUS_BIT_DC, dc);
			controller.SetRegisterBitWithEvent(PICController.ADDR_STATUS, PICController.STATUS_BIT_C, Result > 0xFF);

			Result %= 0xFF;

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
