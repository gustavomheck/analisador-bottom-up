using AnalisadorSintatico.Wpf.Modelos;
using AnalisadorSintatico.Wpf.Util;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Media;

namespace AnalisadorSintatico.Wpf.Views
{
    /// <summary>
    /// Lógica interna para TabelaWindow.xaml
    /// </summary>
    public partial class TabelaWindow : Window
    {
        private const double GridLength = 40;
        private List<List<string>> tabelaLSR;
        private readonly Gramatica gramatica;

        public TabelaWindow(Gramatica gramatica, List<List<string>> tabelaLSR)
        {
            InitializeComponent();

            this.gramatica = gramatica;
            this.gramatica.NumerarProducoes();
            this.tabelaLSR = tabelaLSR;

            PrintarProducoes();
        }

        protected override void OnSourceInitialized(EventArgs e)
        {
            base.OnSourceInitialized(e);

            System.Drawing.Rectangle bounds = Screen.PrimaryScreen.Bounds;
            MaxHeight = bounds.Height - 50;             
        }

        private void ReconhecerEntrada(object sender, RoutedEventArgs e)
        {
            try
            {
                ReconhecerEntrada();
            }
            catch (Exception)
            {
                System.Windows.MessageBox.Show("Tabela SLR incorreta", "Algo deu errado");
            }
        }

        private void ReconhecerEntrada()
        {
            var linhas = new ObservableCollection<Linha>();
            itemsControl.ItemsSource = linhas;

            if (String.IsNullOrWhiteSpace(textBoxEntrada.Text))
            {
                AdicionarAviso("Entrada vazia  ", Brushes.Goldenrod);
                return;
            }

            // Inicializar variáveis
            var fim = false;
            var estado = 0;
            var linha = 0;

            var pilha = new Stack<string>();
            var entrada = gramatica.EntradaParaPilha(textBoxEntrada.Text.Replace(" ", ""));

            // A entrada tem tamanho 1 ($) significa que o usário informou caracteres que não fazem parte da gramática.
            if (entrada.Count == 1)
            {
                AdicionarAviso("Entrada não reconhecida  ", Brushes.Red);
                return;
            }

            borderEntrada.Visibility = Visibility.Visible;
            labelAviso.Content = "";

            // Empilha o '0' inicial.
            pilha.Push("0");

            int j = 0;
            while (!fim)
            {
                j++;
                linha++;
                string topoEntrada = entrada.First();

                // Obtém o índice da coluna onde está o topo da entrada.
                var index = tabelaLSR[0].FindIndex(x => x == topoEntrada);

                // Obtém a ação a ser efetuada.
                var acao = tabelaLSR[estado + 1][index];

                // Se a ação está em braco, então a entrada não é reconhecida.
                if (String.IsNullOrEmpty(acao))
                {
                    TerminarExecucao("Rejeita");
                    break;
                }
                else if (acao.ToUpper() == "ACEITA")
                {
                    TerminarExecucao("Aceita");
                    break;
                }

                // Transforma a ação em um vetor de char, para identificar se deve empilhar ou reduzir.
                var partes = acao.ToCharArray();

                if (partes[0] == 's' || partes[0] == 'e')
                {
                    AdicionarLinha("Empilhar");

                    // Empilha o símbolo e o estado
                    pilha.Push(tabelaLSR[0][index]);
                    pilha.Push(partes[1].ToString());

                    // Desempilha a entrada
                    entrada.Pop();

                    // Obtém o próximo estado 's4' -> estado = 4
                    estado = Convert.ToInt32(partes[1].ToString());
                }
                else if (partes[0] == 'r')
                {
                    Producao prod = gramatica.Producoes[Convert.ToInt32(partes[1].ToString()) - 1];
                    AdicionarLinha("Reduzir " + prod.ProducaoFormatada);

                    // Reduz e retorna o próximo estado.
                    estado = gramatica.Reduzir(pilha, tabelaLSR, prod);
                }
            }

            void AdicionarAviso(string aviso, SolidColorBrush cor)
            {
                labelAviso.Content = aviso;
                labelAviso.Foreground = cor;
            }

            void AdicionarLinha(string acao)
            {
                var l = new Linha()
                {
                    Acao = acao,
                    Passo = linha,
                    Entrada = gramatica.EntradaParaInline(entrada),
                    Pilha = gramatica.PilhaParaInline(pilha)
                };

                linhas.Add(l);
            }

            void TerminarExecucao(string acao)
            {
                fim = true;
                linha++;
                AdicionarLinha(acao);
            }
        }

        public bool GerarTabelaLSR()
        {
            try
            {
                double lenght1 = ((gramatica.Terminais.Count + 1) * 100) / (gramatica.Terminais.Count + gramatica.NaoTerminais.Count + 1) + 0.4;
                double lenght2 = 100 - lenght1 - 0.4;

                UIUtil.AddColumns(gridHeader, lenght1, lenght2);
                UIUtil.AddColumns(gramatica.Terminais, gridAcao);
                gridAcao.ColumnDefinitions.Add(new ColumnDefinition());
                gridAcao.Children.Add(UIUtil.CreateTextBlock("$", 0, gramatica.Terminais.Count));
                UIUtil.AddColumns(gramatica.NaoTerminais, gridDesvio);

                for (int i = 1; i < tabelaLSR.Count; i++)
                {
                    var gridTemplate = new Grid();
                    UIUtil.AddColumns(gridTemplate, lenght1, lenght2);

                    var tbEstado = UIUtil.CreateTextBlock((i - 1).ToString(), 0, 0);
                    var borderAcao = UIUtil.CreateGrid(1);
                    var borderDesvio = UIUtil.CreateGrid(2);

                    UIUtil.AddColumns(tabelaLSR[i].GetRange(0, gramatica.Terminais.Count + 1), (Grid)borderAcao.Child, true);
                    UIUtil.AddColumns(tabelaLSR[i].GetRange(gramatica.Terminais.Count + 1, gramatica.NaoTerminais.Count), (Grid)borderDesvio.Child);

                    gridTemplate.Children.Add(tbEstado);
                    gridTemplate.Children.Add(borderAcao);
                    gridTemplate.Children.Add(borderDesvio);
                    stackPanelTabela.Children.Add(gridTemplate);
                }

                return true;
            }
            catch (Exception)
            {
                System.Windows.MessageBox.Show("Arquivo Excel com formato incorreto", "Algo deu errado");
                return false;
            }
        }

        private void PrintarProducoes()
        {
            for (int i = 0; i < gramatica.Producoes.Count; i++)
            {
                textBlockProducoes.Inlines.Add((i + 1) + ".  ");
                textBlockProducoes.Inlines.Add(new Run()
                {
                    FontStyle = FontStyles.Italic,
                    Text = gramatica.Producoes[i].ProducaoFormatada + Environment.NewLine
                });
            }
        }
    }
}
