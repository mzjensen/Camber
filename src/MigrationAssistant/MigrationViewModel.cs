using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;
using MessageBox = System.Windows.MessageBox;

namespace Camber.MigrationAssistant
{
    public class MigrationViewModel : INotifyPropertyChanged
    {
        private string sourceFolder;
        private string outputFolder;
        private int filesProcessed;
        private int filesSkipped;
        private int filesErrored;
        private string status;
        private double progress;
        private bool canMigrate;
        private bool isProcessingComplete;
        private string _logFile;
        private string xmlMigrationFile;

        public string SourceFolder
        {
            get => sourceFolder;
            set
            {
                sourceFolder = value;
                OnPropertyChanged();
                CheckReadyToMigrate();
            }
        }

        public string OutputFolder
        {
            get => outputFolder;
            set
            {
                outputFolder = value;
                OnPropertyChanged();
                CheckReadyToMigrate();
            }
        }

        public int FilesProcessed
        {
            get => filesProcessed;
            set
            {
                filesProcessed = value;
                OnPropertyChanged();
            }
        }

        public int FilesSkipped
        {
            get => filesSkipped;
            set
            {
                filesSkipped = value;
                OnPropertyChanged();
            }
        }

        public int FilesErrored
        {
            get => filesErrored;
            set
            {
                filesErrored = value;
                OnPropertyChanged();
            }
        }

        public string Status
        {
            get => status;
            set
            {
                status = value;
                OnPropertyChanged();
            }
        }

        public double Progress
        {
            get => progress;
            set
            {
                progress = value;
                OnPropertyChanged();
            }
        }

        public bool CanMigrate
        {
            get => canMigrate;
            set
            {
                canMigrate = value;
                OnPropertyChanged();
            }
        }

        public bool IsProcessingComplete
        {
            get => isProcessingComplete;
            set
            {
                isProcessingComplete = value;
                OnPropertyChanged();
            }
        }

        public string LogFile
        {
            get => _logFile;
            set
            {
                _logFile = value;
                OnPropertyChanged();
            }
        }

        public string XmlMigrationFile
        {
            get => xmlMigrationFile;
            set
            {
                xmlMigrationFile = value;
                OnPropertyChanged();
            }
        }

        public ICommand SelectSourceFolderCommand { get; }
        public ICommand SelectOutputFolderCommand { get; }
        public ICommand MigrateCommand { get; }
        public ICommand ViewLogCommand { get; }
        public ICommand HelpCommand { get; }

        public event PropertyChangedEventHandler PropertyChanged;

        public MigrationViewModel()
        {
            SelectSourceFolderCommand = new RelayCommand(SelectSourceFolder);
            SelectOutputFolderCommand = new RelayCommand(SelectOutputFolder);
            MigrateCommand = new RelayCommand(Migrate, () => CanMigrate);
            ViewLogCommand = new RelayCommand(ViewLog, () => IsProcessingComplete);
            HelpCommand = new RelayCommand(ViewHelp);
            LogFile = "Migration.log";
            XmlMigrationFile = "Camber.Migrations.xml";
        }

        private void CheckReadyToMigrate()
        {
            CanMigrate = !string.IsNullOrEmpty(SourceFolder) && !string.IsNullOrEmpty(OutputFolder);
        }

        private void SelectSourceFolder()
        {
            using var dialog = new FolderBrowserDialog();
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                SourceFolder = dialog.SelectedPath;
            }
        }

        private void SelectOutputFolder()
        {
            using var dialog = new FolderBrowserDialog();
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                OutputFolder = dialog.SelectedPath;
            }
        }

        private void Migrate()
        {
            FilesProcessed = 0;
            FilesSkipped = 0;
            FilesErrored = 0;
            IsProcessingComplete = false;

            string[] files = Directory.GetFiles(SourceFolder);
            Progress = 0;
            double progressIncrement = 100.0 / files.Length;

            var logFilePath = Path.Combine(OutputFolder, LogFile);

            var xmlFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, XmlMigrationFile);

            var worker = new MigrationWorker(logFilePath, xmlFilePath);

            try
            {
                worker.Initialize();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"{ex.Message}", 
                    "Error", 
                    MessageBoxButton.OK, 
                    MessageBoxImage.Error);
                return;
            }

            foreach (string file in files)
            {
                try
                {
                    var result = worker.MigrateGraph(file, OutputFolder);
                    switch (result)
                    {
                        case MigrationResult.Success:
                            FilesProcessed++;
                            break;
                        case MigrationResult.Skipped:
                            FilesSkipped++;
                            break;
                        case MigrationResult.Error:
                            FilesErrored++;
                            break;
                    }
                }
                catch (Exception)
                {
                    FilesErrored++;
                }
                finally
                {
                    Progress += progressIncrement;
                }
            }

            Status = $"Successful: {FilesProcessed}  |  Skipped: {FilesSkipped}  |  Errors: {FilesErrored}";
            IsProcessingComplete = true;

            if (FilesErrored > 0)
            {
                MessageBox.Show(
                    "Errors were encountered during migration. Please check the log file.",
                    "Migration Complete",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);
            }
            else
            {
                MessageBox.Show(
                    $"{FilesProcessed} graphs migrated successfully. Please open them in Dynamo to verify the results.",
                    "Migration Complete",
                    MessageBoxButton.OK,
                    MessageBoxImage.Information);
            }
        }

        private void ViewLog()
        {
            var file = Path.Combine(OutputFolder, LogFile);

            if (File.Exists(file))
            {
                Process.Start(new ProcessStartInfo
                {
                    FileName = file,
                    UseShellExecute = true
                });
            }
            else
            {
                MessageBox.Show(
                    "Log file not found",
                    "Error",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }

        private void ViewHelp()
        {
            string exePath = Assembly.GetExecutingAssembly().Location;
            string exeDirectory = Path.GetDirectoryName(exePath);
            string readmePath = Path.Combine(exeDirectory, "README.md");

            if (File.Exists(readmePath))
            {
                Process.Start(new ProcessStartInfo
                {
                    FileName = readmePath,
                    UseShellExecute = true
                });
            }
            else
            {
                MessageBox.Show(
                    "README file not found",
                    "Error",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }

        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
