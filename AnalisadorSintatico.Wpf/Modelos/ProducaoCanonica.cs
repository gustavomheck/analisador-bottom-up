using System;
using System.Collections.Generic;

namespace AnalisadorSintatico.Wpf.Modelos
{
    public class ProducaoCanonica
    {
        public ProducaoCanonica(string gerador, List<string> producao)
        {
            Gerador = gerador;
            Producao = producao;
        }

        public string Gerador { get; set; }
        public List<string> Producao { get; set; }

        public override string ToString() => Gerador + " -> " + String.Join("", Producao);
    }
}
