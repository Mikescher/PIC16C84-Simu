namespace PICSimulator.Model.Commands
{
	class PICCommand_DECFSZ : PICCommand
	{
		public const string COMMANDCODE = "00 1011 dfff fff";

		public PICCommand_DECFSZ(string sct, uint scl, uint pos, uint cmd)
			: base(sct, scl, pos, cmd)
		{

		}
	}
}
