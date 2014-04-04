using System.Windows.Input;

namespace PICSimulator.View
{
	public static class CustomCmds
	{
		public static RoutedUICommand Compile = new RoutedUICommand("Compile", "Compile", typeof(CustomCmds));
		public static RoutedUICommand Run = new RoutedUICommand("Run", "Run", typeof(CustomCmds));
		public static RoutedUICommand Pause = new RoutedUICommand("Pause", "Pause", typeof(CustomCmds));
		public static RoutedUICommand Continue = new RoutedUICommand("Continue", "Continue", typeof(CustomCmds));
		public static RoutedUICommand Stop = new RoutedUICommand("Continue", "Continue", typeof(CustomCmds));
	}
}
