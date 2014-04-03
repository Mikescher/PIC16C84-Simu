
namespace PICSimulator.Model.Commands
{
	abstract class PICCommand
	{
		public readonly string SourceCodeText;	// Line in the src file
		public readonly uint SourceCodeLine;	// LineNmbr in the src file

		public readonly uint Position;
		public readonly uint Command;

		protected readonly BinaryFormatParser Parameter;

		public PICCommand(string sct, uint scl, uint pos, uint cmd)
		{
			SourceCodeText = sct;
			SourceCodeLine = scl;
			Position = pos;
			Command = cmd;

			Parameter = BinaryFormatParser.Parse(GetCommandCodeFormat(), cmd);
		}

		public override string ToString()
		{
			return string.Format("[{0:X04}] {1}:<{2:X04}> ({3}: {4})", Position, this.GetType().Name, Command, SourceCodeLine, SourceCodeText);
		}

		public abstract void Execute(PICController controller);
		public abstract string GetCommandCodeFormat();
	}
}
