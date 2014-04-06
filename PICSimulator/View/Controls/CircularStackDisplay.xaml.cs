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
				string ind = (Stack.Count - 1 == i) ? "\u25BA" : "";
				string txt = (Stack.Count > i) ? string.Format(@"[0x{0:X04}] {1}", Stack[i].Item1, Stack[i].Item2) : "";

				if (IndicatorTextBlocks[i].Text != ind)
					IndicatorTextBlocks[i].Text = ind;

				if (LineTextBlocks[i].Text != txt)
					LineTextBlocks[i].Text = txt;
			}
		}

		public void UpdateValues(Stack<uint> v, PICController controller)
		{
			Stack.Clear();

			while (v.Count > 0)
			{
				uint sv = v.Pop();
				Stack.Insert(0, Tuple.Create(sv, controller.GetSourceCodeForPC(sv)));
			}

			Update();
		}
	}
}
