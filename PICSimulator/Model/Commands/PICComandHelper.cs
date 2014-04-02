
namespace PICSimulator.Model.Commands
{
	static class PICComandHelper
	{
		public static PICCommand CreateCommand(string sct, uint scl, uint pos, uint cmd)
		{
			if (BinaryFormatParser.TryParse(PICCommand_ADDWF.COMMANDCODE, cmd))
				return new PICCommand_ADDWF(sct, scl, pos, cmd);
			else if (BinaryFormatParser.TryParse(PICCommand_ANDWF.COMMANDCODE, cmd))
				return new PICCommand_ANDWF(sct, scl, pos, cmd);
			else if (BinaryFormatParser.TryParse(PICCommand_CLRF.COMMANDCODE, cmd))
				return new PICCommand_CLRF(sct, scl, pos, cmd);
			else if (BinaryFormatParser.TryParse(PICCommand_COMF.COMMANDCODE, cmd))
				return new PICCommand_COMF(sct, scl, pos, cmd);
			else if (BinaryFormatParser.TryParse(PICCommand_DECF.COMMANDCODE, cmd))
				return new PICCommand_DECF(sct, scl, pos, cmd);
			else if (BinaryFormatParser.TryParse(PICCommand_DECFSZ.COMMANDCODE, cmd))
				return new PICCommand_DECFSZ(sct, scl, pos, cmd);
			else if (BinaryFormatParser.TryParse(PICCommand_INCF.COMMANDCODE, cmd))
				return new PICCommand_INCF(sct, scl, pos, cmd);
			else if (BinaryFormatParser.TryParse(PICCommand_INCFSZ.COMMANDCODE, cmd))
				return new PICCommand_INCFSZ(sct, scl, pos, cmd);
			else
				return null;
		}
	}
}
