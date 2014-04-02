namespace PICSimulator.Model.Commands
{
	class PICCommand_RETURN : PICCommand
	{
		public const string COMMANDCODE = "00 0000 0000 1000";

		public PICCommand_RETURN(string sct, uint scl, uint pos, uint cmd)
			: base(sct, scl, pos, cmd)
		{

		}
	}
}
