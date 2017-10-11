using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Interactivity;

namespace Gui.Utils
{
	public class RichTextBinder : Behavior<RichTextBox>
	{
		public static readonly DependencyProperty DocumentProperty = DependencyProperty.Register("Document",
			typeof(FlowDocument), typeof(RichTextBinder), new PropertyMetadata(OnDocumentChanged));

		public FlowDocument Document
		{
			get => (FlowDocument)GetValue(DocumentProperty);
			set => SetValue(DocumentProperty, value);
		}

		private static void OnDocumentChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			((RichTextBinder)d).AssociatedObject.Document = (FlowDocument)e.NewValue??new FlowDocument();
		}
	}
}