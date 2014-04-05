using System;

namespace PICSimulator.Model.Commands
{
	class PICCommand_DECFSZ : PICCommand
	{
		public const string COMMANDCODE = "00 1011 dfff fff";

		public PICCommand_DECFSZ(string sct, uint scl, uint pos, uint cmd)
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
