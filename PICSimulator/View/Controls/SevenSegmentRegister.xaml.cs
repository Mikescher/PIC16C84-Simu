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

		public void SourceChanged(object sender, SelectionChangedEventArgs e)
		{
			display.ChangeSource(box.Value);
		}

		public void Initialize(RegisterGrid parent)
		{
			display.Initialize(parent);
		}
	}
}
