
namespace PICSimulator.Model
{
	class PICController
	{
		public bool isRunning { get; private set; }

		public PICController()
		{
			isRunning = false;
		}
	}
}
