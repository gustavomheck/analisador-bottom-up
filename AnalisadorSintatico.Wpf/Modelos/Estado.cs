using System;
using System.Collections.Generic;

namespace AnalisadorSintatico.Wpf.Modelos
{
    public class Estado
    {
        public Estado()
        {
            Producoes = new List<ProducaoCanonica>();
        }

        public int EstadoAtual { get; set; }
        public int EstadoAnterior { get; set; }
        public NT NaoTerminalGerador { get; set; }
        public List<ProducaoCanonica> Producoes { get; set; }

        public override string ToString()
        {
            string str = "I" + EstadoAtual + " = { " + Environment.NewLine;

            for (int i = 0; i < Producoes.Count - 1; i++)
            {
                str += "   " + Producoes[i].ToString() + Environment.NewLine;
            }

            str += "   " + Producoes[Producoes.Count - 1] + " }";

            return str;
        }
    }
}
