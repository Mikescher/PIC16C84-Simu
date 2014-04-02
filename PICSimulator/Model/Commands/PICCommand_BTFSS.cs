namespace PICSimulator.Model.Commands
{
	class PICCommand_BTFSS : PICCommand
	{
		public const string COMMANDCODE = "01 11bb bfff ffff";

		public PICCommand_BTFSS(string sct, uint scl, uint pos, uint cmd)
			: base(sct, scl, pos, cmd)
		{

		}
	}
}
