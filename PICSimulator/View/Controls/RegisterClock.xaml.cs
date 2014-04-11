using PICSimulator.Model;
using PICSimulator.Model.Events;
using System.Windows.Controls;

namespace PICSimulator.View
{
	/// <summary>
	/// Interaction logic for RegisterClock.xaml
	/// </summary>
	public partial class RegisterClock : UserControl
	{
		private MainWindow ParentWindow = null;
		private uint Clock_ID = 999;

		public RegisterClock()
		{
			InitializeComponent();
		}

		public void Intialize(MainWindow p, uint id)
		{
			ParentWindow = p;
			Clock_ID = id;
		}

		private void enabledBox_Checked(object sender, System.Windows.RoutedEventArgs e)
		{
			bool v = (sender as CheckBox).IsChecked.Value;

			regBox.IsEnabled = !v;
			bitBox.IsEnabled = !v;
			freqCtrl.IsEnabled = !v;

			uint reg = regBox.Value;
			uint b = (uint)bitBox.SelectedIndex;
			uint freq = (uint)freqCtrl.Value.Value;

			if (ParentWindow != null)
			{
				ParentWindow.SendEventToController(new ExternalClockChangedEvent()
				{
					ClockID = Clock_ID,
					Enabled = v,
					Frequency = freq,
					Bit = b,
					Register = reg,
				});
			}
		}

		public void ResetUI()
		{
			if (enabledBox.IsChecked.Value)
			{
				if (Clock_ID < 4)
				{
					bool e = false;
					uint b = 0;
					uint r = PICMemory.ADDR_UNIMPL_A;
					uint f = 1000000;

					freqCtrl.Value = (int)f;
					regBox.Value = r;
					bitBox.SelectedIndex = (int)b;

					if (e != enabledBox.IsChecked)
					{
						enabledBox.IsChecked = e;
					}
				}
			}
		}

		public void UpdateUI(PICController controller)
		{
			if (enabledBox.IsChecked.Value)
			{
				if (Clock_ID < 4)
				{
					PICClock c = controller.GetExternalClock(Clock_ID);

					bool e = c.Enabled;
					uint b = c.Bit;
					uint r = c.Register;
					uint f = c.Frequency;

					freqCtrl.Value = (int)f;
					regBox.Value = r;
					bitBox.SelectedIndex = (int)b;

					if (e != enabledBox.IsChecked)
					{
						enabledBox.IsChecked = e;
					}
				}
			}
		}
	}
}
