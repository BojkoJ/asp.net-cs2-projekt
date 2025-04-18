using System.Windows;
using BOJ0043_App.Models;
using BOJ0043_App.ViewModels;

namespace BOJ0043_App.Views
{
    public partial class WorkspaceChangeStatusWindow : Window
    {
        private readonly WorkspaceChangeStatusViewModel _viewModel;
        public WorkspaceChangeStatusWindow(Workspace workspace)
        {
            InitializeComponent();
            _viewModel = new WorkspaceChangeStatusViewModel(workspace);
            DataContext = _viewModel;
        }

        private async void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            var result = await _viewModel.SaveStatusChangeAsync();
            if (result)
            {
                MessageBox.Show("Stav byl úspěšně změněn.", "Hotovo", MessageBoxButton.OK, MessageBoxImage.Information);
                DialogResult = true;
                Close();
            }
            else
            {
                var errorMsg = string.IsNullOrWhiteSpace(_viewModel.LastError) ? "Nepodařilo se změnit stav." : $"Nepodařilo se změnit stav: {_viewModel.LastError}";
                MessageBox.Show(errorMsg, "Chyba", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
