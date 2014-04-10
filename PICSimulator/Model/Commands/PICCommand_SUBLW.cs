using PICSimulator.Helper;

namespace PICSimulator.Model.Commands
{
	/// <summary>
	/// The W register is subtracted (2’s comple-
	/// ment method) from the eight bit literal 'k'.
	/// The result is placed in the W register.
	/// </summary>
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

			controller.SetUnbankedRegisterBit(PICMemory.ADDR_STATUS, PICMemory.STATUS_BIT_Z, Result == 0);
			controller.SetUnbankedRegisterBit(PICMemory.ADDR_STATUS, PICMemory.STATUS_BIT_DC, dc);
			controller.SetUnbankedRegisterBit(PICMemory.ADDR_STATUS, PICMemory.STATUS_BIT_C, !carry);

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
