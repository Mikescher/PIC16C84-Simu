using System.Windows.Input;

namespace PICSimulator.View
{
	public static class CustomCmds
	{
		public static RoutedUICommand Compile = new RoutedUICommand("Compile", "Compile", typeof(CustomCmds));

	}
}
