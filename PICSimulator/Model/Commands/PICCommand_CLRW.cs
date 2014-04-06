
namespace PICSimulator.Model.Commands
{
	class PICCommand_CLRW : PICCommand
	{
		public const string COMMANDCODE = "00 0001 0xxx xxxx";

		public PICCommand_CLRW(string sct, uint scl, uint pos, uint cmd)
			: base(sct, scl, pos, cmd)
		{

		}

		public override void Execute(PICController controller)
		{
			controller.SetWRegisterWithEvent(0x00);
			controller.SetRegisterBitWithEvent(PICController.ADDR_STATUS, PICController.STATUS_BIT_Z, true);
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
