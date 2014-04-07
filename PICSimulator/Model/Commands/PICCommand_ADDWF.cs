using PICSimulator.Helper;

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

		public override void Execute(PICController controller)
		{
			uint a = controller.GetRegister(Register);
			uint b = controller.GetWRegister();

			uint Result = a + b;
			bool dc = BinaryHelper.getAdditionDigitCarry(a, b);

			controller.SetRegisterBit(PICMemory.ADDR_STATUS, PICMemory.STATUS_BIT_Z, Result == 0);
			controller.SetRegisterBit(PICMemory.ADDR_STATUS, PICMemory.STATUS_BIT_DC, dc);
			controller.SetRegisterBit(PICMemory.ADDR_STATUS, PICMemory.STATUS_BIT_C, Result > 0xFF);

			Result %= 0x100;

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
