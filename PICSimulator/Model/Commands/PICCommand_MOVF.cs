namespace PICSimulator.Model.Commands
{
	class PICCommand_MOVF : PICCommand
	{
		public const string COMMANDCODE = "00 1000 dfff ffff";

		public PICCommand_MOVF(string sct, uint scl, uint pos, uint cmd)
			: base(sct, scl, pos, cmd)
		{

		}
	}
}
