using PICSimulator.Helper;

namespace PICSimulator.Model.Commands
{
	/// <summary>
	/// If bit 'b' in register 'f' is '0' then the next
	/// instruction is executed.
	/// If bit 'b' is '1', then the next instruction is
	/// discarded and a NOP is executed
	/// instead, making this a 2T CY instruction.
	/// </summary>
	class PICCommand_BTFSS : PICCommand
	{
		public const string COMMANDCODE = "01 11bb bfff ffff";

		public readonly uint Register;
		public readonly uint Bit;

		public PICCommand_BTFSS(string sct, uint scl, uint pos, uint cmd)
			: base(sct, scl, pos, cmd)
		{
			Register = Parameter.GetParam('f').Value;
			Bit = Parameter.GetParam('b').Value;
		}

		private bool TestCondition(PICController controller) // Returns True if Skip
		{
			return BinaryHelper.GetBit(controller.GetBankedRegister(Register), Bit);
		}

		public override void Execute(PICController controller)
		{
			if (TestCondition(controller))
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
