using PICSimulator.Helper;

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

		public override void Execute(PICController controller)
		{
			uint a = controller.GetWRegister();
			uint b = Literal;

			uint Result = a + b;
			bool dc = BinaryHelper.getAdditionDigitCarry(a, b);

			controller.SetUnbankedRegisterBit(PICMemory.ADDR_STATUS, PICMemory.STATUS_BIT_Z, Result == 0);
			controller.SetUnbankedRegisterBit(PICMemory.ADDR_STATUS, PICMemory.STATUS_BIT_DC, dc);
			controller.SetUnbankedRegisterBit(PICMemory.ADDR_STATUS, PICMemory.STATUS_BIT_C, Result > 0xFF);

			Result %= 0x100;

			controller.SetWRegister(Result);
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
