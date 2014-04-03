namespace PICSimulator.Model.Commands
{
	class PICCommand_ADDLW : PICCommand
	{
		public const string COMMANDCODE = "11 111x kkkk kkkk";

		public PICCommand_ADDLW(string sct, uint scl, uint pos, uint cmd)
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
