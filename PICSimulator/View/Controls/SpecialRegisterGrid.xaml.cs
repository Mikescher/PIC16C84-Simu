using PICSimulator.Helper;
using System.Windows.Controls;

namespace PICSimulator.View
{
	/// <summary>
	/// Interaction logic for IORegisterGrid.xaml
	/// </summary>
	public partial class SpecialRegisterGrid : UserControl
	{
		private ChangeMarkTextBlock[] txtReg;

		public bool[] reg { get; private set; }

		public string Caption { get; set; }

		public string Title_0 { get; set; }
		public string Title_1 { get; set; }
		public string Title_2 { get; set; }
		public string Title_3 { get; set; }
		public string Title_4 { get; set; }
		public string Title_5 { get; set; }
		public string Title_6 { get; set; }
		public string Title_7 { get; set; }

		private RegisterGrid ParentWindow;
		private uint Position;

		public SpecialRegisterGrid()
		{
			InitializeComponent();

			this.DataContext = this;

			reg = new bool[8];

			txtReg = new ChangeMarkTextBlock[] { Pin0, Pin1, Pin2, Pin3, Pin4, Pin5, Pin6, Pin7 };

			Caption = "XXX";

			Title_0 = "[0]";
			Title_1 = "[1]";
			Title_2 = "[2]";
			Title_3 = "[3]";
			Title_4 = "[4]";
			Title_5 = "[5]";
			Title_6 = "[6]";
			Title_7 = "[7]";
		}

		public void Initialize(RegisterGrid parent, uint pos)
		{
			ParentWindow = parent;
			Position = pos;

			ParentWindow.RegisterChanged += OnRegisterChanged;

			SetValue(ParentWindow.get(Position));
		}

		private void OnRegisterChanged(uint pos, uint val)
		{
			if (pos == Position)
			{
				SetValue(val);
			}
		}

		public void SetValue(uint val)
		{
			for (uint i = 0; i < 8; i++)
			{
				SetValue(i, BinaryHelper.GetBit(val, i));
			}
		}

		public void SetValue(uint pos, bool val)
		{
			reg[pos] = val;

			txtReg[pos].Text = val ? "1" : "0";
		}

		private void Reg_MouseDown(uint nmbr)
		{
			SetValue(nmbr, !reg[nmbr]);
			ParentWindow.Set(Position, GetValue());
		}

		public uint GetValue()
		{
			uint r = reg[7] ? 1u : 0u;

			for (int i = 6; i >= 0; i--)
			{
				r *= 2;
				r += reg[i] ? 1u : 0u;
			}

			return r;
		}

		#region Obnoxious Mouse Events

		private void Reg7_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
		{
			Reg_MouseDown(7);
		}

		private void Reg6_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
		{
			Reg_MouseDown(6);
		}

		private void Reg5_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
		{
			Reg_MouseDown(5);
		}

		private void Reg4_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
		{
			Reg_MouseDown(4);
		}

		private void Reg3_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
		{
			Reg_MouseDown(3);
		}

		private void Reg2_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
		{
			Reg_MouseDown(2);
		}

		private void Reg1_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
		{
			Reg_MouseDown(1);
		}

		private void Reg0_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
		{
			Reg_MouseDown(0);
		}

		#endregion
	}
}
