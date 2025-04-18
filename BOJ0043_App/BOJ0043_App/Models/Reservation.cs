using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace BOJ0043_App.Models
{
    /// <summary>
    /// Model reprezentující rezervaci pracovního místa
    /// </summary>
    public class Reservation : INotifyPropertyChanged
    {
        private int _id;
        private int _workspaceId;
        private Workspace? _workspace;
        private string _customerEmail = string.Empty;
        private string _customerName = string.Empty;
        private DateTime _startTime;
        private DateTime _endTime;
        private double _durationHours;
        private decimal _totalPrice;
        private bool _isCompleted;
        private string? _note;
        private DateTime _createdAt = DateTime.Now;


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

        public string CustomerName
        {
            get => _customerName;
            set
            {
                if (_customerName != value)
                {
                    _customerName = value;
                    OnPropertyChanged();
                }
            }
        }

        public string Note
        {
            get => _note;
            set
            {
                if (_note != value)
                {
                    _note = value;
                    OnPropertyChanged();
                }
            }
        }

        public DateTime CreatedAt
        {
            get => _createdAt;
            set
            {
                if (_createdAt != value)
                {
                    _createdAt = value;
                    OnPropertyChanged();
                }
            }
        }

        public double DurationHours
        {
            get => _durationHours;
            set
            {
                if (_durationHours != value)
                {
                    _durationHours = value;
                    OnPropertyChanged();
                }
            }
        }

        public decimal TotalPrice
        {
            get => _totalPrice;
            set
            {
                if (_totalPrice != value)
                {
                    _totalPrice = value;
                    OnPropertyChanged();
                }
            }
        }

        public bool IsCompleted
        {
            get => _isCompleted;
            set
            {
                if (_isCompleted != value)
                {
                    _isCompleted = value;
                    OnPropertyChanged();
                }
            }
        }

        public int IdWorkspace
        {
            get => _workspaceId;
            set
            {
                if (_workspaceId != value)
                {
                    _workspaceId = value;
                    OnPropertyChanged();
                }
            }
        }

        public Workspace? Workspace
        {
            get => _workspace;
            set
            {
                if (_workspace != value)
                {
                    _workspace = value;
                    OnPropertyChanged();
                }
            }
        }

        public string CustomerEmail
        {
            get => _customerEmail;
            set
            {
                if (_customerEmail != value)
                {
                    _customerEmail = value;
                    OnPropertyChanged();
                }
            }
        }

        public DateTime StartTime
        {
            get => _startTime;
            set
            {
                if (_startTime != value)
                {
                    _startTime = value;
                    OnPropertyChanged();
                }
            }
        }

        public DateTime EndTime
        {
            get => _endTime;
            set
            {
                if (_endTime != value)
                {
                    _endTime = value;
                    OnPropertyChanged();
                }
            }
        }



        public int WorkspaceId
        {
            get => _workspaceId;
            set
            {
                if (_workspaceId != value)
                {
                    _workspaceId = value;
                    OnPropertyChanged();
                }
            }
        }

        // Vlastnosti pro zobrazení
        public string FormattedTimeRange => $"{StartTime:dd.MM.yyyy HH:mm} - {EndTime:HH:mm}";
        public TimeSpan Duration => EndTime - StartTime;
        public string FormattedDuration => $"{Duration.TotalHours:F1} hodin";
        public string StatusText => IsCompleted ? "Ukončeno" : "Aktivní";

        public event PropertyChangedEventHandler? PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
