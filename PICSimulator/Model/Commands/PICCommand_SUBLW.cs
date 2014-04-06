using PICSimulator.Helper;

namespace PICSimulator.Model.Commands
{
	class PICCommand_SUBLW : PICCommand
	{
		public const string COMMANDCODE = "11 110x kkkk kkkk";

		public readonly uint Literal;

		public PICCommand_SUBLW(string sct, uint scl, uint pos, uint cmd)
			: base(sct, scl, pos, cmd)
		{
			Literal = Parameter.GetParam('k').Value;
		}

		public override void Execute(PICController controller)
		{
			uint a = Literal;
			uint b = controller.GetWRegister();

			bool carry;

			bool dc = BinaryHelper.getSubtractionDigitCarry(a, b);

			if (carry = a < b)
			{
				a += 0xFF;
			}

			uint Result = a - b;

			controller.SetRegisterBit(PICController.ADDR_STATUS, PICController.STATUS_BIT_Z, Result == 0);
			controller.SetRegisterBit(PICController.ADDR_STATUS, PICController.STATUS_BIT_DC, dc);
			controller.SetRegisterBit(PICController.ADDR_STATUS, PICController.STATUS_BIT_C, carry);

			Result %= 0xFF;

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
