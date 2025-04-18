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
        
        public CoworkingSpaceEditWindow(CoworkingSpace? coworkingSpace = null)
        {
            InitializeComponent();
            
            _coworkingSpaceService = new CoworkingSpaceService();
            
            // Určíme, zda jde o vytvoření nového nebo úpravu existujícího
            _isNew = coworkingSpace == null;
            
            // Vytvoříme nový objekt, pokud nebyl předán existující
            CoworkingSpace = coworkingSpace ?? new CoworkingSpace();
            
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
    }
}
