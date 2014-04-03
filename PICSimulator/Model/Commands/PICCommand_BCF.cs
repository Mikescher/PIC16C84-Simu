namespace PICSimulator.Model.Commands
{
	class PICCommand_BCF : PICCommand
	{
		public const string COMMANDCODE = "01 00bb bfff ffff";

		public PICCommand_BCF(string sct, uint scl, uint pos, uint cmd)
			: base(sct, scl, pos, cmd)
		{

		}

		public override void Execute(PICController controller)
		{
			throw new System.NotImplementedException();
		}

		public override string GetCommandCodeFormat()
		{
			return COMMANDCODE;
		}
	}
}
