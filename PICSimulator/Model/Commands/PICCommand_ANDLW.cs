namespace PICSimulator.Model.Commands
{
	class PICCommand_ANDLW : PICCommand
	{
		public const string COMMANDCODE = "11 1001 kkkk kkkk";

		public PICCommand_ANDLW(string sct, uint scl, uint pos, uint cmd)
			: base(sct, scl, pos, cmd)
		{

		}
	}
}
