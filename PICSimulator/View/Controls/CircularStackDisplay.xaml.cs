using PICSimulator.Model;
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
				string txt = (Stack.Count > i) ? FormatLine(Stack[i].Item1, Stack[i].Item2) : "";

				if (IndicatorTextBlocks[i].Text != ind)
					IndicatorTextBlocks[i].Text = ind;

				if (LineTextBlocks[i].Text != txt)
					LineTextBlocks[i].Text = txt;
			}
		}

		private string FormatLine(uint p, string line)
		{
			string t_pc = string.Format(@"[{0:X03}] ", p);
			string t_line = line.Trim(' ', '\t', '\r', '\n');

			t_line = t_line.Replace("\t", " ");

			while (t_line != t_line.Replace("  ", " "))
			{
				t_line = t_line.Replace("  ", " ");
			}

			t_line = t_line.Substring(0, Math.Min(11, t_line.Length));

			return t_pc + t_line;
		}

		public void UpdateValues(Stack<uint> v, PICController controller)
		{
			Stack.Clear();

			while (v.Count > 0)
			{
				uint sv = v.Pop();
				Stack.Insert(0, Tuple.Create(sv, controller.GetSourceCodeForPC(sv)));
			}

			Stack.Insert(0, Tuple.Create(0U, "MAIN"));

			Update();
		}
	}
}
