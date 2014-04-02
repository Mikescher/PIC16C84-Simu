using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PICSimulator.Model.Commands
{
    class PICCommand_DECF: PICCommand
	{
		public const string COMMANDCODE = "00 0011 dfff ffff";

        public PICCommand_DECF(string sct, uint scl, uint pos, uint cmd)
			: base(sct, scl, pos, cmd)
		{

		}
	}
}
