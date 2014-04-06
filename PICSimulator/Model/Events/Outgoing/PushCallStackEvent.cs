
using System;
namespace PICSimulator.Model.Events
{
	class PushCallStackEvent : PICEvent
	{
		public uint Value;

		public override string ToString()
		{
			return String.Format(@"PushCallStackEvent :> CallStack += {0}", Value);
		}
	}
}
