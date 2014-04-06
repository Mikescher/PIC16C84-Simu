
namespace PICSimulator.Model.Commands
{
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
			return controller.GetRegister(Register) == 0xFF; // skip if 0xFF -> After DEC will be Zero
		}

		public override void Execute(PICController controller)
		{
			bool Cond = TestCondition(controller);

			uint Result = controller.GetRegister(Register);

			Result += 1;

			Result %= 0xFF;

			if (Target)
				controller.SetRegister(Register, Result);
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
