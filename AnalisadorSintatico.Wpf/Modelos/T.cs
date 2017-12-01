namespace AnalisadorSintatico.Wpf.Modelos
{
    public class T
    {
        public T(string valor)
        {
            Valor = valor;
        }

        public string Valor { get; set; }

        /// <summary>
        /// Obtém ou define o símbolo de produção vazio "ε".
        /// </summary>
        public static string Vazio => "ε";

        public override string ToString() => Valor;
    }
}
