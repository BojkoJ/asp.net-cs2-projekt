using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using BOJ0043_App.Models;
using BOJ0043_App.Services;

namespace BOJ0043_App.ViewModels
{
    public class WorkspaceChangeStatusViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;

        public string CurrentStatusText { get; set; }
        public ObservableCollection<string> AvailableStatuses { get; set; } = new();
        private string _selectedStatus = string.Empty;
        public string SelectedStatus
        {
            get => _selectedStatus;
            set { _selectedStatus = value; OnPropertyChanged(); }
        }
        public string StatusChangeComment { get; set; } = string.Empty;
        public Workspace Workspace { get; }

        public bool IsOccupied => CurrentStatusText == "Obsazené";
        public string OccupiedWarning => IsOccupied ? "Pracovní místo je momentálně obsazené a jeho stav nelze ručně změnit. Stav se automaticky změní na \"Dostupné\" po ukončení rezervace." : string.Empty;
        public bool CanChangeStatus => !IsOccupied;
        public string? LastError { get; private set; }

        public async Task<bool> SaveStatusChangeAsync()
        {
            var service = new WorkspaceService();
            var (success, error) = await service.ChangeWorkspaceStatusWithErrorAsync(Workspace.Id, SelectedStatus, StatusChangeComment);
            LastError = error;
            return success;
        }

        private void OnPropertyChanged([CallerMemberName] string? name = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));

        public WorkspaceChangeStatusViewModel(Workspace workspace)
        {
            Workspace = workspace;
            CurrentStatusText = workspace.CurrentStatusText;
            if (CurrentStatusText == "Dostupné")
            {
                AvailableStatuses.Add("V údržbě");
                SelectedStatus = "V údržbě";
            }
            else if (CurrentStatusText == "V údržbě")
            {
                AvailableStatuses.Add("Dostupné");
                SelectedStatus = "Dostupné";
            }
            // If Obsazené, do not allow any status change
        }
    }
}
