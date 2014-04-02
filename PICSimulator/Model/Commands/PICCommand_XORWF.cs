namespace PICSimulator.Model.Commands
{
	class PICCommand_XORWF : PICCommand
	{
		public const string COMMANDCODE = "00 0110 dfff ffff";

		public PICCommand_XORWF(string sct, uint scl, uint pos, uint cmd)
			: base(sct, scl, pos, cmd)
		{

		}
	}
}
