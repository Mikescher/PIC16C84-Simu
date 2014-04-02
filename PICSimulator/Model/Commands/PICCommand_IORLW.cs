namespace PICSimulator.Model.Commands
{
	class PICCommand_IORLW : PICCommand
	{
		public const string COMMANDCODE = "11 1000 kkkk kkkk";

		public PICCommand_IORLW(string sct, uint scl, uint pos, uint cmd)
			: base(sct, scl, pos, cmd)
		{

		}
	}
}
