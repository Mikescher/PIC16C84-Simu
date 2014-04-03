namespace PICSimulator.Model.Commands
{
	class PICCommand_SUBLW : PICCommand
	{
		public const string COMMANDCODE = "11 110x kkkk kkkk";

		public PICCommand_SUBLW(string sct, uint scl, uint pos, uint cmd)
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
