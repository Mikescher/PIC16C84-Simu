
using System.Collections.Generic;
namespace PICSimulator.Model
{
	class CircularStack
	{
		private uint[] data = new uint[8];
		private uint pos = 7; // Is the Position of the current (set) Field

		public void Push(uint v)
		{
			lock (this)
			{
				pos = (pos + 1) % 8; // pos++

				data[pos] = v;
			}
		}

		public uint Pop()
		{
			lock (this)
			{
				uint v = data[pos];

				pos = (pos + 7) % 8; // pos--;

				return v;
			}
		}

		public uint Peek()
		{
			lock (this)
			{
				return data[(pos + 7) % 8];
			}
		}

		public Stack<uint> getAsNativeStack()
		{
			Stack<uint> a = new Stack<uint>();

			lock (this)
			{
				if (pos < 7)
				{
					for (uint i = 0; i < pos; i++)
					{
						a.Push(data[i]);
					}
				}
			}

			return a;
		}
	}
}
