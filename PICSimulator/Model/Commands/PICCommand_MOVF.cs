
namespace PICSimulator.Model.Commands
{
	class PICCommand_MOVF : PICCommand
	{
		public const string COMMANDCODE = "00 1000 dfff ffff";

		public readonly bool Target;
		public readonly uint Register;

		public PICCommand_MOVF(string sct, uint scl, uint pos, uint cmd)
			: base(sct, scl, pos, cmd)
		{
			Target = Parameter.GetBoolParam('d').Value;
			Register = Parameter.GetParam('f').Value;
		}

		public override void Execute(PICController controller)
		{
			uint Result = controller.GetRegister(Register);

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
