using BOJ0043_App.Models;
using BOJ0043_App.Services;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows;

namespace BOJ0043_App.Views
{
    public partial class WorkspaceDetailWindow : Window
    {
        public ObservableCollection<Reservation> ActiveReservations { get; set; }

        public WorkspaceDetailWindow(Workspace workspace)
        {
            InitializeComponent();
            DataContext = workspace;
            ActiveReservations = new ObservableCollection<Reservation>();
            LoadActiveReservationsAsync(workspace.Id);
        }

        private async void LoadActiveReservationsAsync(int workspaceId)
        {
            var service = new ReservationService();
            var reservations = await service.GetActiveReservationsByWorkspaceIdAsync(workspaceId);
            ActiveReservations.Clear();
            if (reservations != null)
            {
                foreach (var r in reservations)
                    ActiveReservations.Add(r);
            }
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
