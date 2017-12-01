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

        public override string ToString() => Gerador + " ← " + Valor;
    }
}
