using System.Collections.ObjectModel;
using System.Globalization;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Input;
using StudentManagementSystem.Models;

namespace StudentManagementSystem.ViewModels
{
    public class MainViewModel : ViewModelBase
    {
        private readonly List<Student> _masterStudents = new();

        // Collections
        public ObservableCollection<Student> Students { get; } = new();
        public ObservableCollection<string> ProdiList { get; } = new();
        public ObservableCollection<string> StatusList { get; } = new();
        public ObservableCollection<string> FilterProdiList { get; } = new();
        public ObservableCollection<string> FilterStatusList { get; } = new();
        public ObservableCollection<int> AngkatanList { get; } = new();

        // Selection
        private Student? _selectedStudent;
        public Student? SelectedStudent
        {
            get => _selectedStudent;
            set
            {
                if (SetProperty(ref _selectedStudent, value))
                {
                    OnPropertyChanged(nameof(IsStudentSelected));
                    (EditCommand as RelayCommand)?.RaiseCanExecuteChanged();
                    (DeleteCommand as RelayCommand)?.RaiseCanExecuteChanged();
                    if (value != null && !IsEditMode)
                    {
                        StatusMessage = $"Memilih: {value.Nama} ({value.NRP}) — IPK: {value.IPKFormat}";
                    }
                }
            }
        }

        public bool IsStudentSelected => SelectedStudent != null;

        // Form Fields (Two-Way Data Binding)
        private string _formNRP = string.Empty;
        public string FormNRP
        {
            get => _formNRP;
            set => SetProperty(ref _formNRP, value);
        }

        private string _formNama = string.Empty;
        public string FormNama
        {
            get => _formNama;
            set => SetProperty(ref _formNama, value);
        }

        private string _formProdi = "Teknik Informatika";
        public string FormProdi
        {
            get => _formProdi;
            set => SetProperty(ref _formProdi, value);
        }

        private int _formAngkatan = 2026;
        public int FormAngkatan
        {
            get => _formAngkatan;
            set => SetProperty(ref _formAngkatan, value);
        }

        private string _formIPK = "3.75";
        public string FormIPK
        {
            get => _formIPK;
            set => SetProperty(ref _formIPK, value);
        }

        private string _formStatus = "Aktif";
        public string FormStatus
        {
            get => _formStatus;
            set => SetProperty(ref _formStatus, value);
        }

        private string _formEmail = string.Empty;
        public string FormEmail
        {
            get => _formEmail;
            set => SetProperty(ref _formEmail, value);
        }

        private string _formTelepon = string.Empty;
        public string FormTelepon
        {
            get => _formTelepon;
            set => SetProperty(ref _formTelepon, value);
        }

        private bool _isEditMode = false;
        public bool IsEditMode
        {
            get => _isEditMode;
            set
            {
                if (SetProperty(ref _isEditMode, value))
                {
                    OnPropertyChanged(nameof(FormHeaderTitle));
                    OnPropertyChanged(nameof(SaveButtonText));
                    OnPropertyChanged(nameof(IsNRPReadOnly));
                }
            }
        }

        public string FormHeaderTitle => IsEditMode ? "✏️ Perbarui Data Mahasiswa" : "➕ Tambah Mahasiswa Baru";
        public string SaveButtonText => IsEditMode ? "💾 Simpan Perubahan" : "➕ Tambah ke Sistem";
        public bool IsNRPReadOnly => IsEditMode;

        // Search & Filter Properties
        private string _searchKeyword = string.Empty;
        public string SearchKeyword
        {
            get => _searchKeyword;
            set
            {
                if (SetProperty(ref _searchKeyword, value))
                {
                    ApplyFilter();
                }
            }
        }

        private string _selectedFilterProdi = "Semua Prodi";
        public string SelectedFilterProdi
        {
            get => _selectedFilterProdi;
            set
            {
                if (SetProperty(ref _selectedFilterProdi, value))
                {
                    ApplyFilter();
                }
            }
        }

        private string _selectedFilterStatus = "Semua Status";
        public string SelectedFilterStatus
        {
            get => _selectedFilterStatus;
            set
            {
                if (SetProperty(ref _selectedFilterStatus, value))
                {
                    ApplyFilter();
                }
            }
        }

        // Dashboard KPI Metrics
        private int _totalStudents;
        public int TotalStudents
        {
            get => _totalStudents;
            private set => SetProperty(ref _totalStudents, value);
        }

        private string _averageIPK = "0.00";
        public string AverageIPK
        {
            get => _averageIPK;
            private set => SetProperty(ref _averageIPK, value);
        }

        private int _activeStudents;
        public int ActiveStudents
        {
            get => _activeStudents;
            private set => SetProperty(ref _activeStudents, value);
        }

        private int _cumlaudeStudents;
        public int CumlaudeStudents
        {
            get => _cumlaudeStudents;
            private set => SetProperty(ref _cumlaudeStudents, value);
        }

        // Status bar
        private string _statusMessage = "Sistem Manajemen Data Mahasiswa siap digunakan.";
        public string StatusMessage
        {
            get => _statusMessage;
            set => SetProperty(ref _statusMessage, value);
        }

        // Commands
        public ICommand SaveCommand { get; }
        public ICommand NewCommand { get; }
        public ICommand EditCommand { get; }
        public ICommand DeleteCommand { get; }
        public ICommand ResetFilterCommand { get; }

        public MainViewModel()
        {
            // Initialize Dropdown Options
            var prodis = new[] { "Teknik Informatika", "Sistem Informasi", "Sains Data", "Teknik Komputer", "Teknologi Informasi", "Rekayasa Perangkat Lunak" };
            foreach (var p in prodis) ProdiList.Add(p);

            FilterProdiList.Add("Semua Prodi");
            foreach (var p in prodis) FilterProdiList.Add(p);

            var statuses = new[] { "Aktif", "Cuti", "Lulus" };
            foreach (var s in statuses) StatusList.Add(s);

            FilterStatusList.Add("Semua Status");
            foreach (var s in statuses) FilterStatusList.Add(s);

            for (int y = 2026; y >= 2020; y--) AngkatanList.Add(y);

            // Initialize Commands
            SaveCommand = new RelayCommand(ExecuteSave);
            NewCommand = new RelayCommand(ExecuteNew);
            EditCommand = new RelayCommand(ExecuteEdit, () => SelectedStudent != null);
            DeleteCommand = new RelayCommand(ExecuteDelete, () => SelectedStudent != null);
            ResetFilterCommand = new RelayCommand(ExecuteResetFilter);

            SeedData();
            ApplyFilter();
            UpdateDashboardMetrics();
        }

        private void SeedData()
        {
            _masterStudents.Add(new Student("5025241001", "Muhammad Rizky Pratama", "Teknik Informatika", 2024, 3.88, "Aktif", "rizky.pratama@mhs.its.ac.id", "081234567890"));
            _masterStudents.Add(new Student("5025241025", "Aulia Dewi Safitri", "Sistem Informasi", 2024, 3.74, "Aktif", "aulia.safitri@mhs.its.ac.id", "081234567891"));
            _masterStudents.Add(new Student("5025241054", "Dimas Arya Nugroho", "Sains Data", 2025, 3.65, "Aktif", "dimas.arya@mhs.its.ac.id", "081234567892"));
            _masterStudents.Add(new Student("5025241078", "Vania Kusuma Wardhani", "Teknik Informatika", 2025, 3.92, "Aktif", "vania.wardhani@mhs.its.ac.id", "081234567893"));
            _masterStudents.Add(new Student("5025241099", "Fajar Hidayatullah", "Teknik Komputer", 2023, 3.25, "Cuti", "fajar.hidayat@mhs.its.ac.id", "081234567894"));
            _masterStudents.Add(new Student("5025241112", "Gabriella Natasha", "Teknologi Informasi", 2022, 3.80, "Lulus", "gabriella.n@mhs.its.ac.id", "081234567895"));
        }

        private void ApplyFilter()
        {
            Students.Clear();
            var filtered = _masterStudents.AsEnumerable();

            if (!string.IsNullOrWhiteSpace(SearchKeyword))
            {
                string kw = SearchKeyword.Trim();
                filtered = filtered.Where(s =>
                    s.NRP.Contains(kw, StringComparison.OrdinalIgnoreCase) ||
                    s.Nama.Contains(kw, StringComparison.OrdinalIgnoreCase) ||
                    s.Email.Contains(kw, StringComparison.OrdinalIgnoreCase));
            }

            if (SelectedFilterProdi != "Semua Prodi" && !string.IsNullOrEmpty(SelectedFilterProdi))
            {
                filtered = filtered.Where(s => s.Prodi == SelectedFilterProdi);
            }

            if (SelectedFilterStatus != "Semua Status" && !string.IsNullOrEmpty(SelectedFilterStatus))
            {
                filtered = filtered.Where(s => s.Status == SelectedFilterStatus);
            }

            foreach (var student in filtered)
            {
                Students.Add(student);
            }

            UpdateDashboardMetrics();
        }

        private void UpdateDashboardMetrics()
        {
            TotalStudents = _masterStudents.Count;

            if (_masterStudents.Count > 0)
            {
                double avg = _masterStudents.Average(s => s.IPK);
                AverageIPK = avg.ToString("F2", CultureInfo.InvariantCulture);
            }
            else
            {
                AverageIPK = "0.00";
            }

            ActiveStudents = _masterStudents.Count(s => s.Status == "Aktif");
            CumlaudeStudents = _masterStudents.Count(s => s.IPK >= 3.51);
        }

        private void ExecuteSave()
        {
            string nrp = FormNRP.Trim();
            string nama = FormNama.Trim();
            string email = FormEmail.Trim();
            string telepon = FormTelepon.Trim();

            // Validasi Input
            if (string.IsNullOrWhiteSpace(nrp))
            {
                ShowAlert("NRP tidak boleh kosong!", "Validasi Form");
                return;
            }

            if (!Regex.IsMatch(nrp, @"^\d{9,12}$"))
            {
                ShowAlert("Format NRP tidak valid! NRP harus terdiri dari 9 hingga 12 digit angka.", "Validasi Form");
                return;
            }

            if (string.IsNullOrWhiteSpace(nama))
            {
                ShowAlert("Nama lengkap mahasiswa tidak boleh kosong!", "Validasi Form");
                return;
            }

            string ipkNormalized = FormIPK.Replace(',', '.').Trim();
            if (!double.TryParse(ipkNormalized, NumberStyles.Float, CultureInfo.InvariantCulture, out double ipk) || ipk < 0.00 || ipk > 4.00)
            {
                ShowAlert("IPK harus berupa angka dalam rentang 0.00 hingga 4.00!", "Validasi Form");
                return;
            }

            if (string.IsNullOrWhiteSpace(email) || !Regex.IsMatch(email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
            {
                ShowAlert("Format email tidak valid! Gunakan format email yang benar (contoh: nama@domain.com).", "Validasi Form");
                return;
            }

            if (!IsEditMode)
            {
                // CREATE Operation
                if (_masterStudents.Any(s => s.NRP.Equals(nrp, StringComparison.OrdinalIgnoreCase)))
                {
                    ShowAlert($"Mahasiswa dengan NRP {nrp} sudah terdaftar dalam sistem!", "Duplikasi NRP");
                    return;
                }

                var newStudent = new Student(nrp, nama, FormProdi, FormAngkatan, ipk, FormStatus, email, telepon);
                _masterStudents.Add(newStudent);
                StatusMessage = $"Sukses menambahkan data mahasiswa '{nama}' ({nrp}).";
                MessageBox.Show($"Data mahasiswa '{nama}' ({nrp}) berhasil ditambahkan!", "Sukses Tambah Data", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else
            {
                // UPDATE Operation
                var existing = _masterStudents.FirstOrDefault(s => s.NRP.Equals(nrp, StringComparison.OrdinalIgnoreCase));
                if (existing == null)
                {
                    ShowAlert("Data mahasiswa yang ingin diubah tidak ditemukan!", "Error");
                    return;
                }

                existing.Nama = nama;
                existing.Prodi = FormProdi;
                existing.Angkatan = FormAngkatan;
                existing.IPK = ipk;
                existing.Status = FormStatus;
                existing.Email = email;
                existing.Telepon = telepon;

                StatusMessage = $"Sukses memperbarui data mahasiswa '{nama}' ({nrp}).";
                MessageBox.Show($"Data mahasiswa '{nama}' ({nrp}) berhasil diperbarui!", "Sukses Update Data", MessageBoxButton.OK, MessageBoxImage.Information);
            }

            ExecuteNew();
            ApplyFilter();
        }

        private void ExecuteEdit()
        {
            if (SelectedStudent == null)
            {
                ShowAlert("Pilih baris mahasiswa pada tabel terlebih dahulu untuk mengedit data.", "Peringatan");
                return;
            }

            FormNRP = SelectedStudent.NRP;
            FormNama = SelectedStudent.Nama;
            FormProdi = SelectedStudent.Prodi;
            FormAngkatan = SelectedStudent.Angkatan;
            FormIPK = SelectedStudent.IPKFormat;
            FormStatus = SelectedStudent.Status;
            FormEmail = SelectedStudent.Email;
            FormTelepon = SelectedStudent.Telepon;

            IsEditMode = true;
            StatusMessage = $"Sedang mengedit data mahasiswa: {SelectedStudent.Nama} ({SelectedStudent.NRP}).";
        }

        private void ExecuteDelete()
        {
            if (SelectedStudent == null)
            {
                ShowAlert("Pilih baris mahasiswa pada tabel terlebih dahulu untuk menghapus data.", "Peringatan");
                return;
            }

            var confirm = MessageBox.Show(
                $"Apakah Anda yakin ingin menghapus data mahasiswa:\n\nNRP : {SelectedStudent.NRP}\nNama: {SelectedStudent.Nama}\nProdi: {SelectedStudent.Prodi}\n\nTindakan ini tidak dapat dibatalkan.",
                "Konfirmasi Hapus Data (MVVM)",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question
            );

            if (confirm == MessageBoxResult.Yes)
            {
                string namaDihapus = SelectedStudent.Nama;
                string nrpDihapus = SelectedStudent.NRP;

                _masterStudents.Remove(SelectedStudent);
                SelectedStudent = null;

                if (IsEditMode && FormNRP == nrpDihapus)
                {
                    ExecuteNew();
                }

                ApplyFilter();
                StatusMessage = $"Data mahasiswa {namaDihapus} ({nrpDihapus}) berhasil dihapus.";
            }
        }

        private void ExecuteNew()
        {
            FormNRP = string.Empty;
            FormNama = string.Empty;
            FormProdi = "Teknik Informatika";
            FormAngkatan = 2026;
            FormIPK = "3.75";
            FormStatus = "Aktif";
            FormEmail = string.Empty;
            FormTelepon = string.Empty;
            IsEditMode = false;
            StatusMessage = "Formulir siap untuk penambahan mahasiswa baru.";
        }

        private void ExecuteResetFilter()
        {
            SearchKeyword = string.Empty;
            SelectedFilterProdi = "Semua Prodi";
            SelectedFilterStatus = "Semua Status";
            StatusMessage = "Filter pencarian berhasil direset.";
        }

        private static void ShowAlert(string msg, string title)
        {
            MessageBox.Show(msg, title, MessageBoxButton.OK, MessageBoxImage.Warning);
        }
    }
}
