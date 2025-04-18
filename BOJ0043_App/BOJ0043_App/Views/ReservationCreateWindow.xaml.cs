using BOJ0043_App.Commands;
using BOJ0043_App.Models;
using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using BOJ0043_App.Services;

namespace BOJ0043_App.Views
{
    public partial class ReservationCreateWindow : Window
    {
        public ObservableCollection<Workspace> Workspaces { get; set; }
        public Workspace? SelectedWorkspace { get; set; }
        public string CustomerEmail { get; set; } = string.Empty;
        public string CustomerName { get; set; } = string.Empty;
        public DateTime StartDate { get; set; } = DateTime.Now.Date;
        public string StartTime { get; set; } = DateTime.Now.ToString("HH:mm");
        public DateTime EndDate { get; set; } = DateTime.Now.Date.AddDays(1);
        public string EndTime { get; set; } = DateTime.Now.AddHours(1).ToString("HH:mm");
        public string Note { get; set; } = string.Empty;
        public ICommand CreateCommand { get; set; }

        private readonly ReservationService _reservationService = new();

        public Reservation? CreatedReservation { get; private set; }

        public ReservationCreateWindow(ObservableCollection<Workspace> workspaces)
        {
            InitializeComponent();
            Workspaces = workspaces;
            DataContext = this;
        }

        private async void CreateButton_Click(object sender, RoutedEventArgs e)
        {
            if (SelectedWorkspace == null)
            {
                MessageBox.Show("Vyberte pracovní místo.", "Chyba", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            if (string.IsNullOrWhiteSpace(CustomerEmail))
            {
                MessageBox.Show("Zadejte email zákazníka.", "Chyba", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            if (string.IsNullOrWhiteSpace(CustomerName))
            {
                MessageBox.Show("Zadejte jméno zákazníka.", "Chyba", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            if (!TimeSpan.TryParse(StartTime, out var startTimeSpan) || !TimeSpan.TryParse(EndTime, out var endTimeSpan))
            {
                MessageBox.Show("Zadejte platný čas začátku a konce ve formátu HH:mm.", "Chyba", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            var start = StartDate.Date + startTimeSpan;
            var end = EndDate.Date + endTimeSpan;
            if (end <= start)
            {
                MessageBox.Show("Konec rezervace musí být po začátku.", "Chyba", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            bool success;

            var result = await _reservationService.CreateReservationAsync(new Reservation
            {
                CustomerEmail = CustomerEmail,
                CustomerName = CustomerName,
                StartTime = start,
                EndTime = end,
                Note = Note,
                WorkspaceId = SelectedWorkspace.Id
            });

            success = result != null;

            if (success)
            {
                DialogResult = true;
                Close();
            }
            else
            {
                MessageBox.Show("Nepodařilo se vytvořit rezervaci.", "Chyba", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}
