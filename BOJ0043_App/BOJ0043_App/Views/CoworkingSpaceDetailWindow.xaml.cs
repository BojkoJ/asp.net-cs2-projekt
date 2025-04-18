using BOJ0043_App.Models;
using System.Windows;

namespace BOJ0043_App.Views
{
    public partial class CoworkingSpaceDetailWindow : Window
    {
        public CoworkingSpaceDetailWindow(CoworkingSpace coworkingSpace)
        {
            InitializeComponent();
            DataContext = coworkingSpace;
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
            Close();
        }

        private void Hyperlink_RequestNavigate(object sender, System.Windows.Navigation.RequestNavigateEventArgs e)
        {
            System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo(e.Uri.AbsoluteUri) { UseShellExecute = true });
            e.Handled = true;
        }
    }
}
