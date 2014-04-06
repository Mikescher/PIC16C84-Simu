
namespace PICSimulator.Model
{
	class CircularStack
	{
		private uint[] data = new uint[8];
		private uint pos = 7; // Is the Position of the current (set) Field

		public void Push(uint v)
		{
			pos = (pos + 1) % 8; // pos++

			data[pos] = v;
		}

		public uint Pop()
		{
			uint v = data[pos];

			pos = (pos + 7) % 8; // pos--;

			return v;
		}

		public uint Peek()
		{
			return data[(pos + 7) % 8];
		}
	}
}
