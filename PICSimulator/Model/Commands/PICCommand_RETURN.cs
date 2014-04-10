
namespace PICSimulator.Model.Commands
{
	/// <summary>
	/// Return from subroutine. The stack is
	/// POPed and the top of the stack (TOS)
	/// is loaded into the program counter. This
	/// is a two cycle instruction.
	/// </summary>
	class PICCommand_RETURN : PICCommand
	{
		public const string COMMANDCODE = "00 0000 0000 1000";

		public PICCommand_RETURN(string sct, uint scl, uint pos, uint cmd)
			: base(sct, scl, pos, cmd)
		{

		}

		public override void Execute(PICController controller)
		{
			controller.SetPC_13Bit(controller.PopCallStack());
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
