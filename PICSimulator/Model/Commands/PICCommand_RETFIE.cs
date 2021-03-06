﻿
namespace PICSimulator.Model.Commands
{
	/// <summary>
	/// Return from Interrupt. Stack is POPed
	/// and Top of Stack (TOS) is loaded in the
	/// PC. Interrupts are enabled by setting
	/// Global Interrupt Enable bit, GIE
	/// (INTCON<7>). This is a two cycle
	/// instruction.
	/// </summary>
	class PICCommand_RETFIE : PICCommand
	{
		public const string COMMANDCODE = "00 0000 0000 1001";

		public PICCommand_RETFIE(string sct, uint scl, uint pos, uint cmd)
			: base(sct, scl, pos, cmd)
		{

		}

		public override void Execute(PICController controller)
		{
			controller.SetPC_13Bit(controller.PopCallStack());
			controller.SetUnbankedRegisterBit(PICMemory.ADDR_INTCON, PICMemory.INTCON_BIT_GIE, true);
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
