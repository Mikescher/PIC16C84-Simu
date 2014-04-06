
using System;
namespace PICSimulator.Model.Events
{
	class RegisterChangedEvent : PICEvent
	{
		public uint Position;
		public uint Value;

		public override string ToString()
		{
			return String.Format(@"RegisterChangedEvent :> register[{0}] := {1}", Position, Value);
		}
	}
}
