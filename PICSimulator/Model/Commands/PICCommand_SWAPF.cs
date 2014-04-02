namespace PICSimulator.Model.Commands
{
	class PICCommand_SWAPF : PICCommand
	{
		public const string COMMANDCODE = "00 1110 dfff ffff";

		public PICCommand_SWAPF(string sct, uint scl, uint pos, uint cmd)
			: base(sct, scl, pos, cmd)
		{

		}
	}
}
