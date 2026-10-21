using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace StudentRegistrationApp.Models
{
    public class Mahasiswa : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;

        private void OnPropertyChanged([CallerMemberName] string? propName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propName));
        }

        private string _nrp = string.Empty;
        public string NRP
        {
            get => _nrp;
            set { if (_nrp != value) { _nrp = value; OnPropertyChanged(); } }
        }

        private string _nama = string.Empty;
        public string Nama
        {
            get => _nama;
            set { if (_nama != value) { _nama = value; OnPropertyChanged(); } }
        }

        private string _prodi = string.Empty;
        public string Prodi
        {
            get => _prodi;
            set { if (_prodi != value) { _prodi = value; OnPropertyChanged(); } }
        }

        private string _jenisKelamin = string.Empty;
        public string JenisKelamin
        {
            get => _jenisKelamin;
            set { if (_jenisKelamin != value) { _jenisKelamin = value; OnPropertyChanged(); } }
        }

        private DateTime _tanggalLahir;
        public DateTime TanggalLahir
        {
            get => _tanggalLahir;
            set
            {
                if (_tanggalLahir != value)
                {
                    _tanggalLahir = value;
                    OnPropertyChanged();
                    OnPropertyChanged(nameof(TanggalLahirFormat));
                }
            }
        }

        private int _angkatan;
        public int Angkatan
        {
            get => _angkatan;
            set { if (_angkatan != value) { _angkatan = value; OnPropertyChanged(); } }
        }

        private string _jalurMasuk = string.Empty;
        public string JalurMasuk
        {
            get => _jalurMasuk;
            set { if (_jalurMasuk != value) { _jalurMasuk = value; OnPropertyChanged(); } }
        }

        private string _email = string.Empty;
        public string Email
        {
            get => _email;
            set { if (_email != value) { _email = value; OnPropertyChanged(); } }
        }

        private DateTime _tanggalRegistrasi = DateTime.Now;
        public DateTime TanggalRegistrasi
        {
            get => _tanggalRegistrasi;
            set
            {
                if (_tanggalRegistrasi != value)
                {
                    _tanggalRegistrasi = value;
                    OnPropertyChanged();
                    OnPropertyChanged(nameof(TanggalRegistrasiFormat));
                }
            }
        }

        public string TanggalLahirFormat => TanggalLahir.ToString("dd/MM/yyyy");
        public string TanggalRegistrasiFormat => TanggalRegistrasi.ToString("dd/MM/yyyy HH:mm");

        public Mahasiswa() { }

        public Mahasiswa(string nrp, string nama, string prodi, string jenisKelamin, DateTime tanggalLahir, int angkatan, string jalurMasuk, string email)
        {
            NRP = nrp;
            Nama = nama;
            Prodi = prodi;
            JenisKelamin = jenisKelamin;
            TanggalLahir = tanggalLahir;
            Angkatan = angkatan;
            JalurMasuk = jalurMasuk;
            Email = email;
            TanggalRegistrasi = DateTime.Now;
        }
    }
}
