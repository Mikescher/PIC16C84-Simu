
namespace PICSimulator.Model.Commands
{
	class PICCommand_CALL : PICCommand
	{
		public const string COMMANDCODE = "10 0kkk kkkk kkkk";

		public readonly uint Address;

		public PICCommand_CALL(string sct, uint scl, uint pos, uint cmd)
			: base(sct, scl, pos, cmd)
		{
			Address = Parameter.GetParam('k').Value;
		}

		public override void Execute(PICController controller)
		{
			controller.PushCallStack(controller.GetPC());
			controller.SetPCWithEvent_11Bit(Address);
		}

		public override string GetCommandCodeFormat()
		{
			return COMMANDCODE;
		}

		public override uint GetCycleCount(PICController controller)
		{
			return 2;
		}
	}
}
