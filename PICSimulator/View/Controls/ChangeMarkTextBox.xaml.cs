using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace PICSimulator.View
{
	/// <summary>
	/// Interaction logic for ChangeMarkTextBox.xaml
	/// </summary>
	public partial class ChangeMarkTextBox : UserControl
	{
		public string Text
		{
			get
			{
				return box.Text;
			}

			set
			{
				if (value != box.Text)
				{
					box.Text = value;
					Animate();
				}
			}
		}

		public Color _Color = Colors.Transparent;
		public Color Color
		{
			get
			{
				return _Color;
			}

			set
			{
				panel.Background = new SolidColorBrush(value);
				_Color = value;
			}
		}

		public new double FontSize { get { return box.FontSize; } set { box.FontSize = value; } }
		public new object Tag { get { return box.Tag; } set { box.Tag = value; } }
		public int MaxLength { get { return box.MaxLength; } set { box.MaxLength = value; } }

		public event TextChangedEventHandler TextChanged { add { box.TextChanged += value; } remove { box.TextChanged -= value; } }
		public new event TextCompositionEventHandler PreviewTextInput { add { box.PreviewTextInput += value; } remove { box.PreviewTextInput -= value; } }
		public new event RoutedEventHandler LostFocus { add { box.LostFocus += value; } remove { box.LostFocus -= value; } }
		public new event KeyboardFocusChangedEventHandler LostKeyboardFocus { add { box.LostKeyboardFocus += value; } remove { box.LostKeyboardFocus -= value; } }

		public ChangeMarkTextBox()
		{
			InitializeComponent();
		}

		private void Animate()
		{
			panel.Background = new SolidColorBrush(Color);

			var anim = new ColorAnimation()
			{
				From = Colors.Red,
				To = Color,
				Duration = TimeSpan.FromMilliseconds(1500),
			};

			panel.Background.BeginAnimation(SolidColorBrush.ColorProperty, anim);
		}

		private void box_TextChanged(object sender, TextChangedEventArgs e)
		{
			Animate();
		}
	}
}
