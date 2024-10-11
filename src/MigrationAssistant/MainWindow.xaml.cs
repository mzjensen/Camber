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

        private void btnSelectSource_Click(object sender, RoutedEventArgs e)
        {
            using (var dialog = new System.Windows.Forms.FolderBrowserDialog())
            {
                if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    ViewModel.SourceFolder = dialog.SelectedPath;
                }
            }
        }

        private void btnSelectOutput_Click(object sender, RoutedEventArgs e)
        {
            using (var dialog = new System.Windows.Forms.FolderBrowserDialog())
            {
                if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    ViewModel.OutputFolder = dialog.SelectedPath;
                }
            }
        }
    }
}