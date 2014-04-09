using System.Windows;

namespace PICSimulator.View
{
	/// <summary>
	/// Interaction logic for FrequencyInputDialog.xaml
	/// </summary>
	public partial class FrequencyInputDialog : Window
	{
		public delegate void FreqChangedEvent(int v);

		internal event FreqChangedEvent Event;

		protected FrequencyInputDialog()
		{
			InitializeComponent();
		}

		public static void Show(uint initial, FreqChangedEvent evt)
		{
			FrequencyInputDialog d = new FrequencyInputDialog();

			d.nmbrCtrl.Value = (int)initial;

			d.Event += evt;

			d.ShowDialog();
		}

		private void Button_Click(object sender, RoutedEventArgs e)
		{
			Event(nmbrCtrl.Value.Value);

			Close();
		}

		private void Button_Click_1(object sender, RoutedEventArgs e)
		{
			Close();
		}
	}
}
