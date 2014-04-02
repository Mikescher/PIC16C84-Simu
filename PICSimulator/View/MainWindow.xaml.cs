using PICSimulator.Model;
using PICSimulator.Model.Events;
using System;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;


namespace PICSimulator.View
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		private SourcecodeDocument sc_document;
		private PICController controller = null;

		public MainWindow()
		{
			InitializeComponent();

			sc_document = new SourcecodeDocument(this, txtCode);

			sc_document = new SourcecodeDocument( //TODO Remove Me - Only for ... reasons
				this,
				txtCode,
				File.ReadAllText(@"E:\Eigene Dateien\Dropbox\Eigene EDV\Visual Studio\Projects\PIC16C84-Simu\PICSimulator\Testdata\test.src"),
				@"E:\Eigene Dateien\Dropbox\Eigene EDV\Visual Studio\Projects\PIC16C84-Simu\PICSimulator\Testdata\test.src");

			this.Dispatcher.BeginInvoke(new Action(onIdle), DispatcherPriority.ApplicationIdle);
		}

		#region Event Handler

		private void NewExecuted(object sender, ExecutedRoutedEventArgs e)
		{
			sc_document.AskSaveIfDirty();

			sc_document = new SourcecodeDocument(this, txtCode);
		}

		private void OpenExecuted(object sender, ExecutedRoutedEventArgs e)
		{
			sc_document.AskSaveIfDirty();


			sc_document = SourcecodeDocument.OpenNew(this, txtCode) ?? sc_document;
		}

		private void SaveExecuted(object sender, ExecutedRoutedEventArgs e)
		{
			if (!sc_document.Save())
			{
				MessageBox.Show("Error while saving");
			}
		}

		private void SaveAsExecuted(object sender, ExecutedRoutedEventArgs e)
		{
			if (!sc_document.SaveAs())
			{
				MessageBox.Show("Error while saving");
			}
		}

		private void CloseExecuted(object sender, ExecutedRoutedEventArgs e)
		{
			sc_document.AskSaveIfDirty();

			this.Close();
		}

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

		private void RunEnabled(object sender, CanExecuteRoutedEventArgs e)
		{
			e.CanExecute = controller != null && !controller.isRunning;

			e.Handled = true;
		}

		private void RunExecuted(object sender, ExecutedRoutedEventArgs e)
		{
			controller.Start();
			txtCode.IsReadOnly = true;
		}

		#endregion

		private void onIdle()
		{
			PICEvent e;

			if (controller != null && controller.Outgoing_Events.TryDequeue(out e))
			{
				if (e is RegisterChangedEvent)
				{
					rgridMain.Set((e as RegisterChangedEvent).RegisterPos, (e as RegisterChangedEvent).NewValue);
				}
				else
				{
					throw new ArgumentException();
				}
			}


			this.Dispatcher.BeginInvoke(new Action(onIdle), DispatcherPriority.ApplicationIdle);
		}
	}
}
