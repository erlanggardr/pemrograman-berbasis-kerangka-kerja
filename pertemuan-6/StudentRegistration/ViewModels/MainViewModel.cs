using System.Collections.ObjectModel;
using System.Globalization;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Input;
using StudentRegistration.Config;
using StudentRegistration.Models;
using StudentRegistration.Repositories;

namespace StudentRegistration.ViewModels
{
    public class MainViewModel : ViewModelBase
    {
        private readonly IStudentRepository _repository;

        // -------------------------------------------------------------
        // Status Koneksi Database
        // -------------------------------------------------------------
        private bool _isConnected = false;
        public bool IsConnected
        {
            get => _isConnected;
            set
            {
                if (SetProperty(ref _isConnected, value))
                {
                    OnPropertyChanged(nameof(ConnectionBadgeBackground));
                    OnPropertyChanged(nameof(ConnectionBadgeForeground));
                }
            }
        }

        private string _connectionStatusText = "Mengecek koneksi database...";
        public string ConnectionStatusText
        {
            get => _connectionStatusText;
            set => SetProperty(ref _connectionStatusText, value);
        }

        public string ConnectionBadgeBackground => IsConnected ? "#ECFDF5" : "#FEF2F2";
        public string ConnectionBadgeForeground => IsConnected ? "#059669" : "#DC2626";

        private string _serverName = DatabaseConfig.ServerName;
        public string ServerName
        {
            get => _serverName;
            set => SetProperty(ref _serverName, value);
        }

        // -------------------------------------------------------------
        // Koleksi Data
        // -------------------------------------------------------------
        public ObservableCollection<Student> Students { get; } = new();
        public ObservableCollection<ProgramStudy> Programs { get; } = new();
        public ObservableCollection<ProgramStudy> FilterPrograms { get; } = new();
        public ObservableCollection<string> FilterStatusList { get; } = new();

        // -------------------------------------------------------------
        // Search & Filter Properties
        // -------------------------------------------------------------
        private string _searchKeyword = string.Empty;
        public string SearchKeyword
        {
            get => _searchKeyword;
            set
            {
                if (SetProperty(ref _searchKeyword, value))
                {
                    LoadStudentsFromDatabase();
                }
            }
        }

        private ProgramStudy? _selectedFilterProgram;
        public ProgramStudy? SelectedFilterProgram
        {
            get => _selectedFilterProgram;
            set
            {
                if (SetProperty(ref _selectedFilterProgram, value))
                {
                    LoadStudentsFromDatabase();
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
                    LoadStudentsFromDatabase();
                }
            }
        }

        // -------------------------------------------------------------
        // Selection
        // -------------------------------------------------------------
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
                        StatusMessage = $"Memilih: {value.Name} ({value.NIM}) • {value.ProgramName}";
                    }
                }
            }
        }

        public bool IsStudentSelected => SelectedStudent != null;

        // -------------------------------------------------------------
        // Form Fields (Two-Way Binding)
        // -------------------------------------------------------------
        private int _formStudentId;
        public int FormStudentId
        {
            get => _formStudentId;
            set => SetProperty(ref _formStudentId, value);
        }

        private string _formNIM = string.Empty;
        public string FormNIM
        {
            get => _formNIM;
            set => SetProperty(ref _formNIM, value);
        }

        private string _formName = string.Empty;
        public string FormName
        {
            get => _formName;
            set => SetProperty(ref _formName, value);
        }

        private ProgramStudy? _selectedFormProgram;
        public ProgramStudy? SelectedFormProgram
        {
            get => _selectedFormProgram;
            set => SetProperty(ref _selectedFormProgram, value);
        }

        private DateTime? _formBirthDate = new DateTime(2005, 1, 1);
        public DateTime? FormBirthDate
        {
            get => _formBirthDate;
            set => SetProperty(ref _formBirthDate, value);
        }

        private string _formAddress = string.Empty;
        public string FormAddress
        {
            get => _formAddress;
            set => SetProperty(ref _formAddress, value);
        }

        private string _formPhoneNumber = string.Empty;
        public string FormPhoneNumber
        {
            get => _formPhoneNumber;
            set => SetProperty(ref _formPhoneNumber, value);
        }

        private string _formEmail = string.Empty;
        public string FormEmail
        {
            get => _formEmail;
            set => SetProperty(ref _formEmail, value);
        }

        private string _formGPA = "3.75";
        public string FormGPA
        {
            get => _formGPA;
            set => SetProperty(ref _formGPA, value);
        }

        private bool _formIsActive = true;
        public bool FormIsActive
        {
            get => _formIsActive;
            set => SetProperty(ref _formIsActive, value);
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
                    OnPropertyChanged(nameof(IsNIMReadOnly));
                }
            }
        }

        public string FormHeaderTitle => IsEditMode ? "✏️ Perbarui Data Mahasiswa" : "➕ Registrasi Mahasiswa Baru";
        public string SaveButtonText => IsEditMode ? "💾 Simpan Perubahan (SQL)" : "➕ Simpan ke Database (SQL)";
        public bool IsNIMReadOnly => IsEditMode;

        // -------------------------------------------------------------
        // Metric Cards Properties
        // -------------------------------------------------------------
        private int _totalStudents;
        public int TotalStudents
        {
            get => _totalStudents;
            private set => SetProperty(ref _totalStudents, value);
        }

        private string _averageGPA = "0.00";
        public string AverageGPA
        {
            get => _averageGPA;
            private set => SetProperty(ref _averageGPA, value);
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

        // -------------------------------------------------------------
        // Status Bar
        // -------------------------------------------------------------
        private string _statusMessage = "Sistem Registrasi Mahasiswa (ADO.NET) siap digunakan.";
        public string StatusMessage
        {
            get => _statusMessage;
            set => SetProperty(ref _statusMessage, value);
        }

        private string _databaseInfo = "SQL Server LocalDB • StudentRegistrationDB";
        public string DatabaseInfo
        {
            get => _databaseInfo;
            set => SetProperty(ref _databaseInfo, value);
        }

        // -------------------------------------------------------------
        // Commands
        // -------------------------------------------------------------
        public ICommand RefreshCommand { get; }
        public ICommand SaveCommand { get; }
        public ICommand NewCommand { get; }
        public ICommand EditCommand { get; }
        public ICommand DeleteCommand { get; }
        public ICommand ResetFilterCommand { get; }
        public ICommand TestConnectionCommand { get; }
        public ICommand ProvisionDatabaseCommand { get; }

        // -------------------------------------------------------------
        // Constructor
        // -------------------------------------------------------------
        public MainViewModel(IStudentRepository? repository = null)
        {
            _repository = repository ?? new StudentRepository();

            FilterStatusList.Add("Semua Status");
            FilterStatusList.Add("Aktif");
            FilterStatusList.Add("Non-Aktif");

            // Commands Initialization
            RefreshCommand = new RelayCommand(ExecuteRefresh);
            SaveCommand = new RelayCommand(ExecuteSave);
            NewCommand = new RelayCommand(ExecuteNew);
            EditCommand = new RelayCommand(ExecuteEdit, () => SelectedStudent != null);
            DeleteCommand = new RelayCommand(ExecuteDelete, () => SelectedStudent != null);
            ResetFilterCommand = new RelayCommand(ExecuteResetFilter);
            TestConnectionCommand = new RelayCommand(ExecuteTestConnection);
            ProvisionDatabaseCommand = new RelayCommand(ExecuteProvisionDatabase);

            InitializeSystem();
        }

        private void InitializeSystem()
        {
            // 1. Cek koneksi ke SQL Server
            if (_repository.CheckConnection(out string serverInfo, out string err))
            {
                IsConnected = true;
                ConnectionStatusText = "🟢 Terhubung ke SQL Server LocalDB";
                DatabaseInfo = serverInfo;
            }
            else
            {
                // Coba auto-provision database jika belum ada
                if (_repository.ProvisionDatabase(out string provMsg))
                {
                    IsConnected = true;
                    ConnectionStatusText = "🟢 Terhubung (Auto-Provisioned)";
                    DatabaseInfo = $"SQL Server LocalDB • {provMsg}";
                }
                else
                {
                    IsConnected = false;
                    ConnectionStatusText = "🔴 Terputus dari SQL Server";
                    DatabaseInfo = $"Error: {err}";
                    StatusMessage = $"Peringatan koneksi: {err}";
                }
            }

            // 2. Muat Master Program Studi
            LoadPrograms();

            // 3. Muat Data Mahasiswa
            LoadStudentsFromDatabase();
        }

        private void LoadPrograms()
        {
            Programs.Clear();
            FilterPrograms.Clear();

            // Item default untuk Filter
            var allProdiOption = new ProgramStudy
            {
                ProgramId = 0,
                ProgramCode = "ALL",
                ProgramName = "Semua Program Studi"
            };
            FilterPrograms.Add(allProdiOption);
            SelectedFilterProgram = allProdiOption;

            var list = _repository.GetAllPrograms();
            foreach (var p in list)
            {
                Programs.Add(p);
                FilterPrograms.Add(p);
            }

            if (Programs.Count > 0)
            {
                SelectedFormProgram = Programs.FirstOrDefault();
            }
        }

        private void LoadStudentsFromDatabase()
        {
            Students.Clear();

            int? filterProgId = (SelectedFilterProgram == null || SelectedFilterProgram.ProgramId == 0)
                ? null
                : SelectedFilterProgram.ProgramId;

            bool? filterActive = SelectedFilterStatus switch
            {
                "Aktif" => true,
                "Non-Aktif" => false,
                _ => null
            };

            var list = _repository.GetAll(SearchKeyword, filterProgId, filterActive);
            foreach (var student in list)
            {
                Students.Add(student);
            }

            UpdateMetrics(list);
            StatusMessage = $"Berhasil memuat {list.Count} data mahasiswa dari SQL Server.";
        }

        private void UpdateMetrics(List<Student> currentStudents)
        {
            TotalStudents = currentStudents.Count;

            if (currentStudents.Count > 0)
            {
                decimal avg = currentStudents.Average(s => s.GPA);
                AverageGPA = avg.ToString("F2", CultureInfo.InvariantCulture);
            }
            else
            {
                AverageGPA = "0.00";
            }

            ActiveStudents = currentStudents.Count(s => s.IsActive);
            CumlaudeStudents = currentStudents.Count(s => s.GPA >= 3.51m);
        }

        private void ExecuteSave()
        {
            string nim = FormNIM.Trim();
            string name = FormName.Trim();
            string email = FormEmail.Trim();
            string phone = FormPhoneNumber.Trim();
            string address = FormAddress.Trim();

            // Validasi Input
            if (string.IsNullOrWhiteSpace(nim))
            {
                ShowAlert("NIM / NRP tidak boleh kosong!", "Validasi Form");
                return;
            }

            if (!Regex.IsMatch(nim, @"^\d{9,12}$"))
            {
                ShowAlert("Format NIM tidak valid! NIM harus terdiri dari 9 hingga 12 digit angka.", "Validasi Form");
                return;
            }

            if (string.IsNullOrWhiteSpace(name))
            {
                ShowAlert("Nama lengkap mahasiswa tidak boleh kosong!", "Validasi Form");
                return;
            }

            if (SelectedFormProgram == null || SelectedFormProgram.ProgramId <= 0)
            {
                ShowAlert("Silakan pilih Program Studi yang valid!", "Validasi Form");
                return;
            }

            string gpaNormalized = FormGPA.Replace(',', '.').Trim();
            if (!decimal.TryParse(gpaNormalized, NumberStyles.Float, CultureInfo.InvariantCulture, out decimal gpa) || gpa < 0.00m || gpa > 4.00m)
            {
                ShowAlert("IPK/GPA harus berupa angka desimal dalam rentang 0.00 hingga 4.00!", "Validasi Form");
                return;
            }

            if (!string.IsNullOrWhiteSpace(email) && !Regex.IsMatch(email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
            {
                ShowAlert("Format email tidak valid! Gunakan format email yang benar (contoh: mhs@its.ac.id).", "Validasi Form");
                return;
            }

            if (!IsEditMode)
            {
                // INSERT OPERATION (ADO.NET)
                var newStudent = new Student
                {
                    NIM = nim,
                    Name = name,
                    ProgramId = SelectedFormProgram.ProgramId,
                    BirthDate = FormBirthDate,
                    Address = address,
                    PhoneNumber = phone,
                    Email = email,
                    GPA = gpa,
                    IsActive = FormIsActive
                };

                if (_repository.Insert(newStudent, out string errMsg))
                {
                    StatusMessage = $"Sukses menambahkan data mahasiswa '{name}' ({nim}) ke SQL Server.";
                    MessageBox.Show($"Data mahasiswa '{name}' ({nim}) berhasil disimpan ke database SQL Server!",
                        "Sukses Insert Data (ADO.NET)", MessageBoxButton.OK, MessageBoxImage.Information);
                    ExecuteNew();
                    LoadStudentsFromDatabase();
                }
                else
                {
                    ShowAlert(errMsg, "Gagal Insert Database");
                }
            }
            else
            {
                // UPDATE OPERATION (ADO.NET)
                var updatedStudent = new Student
                {
                    StudentId = FormStudentId,
                    NIM = nim,
                    Name = name,
                    ProgramId = SelectedFormProgram.ProgramId,
                    BirthDate = FormBirthDate,
                    Address = address,
                    PhoneNumber = phone,
                    Email = email,
                    GPA = gpa,
                    IsActive = FormIsActive
                };

                if (_repository.Update(updatedStudent, out string errMsg))
                {
                    StatusMessage = $"Sukses memperbarui data mahasiswa '{name}' ({nim}) di SQL Server.";
                    MessageBox.Show($"Data mahasiswa '{name}' ({nim}) berhasil diperbarui di database SQL Server!",
                        "Sukses Update Data (ADO.NET)", MessageBoxButton.OK, MessageBoxImage.Information);
                    ExecuteNew();
                    LoadStudentsFromDatabase();
                }
                else
                {
                    ShowAlert(errMsg, "Gagal Update Database");
                }
            }
        }

        private void ExecuteEdit()
        {
            if (SelectedStudent == null)
            {
                ShowAlert("Pilih baris mahasiswa pada tabel terlebih dahulu untuk mengedit data.", "Peringatan");
                return;
            }

            FormStudentId = SelectedStudent.StudentId;
            FormNIM = SelectedStudent.NIM;
            FormName = SelectedStudent.Name;
            FormBirthDate = SelectedStudent.BirthDate;
            FormAddress = SelectedStudent.Address;
            FormPhoneNumber = SelectedStudent.PhoneNumber;
            FormEmail = SelectedStudent.Email;
            FormGPA = SelectedStudent.GPAFormat;
            FormIsActive = SelectedStudent.IsActive;

            // Pilih program studi yang cocok di combobox form
            SelectedFormProgram = Programs.FirstOrDefault(p => p.ProgramId == SelectedStudent.ProgramId) ?? Programs.FirstOrDefault();

            IsEditMode = true;
            StatusMessage = $"Mode Edit: Memuat data {SelectedStudent.Name} ({SelectedStudent.NIM}) ke formulir.";
        }

        private void ExecuteDelete()
        {
            if (SelectedStudent == null)
            {
                ShowAlert("Pilih baris mahasiswa pada tabel terlebih dahulu untuk menghapus data.", "Peringatan");
                return;
            }

            var confirm = MessageBox.Show(
                $"Apakah Anda yakin ingin menghapus data mahasiswa berikut dari SQL Server?\n\n" +
                $"NIM         : {SelectedStudent.NIM}\n" +
                $"Nama        : {SelectedStudent.Name}\n" +
                $"Prodi       : {SelectedStudent.ProgramName}\n\n" +
                $"Tindakan ini akan mengeksekusi perintah DELETE pada database secara permanen.",
                "Konfirmasi Hapus Data (SQL Server)",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question
            );

            if (confirm == MessageBoxResult.Yes)
            {
                int idToDelete = SelectedStudent.StudentId;
                string name = SelectedStudent.Name;
                string nim = SelectedStudent.NIM;

                if (_repository.Delete(idToDelete, out string errMsg))
                {
                    if (IsEditMode && FormStudentId == idToDelete)
                    {
                        ExecuteNew();
                    }

                    LoadStudentsFromDatabase();
                    StatusMessage = $"Data mahasiswa {name} ({nim}) berhasil dihapus dari SQL Server.";
                    MessageBox.Show($"Data mahasiswa '{name}' ({nim}) berhasil dihapus dari database!",
                        "Sukses Hapus Data", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else
                {
                    ShowAlert(errMsg, "Gagal Hapus Database");
                }
            }
        }

        private void ExecuteNew()
        {
            FormStudentId = 0;
            FormNIM = string.Empty;
            FormName = string.Empty;
            FormBirthDate = new DateTime(2005, 1, 1);
            FormAddress = string.Empty;
            FormPhoneNumber = string.Empty;
            FormEmail = string.Empty;
            FormGPA = "3.75";
            FormIsActive = true;
            SelectedFormProgram = Programs.FirstOrDefault();

            IsEditMode = false;
            StatusMessage = "Formulir siap untuk penambahan data mahasiswa baru.";
        }

        private void ExecuteRefresh()
        {
            LoadPrograms();
            LoadStudentsFromDatabase();
            StatusMessage = "Data mahasiswa dan program studi berhasil di-refresh dari SQL Server.";
        }

        private void ExecuteResetFilter()
        {
            SearchKeyword = string.Empty;
            SelectedFilterProgram = FilterPrograms.FirstOrDefault();
            SelectedFilterStatus = "Semua Status";
            StatusMessage = "Filter pencarian berhasil direset.";
        }

        private void ExecuteTestConnection()
        {
            if (_repository.CheckConnection(out string serverInfo, out string err))
            {
                IsConnected = true;
                ConnectionStatusText = "🟢 Terhubung ke SQL Server LocalDB";
                DatabaseInfo = serverInfo;
                MessageBox.Show($"Koneksi ke basis data berhasil diverifikasi!\n\n{serverInfo}",
                    "Koneksi Database Sukses", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else
            {
                IsConnected = false;
                ConnectionStatusText = "🔴 Terputus dari SQL Server";
                DatabaseInfo = $"Error: {err}";
                MessageBox.Show($"Gagal terhubung ke SQL Server:\n\n{err}\n\nPastikan instance SQL Server / LocalDB berjalan.",
                    "Koneksi Database Gagal", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ExecuteProvisionDatabase()
        {
            var confirm = MessageBox.Show(
                "Apakah Anda ingin menginisialisasi ulang database 'StudentRegistrationDB'?\n\n" +
                "Perintah ini akan membuat database, tabel Programs, tabel Students, serta menyisipkan seed data awal.",
                "Inisialisasi Database",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);

            if (confirm == MessageBoxResult.Yes)
            {
                if (_repository.ProvisionDatabase(out string msg))
                {
                    ExecuteRefresh();
                    MessageBox.Show(msg, "Sukses Inisialisasi", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else
                {
                    ShowAlert(msg, "Gagal Inisialisasi");
                }
            }
        }

        private static void ShowAlert(string msg, string title)
        {
            MessageBox.Show(msg, title, MessageBoxButton.OK, MessageBoxImage.Warning);
        }
    }
}
