
namespace PICSimulator.Model
{
	enum PICControllerMode
	{
		WAITING, // Pre Running
		PAUSED,  // Manually paused or by Breakpoint
		RUNNING, // Normal Running
		FINISHED // Programm finished
	}
}
