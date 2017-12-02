using AnalisadorSintatico.Wpf.Modelos;
using AnalisadorSintatico.Wpf.Util;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Shell;

namespace AnalisadorSintatico.Wpf.Views
{
    /// <summary>
    /// Lógica interna para TabelaWindow.xaml
    /// </summary>
    public partial class TabelaWindow : Window
    {
        private const double GridLength = 40;
        private readonly List<List<string>> tabelaLSR;
        private readonly List<Producao> producoes;
        private readonly Gramatica gramatica;

        public TabelaWindow(Gramatica gramatica)
        {
            InitializeComponent();

            this.gramatica = gramatica;

            this.gramatica = new Gramatica()
            {
                NaoTerminais = new List<NT>()
                {
                    new NT("E") { Producoes = new List<Producao> { new Producao("E", "E+T"), new Producao("E", "T") } },
                    new NT("T") { Producoes = new List<Producao> { new Producao("T", "T*F"), new Producao("T", "F") } },
                    new NT("F") { Producoes = new List<Producao> { new Producao("F", "(E)"), new Producao("F", "id") } },
                },
                Terminais = new List<T>()
                {
                    new T("+"),
                    new T("*"),
                    new T("("),
                    new T(")"),
                    new T("id")
                }
            };

            this.gramatica.NumerarProducoes();

            tabelaLSR = new List<List<string>>();

            this.gramatica.OrdenarTerminaisAlfabeticamente();
            GerarTabelaLSR();
        }

        protected override void OnSourceInitialized(EventArgs e)
        {
            base.OnSourceInitialized(e);

            System.Drawing.Rectangle bounds = Screen.PrimaryScreen.Bounds;
            MaxHeight = bounds.Height - 50;             
        }

        private void GerarTabelaLSR()
        {
            tabelaLSR.Add(new List<string>() { "id", "+", "*", "(", ")", "$", "E", "T", "F" });
            /*0*/tabelaLSR.Add(new List<string>() { "s5", "", "", "s4", "", "", "1", "2", "3" });
            /*1*/tabelaLSR.Add(new List<string>() { "", "s6", "", "", "", "aceita", "", "", "" });
            /*2*/tabelaLSR.Add(new List<string>() { "", "r2", "s7", "", "r2", "r2", "", "", "" });
            /*3*/tabelaLSR.Add(new List<string>() { "", "r4", "r4", ""  , "r4", "r4", "", "", "" });
            /*4*/tabelaLSR.Add(new List<string>() { "s5", "", "", "s4", "", "", "8", "2", "3" });
            /*5*/tabelaLSR.Add(new List<string>() { "", "r6", "r6", "  ", "r6", "r6", "", "", "" });
            /*6*/tabelaLSR.Add(new List<string>() { "s5", "", "", "s4", "", "", "", "9", "3" });
            /*7*/tabelaLSR.Add(new List<string>() { "s5", "", "", "s4", "", "", "", "", "10" });
            /*8*/tabelaLSR.Add(new List<string>() { "", "s6", "", "", "s11", "", "", "", "" });
            /*9*/tabelaLSR.Add(new List<string>() { "", "r1", "s7", "", "r1", "r1", "", "", "" });
            /*10*/tabelaLSR.Add(new List<string>() { "", "r3", "r3", "", "r3", "r3", "", "", "" });
            /*11*/tabelaLSR.Add(new List<string>() { "", "r3", "r5", "", "r5", "r5", "", "", "" });

            //itemsControlLSR.ItemsSource = tabelaLSR;
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
        }

        private void ReconhecerEntrada(object sender, RoutedEventArgs e)
        {
            var linhas = new List<Linha>();
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

                // Transforma a ação em um vetor de char, para identificar se deve empilhar ou reduzir.
                var partes = acao.ToCharArray();

                if (partes[0] == 's')
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
                    AdicionarLinha("Reduzir " + prod);

                    // Reduz e retorna o próximo estado.
                    estado = gramatica.Reduzir(pilha, tabelaLSR, prod);   
                    
                    if (prod.Gerador == gramatica.SimboloInicial.Valor && entrada.Count == 1)
                    {
                        TerminarExecucao("Aceita");
                    }
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
    }
}
