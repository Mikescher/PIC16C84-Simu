using System;
using System.Collections.Generic;
using System.Linq;

namespace PICSimulator.Model.Commands
{
	class BinaryFormatParser
	{
		private Dictionary<char, uint> Parameter;

		private BinaryFormatParser(Dictionary<char, uint> pms)
		{
			Parameter = pms;
		}

		public uint? GetParam(char c)
		{
			if (Parameter.ContainsKey(c))
			{
				return Parameter[c];
			}
			else
			{
				return null;
			}
		}

		public static BinaryFormatParser Parse(string fmt, uint val)
		{
			string format = fmt.Replace(" ", String.Empty);
			string value = Convert.ToString(val, 2);

			if (value.Length > format.Length)
				return null;

			while (format.Length > value.Length)
				value = "0" + value;

			Dictionary<char, string> parameter_str = new Dictionary<char, string>();

			for (int i = 0; i < format.Length; i++)
			{
				if (format[i] == '0' || format[i] == '1')
				{
					if (format[i] == value[i])
						continue;
				}
				else if (format[i] == 'x')
				{
					continue;
				}
				else
				{
					if (parameter_str.ContainsKey(format[i]))
					{
						parameter_str[format[i]] += value[i];
					}
					else
					{
						parameter_str[format[i]] = "" + value[i];
					}
				}
			}

			Dictionary<char, uint> parameter = new Dictionary<char, uint>();
			parameter_str.Select(p => new KeyValuePair<char, uint>(p.Key, Convert.ToUInt32(p.Value, 2))).ToList().ForEach(p => parameter.Add(p.Key, p.Value));

			return new BinaryFormatParser(parameter);
		}

		public static bool TryParse(string fmt, uint val)
		{
			return Parse(fmt, val) != null;
		}
	}
}
