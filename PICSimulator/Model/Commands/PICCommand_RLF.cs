
namespace PICSimulator.Model.Commands
{
	class PICCommand_RLF : PICCommand
	{
		public const string COMMANDCODE = "00 1101 dfff ffff";

		public readonly bool Target;
		public readonly uint Register;

		public PICCommand_RLF(string sct, uint scl, uint pos, uint cmd)
			: base(sct, scl, pos, cmd)
		{
			Target = Parameter.GetBoolParam('d').Value;
			Register = Parameter.GetParam('f').Value;
		}

		public override void Execute(PICController controller)
		{
			uint Result = controller.GetRegister(Register);

			uint Carry_Old = controller.GetRegisterBit(PICMemory.ADDR_STATUS, PICMemory.STATUS_BIT_C) ? 1u : 0u;
			uint Carry_New = (Result & 0x80) >> 7;

			Result = Result << 1;
			Result &= 0xFF;

			Result |= Carry_Old;

			controller.SetRegisterBit(PICMemory.ADDR_STATUS, PICMemory.STATUS_BIT_C, Carry_New != 0);

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
