namespace PICSimulator.Model.Commands
{
	class PICCommand_IORWF : PICCommand
	{
		public const string COMMANDCODE = "00 0100 dfff ffff";

		public PICCommand_IORWF(string sct, uint scl, uint pos, uint cmd)
			: base(sct, scl, pos, cmd)
		{

		}
	}
}
