using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;

namespace AnalisadorSintatico.Wpf.Controles
{
    public class BindableTextBlock : TextBlock
    {
        public static readonly DependencyProperty InlineListProperty =
            DependencyProperty.Register(
                "InlineList", 
                typeof(ObservableCollection<Inline>), 
                typeof(BindableTextBlock),
                new UIPropertyMetadata(null, OnPropertyChanged));

        public ObservableCollection<Inline> InlineList
        {
            get { return (ObservableCollection<Inline>)GetValue(InlineListProperty); }
            set { SetValue(InlineListProperty, value); }
        }

        private static void OnPropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            BindableTextBlock textBlock = (BindableTextBlock)sender;
            textBlock.Inlines.AddRange((ObservableCollection<Inline>)e.NewValue);
        }
    }
}
