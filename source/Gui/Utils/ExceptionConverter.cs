using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Markup;
using System.Windows.Media;

namespace Gui.Utils
{
	public class ExceptionConverter : MarkupExtension, IValueConverter
	{
		public override object ProvideValue(IServiceProvider serviceProvider)
		{
			return this;
		}

		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			var exception = value as Exception;
			if (exception == null)
			{
				return null;
			}

			var p = new Paragraph();
			Add(p, exception, string.Empty);
			return new FlowDocument(p);
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			return null;
		}

		private void Add(Paragraph p, Exception e, string parentMessage)
		{
			p.Inlines.Add(new Run(e.GetType().FullName) { Foreground = Brushes.Blue });
			if (e.Message != parentMessage)
			{
				p.Inlines.Add(" : ");
				p.Inlines.Add(new Run(e.Message) { FontWeight = FontWeights.Bold });
			}
			if (e.InnerException != null)
			{
				p.Inlines.Add(Environment.NewLine);
				p.Inlines.Add(new Run("---> ") { Foreground = Brushes.Gray });
				Add(p, e.InnerException, e.Message);
				p.Inlines.Add(Environment.NewLine);
				p.Inlines.Add(new Run("   --- End of inner exception stack trace ---") { Foreground = Brushes.Gray });
			}
			p.Inlines.Add(Environment.NewLine);
			p.Inlines.Add(new Run(e.StackTrace) { Foreground = Brushes.DarkGreen });
		}
	}
}