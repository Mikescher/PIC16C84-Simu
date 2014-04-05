using System;

namespace PICSimulator.Model.Commands
{
	class PICCommand_IORLW : PICCommand
	{
		public const string COMMANDCODE = "11 1000 kkkk kkkk";

		public PICCommand_IORLW(string sct, uint scl, uint pos, uint cmd)
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
