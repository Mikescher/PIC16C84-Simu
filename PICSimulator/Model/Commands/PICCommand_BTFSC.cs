
using PICSimulator.Helper;
namespace PICSimulator.Model.Commands
{
	class PICCommand_BTFSC : PICCommand
	{
		public const string COMMANDCODE = "01 10bb bfff ffff";

		public readonly uint Register;
		public readonly uint Bit;

		public PICCommand_BTFSC(string sct, uint scl, uint pos, uint cmd)
			: base(sct, scl, pos, cmd)
		{
			Register = Parameter.GetParam('f').Value;
			Bit = Parameter.GetParam('b').Value;
		}

		private bool TestCondition(PICController controller) // Returns True if Skip
		{
			return !BinaryHelper.GetBit(controller.GetBankedRegister(Register), Bit);
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
