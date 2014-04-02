using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PICSimulator.Model.Commands
{
    class PICCommand_ANDWF: PICCommand
	{
		public const string COMMANDCODE = "00 0101 dfff fff";

		public PICCommand_ANDWF(string sct, uint scl, uint pos, uint cmd)
			: base(sct, scl, pos, cmd)
		{

		}
	}
}
