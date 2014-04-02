namespace PICSimulator.Model.Commands
{
	class PICCommand_MOVWF : PICCommand
	{
		public const string COMMANDCODE = "00 0000 lfff ffff";

		public PICCommand_MOVWF(string sct, uint scl, uint pos, uint cmd)
			: base(sct, scl, pos, cmd)
		{

		}
	}
}
