using ICSharpCode.AvalonEdit.Editing;
using ICSharpCode.AvalonEdit.Rendering;
using ICSharpCode.AvalonEdit.Utils;
using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace PICSimulator.View
{
	//
	// Reference: https://github.com/icsharpcode/SharpDevelop/blob/master/src/AddIns/DisplayBindings/AvalonEdit.AddIn/Src/IconBarMargin.cs
	//
	public class IconBarMargin : AbstractMargin, IDisposable
	{
		private long currPC = -1;

		private ImageSource img_Arrow;

		public IconBarMargin()
		{
			BitmapImage logo = new BitmapImage();
			logo.BeginInit();
			logo.UriSource = new Uri("pack://application:,,,/PICSimulator;component/icons/arrow-left.png");
			logo.EndInit();

			img_Arrow = logo;
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

					if (lineNumber == currPC)
					{
						double lineMiddle = line.GetTextLineVisualYPosition(line.TextLines[0], VisualYPosition.TextMiddle) - textView.VerticalOffset;
						Rect rect = new Rect(0, PixelSnapHelpers.Round(lineMiddle - 8, pixelSize.Height), 16, 16);


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
	}
}