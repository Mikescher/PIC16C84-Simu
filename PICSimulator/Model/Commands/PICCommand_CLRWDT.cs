namespace PICSimulator.Model.Commands
{
	class PICCommand_CLRWDT : PICCommand
	{
		public const string COMMANDCODE = "00 0000 0110 0100";

		public PICCommand_CLRWDT(string sct, uint scl, uint pos, uint cmd)
			: base(sct, scl, pos, cmd)
		{

		}
	}
}
