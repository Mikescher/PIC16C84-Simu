using System;

namespace PICSimulator.Model.Events
{
	class PopCallStackEvent : PICEvent
	{
		public override string ToString()
		{
			return String.Format(@"PopCallStackEvent :> CallStack --");
		}
	}
}
