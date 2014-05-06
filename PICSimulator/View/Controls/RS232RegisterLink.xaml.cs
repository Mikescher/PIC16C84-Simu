using PICSimulator.Model;
using PICSimulator.Model.Events;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO.Ports;
using System.Linq;
using System.Threading;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Threading;

namespace PICSimulator.View
{
	/// <summary>
	/// Interaction logic for RS232RegisterLink.xaml
	/// </summary>
	public partial class RS232RegisterLink : UserControl
	{
		private const char CR = '\r';
		private const char LF = '\n';

		public bool IsConnected { get; private set; }

		private SerialPort ComPort = new SerialPort();
		private PICController Ctrl = null;
		private Timer Tmr;

		public RS232RegisterLink()
		{
			InitializeComponent();
		}

		public void Intialize()
		{
			cbxPorts.Items.Clear();
			GetSerialPorts().ForEach(lp => cbxPorts.Items.Add(lp));

			Tmr = new Timer((x) => Tmr_Tick(), null, 2500, 500);
		}

		private void Tmr_Tick()
		{
			if (IsConnected && Ctrl != null)
			{
				try
				{
					SendData();
					RecieveData();
				}
				catch (Exception)
				{
					// IGNORE
				}
			}
		}

		private void SendData()
		{
			string p_a = encodeByte(Ctrl.GetUnbankedRegister(PICMemory.ADDR_PORT_A));
			string t_a = encodeByte(Ctrl.GetUnbankedRegister(PICMemory.ADDR_TRIS_A));
			string p_b = encodeByte(Ctrl.GetUnbankedRegister(PICMemory.ADDR_PORT_B));
			string t_b = encodeByte(Ctrl.GetUnbankedRegister(PICMemory.ADDR_TRIS_B));

			string send = t_a + p_a + t_b + p_b;

			Send_RS232(send);
		}

		private void RecieveData()
		{
			string x = ReadDataSegment();

			if (x != string.Empty)
			{
				if (x == null)
				{
					addLog("< [ERR-R] NULL");
				}
				else if (x.Length == 4)
				{
					addLog("< " + x);

					var v = decodeBytes(x);

					if (v == null)
					{
						addLog("< [ERR-D] " + x);
					}
					else
					{
						Ctrl.Incoming_Events.Enqueue(new ManuallyRegisterChangedEvent()
						{
							Position = PICMemory.ADDR_PORT_A,
							Value = v.Item1,
						});

						Ctrl.Incoming_Events.Enqueue(new ManuallyRegisterChangedEvent()
						{
							Position = PICMemory.ADDR_PORT_B,
							Value = v.Item2,
						});
					}
				}
				else
				{
					addLog("< [ERR-L] " + x);
				}

				x = ReadDataSegment();
			}
		}

		private string ReadDataSegment()
		{
			string s = "";

			if (ComPort.BytesToRead >= 5)
			{
				char? c = null;

				int idx = 5;
				while (c != CR && idx > 0)
				{
					c = (char)ComPort.ReadByte();

					s += c;

					idx--;
				}

				if (idx <= 0 && c != CR)
				{
					return null;
				}
			}
			else
			{
				return string.Empty;
			}

			return s.Trim(CR);
		}

		private void addLog(string addtxt)
		{
			Dispatcher.BeginInvoke(new Action(delegate()
			{
				if (edLog.Text.Length > 0)
					edLog.Text += Environment.NewLine;
				edLog.Text += addtxt;

				scrView.ScrollToBottom();
			}));
		}

		public void Update(PICController controller)
		{
			Ctrl = controller;
		}

		private void Send_RS232(string txt)
		{
			if (txt != string.Empty)
			{
				ComPort.Write(txt + CR);
				addLog("> " + txt);
			}
		}

		private List<string> GetSerialPorts()
		{
			string[] ArrayComPortsNames = null;
			int index = -1;
			string ComPortName = null;

			List<string> result = new List<string>();

			ArrayComPortsNames = SerialPort.GetPortNames();
			do
			{
				index += 1;
				result.Add(ArrayComPortsNames[index]);
			}
			while (!((ArrayComPortsNames[index] == ComPortName) || (index == ArrayComPortsNames.GetUpperBound(0))));

			return result.OrderBy(p => p).ToList();
		}

		private string encodeByte(uint b)
		{
			char c1 = (char)(0x30 + ((b & 0xF0) >> 4));
			char c2 = (char)(0x30 + (b & 0x0F));

			return "" + c1 + c2;
		}

		private Tuple<uint, uint> decodeBytes(string s)
		{
			int i0 = s[0] - 0x30;
			int i1 = s[1] - 0x30;
			int i2 = s[2] - 0x30;
			int i3 = s[3] - 0x30;

			if (i0 >= 0 && i1 >= 0 && i2 >= 0 && i3 >= 0 && i0 <= 0xF && i1 <= 0xF && i2 <= 0xF && i3 <= 0xF)
			{
				uint a = (((uint)i0 & 0x0F) << 4) | ((uint)i1 & 0x0F);
				uint b = (((uint)i2 & 0x0F) << 4) | ((uint)i3 & 0x0F);

				return Tuple.Create(a, b);
			}
			else
			{
				return null;
			}
		}

		private void Button_Click(object sender, System.Windows.RoutedEventArgs e)
		{
			if (IsConnected)
			{
				Disconnect();
			}
			else
			{
				Connect();
			}
		}

		private void Connect()
		{
			if (cbxPorts.SelectedItem == null)
				return;

			string selport = cbxPorts.SelectedItem.ToString();

			ComPort = new SerialPort();

			ComPort.PortName = selport;
			ComPort.BaudRate = 4800;
			ComPort.DataBits = 8;
			ComPort.Parity = Parity.None;
			ComPort.StopBits = StopBits.One;

			try
			{
				ComPort.Open();
			}
			catch (Exception ex)
			{
				Debug.WriteLine(ex.ToString());
				return;
			}

			IsConnected = ComPort.IsOpen;

			UpdateUI();
		}

		private void Disconnect()
		{
			ComPort.Close();
			ComPort = null;

			IsConnected = false;

			UpdateUI();
		}

		private void UpdateUI()
		{
			lblStatus.Text = IsConnected ? "Connected" : "Disconnected";
			lblStatus.Foreground = IsConnected ? new SolidColorBrush(Colors.Green) : new SolidColorBrush(Colors.Red);
			cbxPorts.IsEnabled = !IsConnected;
			btnConnect.Content = IsConnected ? "Disconnect" : "Connect";
		}
	}
}
