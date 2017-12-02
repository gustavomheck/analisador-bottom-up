using System;
using System.Collections.Generic;
using System.Linq;

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
        /// Obtém ou define as produções da gramática.
        /// </summary>
        public List<Producao> Producoes { get; internal set; }
        
        public bool IsTerminal(string simbolo)
        {
            foreach (T t in Terminais)
                if (t.Valor.Equals(simbolo))
                    return true;

            return false;
        }

        /// <summary>
        /// Obtém ou define o primeiro símbolo de uma produção.
        /// </summary>
        /// <param name="producao">A produção para extrair o símbolo.</param>
        /// <returns>O primeiro símbolo ou a própria produção.</returns>
        public string ObterPrimeiroSimbolo(string producao)
        {
            string simbolo = "";

            foreach (char c in producao)
            {
                simbolo += c;

                foreach (T t in Terminais)
                    if (simbolo.Equals(t.Valor))
                        return simbolo;

                if (Char.IsUpper(c))
                    foreach (NT nt in NaoTerminais)
                        if (simbolo.Equals(nt.Valor))
                            return simbolo;
            }

            return producao;
        }

        public void NumerarProducoes()
        {
            Producoes = new List<Producao>()
            {
                new Producao("E", "E+T"),
                new Producao("E", "T"),
                new Producao("T", "T*F"),
                new Producao("T", "F"),
                new Producao("F", "(E)"),
                new Producao("F", "id")
            };
        }

        /// <summary>
        /// Obtém o próximo símbolo Não-Terminal depois de um determinado Não-Terminal.
        /// </summary>
        /// <param name="naoTerminal">O Não-Terminal para tomar como parâmetro.</param>
        /// <param name="producao">A produção para analisar.</param>
        /// <returns>O próximo Não-Terminal.</returns>
        public string ObterProximoNaoTerminal(string naoTerminal, string producao)
        {
            int indice = producao.ContainsAt(naoTerminal);

            if (indice == -1) return null;

            if (naoTerminal.Length == 1 && indice == (producao.Length - 1) ||
                naoTerminal.Length == 2 && indice == (producao.Length - 2))
            {
                return producao.Substring(indice);
            }
            else
            {
                int i = indice + (naoTerminal.Length == 1 ? 1 : 2);

                if (i < (producao.Length - 1) && producao[i + 1].ToString().Equals("'"))
                {
                    return producao.Substring(i, 2);
                }

                return producao.Substring(i, 1);
            }            
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
        /// Transforma os simbolos de uma produção em uma lista.
        /// </summary>
        /// <param name="p">A produção a ser transformada.</param>
        /// <returns>Uma lista com todos os símbolos da produção.</returns>
        public List<string> ObterSimbolosDaProducao(string p)
        {
            var simbolos = new List<string>();
            string simbolo = "";

            p = String.Join("", p.Split(new[] { '→' }, StringSplitOptions.RemoveEmptyEntries)).Trim();

            for (int i = p.Length - 1; i >= 0; i--)
            {
                simbolo = simbolo.Insert(0, p[i].ToString());

                if (Terminais.Any(t => t.Valor.Equals(simbolo)) ||
                    NaoTerminais.Any(nt => nt.Valor.Equals(simbolo)))
                {
                    simbolos.Add(simbolo);
                    simbolo = "";
                }
            }

            return simbolos;
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

                    return Convert.ToInt32(proximoEstado) + 1;
                }
                else
                {
                    ladoDireito += simbolo;

                    if (producao.Valor == ladoDireito)
                        achou = true;
                }
            }

            return count;
        }
    }
}
