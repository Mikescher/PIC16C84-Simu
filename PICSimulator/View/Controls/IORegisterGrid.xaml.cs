using PICSimulator.Helper;
using System.Windows.Controls;

namespace PICSimulator.View
{
	/// <summary>
	/// Interaction logic for IORegisterGrid.xaml
	/// </summary>
	public partial class IORegisterGrid : UserControl
	{
		private ChangeMarkTextBlock[] txtTRIS;
		private ChangeMarkTextBlock[] txtPINS;

		public bool[] tris { get; private set; } // false = 'o' || true = 'i'
		public bool[] pins { get; private set; }

		public string Caption { get; set; }

		private RegisterGrid ParentWindow;
		private uint Position_PINS;
		private uint Position_TRIS;

		public IORegisterGrid()
		{
			InitializeComponent();

			this.DataContext = this;

			tris = new bool[8];
			pins = new bool[8];

			txtTRIS = new ChangeMarkTextBlock[] { Tris0, Tris1, Tris2, Tris3, Tris4, Tris5, Tris6, Tris7 };
			txtPINS = new ChangeMarkTextBlock[] { Pin0, Pin1, Pin2, Pin3, Pin4, Pin5, Pin6, Pin7 };

			Caption = "XXX";
		}

		public void Initialize(RegisterGrid parent, uint pins_pos, uint tris_pos)
		{
			ParentWindow = parent;
			Position_PINS = pins_pos;
			Position_TRIS = tris_pos;

			ParentWindow.RegisterChanged += OnRegisterChanged;

			setTRIS(ParentWindow.get(Position_TRIS));
			setPINS(ParentWindow.get(Position_PINS));
		}

		private void OnRegisterChanged(uint pos, uint val)
		{
			if (pos == Position_TRIS)
			{
				setTRIS(val);
			}
			else if (pos == Position_PINS)
			{
				setPINS(val);
			}
		}

		public void setTRIS(uint val)
		{
			for (uint i = 0; i < 8; i++)
			{
				setTRIS(i, BinaryHelper.GetBit(val, i));
			}
		}

		public void setTRIS(uint pos, bool val)
		{
			tris[pos] = val;

			txtTRIS[pos].Text = val ? "i" : "o";
		}

		public void setPINS(uint val)
		{
			for (uint i = 0; i < 8; i++)
			{
				setPINS(i, BinaryHelper.GetBit(val, i));
			}
		}

		public void setPINS(uint pos, bool val)
		{
			pins[pos] = val;

			txtPINS[pos].Text = val ? "1" : "0";
		}

		private void Pin_MouseDown(uint nmbr)
		{
			setPINS(nmbr, !pins[nmbr]);
			ParentWindow.Set(Position_PINS, GetValue_PINS());
		}

		private void Tris_MouseDown(uint nmbr)
		{
			setTRIS(nmbr, !tris[nmbr]);
			ParentWindow.Set(Position_TRIS, GetValue_TRIS());
		}

		public uint GetValue_PINS()
		{
			uint r = pins[7] ? 1u : 0u;

			for (int i = 6; i >= 0; i--)
			{
				r *= 2;
				r += pins[i] ? 1u : 0u;
			}

			return r;
		}

		public uint GetValue_TRIS()
		{
			uint r = tris[7] ? 1u : 0u;

			for (int i = 6; i >= 0; i--)
			{
				r *= 2;
				r += tris[i] ? 1u : 0u;
			}

			return r;
		}

		#region Obnoxious Mouse Events

		private void Pin7_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
		{
			Pin_MouseDown(7);
		}

		private void Pin6_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
		{
			Pin_MouseDown(6);
		}

		private void Pin5_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
		{
			Pin_MouseDown(5);
		}

		private void Pin4_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
		{
			Pin_MouseDown(4);
		}

		private void Pin3_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
		{
			Pin_MouseDown(3);
		}

		private void Pin2_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
		{
			Pin_MouseDown(2);
		}

		private void Pin1_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
		{
			Pin_MouseDown(1);
		}

		private void Pin0_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
		{
			Pin_MouseDown(0);
		}

		private void Tris7_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
		{
			Tris_MouseDown(7);
		}

		private void Tris6_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
		{
			Tris_MouseDown(6);
		}

		private void Tris5_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
		{
			Tris_MouseDown(5);
		}

		private void Tris4_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
		{
			Tris_MouseDown(4);
		}

		private void Tris3_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
		{
			Tris_MouseDown(3);
		}

		private void Tris2_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
		{
			Tris_MouseDown(2);
		}

		private void Tris1_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
		{
			Tris_MouseDown(1);
		}

		private void Tris0_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
		{
			Tris_MouseDown(0);
		}

		#endregion
	}
}
