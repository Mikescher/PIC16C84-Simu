﻿
namespace PICSimulator.Model.Commands
{
	/// <summary>
	/// Exclusive OR the contents of the W
	/// register with register 'f'. If 'd' is 0 the
	/// result is stored in the W register. If 'd' is
	/// 1 the result is stored back in register 'f'.
	/// </summary>
	class PICCommand_XORWF : PICCommand
	{
		public const string COMMANDCODE = "00 0110 dfff ffff";

		public readonly uint Register;
		public readonly bool Target;

		public PICCommand_XORWF(string sct, uint scl, uint pos, uint cmd)
			: base(sct, scl, pos, cmd)
		{
			Register = Parameter.GetParam('f').Value;
			Target = Parameter.GetBoolParam('d').Value;
		}

		public override void Execute(PICController controller)
		{
			uint Result = controller.GetWRegister() ^ controller.GetBankedRegister(Register);

			controller.SetUnbankedRegisterBit(PICMemory.ADDR_STATUS, PICMemory.STATUS_BIT_Z, Result == 0);

			if (Target)
				controller.SetBankedRegister(Register, Result);
			else
				controller.SetWRegister(Result);
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
