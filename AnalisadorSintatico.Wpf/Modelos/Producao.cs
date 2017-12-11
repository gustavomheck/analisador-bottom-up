namespace AnalisadorSintatico.Wpf.Modelos
{
    public class Producao
    {
        public Producao(string gerador, string valor)
        {
            Gerador = gerador;
            Valor = valor;
        }

        public string Gerador { get; set; }
        public string Valor { get; set; }

        public string ProducaoFormatada => Gerador + " ← " + Valor;

        public override string ToString() => Valor;
    }
}
