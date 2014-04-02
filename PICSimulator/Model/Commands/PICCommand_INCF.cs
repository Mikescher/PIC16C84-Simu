﻿namespace PICSimulator.Model.Commands
{
	class PICCommand_INCF : PICCommand
	{
		public const string COMMANDCODE = "00 1010 dfff ffff";

		public PICCommand_INCF(string sct, uint scl, uint pos, uint cmd)
			: base(sct, scl, pos, cmd)
		{

		}
	}
}