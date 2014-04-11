using PICSimulator.Model.Events;

namespace PICSimulator.Model
{
	public class PICClock
	{
		public double time { get; private set; }

		public bool Enabled { get; private set; }
		public uint Register { get; private set; }
		public uint Bit { get; private set; }
		public uint Frequency { get; private set; }

		public PICClock()
		{
			Reset();
		}

		public void Reset()
		{
			time = 0;

			Enabled = false;
			Register = PICMemory.ADDR_UNIMPL_A;
			Bit = 0;
			Frequency = 1000000;
		}

		public void UpdateState(ExternalClockChangedEvent e)
		{
			Reset();

			Frequency = e.Frequency;
			Bit = e.Bit;
			Register = e.Register;
			Enabled = e.Enabled;
		}

		public void Update(PICController controller)
		{
			if (Enabled)
			{
				time += 1.0 / controller.EmulatedFrequency;

				if (time >= (1.0 / Frequency))
				{
					controller.SetUnbankedRegisterBit(Register, Bit, !controller.GetUnbankedRegisterBit(Register, Bit));
					time -= (1.0 / Frequency);
				}
			}
			else
			{
				time = 0;
			}
		}
	}
}
