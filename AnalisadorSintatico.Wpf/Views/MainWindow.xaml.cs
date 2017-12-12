﻿///<summary>
/// Gustavo Miguel Heck M96619
/// Leonardo dos Santos MXXXXX
/// Universidade de Santa Cruz do Sul
/// Compiladores - 2017/2
///</summary>

using AnalisadorSintatico.Wpf.Modelos;
using AnalisadorSintatico.Wpf.Util;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace AnalisadorSintatico.Wpf.Views
{
    /// <summary>
    /// Interação lógica para MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private Gramatica _gramatica;
        private ObservableCollection<NT> _producoes;

        public MainWindow()
        {
            InitializeComponent();

            _gramatica = new Gramatica();
            _producoes = new ObservableCollection<NT>();
            itemsControl.ItemsSource = _producoes;
        }

        private void Setup()
        {
            labelAviso.Content = "";
            labelProducoes.Visibility = Visibility.Visible;
            _producoes = new ObservableCollection<NT>(_gramatica.NaoTerminais);
            itemsControl.ItemsSource = null;
            itemsControl.ItemsSource = _producoes;

            PrintarGramatica();
            PrintarProducoes();
        }

        private void GerarTabela(object sender, RoutedEventArgs e)
        {
            List<List<string>> tabela;

            using (var excel = new ExcelInterop())
            {
                tabela = excel.LerTabela(textBoxExcel.Text);
            }

            if (_gramatica.NaoTerminais.Count == 0)
            {
                labelAviso.Content = "Nenhum Não-Terminal";
            }
            else if (_gramatica.NaoTerminais.All(nt => nt.Producoes.Count == 0))
            {
                labelAviso.Content = "Nenhuma produção";
            }
            else
            {
                labelAviso.Content = "";
                var wnd = new TabelaWindow(_gramatica, tabela);
                wnd.Show();
            }
        }

        private void PrintarGramatica()
        {
            gramatica.Text = String.Format("G = ({{{0}}}, {{{1}}}, P, {2})",
                String.Join(", ", _gramatica.NaoTerminais),
                String.Join(", ", _gramatica.Terminais),
                _gramatica.NaoTerminais.FirstOrDefault() ?? new NT("Ø"));
        }

        private void PrintarProducoes()
        {
            string str = "";

            foreach (NT nt in _gramatica.NaoTerminais)
            {
                Producao p = nt.Producoes.FirstOrDefault();

                if (p != null)
                {
                    str += $"{nt.Valor} → {p.Valor}";

                    if (nt.Producoes.Count > 1)
                        str += " | " + String.Join(" | ", nt.Producoes.TryGetRange(1, nt.Producoes.Count));
                }
                else
                {
                    str += nt.Valor;
                }

                str += Environment.NewLine;
            }

            producoes.Text = str;
        }


        private void AdicionarTerminalOnClick(object sender, RoutedEventArgs e)
        {
            if (!_gramatica.Terminais.Any(t => t.Valor == textBoxTerminal.Text))
            {
                _gramatica.Terminais.Add(new T(textBoxTerminal.Text));
                PrintarGramatica();
            }

            textBoxTerminal.Clear();

        }

        private void AdicionarTerminalOnKeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter && !String.IsNullOrWhiteSpace(textBoxTerminal.Text))
                AdicionarTerminalOnClick(null, null);
        }

        private void AdicionarNaoTerminalOnClick(object sender, RoutedEventArgs e)
        {
            if (!_gramatica.NaoTerminais.Any(t => t.Valor == textBoxNaoTerminal.Text))
            {
                var nt = new NT(textBoxNaoTerminal.Text);
                _gramatica.NaoTerminais.Add(nt);
                PrintarGramatica();
                _producoes.Add(nt);
            }

            labelProducoes.Visibility = Visibility.Visible;
            textBoxNaoTerminal.Clear();
        }

        private void AdicionarNaoTerminalOnKeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter && !String.IsNullOrWhiteSpace(textBoxNaoTerminal.Text))
                AdicionarNaoTerminalOnClick(null, null);
        }

        private void TextBoxNaoTerminalPreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = (textBoxNaoTerminal.Text.Length == 1 && e.Text != "'");
        }

        private void Reiniciar(object sender, RoutedEventArgs e)
        {
            Process.Start(Application.ResourceAssembly.Location);
            Application.Current.Shutdown();
        }

        private void AdicionarProducaoOnPreviewKeyUp(object sender, KeyEventArgs e)
        {
            e.Handled = (e.Key == Key.Space);
        }

        private void AdicionarProducaoOnKeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key != Key.Enter) return;

            var tb = (TextBox)sender;
            var obj = (NT)((TextBox)e.Source).DataContext;
            string p = tb.Text;

            if (p == T.Vazio || p.StartsWith("vazio", StringComparison.OrdinalIgnoreCase))
            {
                obj.Producoes.Add(new Producao(obj.Valor, T.Vazio));
            }
            else if (obj.Valor.StartsWith(p))
            {
                labelAviso.Content = "Recursão à esquerda";
                return;
            }
            else
            {
                if (_gramatica.ValidarProducao(p))
                {
                    obj.Producoes.Add(new Producao(obj.Valor, p));

                    PrintarProducoes();
                }
                else
                {
                    labelAviso.Content = "Símbolo não pertence à gramática";
                    return;
                }
            }

            labelAviso.Content = "";
            tb.Text = "";
            itemsControl.ItemsSource = null;
            itemsControl.ItemsSource = _producoes;
        }

        private void AdicionarProducaoOnLostFocus(object sender, RoutedEventArgs e)
        {
            var tb = (TextBox)sender;
            tb.Text = String.Empty;
        }

        private void Gramatica1(object sender, RoutedEventArgs e)
        {
            _gramatica = new Gramatica()
            {
                NaoTerminais = new List<NT>()
                {
                    new NT("E") { Producoes = new List<Producao> { new Producao("E", "E+T"), new Producao("E", "T") } },
                    new NT("T") { Producoes = new List<Producao> { new Producao("T", "T*F"), new Producao("T", "F") } },
                    new NT("F") { Producoes = new List<Producao> { new Producao("F", "(E)"), new Producao("F", "id") } }
                },
                Terminais = new List<T>()
                {
                    new T("+"),
                    new T("*"),
                    new T("("),
                    new T(")"),
                    new T("id"),
                }
            };
            
            Setup();
        }

        private void Gramatica2(object sender, RoutedEventArgs e)
        {
            _gramatica = new Gramatica()
            {
                NaoTerminais = new List<NT>()
                {
                    new NT("S") { Producoes = new List<Producao> { new Producao("S", "a"), new Producao("S", "[L]") } },
                    new NT("L") { Producoes = new List<Producao> { new Producao("L", "L;S"), new Producao("L", "S") } },
                },
                Terminais = new List<T>()
                {
                    new T("a"),
                    new T("["),
                    new T("]"),
                    new T(";"),
                }
            };

            Setup();
        }

        private void Gramatica3(object sender, RoutedEventArgs e)
        {
            _gramatica = new Gramatica()
            {
                NaoTerminais = new List<NT>()
                {
                    new NT("S") { Producoes = new List<Producao> { new Producao("S", "AB") } },
                    new NT("A") { Producoes = new List<Producao> { new Producao("A", "a") } },
                    new NT("B") { Producoes = new List<Producao> { new Producao("B", "b") } }
                },
                Terminais = new List<T>()
                {
                    new T("a"),
                    new T("b")
                }
            };

            Setup();
        }

        private void ProcurarOnClick(object sender, RoutedEventArgs e)
        {
            var ofd = new System.Windows.Forms.OpenFileDialog();
            ofd.Filter = "Arquivos excel|*.xlsx";
            ofd.Title = "Selecione o arquivo com a tebela";
            
            if (ofd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                textBoxExcel.Text = ofd.FileName; 
            }
        }
    }
}
