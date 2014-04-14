namespace PICSimulator.Model.Commands
{
	static class PICComandHelper
	{
		public static PICCommand CreateCommand(string sct, uint scl, uint pos, uint cmd)
		{
			#region BYTE-ORIENTED

			if (BinaryFormatParser.TryParse(PICCommand_ADDWF.COMMANDCODE, cmd))
				return new PICCommand_ADDWF(sct, scl, pos, cmd);

			else if (BinaryFormatParser.TryParse(PICCommand_ANDWF.COMMANDCODE, cmd))
				return new PICCommand_ANDWF(sct, scl, pos, cmd);

			else if (BinaryFormatParser.TryParse(PICCommand_CLRF.COMMANDCODE, cmd))
				return new PICCommand_CLRF(sct, scl, pos, cmd);

			else if (BinaryFormatParser.TryParse(PICCommand_CLRW.COMMANDCODE, cmd))
				return new PICCommand_CLRW(sct, scl, pos, cmd);

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

			else if (BinaryFormatParser.TryParse(PICCommand_IORWF.COMMANDCODE, cmd))
				return new PICCommand_IORWF(sct, scl, pos, cmd);

			else if (BinaryFormatParser.TryParse(PICCommand_MOVF.COMMANDCODE, cmd))
				return new PICCommand_MOVF(sct, scl, pos, cmd);

			else if (BinaryFormatParser.TryParse(PICCommand_MOVWF.COMMANDCODE, cmd))
				return new PICCommand_MOVWF(sct, scl, pos, cmd);

			else if (BinaryFormatParser.TryParse(PICCommand_NOP.COMMANDCODE, cmd))
				return new PICCommand_NOP(sct, scl, pos, cmd);

			else if (BinaryFormatParser.TryParse(PICCommand_RLF.COMMANDCODE, cmd))
				return new PICCommand_RLF(sct, scl, pos, cmd);

			else if (BinaryFormatParser.TryParse(PICCommand_RRF.COMMANDCODE, cmd))
				return new PICCommand_RRF(sct, scl, pos, cmd);

			else if (BinaryFormatParser.TryParse(PICCommand_SUBWF.COMMANDCODE, cmd))
				return new PICCommand_SUBWF(sct, scl, pos, cmd);

			else if (BinaryFormatParser.TryParse(PICCommand_SWAPF.COMMANDCODE, cmd))
				return new PICCommand_SWAPF(sct, scl, pos, cmd);

			else if (BinaryFormatParser.TryParse(PICCommand_XORWF.COMMANDCODE, cmd))
				return new PICCommand_XORWF(sct, scl, pos, cmd);

			#endregion

			#region BIT_ORIENTED

			else if (BinaryFormatParser.TryParse(PICCommand_BCF.COMMANDCODE, cmd))
				return new PICCommand_BCF(sct, scl, pos, cmd);

			else if (BinaryFormatParser.TryParse(PICCommand_BSF.COMMANDCODE, cmd))
				return new PICCommand_BSF(sct, scl, pos, cmd);

			else if (BinaryFormatParser.TryParse(PICCommand_BTFSC.COMMANDCODE, cmd))
				return new PICCommand_BTFSC(sct, scl, pos, cmd);

			else if (BinaryFormatParser.TryParse(PICCommand_BTFSS.COMMANDCODE, cmd))
				return new PICCommand_BTFSS(sct, scl, pos, cmd);

			#endregion

			#region LITERAL AND CONTROL

			else if (BinaryFormatParser.TryParse(PICCommand_ADDLW.COMMANDCODE, cmd))
				return new PICCommand_ADDLW(sct, scl, pos, cmd);

			else if (BinaryFormatParser.TryParse(PICCommand_ANDLW.COMMANDCODE, cmd))
				return new PICCommand_ANDLW(sct, scl, pos, cmd);

			else if (BinaryFormatParser.TryParse(PICCommand_CALL.COMMANDCODE, cmd))
				return new PICCommand_CALL(sct, scl, pos, cmd);

			else if (BinaryFormatParser.TryParse(PICCommand_CLRWDT.COMMANDCODE, cmd))
				return new PICCommand_CLRWDT(sct, scl, pos, cmd);

			else if (BinaryFormatParser.TryParse(PICCommand_GOTO.COMMANDCODE, cmd))
				return new PICCommand_GOTO(sct, scl, pos, cmd);

			else if (BinaryFormatParser.TryParse(PICCommand_IORLW.COMMANDCODE, cmd))
				return new PICCommand_IORLW(sct, scl, pos, cmd);

			else if (BinaryFormatParser.TryParse(PICCommand_MOVLW.COMMANDCODE, cmd))
				return new PICCommand_MOVLW(sct, scl, pos, cmd);

			else if (BinaryFormatParser.TryParse(PICCommand_RETFIE.COMMANDCODE, cmd))
				return new PICCommand_RETFIE(sct, scl, pos, cmd);

			else if (BinaryFormatParser.TryParse(PICCommand_RETLW.COMMANDCODE, cmd))
				return new PICCommand_RETLW(sct, scl, pos, cmd);

			else if (BinaryFormatParser.TryParse(PICCommand_RETURN.COMMANDCODE, cmd))
				return new PICCommand_RETURN(sct, scl, pos, cmd);

			else if (BinaryFormatParser.TryParse(PICCommand_SLEEP.COMMANDCODE, cmd))
				return new PICCommand_SLEEP(sct, scl, pos, cmd);

			else if (BinaryFormatParser.TryParse(PICCommand_SUBLW.COMMANDCODE, cmd))
				return new PICCommand_SUBLW(sct, scl, pos, cmd);

			else if (BinaryFormatParser.TryParse(PICCommand_XORLW.COMMANDCODE, cmd))
				return new PICCommand_XORLW(sct, scl, pos, cmd);

			#endregion

			else
				return null;
		}
	}
}