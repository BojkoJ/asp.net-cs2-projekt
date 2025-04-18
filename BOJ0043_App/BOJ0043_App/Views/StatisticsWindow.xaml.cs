using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;
using System.Collections.Generic;
using System.Threading.Tasks;
using BOJ0043_App.Services;
using BOJ0043_App.Commands;

namespace BOJ0043_App.Views
{
    public partial class StatisticsWindow : Window, INotifyPropertyChanged
    {
        private DateTime _startDate = DateTime.Now.AddMonths(-1);
        private DateTime _endDate = DateTime.Now;
        private ObservableCollection<StatisticItem> _statistics = new();
        private readonly ReservationService _reservationService = new();
        public ICommand LoadStatisticsCommand { get; }

        public DateTime StartDate
        {
            get => _startDate;
            set { _startDate = value; OnPropertyChanged(); }
        }
        public DateTime EndDate
        {
            get => _endDate;
            set { _endDate = value; OnPropertyChanged(); }
        }
        public ObservableCollection<StatisticItem> Statistics
        {
            get => _statistics;
            set { _statistics = value; OnPropertyChanged(); }
        }

        public StatisticsWindow()
        {
            InitializeComponent();
            LoadStatisticsCommand = new RelayCommand(async _ => await LoadStatisticsAsync());
            DataContext = this;
        }

        public async Task LoadStatisticsAsync()
        {
            Statistics.Clear();
            var stats = await _reservationService.GetStatisticsAsync(StartDate, EndDate);
            if (stats != null)
            {
                foreach (var item in stats)
                    Statistics.Add(item);
            }
        }

        public async void ShowStatisticsButton_Click(object sender, RoutedEventArgs e)
        {
            await LoadStatisticsAsync();
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
            Close();
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string? name = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }

    public class StatisticItem
    {
        public string Name { get; set; } = string.Empty;
        public int Count { get; set; }
    }
}
