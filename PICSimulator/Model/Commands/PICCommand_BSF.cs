using System;

namespace PICSimulator.Model.Commands
{
	class PICCommand_BSF : PICCommand
	{
		public const string COMMANDCODE = "01 01bb bfff ffff";

		public PICCommand_BSF(string sct, uint scl, uint pos, uint cmd)
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
