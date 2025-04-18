using BOJ0043_App.Models;
using System.Windows;

namespace BOJ0043_App.Views
{
    public partial class ReservationDetailWindow : Window
    {
        public ReservationDetailWindow(Reservation reservation)
        {
            InitializeComponent();
            DataContext = reservation;
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
            Close();
        }

        private void EndReservationButton_Click(object sender, RoutedEventArgs e)
        {
            if (DataContext is Reservation reservation)
            {
                // Confirm action
                var result = MessageBox.Show("Opravdu chcete ukončit tuto rezervaci?\nPracovní místo bude nastaveno jako dostupné.", "Potvrzení ukončení", MessageBoxButton.YesNo, MessageBoxImage.Warning);
                if (result == MessageBoxResult.Yes)
                {
                    reservation.IsCompleted = true;
                    if (reservation.Workspace != null)
                    {
                        reservation.Workspace.CurrentStatus = 0; // Dostupné
                    }
                    DialogResult = true;
                    Close();
                }
            }
        }
    }
}
