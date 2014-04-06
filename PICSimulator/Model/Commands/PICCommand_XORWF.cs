
namespace PICSimulator.Model.Commands
{
	class PICCommand_XORWF : PICCommand
	{
		public const string COMMANDCODE = "00 0110 dfff ffff";

		public readonly uint Register;
		public readonly bool Target;

		public PICCommand_XORWF(string sct, uint scl, uint pos, uint cmd)
			: base(sct, scl, pos, cmd)
		{
			Register = Parameter.GetParam('f').Value;
			Target = Parameter.GetBoolParam('d').Value;
		}

		public override void Execute(PICController controller)
		{
			uint Result = controller.GetWRegister() ^ controller.GetRegister(Register);

			controller.SetRegisterBit(PICController.ADDR_STATUS, PICController.STATUS_BIT_Z, Result == 0);

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
