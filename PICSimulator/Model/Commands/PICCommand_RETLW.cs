
namespace PICSimulator.Model.Commands
{
	/// <summary>
	/// The W register is loaded with the eight
	/// bit literal 'k'. The program counter is
	/// loaded from the top of the stack (the
	/// return address). This is a two cycle
	/// instruction.
	/// </summary>
	class PICCommand_RETLW : PICCommand
	{
		public const string COMMANDCODE = "11 01xx kkkk kkkk";

		public readonly uint Literal;

		public PICCommand_RETLW(string sct, uint scl, uint pos, uint cmd)
			: base(sct, scl, pos, cmd)
		{
			Literal = Parameter.GetParam('k').Value;
		}

		public override void Execute(PICController controller)
		{
			controller.SetWRegister(Literal);

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
