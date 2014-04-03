namespace PICSimulator.Model.Commands
{
	class PICCommand_CLRF : PICCommand
	{
		public const string COMMANDCODE = "00 0001 1fff ffff";

		public PICCommand_CLRF(string sct, uint scl, uint pos, uint cmd)
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
