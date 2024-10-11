using System.Windows;

namespace Camber.MigrationAssistant
{
    public partial class MainWindow : Window
    {
        public MigrationViewModel ViewModel { get; }

        public MainWindow()
        {
            InitializeComponent();
            ViewModel = new MigrationViewModel();
            DataContext = ViewModel;
        }
    }
}