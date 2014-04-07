
namespace PICSimulator.Helper
{
	static class BinaryHelper
	{
		public static bool GetBit(uint val, uint pos)
		{
			return (val & SHL(1, pos)) != 0;
		}

		public static uint SHL(uint val, uint steps)
		{
			return (uint)((val) << ((int)steps));
		}

		public static uint SHR(uint val, uint steps)
		{
			return (uint)((val) >> ((int)steps));
		}

		public static uint SetBit(uint val, uint pos, bool bit)
		{
			return bit ? (val | SHL(1, pos)) : (val & ~SHL(1, pos));
		}

		public static bool getAdditionDigitCarry(uint a, uint b)
		{
			a %= 0x10;
			b %= 0x10;

			return (a + b) > 0x10;
		}

		public static bool getSubtractionDigitCarry(uint a, uint b)
		{
			a %= 0x10;
			b %= 0x10;

			return (a >= b);
		}
	}
}
