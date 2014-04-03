namespace PICSimulator.Model.Commands
{
	class PICCommand_CALL : PICCommand
	{
		public const string COMMANDCODE = "10 0kkk kkkk kkkk";

		public PICCommand_CALL(string sct, uint scl, uint pos, uint cmd)
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
