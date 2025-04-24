using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace BOJ0043_App.Models
{
    // Model reprezentující coworkingový prostor
    public class CoworkingSpace : INotifyPropertyChanged
    {
        private int _id;
        private string _name = string.Empty;
        private string _address = string.Empty;
        private string _description = string.Empty;
        private decimal _latitude;
        private decimal _longitude;
        private string? _phoneNumber;
        private string? _email;
        private string? _website;
        private ObservableCollection<Workspace> _workspaces = new ObservableCollection<Workspace>();

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

        public string Address 
        { 
            get => _address; 
            set
            {
                if (_address != value)
                {
                    _address = value;
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

        public decimal Latitude 
        { 
            get => _latitude; 
            set
            {
                if (_latitude != value)
                {
                    _latitude = value;
                    OnPropertyChanged();
                }
            }
        }

        public decimal Longitude 
        { 
            get => _longitude; 
            set
            {
                if (_longitude != value)
                {
                    _longitude = value;
                    OnPropertyChanged();
                }
            }
        }

        public string? PhoneNumber 
        { 
            get => _phoneNumber; 
            set
            {
                if (_phoneNumber != value)
                {
                    _phoneNumber = value;
                    OnPropertyChanged();
                }
            }
        }

        public string? Email
        {
            get => _email;
            set
            {
                if (_email != value)
                {
                    _email = value;
                    OnPropertyChanged();
                }
            }
        }

        public string? Website
        {
            get => _website;
            set
            {
                if (_website != value)
                {
                    _website = value;
                    OnPropertyChanged();
                }
            }
        }

        public ObservableCollection<Workspace> Workspaces 
        { 
            get => _workspaces ?? (_workspaces = new ObservableCollection<Workspace>()); 
            set
            {
                if (_workspaces != value)
                {
                    _workspaces = value ?? new ObservableCollection<Workspace>();
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
