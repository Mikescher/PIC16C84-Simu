
namespace PICSimulator.Model.Commands
{
	class PICCommand_ADDWF : PICCommand
	{
		public const string COMMANDCODE = "00 0111 dfff ffff";

		public PICCommand_ADDWF(string sct, uint scl, uint pos, uint cmd)
			: base(sct, scl, pos, cmd)
		{

		}
	}
}
