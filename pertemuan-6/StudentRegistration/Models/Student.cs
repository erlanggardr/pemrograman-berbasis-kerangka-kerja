using System.Globalization;
using StudentRegistration.ViewModels;

namespace StudentRegistration.Models
{
    public class Student : ViewModelBase
    {
        public int StudentId { get; set; }

        private string _nim = string.Empty;
        public string NIM
        {
            get => _nim;
            set => SetProperty(ref _nim, value);
        }

        private string _name = string.Empty;
        public string Name
        {
            get => _name;
            set => SetProperty(ref _name, value);
        }

        private int _programId;
        public int ProgramId
        {
            get => _programId;
            set => SetProperty(ref _programId, value);
        }

        private string _programCode = string.Empty;
        public string ProgramCode
        {
            get => _programCode;
            set => SetProperty(ref _programCode, value);
        }

        private string _programName = string.Empty;
        public string ProgramName
        {
            get => _programName;
            set => SetProperty(ref _programName, value);
        }

        private DateTime? _birthDate;
        public DateTime? BirthDate
        {
            get => _birthDate;
            set
            {
                if (SetProperty(ref _birthDate, value))
                {
                    OnPropertyChanged(nameof(BirthDateFormat));
                }
            }
        }

        private string _address = string.Empty;
        public string Address
        {
            get => _address;
            set => SetProperty(ref _address, value);
        }

        private string _phoneNumber = string.Empty;
        public string PhoneNumber
        {
            get => _phoneNumber;
            set => SetProperty(ref _phoneNumber, value);
        }

        private string _email = string.Empty;
        public string Email
        {
            get => _email;
            set => SetProperty(ref _email, value);
        }

        private decimal _gpa = 0.00m;
        public decimal GPA
        {
            get => _gpa;
            set
            {
                if (SetProperty(ref _gpa, value))
                {
                    OnPropertyChanged(nameof(GPAFormat));
                    OnPropertyChanged(nameof(Predikat));
                }
            }
        }

        private bool _isActive = true;
        public bool IsActive
        {
            get => _isActive;
            set
            {
                if (SetProperty(ref _isActive, value))
                {
                    OnPropertyChanged(nameof(StatusDisplay));
                }
            }
        }

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        // Properti Format Pembantu UI
        public string GPAFormat => GPA.ToString("F2", CultureInfo.InvariantCulture);
        public string StatusDisplay => IsActive ? "Aktif" : "Non-Aktif";
        public string BirthDateFormat => BirthDate.HasValue ? BirthDate.Value.ToString("dd/MM/yyyy") : "-";

        public string Predikat
        {
            get
            {
                if (GPA >= 3.51m) return "Cumlaude";
                if (GPA >= 3.00m) return "Sangat Memuaskan";
                if (GPA >= 2.50m) return "Memuaskan";
                return "Cukup";
            }
        }
    }
}
