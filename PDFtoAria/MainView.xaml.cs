using System.Windows;
using System.Windows.Interactivity;

namespace PDFtoAria
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainView : Window
    {
        public MainView()
        {
            Interaction.GetBehaviors(this);
            InitializeComponent();
        }
    }
}
