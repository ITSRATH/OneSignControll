using System;
using System.Diagnostics;
using System.IO;
using System.Windows;

namespace OneSignControll
{
    public partial class InfoWindow : Window
    {
        private readonly string appDataFolder;
        private readonly string xmlFilePath;

        public InfoWindow()
        {
            InitializeComponent();

            // Set the AppData folder path
            appDataFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "OneSignControll");
            xmlFilePath = Path.Combine(appDataFolder, "programs.xml");
        }

        private void DestroyButton_Click(object sender, RoutedEventArgs e)
        {
            string message = $"Sind Sie sicher, dass Sie den AppData-Ordner ({appDataFolder}) und die gespeicherte XML-Datei ({xmlFilePath}) löschen möchten? Dies kann nicht rückgängig gemacht werden!";
            MessageBoxResult result = MessageBox.Show(message, "Bestätigung erforderlich", MessageBoxButton.YesNo, MessageBoxImage.Warning);

            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    // Löschen der XML-Datei
                    if (File.Exists(xmlFilePath))
                    {
                        File.Delete(xmlFilePath);
                    }

                    // Löschen des gesamten AppData-Ordners
                    if (Directory.Exists(appDataFolder))
                    {
                        Directory.Delete(appDataFolder, true);
                    }

                    MessageBox.Show("Der AppData-Ordner und die XML-Datei wurden erfolgreich gelöscht.", "Löschen erfolgreich", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Fehler beim Löschen: {ex.Message}", "Fehler", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void OpenGitHubLink(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            Process.Start(new ProcessStartInfo("https://github.com/") { UseShellExecute = true });
        }

        private void OpenLink(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            Process.Start(new ProcessStartInfo("https://opensource.org/licenses/MIT") { UseShellExecute = true });
        }
    }
}
