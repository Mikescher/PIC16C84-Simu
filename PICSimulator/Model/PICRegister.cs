using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PICSimulator.Model
{
	class PICRegister
	{
		uint[] register = new uint[0xFF];

		PICRegister()
		{
			// Set all values to Zero
			for (uint i = 0; i < register.Length; i++)
			{
				register[i] = 0;
			}

			// Set teh speacial Vaules.
		}

		uint getFullContent(uint _address)
		{
			if(_address < register.Length)
			return register[_address];
			else
			{
				new System.ArgumentException("Parameter isn't in the allowed ranged.", "address");
				return 0;
			}
		}
	}
}
