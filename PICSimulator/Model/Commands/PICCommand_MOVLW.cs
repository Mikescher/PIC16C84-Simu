namespace PICSimulator.Model.Commands
{
	class PICCommand_MOVLW : PICCommand
	{
		public const string COMMANDCODE = "11 00xx kkkk kkkk";

		public PICCommand_MOVLW(string sct, uint scl, uint pos, uint cmd)
			: base(sct, scl, pos, cmd)
		{

		}
	}
}
