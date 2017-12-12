using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Documents;

namespace AnalisadorSintatico.Wpf.Modelos
{
    public class Gramatica
    {
        /// <summary>
        /// Inicializa uma nova instância da classe Gramatica.
        /// </summary>
        public Gramatica()
        {
            Terminais = new List<T>();
            NaoTerminais = new List<NT>();
        }

        /// <summary>
        /// Obtém ou define os Terminais da gramática.
        /// </summary>
        public List<T> Terminais { get; set; }

        /// <summary>
        /// Obtém ou define os Não-Terminais da gramática.
        /// </summary>
        public List<NT> NaoTerminais { get; set; }

        /// <summary>
        /// Obtém o símbolo de início.
        /// </summary>
        public NT SimboloInicial => NaoTerminais.FirstOrDefault() ?? new NT("?");
        
        /// <summary>
        /// Obtém ou define as produções da gramática.
        /// </summary>
        public List<Producao> Producoes { get; internal set; }
        
        /// <summary>
        /// Determina se o simbolo é um Terminal.
        /// </summary>
        /// <param name="simbolo">O simbolo a ser testado.</param>
        /// <returns>Verdadeiro se for Terminal; senão, falso.</returns>
        public bool IsTerminal(string simbolo)
        {
            foreach (T t in Terminais)
                if (t.Valor.Equals(simbolo))
                    return true;

            return false;
        }

        /// <summary>
        /// Determina se o simbolo é um Não-Terminal.
        /// </summary>
        /// <param name="simbolo">O simbolo a ser testado.</param>
        /// <returns>Verdadeiro se for Não-Terminal; senão, falso.</returns>
        public bool IsNaoTerminal(string simbolo)
        {
            foreach (NT nt in NaoTerminais)
                if (nt.Valor.Equals(simbolo))
                    return true;

            return false;
        }

        /// <summary>
        /// Numera as produções para serem printadas.
        /// </summary>
        public void NumerarProducoes()
        {
            Producoes = new List<Producao>();

            foreach (var nt in NaoTerminais)
                foreach (var p in nt.Producoes)
                    Producoes.Add(p);
        }

        /// <summary>
        /// Enpilha os caracteres de uma string.
        /// </summary>
        /// <param name="entrada">A string a ser empilhada.</param>
        /// <returns>Uma pilha com o conteúdo da string.</returns>
        public Stack<string> EntradaParaPilha(string entrada)
        {   
            var pilha = new Stack<string>();
            pilha.Push("$");

            string simbolo = "";

            for (int i = entrada.Length - 1; i >= 0; i--)
            {
                simbolo = simbolo.Insert(0, entrada[i].ToString());

                if (Terminais.Any(nt => nt.Valor.Equals(simbolo)))
                {
                    pilha.Push(simbolo);
                    simbolo = "";
                }
            }

            return pilha;
        }
        
        /// <summary>
        /// Valida uma produção, antes de adicionar à gramática.
        /// </summary>
        /// <param name="p">A produção a ser validada.</param>
        /// <returns>Verdadeiro se a produção é válida; senão, falso.</returns>
        public bool ValidarProducao(string p)
        {
            var lista = new List<string>();
            string simbolo = "";

            for (int i = p.Length - 1; i >= 0; i--)
            {
                simbolo = simbolo.Insert(0, p[i].ToString());

                if (Terminais.Any(nt => nt.Valor.Equals(simbolo)) ||
                    NaoTerminais.Any(nt => nt.Valor.Equals(simbolo)))
                {
                    lista.Add(simbolo);
                    simbolo = "";
                }
            }

            if (simbolo.Length > 0)
            {
                return false;
            }

            if (lista.Count >= 3)
            {
                if (!IsTerminal(lista[0]) && IsTerminal(lista[1]) && !IsTerminal(lista[2]) && lista[0].Equals(lista[1]))
                {
                    return false;
                }
            }

            bool achou;

            foreach (var s in lista)
            {
                achou = false;

                foreach (var nt in NaoTerminais)
                {
                    if (nt.Valor.Equals(s))
                        achou = true;
                }

                foreach (var t in Terminais)
                {
                    if (t.Valor.Equals(s))
                        achou = true;
                }

                if (!achou)
                    return false;
            }

            return true;
        }
        
        /// <summary>
        /// Ordena os terminais de forma alfabética.
        /// </summary>
        public void OrdenarTerminaisAlfabeticamente()
        {
            Terminais = Terminais.OrderByDescending(t => t.Valor).ToList();

            int i1 = Terminais.FindIndex(t => t.Valor == "(");

            if (i1 != -1)
            {
                int i2 = Terminais.FindIndex(t => t.Valor == ")");

                if (i2 != -1)
                {
                    T tmp = Terminais[i1];
                    Terminais[i1] = Terminais[i2];
                    Terminais[i2] = tmp;
                }
            }
        }
        
        /// <summary>
        /// Processo de redução.
        /// </summary>
        /// <param name="pilha"></param>
        /// <param name="tabelaSLR"></param>
        /// <param name="producao"></param>
        /// <returns></returns>
        public int Reduzir(Stack<string> pilha, List<List<string>> tabelaSLR, Producao producao)
        {
            string ladoDireito = "";
            bool achou = false;
            int count = -1;

            foreach (var simbolo in pilha)
            {
                // Quantidade de simbolos que serão removidos da pilha.
                count++;

                if (Int32.TryParse(simbolo, out int estadoAnterior))
                {
                    if (!achou) continue;

                    for (int i = 0; i < count; i++)
                    {
                        pilha.Pop();
                    }
                    
                    string proximoEstado =
                        tabelaSLR[estadoAnterior + 1][tabelaSLR[0].FindIndex(x => x == producao.Gerador)];

                    pilha.Push(producao.Gerador);
                    pilha.Push(proximoEstado);

                    return Convert.ToInt32(proximoEstado);
                }
                else
                {
                    // Lê o símbolo ao contrário, por isso insere no início ao invés de concatenar.
                    ladoDireito = ladoDireito.Insert(0, simbolo);

                    if (producao.Valor == ladoDireito && ladoDireito.Length == producao.Valor.Length)
                        achou = true;
                }
            }

            return count;
        }

        public ObservableCollection<Inline> PilhaParaInline(Stack<string> pilha)
        {
            int i = 0;
            var inlines = new ObservableCollection<Inline>();

            foreach (string simbolo in pilha.Reverse())
            {
                i++;

                var r = new Run() { Text = simbolo };

                //if (i <= pilha.Count)
                    r.Text += " ";

                if (IsTerminal(simbolo) || IsNaoTerminal(simbolo))
                {
                    r.FontStyle = FontStyles.Italic;
                    r.FontWeight = FontWeights.Bold;
                    r.Text += " ";
                }

                inlines.Add(r);
            }

            return inlines;
        }

        public ObservableCollection<Inline> EntradaParaInline(Stack<string> entrada)
        {
            int i = 0;
            var inlines = new ObservableCollection<Inline>();

            foreach (string simbolo in entrada)
            {
                i++;

                var r = new Run() { Text = simbolo };

                if (!simbolo.Equals("$"))
                {
                    r.FontStyle = FontStyles.Italic;
                    r.FontWeight = FontWeights.Bold;
                    r.Text += " ";
                }
                else
                {
                    r.Text = r.Text.Insert(0, " ");
                }

                inlines.Add(r);
            }

            return inlines;
        }

        private List<string> ProducoesParaLista(Producao producao)
        {
            var lista = new List<string>();

            string simbolo = "";

            for (int i = producao.Valor.Length - 1; i >= 0; i--)
            {
                simbolo = simbolo.Insert(0, producao.Valor[i].ToString());

                if (Terminais.Any(t => t.Valor.Equals(simbolo)) ||
                    NaoTerminais.Any(nt => nt.Valor.Equals(simbolo)))
                {
                    lista.Add(simbolo);
                    simbolo = "";
                }
            }

            lista.Reverse();
            lista.Insert(0, ".");
            return lista;
        }
    }
}
