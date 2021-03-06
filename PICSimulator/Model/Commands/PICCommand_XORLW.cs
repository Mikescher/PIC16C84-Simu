﻿
namespace PICSimulator.Model.Commands
{
	/// <summary>
	/// The contents of the W register are
	/// XOR’ed with the eight bit literal 'k'.
	/// The result is placed in the W regis-
	/// ter.
	/// </summary>
	class PICCommand_XORLW : PICCommand
	{
		public const string COMMANDCODE = "11 1010 kkkk kkkk";

		public readonly uint Literal;

		public PICCommand_XORLW(string sct, uint scl, uint pos, uint cmd)
			: base(sct, scl, pos, cmd)
		{
			Literal = Parameter.GetParam('k').Value;
		}

		public override void Execute(PICController controller)
		{
			uint Result = controller.GetWRegister() ^ Literal;

			controller.SetWRegister(Result);
			controller.SetUnbankedRegisterBit(PICMemory.ADDR_STATUS, PICMemory.STATUS_BIT_Z, Result == 0);
		}

		public override string GetCommandCodeFormat()
		{
			return COMMANDCODE;
		}

		public override uint GetCycleCount(PICController controller)
		{
			return 1;
		}
	}
}
