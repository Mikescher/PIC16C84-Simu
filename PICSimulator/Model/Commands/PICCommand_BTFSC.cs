namespace PICSimulator.Model.Commands
{
	class PICCommand_BTFSC : PICCommand
	{
		public const string COMMANDCODE = "01 10bb bfff ffff";

		public PICCommand_BTFSC(string sct, uint scl, uint pos, uint cmd)
			: base(sct, scl, pos, cmd)
		{

		}
	}
}
