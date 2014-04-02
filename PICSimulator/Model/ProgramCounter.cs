
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PICSimulator.Model
{
    class ProgramCounter
    {
        uint programmCounter = 0;

        void inkrement()
        {
            programmCounter++;
        }

        void PCgoto(uint _addr)
        {
            programmCounter = _addr;
        }

        void PCcall(uint _addr)
        {
            // Addr auf Stack
            programmCounter = _addr;
        }

        void PCreturn()
        {
            // Addr. von Stack
        }

        void reset()
        {
            programmCounter = 0;
        }
    }
}
