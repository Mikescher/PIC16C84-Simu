
using PICSimulator.Helper;
namespace PICSimulator.Model
{
	class PICWatchDogTimer
	{
		private const double TIME_OUT = 0.018; // 18ms;

		public uint Prescale = 1;

		private double time; // in s

		public bool Enabled = false;

		public PICWatchDogTimer()
		{
			time = 0;
		}

		public void Update(PICController controller, uint cycles)
		{
			if (Enabled)
			{
				time += (1.0 / controller.EmulatedFrequency) * cycles;

				if (time > TIME_OUT * GetPreScale(controller))
				{
					// >> WATCHDOG RESET <<

					controller.SoftReset();
					controller.SetUnbankedRegisterBit(PICMemory.ADDR_STATUS, PICMemory.STATUS_BIT_TO, false);
				}
			}
			else
			{
				time = 0;
			}
		}

		private uint GetPreScale(PICController controller)
		{
			bool prescale_mode = !controller.GetUnbankedRegisterBit(PICMemory.ADDR_OPTION, PICMemory.OPTION_BIT_PSA);

			uint scale = 0;
			scale += controller.GetUnbankedRegisterBit(PICMemory.ADDR_OPTION, PICMemory.OPTION_BIT_PS2) ? 1U : 0U;
			scale *= 2;
			scale += controller.GetUnbankedRegisterBit(PICMemory.ADDR_OPTION, PICMemory.OPTION_BIT_PS1) ? 1U : 0U;
			scale *= 2;
			scale += controller.GetUnbankedRegisterBit(PICMemory.ADDR_OPTION, PICMemory.OPTION_BIT_PS0) ? 1U : 0U;

			Prescale = prescale_mode ? 1 : (BinaryHelper.SHL(1, scale));

			return Prescale;
		}

		public void Reset()
		{
			time = 0;
		}

		public double GetPerc()
		{
			return time / (TIME_OUT * Prescale);
		}
	}
}
