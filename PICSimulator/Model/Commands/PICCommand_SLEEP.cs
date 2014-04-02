namespace PICSimulator.Model.Commands
{
	class PICCommand_SLEEP : PICCommand
	{
		public const string COMMANDCODE = "00 0000 0110 0011";

		public PICCommand_SLEEP(string sct, uint scl, uint pos, uint cmd)
			: base(sct, scl, pos, cmd)
		{

		}
	}
}
