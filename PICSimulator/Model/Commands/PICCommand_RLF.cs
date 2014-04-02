﻿namespace PICSimulator.Model.Commands
{
	class PICCommand_RLF : PICCommand
	{
		public const string COMMANDCODE = "00 1101 dfff ffff";

		public PICCommand_RLF(string sct, uint scl, uint pos, uint cmd)
			: base(sct, scl, pos, cmd)
		{

		}
	}
}