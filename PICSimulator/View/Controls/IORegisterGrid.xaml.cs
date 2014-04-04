using System.Windows.Controls;

namespace PICSimulator.View
{
	/// <summary>
	/// Interaction logic for IORegisterGrid.xaml
	/// </summary>
	public partial class IORegisterGrid : UserControl
	{
		private TextBlock[] txtTRIS;
		private TextBlock[] txtPINS;

		public bool[] tris { get; private set; } // false = 'i' || true = 'o'
		public bool[] pins { get; private set; }

		public string Caption { get; set; }

		public IORegisterGrid()
		{
			InitializeComponent();

			this.DataContext = this;

			tris = new bool[8];
			pins = new bool[8];

			txtTRIS = new TextBlock[] { Tris0, Tris1, Tris2, Tris3, Tris4, Tris5, Tris6, Tris7 };
			txtPINS = new TextBlock[] { Pin0, Pin1, Pin2, Pin3, Pin4, Pin5, Pin6, Pin7 };

			Caption = "XXX";
		}

		public void setTRIS(int pos, bool val)
		{
			if (tris[pos] ^ val)
			{
				tris[pos] = val;

				txtTRIS[pos].Text = val ? "o" : "i";
			}
		}

		public void setPINS(int pos, bool val)
		{
			if (pins[pos] ^ val)
			{
				pins[pos] = val;

				txtPINS[pos].Text = val ? "1" : "0";
			}
		}
	}
}
