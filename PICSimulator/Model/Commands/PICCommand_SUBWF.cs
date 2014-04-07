using PICSimulator.Helper;

namespace PICSimulator.Model.Commands
{
	class PICCommand_SUBWF : PICCommand
	{
		public const string COMMANDCODE = "00 0010 dfff ffff";

		public readonly uint Register;
		public readonly bool Target;

		public PICCommand_SUBWF(string sct, uint scl, uint pos, uint cmd)
			: base(sct, scl, pos, cmd)
		{
			Register = Parameter.GetParam('f').Value;
			Target = Parameter.GetBoolParam('d').Value;
		}

		public override void Execute(PICController controller)
		{
			uint a = controller.GetRegister(Register);
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
			controller.SetRegisterBit(PICController.ADDR_STATUS, PICController.STATUS_BIT_C, !carry);

			Result %= 0xFF;

			if (Target)
				controller.SetRegister(Register, Result);
			else
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
