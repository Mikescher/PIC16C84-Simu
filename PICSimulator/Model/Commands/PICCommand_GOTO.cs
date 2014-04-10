
namespace PICSimulator.Model.Commands
{
	/// <summary>
	/// GOTO is an unconditional branch. The
	/// eleven bit immediate value is loaded
	/// into PC bits <10:0>. The upper bits of
	/// PC are loaded from PCLATH<4:3>.
	/// GOTO is a two cycle instruction.
	/// </summary>
	class PICCommand_GOTO : PICCommand
	{
		public const string COMMANDCODE = "10 1kkk kkkk kkkk";

		public readonly uint Address;

		public PICCommand_GOTO(string sct, uint scl, uint pos, uint cmd)
			: base(sct, scl, pos, cmd)
		{
			Address = Parameter.GetParam('k').Value;
		}

		public override void Execute(PICController controller)
		{
			controller.SetPC_11Bit(Address);
		}

		public override string GetCommandCodeFormat()
		{
			return COMMANDCODE;
		}

		public override uint GetCycleCount(PICController controller)
		{
			return 2;
		}
	}
}
