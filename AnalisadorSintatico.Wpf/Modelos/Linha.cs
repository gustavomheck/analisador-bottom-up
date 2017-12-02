using System.Collections.ObjectModel;
using System.Windows.Documents;

namespace AnalisadorSintatico.Wpf.Modelos
{
    public class Linha
    {
        public Linha()
        {
            Pilha = new ObservableCollection<Inline>();
            Entrada = new ObservableCollection<Inline>();
        }

        public int Passo { get; set; }
        //public string Pilha { get; set; }
        //public string Entrada { get; set; }
        public string Acao { get; set; }

        public ObservableCollection<Inline> Pilha { get; set; }
        public ObservableCollection<Inline> Entrada { get; set; }
    }
}
