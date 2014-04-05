using System;

namespace PICSimulator.Model.Commands
{
	class PICCommand_RETFIE : PICCommand
	{
		public const string COMMANDCODE = "00 0000 0000 1001";

		public PICCommand_RETFIE(string sct, uint scl, uint pos, uint cmd)
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

		public override uint GetCycleCount()
		{
			throw new NotImplementedException();
		}
	}
}
