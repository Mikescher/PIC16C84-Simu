
using System;
namespace PICSimulator.Model.Events
{
	class BreakPointChangedEvent : PICEvent
	{
		public uint Position;
		public bool Value;

		public override string ToString()
		{
			return String.Format(@"BreakPointChangedEvent :> register[{0}] := {1}", Position, Value);
		}
	}
}
