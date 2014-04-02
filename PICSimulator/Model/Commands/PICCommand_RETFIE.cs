namespace PICSimulator.Model.Commands
{
	class PICCommand_RETFIE : PICCommand
	{
		public const string COMMANDCODE = "00 0000 0000 1001";

		public PICCommand_RETFIE(string sct, uint scl, uint pos, uint cmd)
			: base(sct, scl, pos, cmd)
		{

		}
	}
}
