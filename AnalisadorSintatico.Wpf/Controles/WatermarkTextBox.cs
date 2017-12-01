using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace AnalisadorSintatico.Wpf.Controles
{
    public class WatermarkTextBox : TextBox
    {
        public static readonly DependencyProperty HasTextProperty =
            DependencyProperty.Register(
                "HasText",
                typeof(bool),
                typeof(WatermarkTextBox),
                new FrameworkPropertyMetadata(false));

        public static readonly DependencyProperty PlaceholderProperty =
            DependencyProperty.Register("Placeholder",
                typeof(string),
                typeof(WatermarkTextBox),
                new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.Journal | FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

        public static readonly DependencyProperty PlaceholderVisibilityProperty =
            DependencyProperty.Register(
                "PlaceholderVisibility",
                typeof(Visibility),
                typeof(WatermarkTextBox),
                new FrameworkPropertyMetadata(Visibility.Visible));

        static WatermarkTextBox()
        {
            DefaultStyleKeyProperty.OverrideMetadata(
                typeof(WatermarkTextBox),
                new FrameworkPropertyMetadata(typeof(WatermarkTextBox)));
        }

        /// <summary>
        /// Obtém ou define um valor do tipo System.Boolean indicando se a TextBox tem ou não texto.
        /// </summary>
        [Description("Obtém ou define um valor do tipo System.Boolean indicando se a TextBox tem ou não texto.")]
        public bool HasText
        {
            get { return (bool)GetValue(HasTextProperty); }
            set
            {
                SetValue(HasTextProperty, value);

                if (value)
                {
                    SetValue(PlaceholderVisibilityProperty, Visibility.Collapsed);
                }
                else
                {
                    SetValue(PlaceholderVisibilityProperty, Visibility.Visible);
                }
            }
        }

        /// <summary>
        /// Obtém ou define o texto para a marca d'água.
        /// </summary>
        [Description("Obtém ou define o texto para a marca d'água.")]
        public string Placeholder
        {
            get { return (string)GetValue(PlaceholderProperty); }
            set { SetValue(PlaceholderProperty, value); }
        }

        /// <summary>
        /// Obtém ou define a visibilidade da marca d'água
        /// </summary>
        [Description("Obtém ou define a visibilidade da marca d'água.")]
        public Visibility PlaceholderVisibility
        {
            get { return (Visibility)GetValue(PlaceholderVisibilityProperty); }
            set { SetValue(PlaceholderVisibilityProperty, value); }
        }

        /// <summary>
        /// Invoked when TextBox control raises TextChanged event.
        /// </summary>
        protected override void OnTextChanged(TextChangedEventArgs e)
        {
            base.OnTextChanged(e);

            HasText = !String.IsNullOrEmpty(Text);
        }

        private void ClearButton_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Text = String.Empty;
        }
    }
}
