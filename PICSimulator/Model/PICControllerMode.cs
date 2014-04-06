
namespace PICSimulator.Model
{
	public enum PICControllerMode
	{
		WAITING, // Pre Running
		PAUSED,  // Manually paused or by Breakpoint
		RUNNING, // Normal Running
		CONTINUE,// Get Away from Pause
		SKIPONE, // Execute One Command in Pause
		FINISHED // Programm finished
	}
}
