
namespace PICSimulator.Model.Commands
{
	class PICCommand_MOVLW : PICCommand
	{
		public const string COMMANDCODE = "11 00xx kkkk kkkk";

		public readonly uint Literal;

		public PICCommand_MOVLW(string sct, uint scl, uint pos, uint cmd)
			: base(sct, scl, pos, cmd)
		{
			Literal = Parameter.GetParam('k').Value;
		}

		public override void Execute(PICController controller)
		{
			controller.SetWRegisterWithEvent(Literal);
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
