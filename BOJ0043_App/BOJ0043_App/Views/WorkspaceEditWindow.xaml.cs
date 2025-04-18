using BOJ0043_App.Models;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using BOJ0043_App.Services;
using System.Windows.Input;

namespace BOJ0043_App.Views
{
    public partial class WorkspaceEditWindow : Window, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;
        private Workspace _workspace;
        private readonly WorkspaceService _workspaceService = new();
        private readonly CoworkingSpaceService _coworkingSpaceService = new();
        private bool _isNew;
        public string WindowTitle { get; set; }
        public List<string> StatusOptions { get; } = new() { "Dostupné", "Obsazené", "V údržbě" };
        private string _selectedStatus;
        public string SelectedStatus
        {
            get => _selectedStatus;
            set
            {
                if (_selectedStatus != value)
                {
                    _selectedStatus = value;
                    OnPropertyChanged();
                }
            }
        }

        private ObservableCollection<CoworkingSpace> _coworkingSpaces = new();
        public ObservableCollection<CoworkingSpace> CoworkingSpaces
        {
            get => _coworkingSpaces;
            set
            {
                _coworkingSpaces = value;
                OnPropertyChanged();
            }
        }

        public WorkspaceEditWindow(Workspace? workspace = null)
        {
            InitializeComponent();
            _isNew = workspace == null;
            _workspace = workspace ?? new Workspace();
            WindowTitle = _isNew ? "Nové pracovní místo" : "Úprava pracovního místa";
            _selectedStatus = StatusOptions[_workspace.CurrentStatus >= 0 && _workspace.CurrentStatus < StatusOptions.Count ? _workspace.CurrentStatus : 0];
            DataContext = this;
            _ = LoadCoworkingSpacesAsync();
        }

        private async Task LoadCoworkingSpacesAsync()
        {
            var spaces = await _coworkingSpaceService.GetAllCoworkingSpacesAsync();
            if (spaces != null)
            {
                CoworkingSpaces = new ObservableCollection<CoworkingSpace>(spaces.OrderBy(s => s.Name));
            }
            else
            {
                MessageBox.Show("Nepodařilo se načíst seznam coworkingových prostorů.", "Chyba", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            // Basic validation
            if (string.IsNullOrWhiteSpace(_workspace.Name))
            {
                MessageBox.Show("Název nemůže být prázdný.", "Chyba", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            if (_workspace.CoworkingSpaceId <= 0)
            {
                MessageBox.Show("Vyberte coworkingový prostor.", "Chyba", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            _workspace.CurrentStatus = StatusOptions.IndexOf(SelectedStatus);
            bool success;
            if (_isNew)
            {
                var result = await _workspaceService.CreateWorkspaceAsync(_workspace);
                success = result != null;
            }
            else
            {
                success = await _workspaceService.UpdateWorkspaceApiAsync(_workspace.Id, _workspace);
            }
            if (success)
            {
                DialogResult = true;
                Close();
            }
            else
            {
                MessageBox.Show(_isNew ? "Nepodařilo se vytvořit pracovní místo." : "Nepodařilo se uložit změny pracovního místa.", "Chyba", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public Workspace Workspace => _workspace;

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }

        protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
