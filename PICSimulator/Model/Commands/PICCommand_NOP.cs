
namespace PICSimulator.Model.Commands
{
	/// <summary>
	/// No operation.
	/// </summary>
	class PICCommand_NOP : PICCommand
	{
		public const string COMMANDCODE = "00 0000 0xx0 0000";

		public PICCommand_NOP(string sct, uint scl, uint pos, uint cmd)
			: base(sct, scl, pos, cmd)
		{

		}

		public override void Execute(PICController controller)
		{
			//DO NOTHING
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
