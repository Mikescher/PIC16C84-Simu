
namespace PICSimulator.Model.Commands
{
	/// <summary>
	/// The upper and lower nibbles of register
	/// 'f' are exchanged. If 'd' is 0 the result is
	/// placed in W register. If 'd' is 1 the result
	/// is placed in register 'f'.
	/// </summary>
	class PICCommand_SWAPF : PICCommand
	{
		public const string COMMANDCODE = "00 1110 dfff ffff";

		public readonly uint Register;
		public readonly bool Target;

		public PICCommand_SWAPF(string sct, uint scl, uint pos, uint cmd)
			: base(sct, scl, pos, cmd)
		{
			Register = Parameter.GetParam('f').Value;
			Target = Parameter.GetBoolParam('d').Value;
		}

		public override void Execute(PICController controller)
		{
			uint Result = controller.GetBankedRegister(Register);

			uint Low = Result & 0x0F;
			uint High = Result & 0xF0;

			Result = (Low << 4) | (High >> 4);

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
