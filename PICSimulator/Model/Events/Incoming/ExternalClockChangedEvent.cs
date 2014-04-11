
namespace PICSimulator.Model.Events
{
	public class ExternalClockChangedEvent : PICEvent
	{
		public uint ClockID;
		public bool Enabled;
		public uint Frequency;
		public uint Register;
		public uint Bit;

		public override string ToString()
		{
			return @"ExternalClockChangedEvent ...";
		}
	}
}
