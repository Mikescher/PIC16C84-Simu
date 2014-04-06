using System;

namespace PICSimulator.Model.Events
{
	class StackResetEvent : PICEvent
	{
		public override string ToString()
		{
			return String.Format(@"StackResetEvent :> CallStack := 0");
		}
	}
}
