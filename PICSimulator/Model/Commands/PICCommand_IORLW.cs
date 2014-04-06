
namespace PICSimulator.Model.Commands
{
	class PICCommand_IORLW : PICCommand
	{
		public const string COMMANDCODE = "11 1000 kkkk kkkk";

		public readonly uint Literal;

		public PICCommand_IORLW(string sct, uint scl, uint pos, uint cmd)
			: base(sct, scl, pos, cmd)
		{
			Literal = Parameter.GetParam('k').Value;
		}

		public override void Execute(PICController controller)
		{
			uint Result = controller.GetWRegister() | Literal;

			controller.SetWRegister(Result);
			controller.SetRegisterBit(PICController.ADDR_STATUS, PICController.STATUS_BIT_Z, Result == 0);
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
