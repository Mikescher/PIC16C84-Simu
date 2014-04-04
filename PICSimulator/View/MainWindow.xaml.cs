using ICSharpCode.AvalonEdit.Highlighting;
using ICSharpCode.AvalonEdit.Highlighting.Xshd;
using PICSimulator.Model;
using PICSimulator.Model.Events;
using System;
using System.Diagnostics;
using System.IO;
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
		private SourcecodeDocument sc_document;
		private PICController controller = null;

		private IconBarMargin IconBar;

		public bool CodeIsReadOnly { get { return false; } set { } }

		public MainWindow()
		{
			InitializeComponent();

			Init();

			DataContext = this;

			sc_document = new SourcecodeDocument(this, txtCode);

			sc_document = new SourcecodeDocument( //TODO Remove Me - Only for ... reasons
				this,
				txtCode,
				File.ReadAllText(@"E:\Eigene Dateien\Dropbox\Eigene EDV\Visual Studio\Projects\PIC16C84-Simu\PICSimulator\Testdata\test.src"),
				@"E:\Eigene Dateien\Dropbox\Eigene EDV\Visual Studio\Projects\PIC16C84-Simu\PICSimulator\Testdata\test.src");

			this.Dispatcher.BeginInvoke(new Action(onIdle), DispatcherPriority.ApplicationIdle);
		}

		private void Init()
		{
			using (XmlReader reader = new XmlTextReader(new StringReader(Properties.Resources.Assembler)))
			{
				IHighlightingDefinition customHighlighting = HighlightingLoader.Load(reader, HighlightingManager.Instance);
				txtCode.SyntaxHighlighting = customHighlighting;
			}

			txtCode.ShowLineNumbers = true;
			txtCode.Options.CutCopyWholeLine = true;

			txtCode.TextArea.LeftMargins.Insert(0, IconBar = new IconBarMargin());
		}

		#region Event Handler

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
			e.CanExecute = controller == null || controller.Mode == PICControllerMode.FINISHED;

			e.Handled = true;
		}

		private void OpenExecuted(object sender, ExecutedRoutedEventArgs e)
		{
			sc_document.AskSaveIfDirty();


			sc_document = SourcecodeDocument.OpenNew(this, txtCode) ?? sc_document;
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

				controller = new PICController(cmds);
				//MessageBox.Show("Yay");
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

		#endregion

		private void onIdle()
		{
			PICEvent e;
			if (controller != null)
			{
				while (controller.Outgoing_Events.TryDequeue(out e))
				{
					HandleEvent(e);
				}
			}

			this.Dispatcher.BeginInvoke(new Action(onIdle), DispatcherPriority.ApplicationIdle);
		}

		private void HandleEvent(PICEvent e)
		{
			if (e is RegisterChangedEvent)
			{
				RegisterChangedEvent ce = e as RegisterChangedEvent;

				rgridMain.Set(ce.RegisterPos, ce.NewValue);

				if (ce.RegisterPos == PICController.ADDR_PC)
				{
					IconBar.SetPC(controller.GetSCLineForPC(ce.NewValue - 1)); // -1 Because PC_INC before Exec (--> just looks better)
					lblRegPCL.Text = "0x" + string.Format("{0:X02}", ce.NewValue);
				}
			}
			else if (e is WRegisterChangedEvent)
			{
				WRegisterChangedEvent ce = e as WRegisterChangedEvent;

				lblRegW.Text = "0x" + string.Format("{0:X02}", ce.NewValue);
			}
			else
			{
				throw new ArgumentException();
			}
		}

		private void txtCode_PreviewTextInput(object sender, TextCompositionEventArgs e)
		{
			e.Handled = !(sc_document != null && (controller == null || controller.Mode == PICControllerMode.FINISHED));
		}

		private void Window_Closed(object sender, EventArgs e)
		{
			if (controller != null)
				controller.Stop(); // Kill 'em with fire
		}
	}
}
