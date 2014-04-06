
namespace PICSimulator.Model.Commands
{
	class PICCommand_ANDWF : PICCommand
	{
		public const string COMMANDCODE = "00 0101 dfff ffff";

		public readonly uint Register;
		public readonly bool Target;

		public PICCommand_ANDWF(string sct, uint scl, uint pos, uint cmd)
			: base(sct, scl, pos, cmd)
		{
			Register = Parameter.GetParam('f').Value;
			Target = Parameter.GetBoolParam('d').Value;
		}

		public override void Execute(PICController controller)
		{
			uint Result = controller.GetWRegister() & controller.GetRegister(Register);

			controller.SetRegisterBitWithEvent(PICController.ADDR_STATUS, PICController.STATUS_BIT_Z, Result == 0);

			if (Target)
				controller.SetRegisterWithEvent(Register, Result);
			else
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
