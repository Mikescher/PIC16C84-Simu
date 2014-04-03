namespace PICSimulator.Model.Commands
{
	class PICCommand_RRF : PICCommand
	{
		public const string COMMANDCODE = "00 1100 dfff ffff";

		public PICCommand_RRF(string sct, uint scl, uint pos, uint cmd)
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
