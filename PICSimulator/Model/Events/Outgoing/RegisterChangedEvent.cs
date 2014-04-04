
namespace PICSimulator.Model.Events
{
	class RegisterChangedEvent : PICEvent
	{
		public uint RegisterPos;
		public uint NewValue;
	}
}
