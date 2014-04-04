using ICSharpCode.AvalonEdit.Editing;
using ICSharpCode.AvalonEdit.Rendering;
using ICSharpCode.AvalonEdit.Utils;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace PICSimulator.View
{
	//
	// Reference: https://github.com/icsharpcode/SharpDevelop/blob/master/src/AddIns/DisplayBindings/AvalonEdit.AddIn/Src/IconBarMargin.cs
	//
	public class IconBarMargin : AbstractMargin, IDisposable
	{
		private MainWindow owner;

		private long currPC = 0;
		private Dictionary<long, bool> breakpoints = new Dictionary<long, bool>();

		private ImageSource img_Arrow;
		private ImageSource img_Breakpoint;
		private ImageSource img_Combined;

		public IconBarMargin(MainWindow _owner)
		{
			this.owner = _owner;

			BitmapImage b;

			b = new BitmapImage();
			b.BeginInit();
			b.UriSource = new Uri("pack://application:,,,/PICSimulator;component/icons/arrow-left.png");
			b.EndInit();
			img_Arrow = b;

			b = new BitmapImage();
			b.BeginInit();
			b.UriSource = new Uri("pack://application:,,,/PICSimulator;component/icons/sealing-wax.png");
			b.EndInit();
			img_Breakpoint = b;

			b = new BitmapImage();
			b.BeginInit();
			b.UriSource = new Uri("pack://application:,,,/PICSimulator;component/icons/sealing-arrow.png");
			b.EndInit();
			img_Combined = b;
		}

		protected override void OnTextViewChanged(TextView oldTextView, TextView newTextView)
		{
			if (oldTextView != null)
			{
				oldTextView.VisualLinesChanged -= OnRedrawRequested;
			}
			base.OnTextViewChanged(oldTextView, newTextView);
			if (newTextView != null)
			{
				newTextView.VisualLinesChanged += OnRedrawRequested;
			}
			InvalidateVisual();
		}

		void OnRedrawRequested(object sender, EventArgs e)
		{
			if (this.TextView != null && this.TextView.VisualLinesValid)
			{
				InvalidateVisual();
			}
		}

		public virtual void Dispose()
		{
			this.TextView = null;
		}

		protected override HitTestResult HitTestCore(PointHitTestParameters hitTestParameters)
		{
			return new PointHitTestResult(this, hitTestParameters.HitPoint);
		}

		protected override Size MeasureOverride(Size availableSize)
		{
			return new Size(18, 0);
		}

		protected override void OnRender(DrawingContext drawingContext)
		{
			Size renderSize = this.RenderSize;
			drawingContext.DrawRectangle(SystemColors.ControlBrush, null,
			new Rect(0, 0, renderSize.Width, renderSize.Height));
			drawingContext.DrawLine(new Pen(SystemColors.ControlDarkBrush, 1),
			new Point(renderSize.Width - 0.5, 0),
			new Point(renderSize.Width - 0.5, renderSize.Height));

			TextView textView = this.TextView;
			if (textView != null && textView.VisualLinesValid)
			{
				Size pixelSize = PixelSnapHelpers.GetPixelSize(this);
				foreach (VisualLine line in textView.VisualLines)
				{
					int lineNumber = line.FirstDocumentLine.LineNumber;

					bool arr = lineNumber == currPC;
					bool bp = breakpoints.ContainsKey(lineNumber) ? breakpoints[lineNumber] : false;

					double lineMiddle = line.GetTextLineVisualYPosition(line.TextLines[0], VisualYPosition.TextMiddle) - textView.VerticalOffset;
					Rect rect = new Rect(0, PixelSnapHelpers.Round(lineMiddle - 8, pixelSize.Height), 16, 16);

					if (bp && arr)
					{
						drawingContext.DrawImage(img_Combined, rect);
					}
					else if (bp)
					{
						drawingContext.DrawImage(img_Breakpoint, rect);
					}
					else if (arr)
					{
						drawingContext.DrawImage(img_Arrow, rect);
					}
				}
			}
		}

		public void SetPC(long p)
		{
			currPC = p;

			if (this.TextView != null && this.TextView.VisualLinesValid)
			{
				InvalidateVisual();
			}
		}

		private int GetLineFromMousePosition(MouseEventArgs e)
		{
			TextView textView = this.TextView;
			if (textView == null)
				return 0;
			VisualLine vl = textView.GetVisualLineFromVisualTop(e.GetPosition(textView).Y + textView.ScrollOffset.Y);
			if (vl == null)
				return 0;
			return vl.FirstDocumentLine.LineNumber;
		}

		protected override void OnMouseDown(MouseButtonEventArgs e)
		{
			base.OnMouseDown(e);
			int line = GetLineFromMousePosition(e);

			if (!e.Handled && line != 0 && e.ChangedButton == MouseButton.Left)
			{
				bool newVal = breakpoints.ContainsKey(line) ? !breakpoints[line] : true;

				if (owner.OnBreakPointChanged(line, newVal))
				{
					breakpoints[line] = newVal;

					if (this.TextView != null && this.TextView.VisualLinesValid)
					{
						InvalidateVisual();
					}

					e.Handled = true;
				}
			}
		}

		public void Reset()
		{
			currPC = 0;
			breakpoints.Clear();

			if (this.TextView != null && this.TextView.VisualLinesValid)
			{
				InvalidateVisual();
			}
		}
	}
}