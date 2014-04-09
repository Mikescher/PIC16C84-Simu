using PICSimulator.Helper;
using PICSimulator.Model;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace PICSimulator.View
{
	/// <summary>
	/// Interaction logic for _7SegmentDisplay.xaml
	/// </summary>
	public partial class _7SegmentDisplay : UserControl
	{
		private Shape[] mainShapes;
		private Shape[] blur1Shapes;
		private Shape[] blur2Shapes;

		private Brush brush_on = new SolidColorBrush(Colors.Red);
		private Brush brush_off = new SolidColorBrush(Colors.Transparent);

		private uint Position = PICMemory.ADDR_UNIMPL_A;

		private RegisterGrid ParentWindow;

		public _7SegmentDisplay()
		{
			InitializeComponent();

			mainShapes = new Shape[8] { seg_0, seg_1, seg_2, seg_3, seg_4, seg_5, seg_6, seg_7 };
			blur1Shapes = new Shape[8] { o1seg_0, o1seg_1, o1seg_2, o1seg_3, o1seg_4, o1seg_5, o1seg_6, o1seg_7 };
			blur2Shapes = new Shape[8] { o2seg_0, o2seg_1, o2seg_2, o2seg_3, o2seg_4, o2seg_5, o2seg_6, o2seg_7 };
		}

		public void Initialize(RegisterGrid parent)
		{
			ParentWindow = parent;

			ParentWindow.RegisterChanged += OnRegisterChanged;
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

		public void SetValue(uint p, bool v)
		{
			mainShapes[p].Fill = v ? brush_on : brush_off;
			blur1Shapes[p].Fill = v ? brush_on : brush_off;
			blur2Shapes[p].Fill = v ? brush_on : brush_off;
		}

		public void ChangeSource(uint p)
		{
			Position = p;

			SetValue(ParentWindow.get(Position));
		}
	}
}
