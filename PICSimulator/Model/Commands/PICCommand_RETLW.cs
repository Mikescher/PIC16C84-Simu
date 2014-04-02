namespace PICSimulator.Model.Commands
{
	class PICCommand_RETLW : PICCommand
	{
		public const string COMMANDCODE = "11 01xx kkkk kkkk";

		public PICCommand_RETLW(string sct, uint scl, uint pos, uint cmd)
			: base(sct, scl, pos, cmd)
		{

		}
	}
}
