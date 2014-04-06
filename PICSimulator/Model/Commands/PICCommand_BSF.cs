
namespace PICSimulator.Model.Commands
{
	class PICCommand_BSF : PICCommand
	{
		public const string COMMANDCODE = "01 01bb bfff ffff";

		public readonly uint Register;
		public readonly uint Bit;

		public PICCommand_BSF(string sct, uint scl, uint pos, uint cmd)
			: base(sct, scl, pos, cmd)
		{
			Register = Parameter.GetParam('f').Value;
			Bit = Parameter.GetParam('b').Value;
		}

		public override void Execute(PICController controller)
		{
			controller.SetRegisterBitWithEvent(Register, Bit, true);
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
