using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace BOJ0043_App.Models
{
    // Model reprezentující pracovní místo v coworkingovém prostoru
    public class Workspace : INotifyPropertyChanged
    {
        private int _id;
        private string _name = string.Empty;
        private string _description = string.Empty;
        private decimal _pricePerHour;
        private int _coworkingSpaceId;
        private CoworkingSpace? _coworkingSpace;
        private int _currentStatus = 0; // Available, Occupied, Maintenance
        private ObservableCollection<Reservation> _reservations = new ObservableCollection<Reservation>();

        public int Id
        {
            get => _id;
            set
            {
                if (_id != value)
                {
                    _id = value;
                    OnPropertyChanged();
                }
            }
        }

        public string Name
        {
            get => _name;
            set
            {
                if (_name != value)
                {
                    _name = value;
                    OnPropertyChanged();
                }
            }
        }

        public string Description
        {
            get => _description;
            set
            {
                if (_description != value)
                {
                    _description = value;
                    OnPropertyChanged();
                }
            }
        }

        public decimal PricePerHour
        {
            get => _pricePerHour;
            set
            {
                if (_pricePerHour != value)
                {
                    _pricePerHour = value;
                    OnPropertyChanged();
                }
            }
        }

        public int CoworkingSpaceId
        {
            get => _coworkingSpaceId;
            set
            {
                if (_coworkingSpaceId != value)
                {
                    _coworkingSpaceId = value;
                    OnPropertyChanged();
                }
            }
        }

        public CoworkingSpace? CoworkingSpace
        {
            get => _coworkingSpace;
            set
            {
                if (_coworkingSpace != value)
                {
                    _coworkingSpace = value;
                    OnPropertyChanged();
                }
            }
        }

        public int CurrentStatus { get; set; } // 0 = Dostupné, 1 = Obsazené, 2 = V údržbě

        public string CurrentStatusText
        {
            get
            {
                return CurrentStatus switch
                {
                    0 => "Dostupné",
                    1 => "Obsazené",
                    2 => "V údržbě",
                    _ => "Neznámý"
                };
            }
        }

        public ObservableCollection<Reservation> Reservations
        {
            get => _reservations;
            set
            {
                if (_reservations != value)
                {
                    _reservations = value;
                    OnPropertyChanged();
                }
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
