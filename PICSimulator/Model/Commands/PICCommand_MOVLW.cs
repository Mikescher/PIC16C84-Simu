
namespace PICSimulator.Model.Commands
{
	/// <summary>
	/// The contents of register f is moved to a
	/// destination dependant upon the status
	/// of d. If d = 0, destination is W register. If
	/// d = 1, the destination is file register f
	/// itself. d = 1 is useful to test a file regis-
	/// ter since status flag Z is affected.
	/// </summary>
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
			controller.SetWRegister(Literal);
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
