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

        // -------------------------------------------------------------
        // View Navigation State
        // -------------------------------------------------------------
        private string _currentView = "Dashboard";
        public string CurrentView
        {
            get => _currentView;
            set
            {
                if (SetProperty(ref _currentView, value))
                {
                    OnPropertyChanged(nameof(CurrentPageTitle));
                    OnPropertyChanged(nameof(CurrentPageSubtitle));

                    OnPropertyChanged(nameof(IsDashboardActive));
                    OnPropertyChanged(nameof(IsDataMahasiswaActive));
                    OnPropertyChanged(nameof(IsStatistikActive));
                    OnPropertyChanged(nameof(IsProgramStudiActive));

                    OnPropertyChanged(nameof(DashboardVisibility));
                    OnPropertyChanged(nameof(DataMahasiswaVisibility));
                    OnPropertyChanged(nameof(StatistikVisibility));
                    OnPropertyChanged(nameof(ProgramStudiVisibility));

                    OnPropertyChanged(nameof(DashboardNavBackground));
                    OnPropertyChanged(nameof(DataMahasiswaNavBackground));
                    OnPropertyChanged(nameof(StatistikNavBackground));
                    OnPropertyChanged(nameof(ProgramStudiNavBackground));

                    OnPropertyChanged(nameof(DashboardNavForeground));
                    OnPropertyChanged(nameof(DataMahasiswaNavForeground));
                    OnPropertyChanged(nameof(StatistikNavForeground));
                    OnPropertyChanged(nameof(ProgramStudiNavForeground));

                    StatusMessage = $"Membuka tampilan: {CurrentPageTitle}";
                }
            }
        }

        public string CurrentPageTitle => CurrentView switch
        {
            "DataMahasiswa" => "Manajemen Data Mahasiswa",
            "Statistik"     => "Statistik & Laporan Akademik",
            "ProgramStudi"  => "Departemen & Program Studi",
            _               => "Dashboard Utama"
        };

        public string CurrentPageSubtitle => CurrentView switch
        {
            "DataMahasiswa" => "Pengelolaan data akademik mahasiswa, pencarian terpadu, dan operasi CRUD.",
            "Statistik"     => "Analisis distribusi nilai IPK, status kelulusan, dan rekapitulasi performa per program studi.",
            "ProgramStudi"  => "Monitoring distribusi mahasiswa dan evaluasi rata-rata nilai per program studi.",
            _               => "Ringkasan performa akademik, metrik mahasiswa, dan aktivitas sistem."
        };

        public bool IsDashboardActive     => CurrentView == "Dashboard";
        public bool IsDataMahasiswaActive => CurrentView == "DataMahasiswa";
        public bool IsStatistikActive     => CurrentView == "Statistik";
        public bool IsProgramStudiActive  => CurrentView == "ProgramStudi";

        public Visibility DashboardVisibility     => CurrentView == "Dashboard" ? Visibility.Visible : Visibility.Collapsed;
        public Visibility DataMahasiswaVisibility => CurrentView == "DataMahasiswa" ? Visibility.Visible : Visibility.Collapsed;
        public Visibility StatistikVisibility     => CurrentView == "Statistik" ? Visibility.Visible : Visibility.Collapsed;
        public Visibility ProgramStudiVisibility  => CurrentView == "ProgramStudi" ? Visibility.Visible : Visibility.Collapsed;

        public string DashboardNavBackground     => IsDashboardActive ? "#2563EB" : "Transparent";
        public string DataMahasiswaNavBackground => IsDataMahasiswaActive ? "#2563EB" : "Transparent";
        public string StatistikNavBackground     => IsStatistikActive ? "#2563EB" : "Transparent";
        public string ProgramStudiNavBackground  => IsProgramStudiActive ? "#2563EB" : "Transparent";

        public string DashboardNavForeground     => IsDashboardActive ? "White" : "#94A3B8";
        public string DataMahasiswaNavForeground => IsDataMahasiswaActive ? "White" : "#94A3B8";
        public string StatistikNavForeground     => IsStatistikActive ? "White" : "#94A3B8";
        public string ProgramStudiNavForeground  => IsProgramStudiActive ? "White" : "#94A3B8";

        // -------------------------------------------------------------
        // Data Collections
        // -------------------------------------------------------------
        public ObservableCollection<Student> Students { get; } = new();
        public ObservableCollection<string> ProdiList { get; } = new();
        public ObservableCollection<string> StatusList { get; } = new();
        public ObservableCollection<string> FilterProdiList { get; } = new();
        public ObservableCollection<string> FilterStatusList { get; } = new();
        public ObservableCollection<int> AngkatanList { get; } = new();

        // Statistics & Overview Collections
        public ObservableCollection<Student> TopStudents { get; } = new();
        public ObservableCollection<PredikatStat> PredikatStats { get; } = new();
        public ObservableCollection<StatusStat> StatusStats { get; } = new();
        public ObservableCollection<ProdiStat> ProdiStats { get; } = new();
        public ObservableCollection<Student> SelectedProdiStudents { get; } = new();

        private ProdiStat? _selectedProdi;
        public ProdiStat? SelectedProdi
        {
            get => _selectedProdi;
            set
            {
                if (SetProperty(ref _selectedProdi, value))
                {
                    RefreshSelectedProdiStudents();
                }
            }
        }

        // Selection in Data Mahasiswa
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

        // -------------------------------------------------------------
        // Form Fields (Two-Way Data Binding)
        // -------------------------------------------------------------
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

        // -------------------------------------------------------------
        // Dashboard KPI Metrics
        // -------------------------------------------------------------
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

        private int _cutiStudents;
        public int CutiStudents
        {
            get => _cutiStudents;
            private set => SetProperty(ref _cutiStudents, value);
        }

        private int _lulusStudents;
        public int LulusStudents
        {
            get => _lulusStudents;
            private set => SetProperty(ref _lulusStudents, value);
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

        // -------------------------------------------------------------
        // Commands
        // -------------------------------------------------------------
        public ICommand NavigateCommand { get; }
        public ICommand NewStudentNavCommand { get; }
        public ICommand CopyReportCommand { get; }
        public ICommand SelectProdiCommand { get; }
        public ICommand NewForSelectedProdiCommand { get; }

        public ICommand SaveCommand { get; }
        public ICommand NewCommand { get; }
        public ICommand EditCommand { get; }
        public ICommand DeleteCommand { get; }
        public ICommand ResetFilterCommand { get; }

        // -------------------------------------------------------------
        // Constructor
        // -------------------------------------------------------------
        public MainViewModel()
        {
            // Initialize Dropdown Options
            var prodis = new[] {
                "Teknik Informatika",
                "Sistem Informasi",
                "Sains Data",
                "Teknik Komputer",
                "Teknologi Informasi",
                "Rekayasa Perangkat Lunak"
            };
            foreach (var p in prodis) ProdiList.Add(p);

            FilterProdiList.Add("Semua Prodi");
            foreach (var p in prodis) FilterProdiList.Add(p);

            var statuses = new[] { "Aktif", "Cuti", "Lulus" };
            foreach (var s in statuses) StatusList.Add(s);

            FilterStatusList.Add("Semua Status");
            foreach (var s in statuses) FilterStatusList.Add(s);

            for (int y = 2026; y >= 2020; y--) AngkatanList.Add(y);

            // Initialize Navigation Commands
            NavigateCommand = new RelayCommand(param =>
            {
                if (param is string viewName && !string.IsNullOrWhiteSpace(viewName))
                {
                    CurrentView = viewName;
                }
            });

            NewStudentNavCommand = new RelayCommand(() =>
            {
                CurrentView = "DataMahasiswa";
                ExecuteNew();
            });

            CopyReportCommand = new RelayCommand(ExecuteCopyReport);

            SelectProdiCommand = new RelayCommand(param =>
            {
                if (param is ProdiStat ps)
                {
                    SelectedProdi = ps;
                    CurrentView = "ProgramStudi";
                }
                else if (param is string prodiName)
                {
                    var found = ProdiStats.FirstOrDefault(p => p.ProdiName.Equals(prodiName, StringComparison.OrdinalIgnoreCase));
                    if (found != null)
                    {
                        SelectedProdi = found;
                        CurrentView = "ProgramStudi";
                    }
                }
            });

            NewForSelectedProdiCommand = new RelayCommand(() =>
            {
                CurrentView = "DataMahasiswa";
                ExecuteNew();
                if (SelectedProdi != null && ProdiList.Contains(SelectedProdi.ProdiName))
                {
                    FormProdi = SelectedProdi.ProdiName;
                }
            });

            // Initialize CRUD & Filter Commands
            SaveCommand = new RelayCommand(ExecuteSave);
            NewCommand = new RelayCommand(ExecuteNew);
            EditCommand = new RelayCommand(ExecuteEdit, () => SelectedStudent != null);
            DeleteCommand = new RelayCommand(ExecuteDelete, () => SelectedStudent != null);
            ResetFilterCommand = new RelayCommand(ExecuteResetFilter);

            SeedData();
            ApplyFilter();
            UpdateStatistics();
        }

        private void SeedData()
        {
            _masterStudents.Add(new Student("5025241001", "Muhammad Rizky Pratama", "Teknik Informatika", 2024, 3.88, "Aktif", "rizky.pratama@mhs.its.ac.id", "081234567890"));
            _masterStudents.Add(new Student("5025241025", "Aulia Dewi Safitri", "Sistem Informasi", 2024, 3.74, "Aktif", "aulia.safitri@mhs.its.ac.id", "081234567891"));
            _masterStudents.Add(new Student("5025241054", "Dimas Arya Nugroho", "Sains Data", 2025, 3.65, "Aktif", "dimas.arya@mhs.its.ac.id", "081234567892"));
            _masterStudents.Add(new Student("5025241078", "Vania Kusuma Wardhani", "Teknik Informatika", 2025, 3.92, "Aktif", "vania.wardhani@mhs.its.ac.id", "081234567893"));
            _masterStudents.Add(new Student("5025241099", "Fajar Hidayatullah", "Teknik Komputer", 2023, 3.25, "Cuti", "fajar.hidayat@mhs.its.ac.id", "081234567894"));
            _masterStudents.Add(new Student("5025241112", "Gabriella Natasha", "Teknologi Informasi", 2022, 3.80, "Lulus", "gabriella.n@mhs.its.ac.id", "081234567895"));
            _masterStudents.Add(new Student("5025241130", "Reza Ananta Putra", "Rekayasa Perangkat Lunak", 2024, 3.68, "Aktif", "reza.ananta@mhs.its.ac.id", "081234567896"));
            _masterStudents.Add(new Student("5025241145", "Siti Nurhaliza", "Sistem Informasi", 2023, 3.45, "Aktif", "siti.nurhaliza@mhs.its.ac.id", "081234567897"));
            _masterStudents.Add(new Student("5025241160", "Budi Santoso", "Teknik Komputer", 2024, 2.95, "Aktif", "budi.santoso@mhs.its.ac.id", "081234567898"));
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

            UpdateStatistics();
        }

        private void UpdateStatistics()
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
            CutiStudents = _masterStudents.Count(s => s.Status == "Cuti");
            LulusStudents = _masterStudents.Count(s => s.Status == "Lulus");
            CumlaudeStudents = _masterStudents.Count(s => s.IPK >= 3.51);

            // Update Top Students
            TopStudents.Clear();
            var top = _masterStudents.OrderByDescending(s => s.IPK).Take(4);
            foreach (var s in top)
            {
                TopStudents.Add(s);
            }

            // Update Predikat Statistics
            PredikatStats.Clear();
            int cumlaudeCount = _masterStudents.Count(s => s.IPK >= 3.51);
            int sangatMemuaskanCount = _masterStudents.Count(s => s.IPK >= 3.00 && s.IPK < 3.51);
            int memuaskanCount = _masterStudents.Count(s => s.IPK >= 2.50 && s.IPK < 3.00);
            int cukupCount = _masterStudents.Count(s => s.IPK < 2.50);

            double totalDbl = TotalStudents > 0 ? (double)TotalStudents : 1.0;

            PredikatStats.Add(new PredikatStat
            {
                PredikatName = "Cumlaude",
                Kriteria = "IPK ≥ 3.51",
                Count = cumlaudeCount,
                Percentage = (cumlaudeCount / totalDbl) * 100.0,
                ColorHex = "#7C3AED"
            });

            PredikatStats.Add(new PredikatStat
            {
                PredikatName = "Sangat Memuaskan",
                Kriteria = "3.00 ≤ IPK < 3.51",
                Count = sangatMemuaskanCount,
                Percentage = (sangatMemuaskanCount / totalDbl) * 100.0,
                ColorHex = "#2563EB"
            });

            PredikatStats.Add(new PredikatStat
            {
                PredikatName = "Memuaskan",
                Kriteria = "2.50 ≤ IPK < 3.00",
                Count = memuaskanCount,
                Percentage = (memuaskanCount / totalDbl) * 100.0,
                ColorHex = "#D97706"
            });

            PredikatStats.Add(new PredikatStat
            {
                PredikatName = "Cukup",
                Kriteria = "IPK < 2.50",
                Count = cukupCount,
                Percentage = (cukupCount / totalDbl) * 100.0,
                ColorHex = "#64748B"
            });

            // Update Status Statistics
            StatusStats.Clear();
            StatusStats.Add(new StatusStat
            {
                StatusName = "Aktif",
                Count = ActiveStudents,
                Percentage = (ActiveStudents / totalDbl) * 100.0,
                BadgeBackground = "#ECFDF5",
                BadgeForeground = "#059669"
            });
            StatusStats.Add(new StatusStat
            {
                StatusName = "Cuti",
                Count = CutiStudents,
                Percentage = (CutiStudents / totalDbl) * 100.0,
                BadgeBackground = "#FEF3C7",
                BadgeForeground = "#D97706"
            });
            StatusStats.Add(new StatusStat
            {
                StatusName = "Lulus",
                Count = LulusStudents,
                Percentage = (LulusStudents / totalDbl) * 100.0,
                BadgeBackground = "#EFF6FF",
                BadgeForeground = "#2563EB"
            });

            // Update Program Studi Statistics
            ProdiStats.Clear();
            var prodiIcons = new Dictionary<string, string>
            {
                ["Teknik Informatika"] = "💻",
                ["Sistem Informasi"] = "🌐",
                ["Sains Data"] = "📊",
                ["Teknik Komputer"] = "⚡",
                ["Teknologi Informasi"] = "🛰️",
                ["Rekayasa Perangkat Lunak"] = "⚙️"
            };

            foreach (var p in ProdiList)
            {
                var prodiMhs = _masterStudents.Where(s => s.Prodi == p).ToList();
                int count = prodiMhs.Count;
                double avgIpk = count > 0 ? prodiMhs.Average(s => s.IPK) : 0.0;
                int actCount = prodiMhs.Count(s => s.Status == "Aktif");
                int cutCount = prodiMhs.Count(s => s.Status == "Cuti");
                int llsCount = prodiMhs.Count(s => s.Status == "Lulus");
                int cumCount = prodiMhs.Count(s => s.IPK >= 3.51);
                var topMhs = prodiMhs.OrderByDescending(s => s.IPK).FirstOrDefault();

                ProdiStats.Add(new ProdiStat
                {
                    ProdiName = p,
                    Icon = prodiIcons.TryGetValue(p, out var icon) ? icon : "🏛️",
                    TotalStudents = count,
                    AverageIPK = avgIpk,
                    ActiveCount = actCount,
                    CutiCount = cutCount,
                    LulusCount = llsCount,
                    CumlaudeCount = cumCount,
                    PercentageOfTotal = (count / totalDbl) * 100.0,
                    TopStudentName = topMhs != null ? topMhs.Nama : "-",
                    TopStudentIPK = topMhs != null ? topMhs.IPK : 0.0
                });
            }

            // Set default selected prodi if none selected
            if (SelectedProdi == null || !ProdiStats.Any(p => p.ProdiName == SelectedProdi.ProdiName))
            {
                SelectedProdi = ProdiStats.FirstOrDefault();
            }
            else
            {
                // Refresh existing selected prodi reference
                SelectedProdi = ProdiStats.FirstOrDefault(p => p.ProdiName == SelectedProdi.ProdiName);
            }

            RefreshSelectedProdiStudents();
        }

        private void RefreshSelectedProdiStudents()
        {
            SelectedProdiStudents.Clear();
            if (SelectedProdi == null) return;

            var list = _masterStudents.Where(s => s.Prodi == SelectedProdi.ProdiName);
            foreach (var student in list)
            {
                SelectedProdiStudents.Add(student);
            }
        }

        private void ExecuteCopyReport()
        {
            var sb = new System.Text.StringBuilder();
            sb.AppendLine("=================================================================");
            sb.AppendLine("   LAPORAN AKADEMIK MAHASISWA — SIMA ITS");
            sb.AppendLine($"   Tanggal Ekspor: {DateTime.Now:dddd, dd MMMM yyyy HH:mm:ss}");
            sb.AppendLine("=================================================================");
            sb.AppendLine();
            sb.AppendLine($"• Total Mahasiswa Terdaftar : {TotalStudents} orang");
            sb.AppendLine($"• Rata-Rata IPK Keseluruhan  : {AverageIPK} (skala 4.00)");
            sb.AppendLine($"• Mahasiswa Aktif           : {ActiveStudents} orang");
            sb.AppendLine($"• Mahasiswa Cuti            : {CutiStudents} orang");
            sb.AppendLine($"• Mahasiswa Lulus           : {LulusStudents} orang");
            sb.AppendLine($"• Predikat Cumlaude (≥3.51) : {CumlaudeStudents} orang");
            sb.AppendLine();
            sb.AppendLine("--- DISTRIBUSI PREDIKAT KELULUSAN ---");
            foreach (var p in PredikatStats)
            {
                sb.AppendLine($"  - {p.PredikatName,-16} ({p.Kriteria}): {p.Count} orang ({p.PercentageFormat})");
            }
            sb.AppendLine();
            sb.AppendLine("--- REKAPITULASI PROGRAM STUDI ---");
            foreach (var pr in ProdiStats)
            {
                sb.AppendLine($"  - {pr.ProdiName,-26} : {pr.TotalStudents} Mhs | Rata-rata IPK: {pr.AverageIPKFormat} | Aktif: {pr.ActiveCount} | Cumlaude: {pr.CumlaudeCount}");
            }
            sb.AppendLine();
            sb.AppendLine("=================================================================");
            sb.AppendLine("Sistem Manajemen Data Mahasiswa — Pemrograman Berbasis Kerangka Kerja");

            try
            {
                Clipboard.SetText(sb.ToString());
                StatusMessage = "Ringkasan laporan akademik berhasil disalin ke Clipboard!";
                MessageBox.Show("Ringkasan laporan akademik berhasil disalin ke Clipboard!\n\nAnda dapat menempelkannya (Paste) langsung ke dokumen atau spreadsheet.",
                    "Sukses Ekspor Laporan", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                StatusMessage = $"Gagal menyalin ke clipboard: {ex.Message}";
            }
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
            CurrentView = "DataMahasiswa";
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
