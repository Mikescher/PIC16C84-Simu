
using System;
namespace PICSimulator.Model.Events
{
	class WRegisterChangedEvent : PICEvent
	{
		public uint Value;

		public override string ToString()
		{
			return String.Format(@"WRegisterChangedEvent :> [W] := {0}", Value);
		}
	}
}
