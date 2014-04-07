
namespace PICSimulator.Model.Commands
{
	class PICCommand_CLRF : PICCommand
	{
		public const string COMMANDCODE = "00 0001 1fff ffff";

		public readonly uint Register;

		public PICCommand_CLRF(string sct, uint scl, uint pos, uint cmd)
			: base(sct, scl, pos, cmd)
		{
			Register = Parameter.GetParam('f').Value;
		}

		public override void Execute(PICController controller)
		{
			controller.SetRegister(Register, 0x00);
			controller.SetRegisterBit(PICMemory.ADDR_STATUS, PICMemory.STATUS_BIT_Z, true);
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
