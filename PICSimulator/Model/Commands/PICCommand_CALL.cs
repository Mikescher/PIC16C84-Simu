
namespace PICSimulator.Model.Commands
{
	/// <summary>
	/// Call Subroutine. First, return address
	/// (PC+1) is pushed onto the stack. The
	/// eleven bit immediate address is loaded
	/// into PC bits <10:0>. The upper bits of
	/// the PC are loaded from PCLATH. CALL
	/// is a two cycle instruction.
	/// </summary>
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
			controller.SetPC_11Bit(Address);
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
