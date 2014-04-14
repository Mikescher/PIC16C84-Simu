using PICSimulator.Model.Commands;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace PICSimulator.Model
{
	static class PICProgramLoader
	{
		public static PICCommand[] LoadFromFile(string file)
		{
			List<PICCommand> list = LoadListFromFile(file);

			if (list == null || list.Count <= 0)
				return null;

			PICCommand[] result = new PICCommand[list.Count];

			for (int p = 0; p < list.Count; p++)
			{
				if (list[p].Position != p)
					return null;

				result[p] = list[p];
			}

			return result;
		}

		public static List<PICCommand> LoadListFromFile(string file)
		{
			string[] lines = File.ReadAllLines(file);

			List<PICCommand> result = new List<PICCommand>();

			foreach (string line in lines)
			{
				if (String.IsNullOrWhiteSpace(line))
					continue;

				var v = splitLine(line);

				if (v == null)
					return null;

				uint? pos = ParseHex(v.Item1);
				uint? cmd = ParseHex(v.Item2);
				uint? scpos = ParseBin(v.Item3);

				string txt = v.Item4;

				if (pos == null || cmd == null || scpos == null)
					continue;

				PICCommand pic_cmd = PICComandHelper.CreateCommand(txt, scpos.Value, pos.Value, cmd.Value);

				if (pic_cmd == null)
					return null;

				result.Add(pic_cmd);
			}

			return result;
		}

		public static string LoadSourceCodeFromText(string file)
		{
			string[] lines = File.ReadAllLines(file);

			List<string> result = new List<string>();

			foreach (string line in lines)
			{
				if (String.IsNullOrWhiteSpace(line))
					continue;

				var v = splitLine(line);

				string txt = v.Item4;

				result.Add(txt);
			}

			return String.Join(Environment.NewLine, result.Select(p => p.Trim()));
		}

		private static Tuple<string, string, string, string> splitLine(string line)
		{
			if (line.Length < 27)
			{
				return null;
			}

			string a = line.Substring(00, 05);
			string b = line.Substring(05, 15);
			string c = line.Substring(20, 05);
			string d = line.Substring(25, line.Length - 25);

			return Tuple.Create(a, b, c, d);
		}

		private static uint? ParseHex(string v)
		{
			v = v.Trim();

			try
			{
				return Convert.ToUInt32(v, 16);
			}
			catch (FormatException)
			{
				return null;
			}
			catch (ArgumentException)
			{
				return null;
			}
		}

		private static uint? ParseBin(string v)
		{
			v = v.Trim();

			try
			{
				return Convert.ToUInt32(v, 10);
			}
			catch (FormatException)
			{
				return null;
			}
			catch (ArgumentException)
			{
				return null;
			}
		}
	}
}
