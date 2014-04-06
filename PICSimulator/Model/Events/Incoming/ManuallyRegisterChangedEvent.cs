
using System;
namespace PICSimulator.Model.Events
{
	class ManuallyRegisterChangedEvent : PICEvent
	{
		public uint Position;
		public uint Value;

		public override string ToString()
		{
			return String.Format(@"ManuallyRegisterChangedEvent :> register[{0}] := {1}", Position, Value);
		}
	}
}
