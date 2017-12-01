using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace AnalisadorSintatico.Wpf.Util
{
    public static class UIUtil
    {
        public static Border CreateGrid(int column)
        {
            var border = new Border()
            {
                BorderThickness = new Thickness(1, 0, 0, 0)
            };
            border.Child = new Grid();
            Grid.SetColumn(border, column);
            return border;
        }

        public static Label CreateLabel(object content, int column, int row)
        {
            var label = new Label()
            {
                Content = content,
                Style = Application.Current.Resources["LabelStyle"] as Style
            };

            Grid.SetColumn(label, column);
            Grid.SetRow(label, row);

            return label;
        }

        public static TextBlock CreateTextBlock(string text, int row, int column)
        {
            var tb = new TextBlock()
            {
                Text = text,
                Style = Application.Current.Resources["TextBlockStyle"] as Style
            };

            Grid.SetColumn(tb, column);
            Grid.SetRow(tb, row);

            return tb;
        }

        public static void AddColumns(Grid grid, double lenght1, double lenght2)
        {
            grid.ColumnDefinitions.Clear();
            grid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(70) });
            grid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(lenght1, GridUnitType.Star), });
            grid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(lenght2, GridUnitType.Star), });
        }

        public static void AddColumns<T>(List<T> list, Grid grid, bool addStyle = false)
        {
            for (int i = 0; i < list.Count; i++)
            {
                grid.ColumnDefinitions.Add(new ColumnDefinition());

                var tb = CreateTextBlock(list[i].ToString(), 0, i);

                if (addStyle)
                {
                    tb.FontStyle = FontStyles.Italic;
                    tb.FontWeight = FontWeights.Bold;
                }

                grid.Children.Add(tb);
            }
        }
    }
}
