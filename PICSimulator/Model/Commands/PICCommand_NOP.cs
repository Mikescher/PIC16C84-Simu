namespace PICSimulator.Model.Commands
{
	class PICCommand_NOP : PICCommand
	{
		public const string COMMANDCODE = "00 0000 0xx0 0000";

		public PICCommand_NOP(string sct, uint scl, uint pos, uint cmd)
			: base(sct, scl, pos, cmd)
		{

		}
	}
}
