
namespace PICSimulator.Model.Events
{
	class ManuallyRegisterChangedEvent : PICEvent
	{
		public uint RegisterPos;
		public uint NewValue;
	}
}
