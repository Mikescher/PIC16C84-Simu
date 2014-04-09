using System;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;

namespace PICSimulator.View
{
	/// <summary>
	/// Interaction logic for RegisterBox.xaml
	/// </summary>
	public partial class RegisterBox : UserControl
	{
		public delegate void RegisterSelChangedEvent(uint reg);

		public event RegisterSelChangedEvent RegisterChanged;

		public uint Value //TODO Implement possibility for custom choosen register
		{
			get
			{
				return Convert.ToUInt32(((box.SelectedItem as FrameworkElement).Tag as string), 16);
			}
		}

		public RegisterBox()
		{
			InitializeComponent();

			box.SelectionChanged += (sender, e) => { if (RegisterChanged != null) RegisterChanged(Value); };
		}

		private bool suppress_TC_Event = false;
		private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
		{
			if (suppress_TC_Event)
				return;

			TextBox t = sender as TextBox;
			if (t == null)
				return;

			if (t.Text.ToUpper() != t.Text)
			{
				int ss = t.SelectionStart;
				t.Text = t.Text.ToUpper();
				t.SelectionStart = ss;
			}

			int tbv = TryB16(cstmBox.Text);

			if (tbv < 0)
			{
				suppress_TC_Event = true;
				cstmBox.Text = "00";
				suppress_TC_Event = false;
			}
			if (tbv > 0xFF)
			{
				suppress_TC_Event = true;
				cstmBox.Text = "FF";
				suppress_TC_Event = false;
			}

			string s = Convert.ToString(TryB16(cstmBox.Text), 16).ToUpper();

			if (s != t.Text)
			{
				int ss = t.SelectionStart;
				t.Text = s;
				t.SelectionStart = ss;
			}

			cstmBox.Tag = s;
		}

		private int TryB16(string s)
		{
			try
			{
				return Convert.ToInt32(cstmBox.Text, 16);
			}
			catch
			{
				return -1;
			}
		}

		private void TextBox_PreviewTextInput(object sender, System.Windows.Input.TextCompositionEventArgs e)
		{
			TextBox t = sender as TextBox;
			if (t == null)
				return;

			e.Handled = !(Regex.Match(e.Text, @"^[0-9A-Fa-f]$").Success && TryB16(t.Text + e.Text) >= 0x00 && TryB16(t.Text + e.Text) <= 0xFF);
		}
	}
}
