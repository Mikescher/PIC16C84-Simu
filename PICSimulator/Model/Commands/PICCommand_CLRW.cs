using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PICSimulator.Model.Commands
{
    class PICCommand_CLRW: PICCommand
	{
		public const string COMMANDCODE = "00 0001 0xxx xxxx";

        public PICCommand_CLRW(string sct, uint scl, uint pos, uint cmd)
			: base(sct, scl, pos, cmd)
		{

		}
	}
}
