
namespace PICSimulator.Model.Commands
{
	class PICCommand_COMF : PICCommand
	{
		public const string COMMANDCODE = "00 1001 dfff ffff";

		public PICCommand_COMF(string sct, uint scl, uint pos, uint cmd)
			: base(sct, scl, pos, cmd)
		{

		}
	}
}
