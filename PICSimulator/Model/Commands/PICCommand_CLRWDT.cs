using System;

namespace PICSimulator.Model.Commands
{
	class PICCommand_CLRWDT : PICCommand
	{
		public const string COMMANDCODE = "00 0000 0110 0100";

		public PICCommand_CLRWDT(string sct, uint scl, uint pos, uint cmd)
			: base(sct, scl, pos, cmd)
		{

		}

		public override void Execute(PICController controller)
		{
			throw new System.NotImplementedException(); //TODO Implement Watchdog
		}

		public override string GetCommandCodeFormat()
		{
			return COMMANDCODE;
		}

		public override uint GetCycleCount(PICController controller)
		{
			throw new NotImplementedException();
		}
	}
}
