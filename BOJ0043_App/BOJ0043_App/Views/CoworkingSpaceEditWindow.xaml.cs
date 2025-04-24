using BOJ0043_App.Commands;
using BOJ0043_App.Models;
using BOJ0043_App.Services;
using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace BOJ0043_App.Views
{
    public partial class CoworkingSpaceEditWindow : Window
    {
        private readonly CoworkingSpaceService _coworkingSpaceService;
        private bool _isNew;

        public string LatitudeInput
        {
            get => _latitudeInput;
            set { _latitudeInput = value; OnPropertyChanged(nameof(LatitudeInput)); }
        }
        private string _latitudeInput = string.Empty;

        public string LongitudeInput
        {
            get => _longitudeInput;
            set { _longitudeInput = value; OnPropertyChanged(nameof(LongitudeInput)); }
        }
        private string _longitudeInput = string.Empty;

        public CoworkingSpaceEditWindow(CoworkingSpace? coworkingSpace = null)
        {
            InitializeComponent();

            _coworkingSpaceService = new CoworkingSpaceService();

            // Určíme, zda jde o vytvoření nového nebo úpravu existujícího
            _isNew = coworkingSpace == null;

            // Vytvoříme nový objekt, pokud nebyl předán existující
            CoworkingSpace = coworkingSpace ?? new CoworkingSpace();

            LatitudeInput = CoworkingSpace.Latitude.ToString(System.Globalization.CultureInfo.InvariantCulture);
            LongitudeInput = CoworkingSpace.Longitude.ToString(System.Globalization.CultureInfo.InvariantCulture);

            // Nastavíme nadpis okna
            WindowTitle = _isNew ? "Nový coworkingový prostor" : "Úprava coworkingového prostoru";

            // Nastavíme příkaz pro uložení
            SaveCommand = new RelayCommand(async param => await SaveCoworkingSpaceAsync());


            DataContext = this;
        }

        public CoworkingSpace CoworkingSpace { get; set; }

        public string WindowTitle { get; set; }

        public ICommand SaveCommand { get; }

        private async Task SaveCoworkingSpaceAsync()
        {
            try
            {
                // Validace
                if (string.IsNullOrWhiteSpace(CoworkingSpace.Name))
                {
                    MessageBox.Show("Název nemůže být prázdný.", "Chyba", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                if (string.IsNullOrWhiteSpace(CoworkingSpace.Address))
                {
                    MessageBox.Show("Adresa nemůže být prázdná.", "Chyba", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                // Validate latitude and longitude as decimals
                if (!decimal.TryParse(LatitudeInput.Replace(',', '.'), System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out var latitude))
                {
                    MessageBox.Show("Zadejte platnou zeměpisnou šířku (číslo).", "Chyba", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
                if (!decimal.TryParse(LongitudeInput.Replace(',', '.'), System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out var longitude))
                {
                    MessageBox.Show("Zadejte platnou zeměpisnou délku (číslo).", "Chyba", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
                CoworkingSpace.Latitude = latitude;
                CoworkingSpace.Longitude = longitude;

                // Uložení
                if (_isNew)
                {
                    var result = await _coworkingSpaceService.CreateCoworkingSpaceAsync(CoworkingSpace);
                    if (result != null)
                    {
                        MessageBox.Show("Coworkingový prostor byl úspěšně vytvořen.", "Úspěch", MessageBoxButton.OK, MessageBoxImage.Information);
                        DialogResult = true;
                        Close();
                    }
                    else
                    {
                        MessageBox.Show("Nepodařilo se vytvořit coworkingový prostor.", "Chyba", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
                else
                {
                    var result = await _coworkingSpaceService.UpdateCoworkingSpaceAsync(CoworkingSpace.Id, CoworkingSpace);
                    if (result != null)
                    {
                        MessageBox.Show("Coworkingový prostor byl úspěšně aktualizován.", "Úspěch", MessageBoxButton.OK, MessageBoxImage.Information);
                        DialogResult = true;
                        Close();
                    }
                    else
                    {
                        MessageBox.Show("Nepodařilo se aktualizovat coworkingový prostor.", "Chyba", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Při ukládání došlo k chybě: {ex.Message}", "Chyba", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }

        private void Decimal_PreviewTextInput(object sender, System.Windows.Input.TextCompositionEventArgs e)
        {
            var textBox = sender as System.Windows.Controls.TextBox;
            string currentText = textBox?.Text ?? string.Empty;
            int selectionStart = textBox?.SelectionStart ?? 0;
            string newText = currentText.Substring(0, selectionStart) + e.Text + currentText.Substring(selectionStart);

            if (e.Text == "." || e.Text == ",")
            {
                if (currentText.Contains(".") || currentText.Contains(","))
                {
                    e.Handled = true;
                    return;
                }
                e.Handled = false;
                return;
            }
            else if (!char.IsDigit(e.Text, 0))
            {
                e.Handled = true;
                return;
            }
        }

        public event System.ComponentModel.PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged(string propertyName) => PropertyChanged?.Invoke(this, new System.ComponentModel.PropertyChangedEventArgs(propertyName));
    }
}
