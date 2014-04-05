using PICSimulator.Model.Events;
using System;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace PICSimulator.View
{
	/// <summary>
	/// Interaction logic for RegisterGrid.xaml
	/// </summary>
	public partial class RegisterGrid : UserControl
	{
		private const int CELL_COUNT_X = 8;
		private const int CELL_COUNT_Y = 32;

		private const int CELL_FONT_SIZE = 14;

		private static string[] CELL_HEADER_Y = 
		{ 
			"00", "08", "10", "18", "20", "28", "30", "38", 
			"40", "48", "50", "58", "60", "68", "70", "78", 
			"80", "88", "90", "98", "A0", "A8", "B0", "B8", 
			"C0", "C8", "D0", "D8", "E0", "E8", "F0", "F8", "XX", "XX", "XX"
		};

		private uint[] _values = new uint[CELL_COUNT_X * CELL_COUNT_Y];
		private TextBox[] textboxes = new TextBox[CELL_COUNT_X * CELL_COUNT_Y];

		public event RegisterChangedEvent RegisterChanged;
		public delegate void RegisterChangedEvent(uint pos, uint val);

		public MainWindow Parent;

		public RegisterGrid()
		{
			InitializeComponent();

			CreateGrid();
		}

		private void CreateGrid()
		{
			#region Definitions

			for (int y = 0; y < CELL_COUNT_Y + 1; y++)
			{
				gridMain.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(0, GridUnitType.Auto) });
			}

			for (int x = 0; x < CELL_COUNT_X + 1; x++)
			{
				gridMain.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(0, GridUnitType.Auto) });
			}

			#endregion

			#region Header

			// Header (TOPLEFT)

			{
				Border b = new Border()
				{
					Background = new SolidColorBrush(Colors.Gainsboro),
					BorderBrush = new SolidColorBrush(Colors.Black),
					BorderThickness = new Thickness(1, 1, 1, 1)
				};

				TextBlock t = new TextBlock()
				{
					Background = new SolidColorBrush(Colors.Gainsboro),
					Text = string.Format(""),
					FontFamily = new FontFamily("Courier New"),
					FontSize = CELL_FONT_SIZE,
					VerticalAlignment = VerticalAlignment.Center,
					HorizontalAlignment = HorizontalAlignment.Center,
					FontWeight = FontWeights.Bold
				};

				gridMain.Children.Add(b);
				Grid.SetRow(b, 0);
				Grid.SetColumn(b, 0);

				b.Child = t;
			}

			// Header (Y)

			for (int y = 0; y < CELL_COUNT_Y; y++)
			{
				Border b = new Border()
				{
					Background = new SolidColorBrush(Colors.Gainsboro),
					BorderBrush = new SolidColorBrush(Colors.Black),
					BorderThickness = new Thickness(1, 0, 1, 1)
				};

				TextBlock t = new TextBlock()
				{
					Background = new SolidColorBrush(Colors.Gainsboro),
					Text = string.Format(CELL_HEADER_Y[y]),
					FontFamily = new FontFamily("Courier New"),
					FontSize = CELL_FONT_SIZE,
					VerticalAlignment = VerticalAlignment.Center,
					HorizontalAlignment = HorizontalAlignment.Center,
					FontWeight = FontWeights.Bold
				};

				gridMain.Children.Add(b);
				Grid.SetRow(b, y + 1);
				Grid.SetColumn(b, 0);

				b.Child = t;
			}

			// Header (X)

			for (int x = 0; x < CELL_COUNT_X; x++)
			{
				Border b = new Border()
				{
					Background = new SolidColorBrush(Colors.Gainsboro),
					BorderBrush = new SolidColorBrush(Colors.Black),
					BorderThickness = new Thickness(0, 1, 1, 1)
				};

				TextBlock t = new TextBlock()
				{
					Background = new SolidColorBrush(Colors.Gainsboro),
					Text = string.Format("{0:X02}", x),
					FontFamily = new FontFamily("Courier New"),
					FontSize = CELL_FONT_SIZE,
					VerticalAlignment = VerticalAlignment.Center,
					HorizontalAlignment = HorizontalAlignment.Center,
					FontWeight = FontWeights.Bold
				};

				gridMain.Children.Add(b);
				Grid.SetRow(b, 0);
				Grid.SetColumn(b, x + 1);

				b.Child = t;
			}

			#endregion

			#region Elements

			// Elements

			for (int x = 0; x < CELL_COUNT_X; x++)
			{
				for (int y = 0; y < CELL_COUNT_Y; y++)
				{
					Border b = new Border()
					{
						BorderBrush = new SolidColorBrush(Colors.Black),
						BorderThickness = new Thickness(0, 0, 1, 1)
					};

					TextBox t = new TextBox()
					{
						Background = new SolidColorBrush(Colors.Transparent),
						BorderThickness = new Thickness(0),
						Text = "00",
						FontFamily = new FontFamily("Courier New"),
						FontSize = CELL_FONT_SIZE,
						VerticalAlignment = VerticalAlignment.Center,
						HorizontalAlignment = HorizontalAlignment.Center,
						Tag = y * CELL_COUNT_X + x,
						MaxLength = 2
					};

					t.PreviewTextInput += cell_PreviewTextInput;
					t.TextChanged += cell_TextChanged;
					t.LostFocus += cell_LostFocus;
					t.LostKeyboardFocus += cell_LostKeyboardFocus;

					textboxes[(int)t.Tag] = t;

					gridMain.Children.Add(b);
					Grid.SetRow(b, y + 1);
					Grid.SetColumn(b, x + 1);

					b.Child = t;
				}
			}

			#endregion
		}

		private void cell_LostKeyboardFocus(object sender, System.Windows.Input.KeyboardFocusChangedEventArgs e)
		{
			TextBox t = sender as TextBox;
			if (t == null)
				return;
			int pos = (int)t.Tag;

			t.Text = String.Format("{0:X02}", _values[pos]);
		}

		private void cell_LostFocus(object sender, RoutedEventArgs e)
		{
			TextBox t = sender as TextBox;
			if (t == null)
				return;
			int pos = (int)t.Tag;

			t.Text = String.Format("{0:X02}", _values[pos]);
		}

		private void cell_TextChanged(object sender, TextChangedEventArgs e)
		{
			TextBox t = sender as TextBox;
			if (t == null)
				return;
			int pos = (int)t.Tag;


			if (t.Text.ToUpper() != t.Text)
			{
				int ss = t.SelectionStart;
				t.Text = t.Text.ToUpper();
				t.SelectionStart = ss;
			}

			if (!string.IsNullOrWhiteSpace(t.Text))
			{
				Set((uint)pos, Convert.ToUInt32(t.Text, 16), false);
			}
		}

		private void cell_PreviewTextInput(object sender, System.Windows.Input.TextCompositionEventArgs e)
		{
			TextBox t = sender as TextBox;
			if (t == null)
				return;

			e.Handled = !Regex.Match(e.Text, @"^[0-9A-Fa-f]$").Success;
		}

		public uint get(uint pos)
		{
			return _values[pos];
		}

		public void Set(uint pos, uint val, bool updateBoxes = true)
		{
			//val = Math.Min(val, 0xFF);

			_values[pos] = val;

			if (updateBoxes)
				textboxes[pos].Text = String.Format("{0:X02}", val);

			if (RegisterChanged != null)
				RegisterChanged(pos, val);
		}

		public void SetWithPICEvent(uint pos, uint val)
		{
			Set(pos, val);

			Parent.SendEventToController(new ManuallyRegisterChangedEvent() { RegisterPos = pos, NewValue = val });
		}
	}
}
