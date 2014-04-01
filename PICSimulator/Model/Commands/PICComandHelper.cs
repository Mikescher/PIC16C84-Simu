
namespace PICSimulator.Model.Commands
{
	static class PICComandHelper
	{
		public static PICCommand CreateCommand(string sct, uint scl, uint pos, uint cmd)
		{
			if (BinaryFormatParser.TryParse(PICCommand_ADDWF.COMMANDCODE, cmd))
			{
				return new PICCommand_ADDWF(sct, scl, pos, cmd);
			}
			else
			{
				return null;
			}
		}
	}
}
