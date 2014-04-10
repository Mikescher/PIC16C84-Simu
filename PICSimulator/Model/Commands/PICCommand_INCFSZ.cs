
namespace PICSimulator.Model.Commands
{
	/// <summary>
	/// The contents of register 'f' are incre-
	/// mented. If 'd' is 0 the result is placed in
	/// the W register. If 'd' is 1 the result is
	/// placed back in register 'f'.
	/// If the result is 1, the next instruction is
	/// executed. If the result is 0, a NOP is exe-
	/// cuted instead making it a 2T CY instruc-
	/// tion .
	/// </summary>
	class PICCommand_INCFSZ : PICCommand
	{
		public const string COMMANDCODE = "00 1111 dfff ffff";

		public readonly bool Target;
		public readonly uint Register;

		public PICCommand_INCFSZ(string sct, uint scl, uint pos, uint cmd)
			: base(sct, scl, pos, cmd)
		{
			Target = Parameter.GetBoolParam('d').Value;
			Register = Parameter.GetParam('f').Value;
		}

		private bool TestCondition(PICController controller) // Returns True if Skip
		{
			return controller.GetBankedRegister(Register) == 0xFF; // skip if 0xFF -> After DEC will be Zero
		}

		public override void Execute(PICController controller)
		{
			bool Cond = TestCondition(controller);

			uint Result = controller.GetBankedRegister(Register);

			Result += 1;

			Result %= 0x100;

			if (Target)
				controller.SetBankedRegister(Register, Result);
			else
				controller.SetWRegister(Result);

			if (Cond)
			{
				controller.SetPC_13Bit(controller.GetPC() + 1);
			}
		}

		public override string GetCommandCodeFormat()
		{
			return COMMANDCODE;
		}

		public override uint GetCycleCount(PICController controller)
		{
			return TestCondition(controller) ? 2u : 1u;
		}
	}
}
