using StudentManagementSystem.ViewModels;

namespace StudentManagementSystem.Models
{
    public class Student : ViewModelBase
    {
        private string _nrp = string.Empty;
        public string NRP
        {
            get => _nrp;
            set => SetProperty(ref _nrp, value);
        }

        private string _nama = string.Empty;
        public string Nama
        {
            get => _nama;
            set => SetProperty(ref _nama, value);
        }

        private string _prodi = string.Empty;
        public string Prodi
        {
            get => _prodi;
            set => SetProperty(ref _prodi, value);
        }

        private int _angkatan = 2026;
        public int Angkatan
        {
            get => _angkatan;
            set => SetProperty(ref _angkatan, value);
        }

        private double _ipk = 0.0;
        public double IPK
        {
            get => _ipk;
            set
            {
                if (SetProperty(ref _ipk, value))
                {
                    OnPropertyChanged(nameof(Predikat));
                    OnPropertyChanged(nameof(IPKFormat));
                }
            }
        }

        private string _status = "Aktif";
        public string Status
        {
            get => _status;
            set => SetProperty(ref _status, value);
        }

        private string _email = string.Empty;
        public string Email
        {
            get => _email;
            set => SetProperty(ref _email, value);
        }

        private string _telepon = string.Empty;
        public string Telepon
        {
            get => _telepon;
            set => SetProperty(ref _telepon, value);
        }

        public string Predikat
        {
            get
            {
                if (IPK >= 3.51) return "Cumlaude";
                if (IPK >= 3.00) return "Sangat Memuaskan";
                if (IPK >= 2.50) return "Memuaskan";
                return "Cukup";
            }
        }

        public string IPKFormat => IPK.ToString("F2", System.Globalization.CultureInfo.InvariantCulture);

        public Student() { }

        public Student(string nrp, string nama, string prodi, int angkatan, double ipk, string status, string email, string telepon)
        {
            _nrp = nrp;
            _nama = nama;
            _prodi = prodi;
            _angkatan = angkatan;
            _ipk = ipk;
            _status = status;
            _email = email;
            _telepon = telepon;
        }

        public Student Clone()
        {
            return new Student(NRP, Nama, Prodi, Angkatan, IPK, Status, Email, Telepon);
        }
    }
}
