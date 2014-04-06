
namespace PICSimulator.Model.Commands
{
	class PICCommand_SWAPF : PICCommand
	{
		public const string COMMANDCODE = "00 1110 dfff ffff";

		public readonly uint Register;
		public readonly bool Target;

		public PICCommand_SWAPF(string sct, uint scl, uint pos, uint cmd)
			: base(sct, scl, pos, cmd)
		{
			Register = Parameter.GetParam('f').Value;
			Target = Parameter.GetBoolParam('d').Value;
		}

		public override void Execute(PICController controller)
		{
			uint Result = controller.GetRegister(Register);

			uint Low = Register & 0x0F;
			uint High = Register & 0xF0;

			Result = (Low << 4) | (High >> 4);

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
