using System;

namespace PICSimulator.Model.Commands
{
	class PICCommand_INCF : PICCommand
	{
		public const string COMMANDCODE = "00 1010 dfff ffff";

		public PICCommand_INCF(string sct, uint scl, uint pos, uint cmd)
			: base(sct, scl, pos, cmd)
		{

		}

		public override void Execute(PICController controller)
		{
			throw new System.NotImplementedException();
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
