using System;
using System.Collections.Generic;

namespace AnalisadorSintatico.Wpf.Modelos
{
    public class NT
    {
        public NT(string valor)
        {
            Valor = valor;

            First = new List<T>();
            Follow = new List<T>();
            Producoes = new List<Producao>();
        }

        public string Valor { get; set; }
        public List<T> First { get; set; }
        public List<T> Follow { get; set; }
        public List<Producao> Producoes { get; set; }

        public override string ToString() => Valor;

        public string ProducoesAsString => String.Join(" | ", Producoes);
        //{
            //get
            //{                
                //string result = "";

                //for (int i = 0; i < Producoes.Count; i++)
                //{
                //    result += Producoes[i].Valor + " | ";
                //}

                //return result;
            //}            
        //}
    }
}
