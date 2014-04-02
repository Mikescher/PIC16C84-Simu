namespace PICSimulator.Model.Commands
{
	class PICCommand_XORLW : PICCommand
	{
		public const string COMMANDCODE = "11 1010 kkkk kkkk";

		public PICCommand_XORLW(string sct, uint scl, uint pos, uint cmd)
			: base(sct, scl, pos, cmd)
		{

		}
	}
}
