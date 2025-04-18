using BOJ0043_App.Models;
using BOJ0043_App.Services;
using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Input;

namespace BOJ0043_App.ViewModels
{
    public class MainViewModel : ViewModelBase
    {
        private readonly CoworkingSpaceService _coworkingSpaceService;
        private readonly WorkspaceService _workspaceService;
        private readonly ReservationService _reservationService;

        private ObservableCollection<CoworkingSpace> _coworkingSpaces;
        private CoworkingSpace? _selectedCoworkingSpace;
        private ObservableCollection<Workspace> _workspaces;
        private Workspace? _selectedWorkspace;
        private ObservableCollection<Reservation> _reservations;
        private Reservation? _selectedReservation;
        private bool _isLoading;
        private string _statusMessage = string.Empty;

        // Příkazy
        public ICommand ShowDetailsCommand { get; private set; }
        public ICommand EditCommand { get; private set; }
        public ICommand DeleteCommand { get; private set; }
        public ICommand AddNewCoworkingSpaceCommand { get; private set; }
        public ICommand ShowWorkspaceDetailCommand { get; private set; }
        public ICommand EditWorkspaceCommand { get; private set; }
        public ICommand DeleteWorkspaceCommand { get; private set; }
        public ICommand ShowStatusHistoryCommand { get; private set; }
        public ICommand ChangeStatusCommand { get; private set; }
        public ICommand AddNewWorkspaceCommand { get; private set; }
        public ICommand ShowReservationDetailCommand { get; private set; }
        public ICommand CompleteReservationCommand { get; private set; }
        public ICommand CreateReservationCommand { get; private set; }
        public ICommand ShowStatisticsCommand { get; private set; }

        public MainViewModel()
        {
            // Inicializace služeb
            _coworkingSpaceService = new CoworkingSpaceService();
            _workspaceService = new WorkspaceService();
            _reservationService = new ReservationService();

            // Inicializace kolekcí
            _coworkingSpaces = new ObservableCollection<CoworkingSpace>();
            _workspaces = new ObservableCollection<Workspace>();
            _reservations = new ObservableCollection<Reservation>();

            // Inicializace příkazů
            ShowDetailsCommand = new Commands.RelayCommand(ShowCoworkingSpaceDetails);
            EditCommand = new Commands.RelayCommand(EditCoworkingSpace);
            DeleteCommand = new Commands.RelayCommand(DeleteCoworkingSpace);
            AddNewCoworkingSpaceCommand = new Commands.RelayCommand(_ => AddNewCoworkingSpace());
            ShowWorkspaceDetailCommand = new Commands.RelayCommand(ShowWorkspaceDetail);
            EditWorkspaceCommand = new Commands.RelayCommand(EditWorkspace);
            DeleteWorkspaceCommand = new Commands.RelayCommand(DeleteWorkspace);
            ShowStatusHistoryCommand = new Commands.RelayCommand(ShowStatusHistory);
            ChangeStatusCommand = new Commands.RelayCommand(ChangeStatus);
            AddNewWorkspaceCommand = new Commands.RelayCommand(_ => AddNewWorkspace());
            ShowReservationDetailCommand = new Commands.RelayCommand(ShowReservationDetail);
            CompleteReservationCommand = new Commands.RelayCommand(CompleteReservation);
            CreateReservationCommand = new Commands.RelayCommand(_ => CreateReservation());
            ShowStatisticsCommand = new Commands.RelayCommand(_ => new Views.StatisticsWindow().ShowDialog());

            // Načtení dat při startu
            _ = LoadDataAsync();
        }

        public ObservableCollection<CoworkingSpace> CoworkingSpaces
        {
            get => _coworkingSpaces;
            set => SetProperty(ref _coworkingSpaces, value);
        }

        public CoworkingSpace? SelectedCoworkingSpace
        {
            get => _selectedCoworkingSpace;
            set
            {
                if (SetProperty(ref _selectedCoworkingSpace, value) && value != null)
                {
                    // Načtení pracovních míst pro vybraný coworking
                    _ = LoadWorkspacesForCoworkingSpaceAsync(value.Id);
                }
            }
        }

        public ObservableCollection<Workspace> Workspaces
        {
            get => _workspaces;
            set => SetProperty(ref _workspaces, value);
        }

        public Workspace? SelectedWorkspace
        {
            get => _selectedWorkspace;
            set
            {
                if (SetProperty(ref _selectedWorkspace, value) && value != null)
                {
                    // Načtení rezervací pro vybrané pracovní místo
                    _ = LoadReservationsForWorkspaceAsync(value.Id);
                }
            }
        }

        public ObservableCollection<Reservation> Reservations
        {
            get => _reservations;
            set => SetProperty(ref _reservations, value);
        }

        public Reservation? SelectedReservation
        {
            get => _selectedReservation;
            set => SetProperty(ref _selectedReservation, value);
        }

        public bool IsLoading
        {
            get => _isLoading;
            set => SetProperty(ref _isLoading, value);
        }

        public string StatusMessage
        {
            get => _statusMessage;
            set => SetProperty(ref _statusMessage, value);
        }

        private async Task LoadDataAsync()
        {
            try
            {
                IsLoading = true;
                StatusMessage = "Načítání dat...";

                var coworkingSpaces = await _coworkingSpaceService.GetAllCoworkingSpacesAsync();
                if (coworkingSpaces != null)
                {
                    CoworkingSpaces = new ObservableCollection<CoworkingSpace>(coworkingSpaces);
                    StatusMessage = $"Načteno {coworkingSpaces.Count} coworkingových prostorů";
                }
                else
                {
                    StatusMessage = "Nepodařilo se načíst data z API";
                }
            }
            catch (Exception ex)
            {
                StatusMessage = $"Chyba při načítání dat: {ex.Message}";
            }
            finally
            {
                IsLoading = false;
            }
        }

        private async Task LoadWorkspacesForCoworkingSpaceAsync(int coworkingSpaceId)
        {
            try
            {
                IsLoading = true;
                StatusMessage = "Načítání pracovních míst...";

                var workspaces = await _workspaceService.GetWorkspacesByCoworkingSpaceIdAsync(coworkingSpaceId);
                if (workspaces != null)
                {
                    // Map CoworkingSpace for each workspace
                    foreach (var ws in workspaces)
                    {
                        ws.CoworkingSpace = CoworkingSpaces.FirstOrDefault(cs => cs.Id == ws.CoworkingSpaceId);
                    }
                    Workspaces = new ObservableCollection<Workspace>(workspaces);
                    StatusMessage = $"Načteno {workspaces.Count} pracovních míst";
                }
                else
                {
                    Workspaces.Clear();
                    StatusMessage = "Nepodařilo se načíst pracovní místa";
                }
            }
            catch (Exception ex)
            {
                StatusMessage = $"Chyba při načítání pracovních míst: {ex.Message}";
            }
            finally
            {
                IsLoading = false;
            }
        }

        private async Task LoadReservationsForWorkspaceAsync(int workspaceId)
        {
            try
            {
                IsLoading = true;
                StatusMessage = "Načítání rezervací...";

                var reservations = await _reservationService.GetReservationsByWorkspaceIdAsync(workspaceId);
                if (reservations != null)
                {
                    // Map Workspace and CoworkingSpace for each reservation
                    foreach (var res in reservations)
                    {
                        res.Workspace = Workspaces.FirstOrDefault(w => w.Id == res.WorkspaceId);
                        if (res.Workspace != null)
                        {
                            res.Workspace.CoworkingSpace = CoworkingSpaces.FirstOrDefault(cs => cs.Id == res.Workspace.CoworkingSpaceId);
                        }
                    }
                    Reservations = new ObservableCollection<Reservation>(reservations);
                    StatusMessage = $"Načteno {reservations.Count} rezervací";
                }
                else
                {
                    Reservations.Clear();
                    StatusMessage = "Nepodařilo se načíst rezervace";
                }
            }
            catch (Exception ex)
            {
                StatusMessage = $"Chyba při načítání rezervací: {ex.Message}";
            }
            finally
            {
                IsLoading = false;
            }
        }

        // Metody pro příkazy
        private async void ShowCoworkingSpaceDetails(object? parameter)
        {
            if (parameter is CoworkingSpace coworkingSpace)
            {
                // Fetch related workspaces using the dedicated endpoint
                var workspaces = await _workspaceService.GetWorkspacesByCoworkingSpaceIdAsync(coworkingSpace.Id);
                coworkingSpace.Workspaces = new System.Collections.ObjectModel.ObservableCollection<Models.Workspace>(workspaces ?? new List<Models.Workspace>());
                var detailWindow = new Views.CoworkingSpaceDetailWindow(coworkingSpace);
                detailWindow.ShowDialog();
            }
        }

        private void EditCoworkingSpace(object? parameter)
        {
            if (parameter is CoworkingSpace coworkingSpace)
            {
                var editWindow = new Views.CoworkingSpaceEditWindow(coworkingSpace);
                if (editWindow.ShowDialog() == true)
                {
                    // Po úspěšné editaci znovu načteme data
                    _ = LoadDataAsync();
                }
            }
        }

        private async void DeleteCoworkingSpace(object? parameter)
        {
            if (parameter is CoworkingSpace coworkingSpace)
            {
                var result = System.Windows.MessageBox.Show(
                    $"Opravdu chcete smazat coworkingový prostor '{coworkingSpace.Name}'?\n\nBudou smazána i všechna pracovní místa v tomto prostoru!",
                    "Potvrzení smazání",
                    System.Windows.MessageBoxButton.YesNo,
                    System.Windows.MessageBoxImage.Warning);

                if (result == System.Windows.MessageBoxResult.Yes)
                {
                    try
                    {
                        IsLoading = true;
                        StatusMessage = "Mazání coworkingového prostoru...";

                        bool success = await _coworkingSpaceService.DeleteCoworkingSpaceAsync(coworkingSpace.Id);
                        if (success)
                        {
                            StatusMessage = "Coworkingový prostor byl úspěšně smazán";
                            // Po úspěšném smazání znovu načteme data
                            await LoadDataAsync();
                        }
                        else
                        {
                            StatusMessage = "Nepodařilo se smazat coworkingový prostor";
                            System.Windows.MessageBox.Show(
                                "Nepodařilo se smazat coworkingový prostor. Zkuste to prosím znovu později.",
                                "Chyba",
                                System.Windows.MessageBoxButton.OK,
                                System.Windows.MessageBoxImage.Error);
                        }
                    }
                    catch (Exception ex)
                    {
                        StatusMessage = $"Chyba při mazání coworkingového prostoru: {ex.Message}";
                        System.Windows.MessageBox.Show(
                            $"Při mazání coworkingového prostoru došlo k chybě: {ex.Message}",
                            "Chyba",
                            System.Windows.MessageBoxButton.OK,
                            System.Windows.MessageBoxImage.Error);
                    }
                    finally
                    {
                        IsLoading = false;
                    }
                }
            }
        }

        private void AddNewCoworkingSpace()
        {
            // Use the edit window for adding a new coworking space
            var editWindow = new Views.CoworkingSpaceEditWindow();
            if (editWindow.ShowDialog() == true)
            {
                // After successful creation, reload data
                _ = LoadDataAsync();
            }
        }

        private async void ShowWorkspaceDetail(object? parameter)
        {
            if (parameter is Workspace workspace)
            {
                // Fetch active reservations for this workspace
                var reservations = await _reservationService.GetActiveReservationsByWorkspaceIdAsync(workspace.Id);
                workspace.Reservations = new System.Collections.ObjectModel.ObservableCollection<Reservation>(reservations ?? new System.Collections.Generic.List<Reservation>());
                var detailWindow = new Views.WorkspaceDetailWindow(workspace);
                detailWindow.ShowDialog();
            }
        }

        private void EditWorkspace(object? parameter)
        {
            if (parameter is Workspace workspace)
            {
                var editWindow = new Views.WorkspaceEditWindow(workspace);
                if (editWindow.ShowDialog() == true)
                {
                    // Reload workspaces after edit
                    if (SelectedCoworkingSpace != null)
                        _ = LoadWorkspacesForCoworkingSpaceAsync(SelectedCoworkingSpace.Id);
                }
            }
        }

        private async void DeleteWorkspace(object? parameter)
        {
            if (parameter is Workspace workspace)
            {
                var result = System.Windows.MessageBox.Show(
                    $"Opravdu chcete smazat pracovní místo '{workspace.Name}'?",
                    "Potvrzení smazání",
                    System.Windows.MessageBoxButton.YesNo,
                    System.Windows.MessageBoxImage.Warning);
                if (result == System.Windows.MessageBoxResult.Yes)
                {
                    await _workspaceService.DeleteWorkspaceAsync(workspace.Id);
                    if (SelectedCoworkingSpace != null)
                        await LoadWorkspacesForCoworkingSpaceAsync(SelectedCoworkingSpace.Id);
                }
            }
        }

        private void ShowStatusHistory(object? parameter)
        {
            if (parameter is Workspace workspace)
            {
                var window = new Views.WorkspaceStatusHistoryWindow(workspace);
                window.ShowDialog();
            }
        }

        private void ChangeStatus(object? parameter)
        {
            if (parameter is Workspace workspace)
            {
                var window = new Views.WorkspaceChangeStatusWindow(workspace);
                if (window.ShowDialog() == true)
                {
                    // Optionally reload workspaces or update status
                    _ = LoadWorkspacesForCoworkingSpaceAsync(workspace.CoworkingSpaceId);
                }
            }
        }

        private void AddNewWorkspace()
        {
            var window = new Views.WorkspaceEditWindow();
            if (window.ShowDialog() == true)
            {
                // Reload workspaces for the selected coworking space (if any)
                if (SelectedCoworkingSpace != null)
                {
                    _ = LoadWorkspacesForCoworkingSpaceAsync(SelectedCoworkingSpace.Id);
                }
                else
                {
                    // If no coworking space is selected, reload all data
                    _ = LoadDataAsync();
                }
            }
        }

        private void ShowReservationDetail(object? parameter)
        {
            if (parameter is Reservation reservation)
            {
                var window = new Views.ReservationDetailWindow(reservation);
                window.ShowDialog();
            }
        }

        private async void CompleteReservation(object? parameter)
        {
            if (parameter is Reservation reservation && !reservation.IsCompleted)
            {
                var result = System.Windows.MessageBox.Show(
                    "Opravdu chcete ukončit tuto rezervaci? Pracovní místo bude nastaveno jako dostupné.",
                    "Potvrzení ukončení",
                    System.Windows.MessageBoxButton.YesNo,
                    System.Windows.MessageBoxImage.Warning);
                if (result == System.Windows.MessageBoxResult.Yes)
                {
                    // Call backend API to complete reservation
                    bool success = await _reservationService.CompleteReservationAsync(reservation.Id);
                    if (success)
                    {
                        reservation.IsCompleted = true;
                        if (reservation.Workspace != null)
                        {
                            reservation.Workspace.CurrentStatus = 0; // Dostupné
                        }
                        // Refresh list
                        if (SelectedWorkspace != null)
                            await LoadReservationsForWorkspaceAsync(SelectedWorkspace.Id);
                    }
                    else
                    {
                        System.Windows.MessageBox.Show("Nepodařilo se ukončit rezervaci na serveru.", "Chyba", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error);
                    }
                }
            }
        }

        private void CreateReservation()
        {
            var window = new Views.ReservationCreateWindow(Workspaces);
            if (window.ShowDialog() == true && window.CreatedReservation != null)
            {
                // Call backend to create reservation
                _ = CreateReservationAsync(window.CreatedReservation);
            }
        }

        private async Task CreateReservationAsync(Reservation reservation)
        {
            try
            {
                var created = await _reservationService.CreateReservationAsync(reservation);
                if (created != null)
                {
                    StatusMessage = "Rezervace byla úspěšně vytvořena.";
                    if (SelectedWorkspace != null)
                        await LoadReservationsForWorkspaceAsync(SelectedWorkspace.Id);
                }
                else
                {
                    StatusMessage = "Nepodařilo se vytvořit rezervaci.";
                }
            }
            catch (Exception ex)
            {
                StatusMessage = $"Chyba při vytváření rezervace: {ex.Message}";
            }
        }

        private void ShowStatistics()
        {
            var window = new Views.StatisticsWindow();
            window.ShowDialog();
        }


    }
}
