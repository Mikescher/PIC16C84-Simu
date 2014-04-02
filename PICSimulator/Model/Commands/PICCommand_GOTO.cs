namespace PICSimulator.Model.Commands
{
	class PICCommand_GOTO : PICCommand
	{
		public const string COMMANDCODE = "10 1kkk kkkk kkkk";

		public PICCommand_GOTO(string sct, uint scl, uint pos, uint cmd)
			: base(sct, scl, pos, cmd)
		{

		}
	}
}
