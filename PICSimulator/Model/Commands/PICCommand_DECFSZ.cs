
namespace PICSimulator.Model.Commands
{
	/// <summary>
	/// Decrement register 'f'. If 'd' is 0 the
	/// result is stored in the W register. If 'd' is
	/// 1 the result is stored back in register 'f' .
	/// </summary>
	class PICCommand_DECFSZ : PICCommand
	{
		public const string COMMANDCODE = "00 1011 dfff ffff";

		public readonly bool Target;
		public readonly uint Register;

		public PICCommand_DECFSZ(string sct, uint scl, uint pos, uint cmd)
			: base(sct, scl, pos, cmd)
		{
			Target = Parameter.GetBoolParam('d').Value;
			Register = Parameter.GetParam('f').Value;
		}

		private bool TestCondition(PICController controller) // Returns True if Skip
		{
			return controller.GetBankedRegister(Register) == 1; // skip if 1 -> After DEC will be Zero
		}

		public override void Execute(PICController controller)
		{
			bool Cond = TestCondition(controller);

			uint Result = controller.GetBankedRegister(Register);

			if (Result == 0)
				Result = 0xFF;
			else
				Result -= 1;

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
