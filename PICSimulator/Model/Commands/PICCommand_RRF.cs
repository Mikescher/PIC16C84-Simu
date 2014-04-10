
namespace PICSimulator.Model.Commands
{
	/// <summary>
	/// The contents of register 'f' are rotated
	/// one bit to the right through the Carry
	/// Flag. If 'd' is 0 the result is placed in the
	/// W register. If 'd' is 1 the result is placed
	/// back in register 'f'.
	/// </summary>
	class PICCommand_RRF : PICCommand
	{
		public const string COMMANDCODE = "00 1100 dfff ffff";

		public readonly bool Target;
		public readonly uint Register;

		public PICCommand_RRF(string sct, uint scl, uint pos, uint cmd)
			: base(sct, scl, pos, cmd)
		{
			Target = Parameter.GetBoolParam('d').Value;
			Register = Parameter.GetParam('f').Value;
		}

		public override void Execute(PICController controller)
		{
			uint Result = controller.GetBankedRegister(Register);

			uint Carry_Old = controller.GetUnbankedRegisterBit(PICMemory.ADDR_STATUS, PICMemory.STATUS_BIT_C) ? 0x80u : 0x00u;
			uint Carry_New = Result & 0x01;

			Result = Result >> 1;
			Result &= 0xFF;

			Result |= Carry_Old;

			controller.SetUnbankedRegisterBit(PICMemory.ADDR_STATUS, PICMemory.STATUS_BIT_C, Carry_New != 0);

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
