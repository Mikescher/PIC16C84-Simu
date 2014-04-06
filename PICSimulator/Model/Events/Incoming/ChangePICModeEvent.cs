
using System;
namespace PICSimulator.Model.Events
{
	class ChangePICModeEvent : PICEvent
	{
		public PICControllerMode Value;

		public override string ToString()
		{
			return String.Format(@"ChangePICModeEvent :> Mode := {0}", Value);
		}
	}
}
