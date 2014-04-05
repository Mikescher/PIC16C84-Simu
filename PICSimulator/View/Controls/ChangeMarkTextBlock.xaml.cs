using System;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace PICSimulator.View
{
	/// <summary>
	/// Interaction logic for ChangeMarkTextBlock.xaml
	/// </summary>
	public partial class ChangeMarkTextBlock : UserControl
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

		public ChangeMarkTextBlock()
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
	}
}
