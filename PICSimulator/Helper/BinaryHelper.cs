
namespace PICSimulator.Helper
{
	static class BinaryHelper
	{
		public static bool GetBit(int pos, uint val)
		{
			return (val & (1 << pos)) != 0;
		}
	}
}
