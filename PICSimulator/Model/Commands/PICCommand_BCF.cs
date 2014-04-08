
namespace PICSimulator.Model.Commands
{
	class PICCommand_BCF : PICCommand
	{
		public const string COMMANDCODE = "01 00bb bfff ffff";

		public readonly uint Register;
		public readonly uint Bit;

		public PICCommand_BCF(string sct, uint scl, uint pos, uint cmd)
			: base(sct, scl, pos, cmd)
		{
			Register = Parameter.GetParam('f').Value;
			Bit = Parameter.GetParam('b').Value;
		}

		public override void Execute(PICController controller)
		{
			controller.SetBankedRegisterBit(Register, Bit, false);
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
