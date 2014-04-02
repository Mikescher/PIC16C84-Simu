namespace PICSimulator.Model.Commands
{
	class PICCommand_INCFSZ : PICCommand
	{
		public const string COMMANDCODE = "00 1111 dfff ffff";

		public PICCommand_INCFSZ(string sct, uint scl, uint pos, uint cmd)
			: base(sct, scl, pos, cmd)
		{

		}
	}
}
