using System.Windows;
using System.Windows.Input;


namespace PICSimulator.View
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		private SourcecodeDocument sc_document;

		public MainWindow()
		{
			InitializeComponent();

			sc_document = new SourcecodeDocument(this, txtCode);
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

		#endregion
	}
}
