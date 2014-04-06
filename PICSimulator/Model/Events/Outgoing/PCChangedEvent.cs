
using System;
namespace PICSimulator.Model.Events
{
	class PCChangedEvent : PICEvent
	{
		public uint Value;

		public override string ToString()
		{
			return String.Format(@"PCChangedEvent :> PC := {0}", Value);
		}
	}
}
