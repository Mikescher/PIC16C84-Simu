namespace PICSimulator.Model.Commands
{
	class PICCommand_MOVWF : PICCommand
	{
		public const string COMMANDCODE = "00 0000 lfff ffff";

		public readonly uint Register;

		public PICCommand_MOVWF(string sct, uint scl, uint pos, uint cmd)
			: base(sct, scl, pos, cmd)
		{
			Register = Parameter.GetParam('f').Value;
		}

		public override void Execute(PICController controller)
		{
			controller.SetRegisterWithEvent(Register, controller.GetWRegister());
		}

		public override string GetCommandCodeFormat()
		{
			return COMMANDCODE;
		}
	}
}
