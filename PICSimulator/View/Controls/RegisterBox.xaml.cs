using System;
using System.Windows.Controls;

namespace PICSimulator.View
{
	/// <summary>
	/// Interaction logic for RegisterBox.xaml
	/// </summary>
	public partial class RegisterBox : UserControl
	{
		public new event SelectionChangedEventHandler SelectionChanged { add { box.SelectionChanged += value; } remove { box.SelectionChanged -= value; } }

		public uint Value //TODO Implement possibility for custom choosen register
		{
			get
			{
				return Convert.ToUInt32(((box.SelectedItem as ComboBoxItem).Tag as string), 16);
			}
		}

		public RegisterBox()
		{
			InitializeComponent();
		}
	}
}
