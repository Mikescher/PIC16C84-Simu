namespace PICSimulator.Model.Commands
{
	class PICCommand_DECF : PICCommand
	{
		public const string COMMANDCODE = "00 0011 dfff ffff";

		public PICCommand_DECF(string sct, uint scl, uint pos, uint cmd)
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
