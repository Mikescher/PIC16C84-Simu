using ICSharpCode.AvalonEdit.Highlighting;
using ICSharpCode.AvalonEdit.Highlighting.Xshd;
using PICSimulator.Helper;
using PICSimulator.Model;
using PICSimulator.Model.Events;
using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Text;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;
using System.Xml;


namespace PICSimulator.View
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		private FrequencyCounter IdleCounter = new FrequencyCounter();

		private SourcecodeDocument sc_document;
		private PICController controller = null;

		private IconBarMargin IconBar;

		public MainWindow()
		{
			InitializeComponent();

			Init();


			sc_document = new SourcecodeDocument(this, txtCode);

			string p = Path.GetFullPath(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), @"..\..\"));
			p = Path.Combine(p, @"Testdata_2\test.src");

			sc_document = new SourcecodeDocument( //TODO Remove Me - Only for ... reasons
				this,
				txtCode,
				File.ReadAllText(p, Encoding.Default),
				p);

			DispatcherTimer itimer = new DispatcherTimer(DispatcherPriority.ApplicationIdle);
			itimer.Tick += (s, e) => onIdle();
			itimer.Start();
		}

		private void Init()
		{
			DataContext = this;

			using (XmlReader reader = new XmlTextReader(new StringReader(Properties.Resources.Assembler)))
			{
				IHighlightingDefinition customHighlighting = HighlightingLoader.Load(reader, HighlightingManager.Instance);
				txtCode.SyntaxHighlighting = customHighlighting;
			}

			txtCode.ShowLineNumbers = true;
			txtCode.Options.CutCopyWholeLine = true;

			txtCode.TextArea.LeftMargins.Insert(0, IconBar = new IconBarMargin(this, txtCode));

			//#####################

			rgridMain.ParentWindow = this;

			iogridA.Initialize(rgridMain, PICMemory.ADDR_PORT_A, PICMemory.ADDR_TRIS_A);
			iogridB.Initialize(rgridMain, PICMemory.ADDR_PORT_B, PICMemory.ADDR_TRIS_B);

			sgridSTATUS.Initialize(rgridMain, PICMemory.ADDR_STATUS);
			sgridINTCON.Initialize(rgridMain, PICMemory.ADDR_INTCON);
			sgridOPTION.Initialize(rgridMain, PICMemory.ADDR_OPTION);

			sevSeg_0.Initialize(rgridMain);
			sevSeg_1.Initialize(rgridMain);
			sevSeg_2.Initialize(rgridMain);
			sevSeg_3.Initialize(rgridMain);
			sevSeg_4.Initialize(rgridMain);
			sevSeg_5.Initialize(rgridMain);
		}

		#region UI Event Handler

		#region New

		private void NewEnabled(object sender, CanExecuteRoutedEventArgs e)
		{
			e.CanExecute = controller == null || controller.Mode == PICControllerMode.FINISHED;

			e.Handled = true;
		}

		private void NewExecuted(object sender, ExecutedRoutedEventArgs e)
		{
			sc_document.AskSaveIfDirty();

			sc_document = new SourcecodeDocument(this, txtCode);
		}

		#endregion

		#region Open

		private void OpenEnabled(object sender, CanExecuteRoutedEventArgs e)
		{
			e.CanExecute = controller == null || controller.Mode == PICControllerMode.FINISHED || controller.Mode == PICControllerMode.WAITING;

			e.Handled = true;
		}

		private void OpenExecuted(object sender, ExecutedRoutedEventArgs e)
		{
			sc_document.AskSaveIfDirty();

			sc_document = SourcecodeDocument.OpenNew(this, txtCode) ?? sc_document;

			if (controller != null && (controller.Mode == PICControllerMode.WAITING || controller.Mode == PICControllerMode.FINISHED))
			{
				controller = null;
			}
		}

		#endregion

		#region Save

		private void SaveEnabled(object sender, CanExecuteRoutedEventArgs e)
		{
			e.CanExecute = sc_document != null && sc_document.isDirty;

			e.Handled = true;
		}

		private void SaveExecuted(object sender, ExecutedRoutedEventArgs e)
		{
			if (!sc_document.Save())
			{
				MessageBox.Show("Error while saving");
			}
		}

		#endregion

		#region SaveAs

		private void SaveAsExecuted(object sender, ExecutedRoutedEventArgs e)
		{
			if (!sc_document.SaveAs())
			{
				MessageBox.Show("Error while saving");
			}
		}

		#endregion

		#region Close

		private void CloseEnabled(object sender, CanExecuteRoutedEventArgs e)
		{
			e.CanExecute = controller == null || controller.Mode == PICControllerMode.FINISHED;

			e.Handled = true;
		}

		private void CloseExecuted(object sender, ExecutedRoutedEventArgs e)
		{
			sc_document.AskSaveIfDirty();

			this.Close();
		}


		#endregion

		#region Compile

		private void CompileEnabled(object sender, CanExecuteRoutedEventArgs e)
		{
			e.CanExecute = sc_document != null && !string.IsNullOrWhiteSpace(sc_document.Path);

			e.Handled = true;
		}

		private void CompileExecuted(object sender, ExecutedRoutedEventArgs e)
		{
			if (!sc_document.Save())
			{
				return;
			}

			string resultPath = Path.Combine(Path.GetDirectoryName(sc_document.Path), Path.GetFileNameWithoutExtension(sc_document.Path) + ".lst");

			if (File.Exists(resultPath))
			{
				File.Delete(resultPath);
			}

			Process proc = new Process()
			{
				StartInfo = new ProcessStartInfo(@"Compiler\il_ass16.exe", '"' + sc_document.Path + '"')
			};

			proc.Start();
			proc.WaitForExit();

			if (File.Exists(resultPath))
			{
				var cmds = PICProgramLoader.LoadFromFile(resultPath);

				if (cmds == null)
				{
					MessageBox.Show("Error while reading compiled file.");
					return;
				}

				controller = new PICController(cmds, getSimuSpeedFromComboBox());
				controller.RaiseCompleteEventResetChain();
				stackList.Reset();
				IconBar.Reset();
			}
		}

		#endregion

		#region Run

		private void RunEnabled(object sender, CanExecuteRoutedEventArgs e)
		{
			e.CanExecute = controller != null && controller.Mode == PICControllerMode.WAITING;

			e.Handled = true;
		}

		private void RunExecuted(object sender, ExecutedRoutedEventArgs e)
		{
			controller.Start();
		}

		#endregion

		#region Pause

		private void PauseEnabled(object sender, CanExecuteRoutedEventArgs e)
		{
			e.CanExecute = controller != null && controller.Mode == PICControllerMode.RUNNING;

			e.Handled = true;
		}

		private void PauseExecuted(object sender, ExecutedRoutedEventArgs e)
		{
			IconBar.MakeNextPCVisible();

			controller.Step();
		}

		#endregion

		#region Stop

		private void StopEnabled(object sender, CanExecuteRoutedEventArgs e)
		{
			e.CanExecute = controller != null && controller.Mode != PICControllerMode.WAITING && controller.Mode != PICControllerMode.FINISHED;

			e.Handled = true;
		}

		private void StopExecuted(object sender, ExecutedRoutedEventArgs e)
		{
			controller.Stop();
		}

		#endregion

		#region Continue

		private void ContinueEnabled(object sender, CanExecuteRoutedEventArgs e)
		{
			e.CanExecute = controller != null && controller.Mode == PICControllerMode.PAUSED;

			e.Handled = true;
		}

		private void ContinueExecuted(object sender, ExecutedRoutedEventArgs e)
		{
			controller.Continue();
		}

		#endregion

		#region Step

		private void StepEnabled(object sender, CanExecuteRoutedEventArgs e)
		{
			e.CanExecute = controller != null && (controller.Mode == PICControllerMode.PAUSED || controller.Mode == PICControllerMode.WAITING);

			e.Handled = true;
		}

		private void StepExecuted(object sender, ExecutedRoutedEventArgs e)
		{
			IconBar.MakeNextPCVisible();

			if (controller.Mode == PICControllerMode.PAUSED)
			{
				controller.Step();
			}
			else if (controller.Mode == PICControllerMode.WAITING)
			{
				controller.StartPaused();
			}
		}

		#endregion

		#region Other

		private void Window_Closed(object sender, EventArgs e)
		{
			if (controller != null)
				controller.Stop(); // Kill 'em with fire
		}

		private void cbxSpeed_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
		{
			if (controller != null)
			{
				controller.SimulationSpeed = getSimuSpeedFromComboBox();
			}
		}

		private void OnHelpClicked(object sender, RoutedEventArgs e)
		{
			string path = @"res\DataSheet.pdf";

			Process.Start(path);
		}

		private void txtCode_TextChanged(object sender, EventArgs e)
		{
			if (controller != null && (controller.Mode == PICControllerMode.WAITING || controller.Mode == PICControllerMode.FINISHED))
			{
				controller = null;
			}
		}

		#endregion

		#endregion

		#region IdleUpdate

		private void onIdle()
		{
			IdleCounter.Inc();

			if (controller != null)
			{
				UpdateRegister();
				UpdateStackDisplay();

				IconBar.SetPC(controller.GetSCLineForPC(controller.GetPC()));

				lblFreqModel.Text = FormatFreq((uint)controller.Frequency.Frequency);
				lblFreqView.Text = FormatFreq((uint)IdleCounter.Frequency);
				lblRunTime.Text = FormatRuntime(controller.GetRunTime());
				lblRegW.Text = "0x" + string.Format("{0:X02}", controller.GetWRegister());
				lblRegPC.Text = "0x" + string.Format("{0:X04}", controller.GetPC());
				lblQuartzFreq.Text = FormatFreq(controller.EmulatedFrequency);
				btnSetQuartzFreq.IsEnabled = true;
				lblWatchDogTmr.Text = string.Format("{0:000.000} %", controller.GetWatchDogPerc() * 100);
				chkbxWatchdog.IsEnabled = true;
				chkbxWatchdog.IsChecked = controller.IsWatchDogEnabled();
			}
			else
			{
				ClearRegister();
				ClearStackDisplay();

				IconBar.SetPC(0);

				lblFreqModel.Text = FormatFreq(0);
				lblFreqView.Text = FormatFreq(0);
				lblRunTime.Text = FormatRuntime(0);
				lblRegW.Text = "0x" + string.Format("{0:X02}", 0);
				lblRegPC.Text = "0x" + string.Format("{0:X04}", 0);
				lblQuartzFreq.Text = FormatFreq(0);
				btnSetQuartzFreq.IsEnabled = false;
				lblWatchDogTmr.Text = string.Format("{0:000,000} %", 0);
				chkbxWatchdog.IsEnabled = false;

			}

			txtCode.IsReadOnly = controller != null && controller.Mode != PICControllerMode.WAITING;
			CommandManager.InvalidateRequerySuggested();
		}

		private void UpdateStackDisplay()
		{
			stackList.UpdateValues(controller.GetThreadSafeCallStack(), controller);
		}

		private void ClearStackDisplay()
		{
			stackList.Reset();
		}

		private void UpdateRegister()
		{
			for (uint i = 0; i < 0x100; i++)
			{
				rgridMain.Set(i, controller.GetUnbankedRegister(i), true, false); // Tests in Set if val has changed ....
			}
		}

		private void ClearRegister()
		{
			for (uint i = 0; i < 0x100; i++)
			{
				rgridMain.Set(i, 0, true, false); // Tests in Set if val has changed ....
			}
		}

		private string FormatFreq(uint f)
		{
			if (f < 2000)
			{
				return string.Format("{0} Hz", f);
			}
			else if (f < 2000000)
			{
				return string.Format("{0} kHz", f / 1000);
			}
			else if (f < 2000000000)
			{
				return string.Format("{0} MHz", f / 1000000);
			}
			else
			{
				return string.Format("#NaN# ({0})", f);
			}
		}

		private string FormatRuntime(uint us)
		{
			if (us < 2000)
			{
				return string.Format("{0:000000} \u00B5" + "s", us);
			}
			else if (us < 2000000)
			{
				return string.Format("{0:0000.0} ms", us / 1000.0);
			}
			else if (us < 2000000000)
			{
				return string.Format("{0:00000.0} s", (us / 1000) / 1000.0);
			}
			else
			{
				return string.Format("#NaN# ({0})", us);
			}
		}

		#endregion

		public bool OnBreakPointChanged(int line, bool newVal)
		{
			if (controller == null)
				return false;

			long pc = controller.GetPCLineForSCLine(line);

			if (pc < 0)
				return false;
			else
			{
				controller.Incoming_Events.Enqueue(new BreakPointChangedEvent() { Position = (uint)pc, Value = newVal });

				return true;
			}
		}

		public void SendEventToController(PICEvent e)
		{
			if (controller != null)
			{
				controller.Incoming_Events.Enqueue(e);
			}
		}

		private PICControllerSpeed getSimuSpeedFromComboBox()
		{
			switch (cbxSpeed.SelectedIndex)
			{
				case 0:
					return PICControllerSpeed.Snail;
				case 1:
					return PICControllerSpeed.Very_Slow;
				case 2:
					return PICControllerSpeed.Slow;
				case 3:
					return PICControllerSpeed.Normal;
				case 4:
					return PICControllerSpeed.Fast;
				case 5:
					return PICControllerSpeed.Very_Fast;
				case 6:
					return PICControllerSpeed.Maximum;
				default:
					throw new Exception();
			}
		}

		private void btnSetQuartzFreq_Click(object sender, RoutedEventArgs e)
		{
			if (controller != null)
			{
				FrequencyInputDialog.Show(controller.EmulatedFrequency, (p) =>
				{
					if (controller != null)
						controller.EmulatedFrequency = (uint)p;
				});
			}
		}

		private void chkbxWatchdog_Checked(object sender, RoutedEventArgs e)
		{
			controller.SetWatchDogEnabled(!controller.IsWatchDogEnabled());
		}
	}
}
