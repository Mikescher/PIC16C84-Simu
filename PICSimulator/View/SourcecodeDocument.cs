using Microsoft.Win32;
using PICSimulator.Model;
using System;
using System.IO;
using System.Text;
using System.Windows;

namespace PICSimulator.View
{
	class SourcecodeDocument
	{
		private Window Owner;

		private string LastSaved_Value; // The Value that is currently on the Filesystem
		public string Value { get; private set; }

		public string Path { get; private set; }

		public bool isDirty { get { return Value != LastSaved_Value; } }

		public SourcecodeDocument(Window owner, ICSharpCode.AvalonEdit.TextEditor handler, string text, string path)
		{
			LastSaved_Value = text;
			Value = text;
			Path = path;

			Owner = owner;

			handler.Text = Value;
			handler.TextChanged += OnChange;

			updateTitle();
		}

		public SourcecodeDocument(Window owner, ICSharpCode.AvalonEdit.TextEditor handler)
		{
			LastSaved_Value = null;
			Value = "";
			Path = null;

			Owner = owner;

			handler.Text = Value;
			handler.TextChanged += OnChange;

			updateTitle();
		}

		public static SourcecodeDocument OpenNew(Window owner, ICSharpCode.AvalonEdit.TextEditor handler)
		{
			OpenFileDialog ofd = new OpenFileDialog();
			ofd.Filter = "All Files|*|Source- and Programmcode|*.src;*.lst|Sourcecode|*.src|Programmcode|*.lst";
			ofd.FilterIndex = 2;
			ofd.CheckFileExists = true;
			ofd.CheckPathExists = true;

			if (ofd.ShowDialog().GetValueOrDefault(false))
			{
				if (System.IO.Path.GetExtension(ofd.FileName).ToLower() == ".src")
				{
					return OpenSRCDocument(owner, handler, ofd.FileName);
				}
				else if (System.IO.Path.GetExtension(ofd.FileName).ToLower() == ".lst")
				{
					return OpenLSTDocument(owner, handler, ofd.FileName);
				}
				else
				{
					return null;
				}
			}
			else
			{
				return null;
			}
		}

		private static SourcecodeDocument OpenSRCDocument(Window owner, ICSharpCode.AvalonEdit.TextEditor handler, string FileName)
		{
			try
			{
				string s = File.ReadAllText(FileName, Encoding.Default);

				return new SourcecodeDocument(owner, handler, s, FileName);
			}
			catch (IOException)
			{
				MessageBox.Show("Error: Could not load File.");
				return null;
			}
		}

		private static SourcecodeDocument OpenLSTDocument(Window owner, ICSharpCode.AvalonEdit.TextEditor handler, string FileName)
		{
			try
			{
				string s = PICProgramLoader.LoadSourceCodeFromText(FileName);

				if (s == null)
				{
					return null;
				}

				string FileNameSrc = System.IO.Path.Combine(System.IO.Path.GetDirectoryName(FileName), System.IO.Path.GetFileNameWithoutExtension(FileName) + ".src");

				if (File.Exists(FileNameSrc))
				{
					if (MessageBox.Show("File \r\n" + FileNameSrc + "\r\nalready exists. Override ?", "Overwrite?", MessageBoxButton.OKCancel) == MessageBoxResult.Cancel)
					{
						return null;
					}
				}

				File.WriteAllText(FileNameSrc, s, Encoding.Default);

				return new SourcecodeDocument(owner, handler, s, FileNameSrc);
			}
			catch (IOException)
			{
				MessageBox.Show("Error: Could not load File.");
				return null;
			}
		}

		private void OnChange(object sender, EventArgs args)
		{
			Value = (sender as ICSharpCode.AvalonEdit.TextEditor).Text;

			updateTitle();
		}

		private void updateTitle()
		{
			Owner.Title = (isDirty ? "*" : "") + (Path ?? "New") + " - PICSimulator";
		}

		public bool Save()
		{
			if (!isDirty)
				return true;

			if (string.IsNullOrWhiteSpace(Path))
			{
				return SaveAs();
			}

			try
			{
				File.WriteAllText(Path, Value, Encoding.Default);
				LastSaved_Value = Value;

				updateTitle();

				return true;
			}
			catch (IOException)
			{
				return false;
			}
		}

		public bool SaveAs()
		{
			SaveFileDialog sfd = new SaveFileDialog();
			sfd.AddExtension = true;
			sfd.DefaultExt = ".src";
			sfd.Filter = "All Files|*|Sourcecode|*.src";
			sfd.FilterIndex = 2;

			if (sfd.ShowDialog().GetValueOrDefault(false))
			{
				if (File.Exists(sfd.FileName))
				{
					if (MessageBox.Show("File already exists. Override ?", "Overwrite?", MessageBoxButton.OKCancel) == MessageBoxResult.Cancel)
					{
						return false;
					}
				}

				try
				{
					File.WriteAllText(sfd.FileName, Value, Encoding.Default);
					Path = sfd.FileName;
					LastSaved_Value = Value;

					updateTitle();

					return true;
				}
				catch (IOException)
				{
					return false;
				}
			}
			else
			{
				return false;
			}
		}

		public void AskSaveIfDirty()
		{
			if (isDirty)
			{
				if (MessageBox.Show("File changed. Do you want to Save ?", "Save?", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
				{
					Save();
				}
			}
		}
	}
}
