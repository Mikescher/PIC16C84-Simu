using System;

namespace PICSimulator.Model.Commands
{
	class PICCommand_XORLW : PICCommand
	{
		public const string COMMANDCODE = "11 1010 kkkk kkkk";

		public PICCommand_XORLW(string sct, uint scl, uint pos, uint cmd)
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
