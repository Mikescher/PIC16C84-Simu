using PICSimulator.Model;
using PICSimulator.Model.Events;
using System;
using System.Collections.Generic;
using System.Windows.Controls;

namespace PICSimulator.View
{
	/// <summary>
	/// Interaction logic for CircularStackDisplay.xaml
	/// </summary>
	public partial class CircularStackDisplay : UserControl
	{
		private List<Tuple<uint, string>> Stack;

		private TextBlock[] IndicatorTextBlocks = new TextBlock[9];
		private TextBlock[] LineTextBlocks = new TextBlock[9];

		public CircularStackDisplay()
		{
			InitializeComponent();

			IndicatorTextBlocks = new TextBlock[9] { Ind_0, Ind_1, Ind_2, Ind_3, Ind_4, Ind_5, Ind_6, Ind_7, Ind_8 };
			LineTextBlocks = new TextBlock[9] { Txt_0, Txt_1, Txt_2, Txt_3, Txt_4, Txt_5, Txt_6, Txt_7, Txt_8 };

			Reset();
		}

		public void Reset()
		{
			Stack = new List<Tuple<uint, string>>();

			Stack.Add(Tuple.Create(0U, "MAIN"));

			Update();
		}

		private void Update()
		{
			for (int i = 0; i < 9; i++)
			{
				IndicatorTextBlocks[i].Text = (Stack.Count - 1 == i) ? "\u25BA" : "";
				LineTextBlocks[i].Text = (Stack.Count > i) ? string.Format(@"[0x{0:X04}] {1}", Stack[i].Item1, Stack[i].Item2) : "";
			}
		}

		public void HandleEvent(PICEvent e, PICController controller)
		{
			if (e is PushCallStackEvent)
			{
				PushCallStackEvent ce = e as PushCallStackEvent;
				Stack.Add(Tuple.Create(ce.Value, controller.GetSourceCodeForPC(ce.Value)));
			}
			else if (e is PopCallStackEvent)
			{
				if (Stack.Count > 0)
					Stack.RemoveAt(Stack.Count - 1);
			}
			else if (e is StackResetEvent)
			{
				Reset();
			}

			Update();
		}
	}
}
