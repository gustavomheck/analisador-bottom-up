///<summary>
/// Gustavo Miguel Heck
/// Leonardo Nunes
/// Universidade de Santa Cruz do Sul
/// Compiladores - 2017/2
///</summary>

using System.Windows;
using AnalisadorSintatico.Wpf.Views;

namespace AnalisadorSintatico.Wpf
{
    /// <summary>
    /// Interação lógica para App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            var mainwnd = new MainWindow();
            mainwnd.ShowDialog();
        }
    }
}
