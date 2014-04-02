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

		private const int CELL_FONT_SIZE = 12;

		public RegisterGrid()
		{
			InitializeComponent();

			CreateGrid();
		}

		private void CreateGrid()
		{
			// Definitions

			for (int y = 0; y < CELL_COUNT_Y + 1; y++)
			{
				gridMain.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(0, GridUnitType.Auto) });
			}

			for (int x = 0; x < CELL_COUNT_X + 1; x++)
			{
				gridMain.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(0, GridUnitType.Auto) });
			}

			// Header

			for (int y = 0; y < CELL_COUNT_Y + 1; y++)
			{
				Border b = new Border()
				{
					BorderBrush = new SolidColorBrush(Colors.Black),
					BorderThickness = new Thickness(1)
				};

				TextBlock t = new TextBlock()
				{
					Background = new SolidColorBrush(Colors.Gainsboro),
					Text = string.Format("{0:X02}", y * CELL_COUNT_X),
					FontFamily = new FontFamily("Courier New"),
					FontSize = CELL_FONT_SIZE
				};

				gridMain.Children.Add(b);
				Grid.SetRow(b, y + 1);
				Grid.SetColumn(b, 0);

				b.Child = t;
			}

			for (int x = 0; x < CELL_COUNT_X + 1; x++)
			{
				Border b = new Border()
				{
					BorderBrush = new SolidColorBrush(Colors.Black),
					BorderThickness = new Thickness(0, 0, 1, 1)
				};

				TextBlock t = new TextBlock()
				{
					Background = new SolidColorBrush(Colors.Gainsboro),
					Text = string.Format("{0:X02}", x),
					FontFamily = new FontFamily("Courier New"),
					FontSize = CELL_FONT_SIZE
				};

				gridMain.Children.Add(b);
				Grid.SetRow(b, 0);
				Grid.SetColumn(b, x + 1);

				b.Child = t;
			}

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
						BorderThickness = new Thickness(0),
						Text = "00",
						FontFamily = new FontFamily("Courier New"),
						FontSize = CELL_FONT_SIZE
					};

					gridMain.Children.Add(b);
					Grid.SetRow(b, y + 1);
					Grid.SetColumn(b, x + 1);

					b.Child = t;
				}
			}
		}
	}
}
