using System.Windows.Controls;

namespace PICSimulator.View
{
	/// <summary>
	/// Interaction logic for SevenSegmentRegister.xaml
	/// </summary>
	public partial class SevenSegmentRegister : UserControl
	{
		public SevenSegmentRegister()
		{
			InitializeComponent();
		}

		public void SourceChanged(uint reg)
		{
			display.ChangeSource(reg);
		}

		public void Initialize(RegisterGrid parent)
		{
			display.Initialize(parent);
		}
	}
}
