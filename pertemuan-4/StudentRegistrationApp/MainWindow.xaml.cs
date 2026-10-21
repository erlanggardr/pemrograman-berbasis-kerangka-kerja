using System.Collections.ObjectModel;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using StudentRegistrationApp.Models;

namespace StudentRegistrationApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly ObservableCollection<Mahasiswa> _mahasiswaList = new();
        private readonly List<Mahasiswa> _masterList = new();

        public MainWindow()
        {
            InitializeComponent();

            dgMahasiswa.ItemsSource = _mahasiswaList;
            dpTanggalLahir.SelectedDate = new DateTime(2005, 5, 15);

            SeedInitialData();
            RefreshDataView();
        }

        private void SeedInitialData()
        {
            var initialStudents = new List<Mahasiswa>
            {
                new("5025241001", "Ahmad Fauzi Rahman", "Teknik Informatika", "Laki-laki", new DateTime(2005, 3, 14), 2026, "SNBP (Prestasi)", "ahmad.fauzi24@mhs.its.ac.id"),
                new("5025241042", "Siti Nurhaliza Putri", "Sistem Informasi", "Perempuan", new DateTime(2005, 7, 22), 2026, "SNBT (Tes Tertulis)", "siti.putri24@mhs.its.ac.id"),
                new("5025241088", "Bima Satria Wicaksono", "Sains Data", "Laki-laki", new DateTime(2004, 11, 5), 2025, "Seleksi Mandiri Kemitraan", "bima.satria25@mhs.its.ac.id"),
                new("5025241120", "Clarissa Aurelia", "Teknik Komputer", "Perempuan", new DateTime(2005, 9, 30), 2026, "International Undergraduate Program (IUP)", "clarissa.a24@mhs.its.ac.id")
            };

            foreach (var m in initialStudents)
            {
                _masterList.Add(m);
            }
        }

        private void RefreshDataView(string filter = "")
        {
            _mahasiswaList.Clear();
            var filtered = string.IsNullOrWhiteSpace(filter)
                ? _masterList
                : _masterList.Where(m =>
                    m.NRP.Contains(filter, StringComparison.OrdinalIgnoreCase) ||
                    m.Nama.Contains(filter, StringComparison.OrdinalIgnoreCase) ||
                    m.Prodi.Contains(filter, StringComparison.OrdinalIgnoreCase));

            foreach (var item in filtered)
            {
                _mahasiswaList.Add(item);
            }

            lblBadgeCount.Text = $"Total Mahasiswa: {_masterList.Count}";
            lblStatus.Text = string.IsNullOrWhiteSpace(filter)
                ? $"Menampilkan seluruh {_masterList.Count} mahasiswa terdaftar."
                : $"Ditemukan {_mahasiswaList.Count} dari {_masterList.Count} mahasiswa untuk pencarian '{filter}'.";
        }

        private void BtnSimpan_Click(object sender, RoutedEventArgs e)
        {
            string nrp = txtNRP.Text.Trim();
            string nama = txtNama.Text.Trim();
            string prodi = (cmbProdi.SelectedItem as ComboBoxItem)?.Content.ToString() ?? "";
            string jenisKelamin = rbLaki.IsChecked == true ? "Laki-laki" : "Perempuan";
            DateTime? tglLahir = dpTanggalLahir.SelectedDate;
            string angkatanStr = (cmbAngkatan.SelectedItem as ComboBoxItem)?.Content.ToString() ?? "2026";
            int angkatan = int.TryParse(angkatanStr, out int angk) ? angk : 2026;
            string jalur = (cmbJalur.SelectedItem as ComboBoxItem)?.Content.ToString() ?? "";
            string email = txtEmail.Text.Trim();

            // Validasi Input
            if (string.IsNullOrWhiteSpace(nrp))
            {
                ShowWarning("NRP tidak boleh kosong!", "Validasi Input");
                txtNRP.Focus();
                return;
            }

            if (!Regex.IsMatch(nrp, @"^\d{9,12}$"))
            {
                ShowWarning("Format NRP tidak valid! NRP harus terdiri dari 9 hingga 12 digit angka.", "Validasi Input");
                txtNRP.Focus();
                return;
            }

            if (string.IsNullOrWhiteSpace(nama))
            {
                ShowWarning("Nama lengkap mahasiswa tidak boleh kosong!", "Validasi Input");
                txtNama.Focus();
                return;
            }

            if (!tglLahir.HasValue)
            {
                ShowWarning("Silakan pilih tanggal lahir mahasiswa!", "Validasi Input");
                dpTanggalLahir.Focus();
                return;
            }

            if (tglLahir.Value > DateTime.Today)
            {
                ShowWarning("Tanggal lahir tidak boleh di masa mendatang!", "Validasi Input");
                dpTanggalLahir.Focus();
                return;
            }

            if (string.IsNullOrWhiteSpace(email))
            {
                ShowWarning("Alamat email tidak boleh kosong!", "Validasi Input");
                txtEmail.Focus();
                return;
            }

            if (!Regex.IsMatch(email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
            {
                ShowWarning("Format email tidak valid! Gunakan format email yang benar (contoh: nama@domain.com).", "Validasi Input");
                txtEmail.Focus();
                return;
            }

            var existing = _masterList.FirstOrDefault(m => m.NRP.Equals(nrp, StringComparison.OrdinalIgnoreCase));
            if (existing != null)
            {
                var confirmUpdate = MessageBox.Show(
                    $"Mahasiswa dengan NRP {nrp} sudah terdaftar:\n\nNama: {existing.Nama}\nProdi: {existing.Prodi}\n\nApakah Anda ingin memperbarui data mahasiswa ini?",
                    "Konfirmasi Perbarui Data",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Question
                );

                if (confirmUpdate == MessageBoxResult.Yes)
                {
                    existing.Nama = nama;
                    existing.Prodi = prodi;
                    existing.JenisKelamin = jenisKelamin;
                    existing.TanggalLahir = tglLahir.Value;
                    existing.Angkatan = angkatan;
                    existing.JalurMasuk = jalur;
                    existing.Email = email;

                    RefreshDataView(txtSearch.Text.Trim());
                    MessageBox.Show(
                        $"Data mahasiswa '{nama}' (NRP: {nrp}) berhasil diperbarui!",
                        "Pembaruan Berhasil",
                        MessageBoxButton.OK,
                        MessageBoxImage.Information
                    );

                    ResetForm();
                    lblStatus.Text = $"Pembaruan berhasil: Data {nama} ({nrp}) telah diperbarui.";
                    return;
                }
                else
                {
                    txtNRP.Focus();
                    return;
                }
            }

            var baru = new Mahasiswa(nrp, nama, prodi, jenisKelamin, tglLahir.Value, angkatan, jalur, email);
            _masterList.Add(baru);
            RefreshDataView(txtSearch.Text.Trim());

            MessageBox.Show(
                $"Data mahasiswa '{nama}' (NRP: {nrp}) berhasil disimpan ke sistem!",
                "Registrasi Berhasil",
                MessageBoxButton.OK,
                MessageBoxImage.Information
            );

            ResetForm();
            lblStatus.Text = $"Pendaftaran berhasil: {nama} ({nrp}) ditambahkan ke database.";
        }

        private void BtnReset_Click(object sender, RoutedEventArgs e)
        {
            ResetForm();
            lblStatus.Text = "Formulir pendaftaran telah dikosongkan.";
        }

        private void ResetForm()
        {
            txtNRP.Clear();
            txtNama.Clear();
            txtEmail.Clear();
            cmbProdi.SelectedIndex = 0;
            rbLaki.IsChecked = true;
            dpTanggalLahir.SelectedDate = new DateTime(2005, 5, 15);
            cmbAngkatan.SelectedIndex = 0;
            cmbJalur.SelectedIndex = 0;
            txtNRP.Focus();
        }

        private void BtnHapus_Click(object sender, RoutedEventArgs e)
        {
            if (dgMahasiswa.SelectedItem is not Mahasiswa selected)
            {
                ShowWarning("Silakan pilih data mahasiswa pada tabel yang ingin dihapus terlebih dahulu.", "Peringatan");
                return;
            }

            var confirm = MessageBox.Show(
                $"Apakah Anda yakin ingin menghapus data mahasiswa:\n\nNRP : {selected.NRP}\nNama: {selected.Nama}\nProdi: {selected.Prodi}\n\nTindakan ini tidak dapat dibatalkan.",
                "Konfirmasi Hapus Data",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question
            );

            if (confirm == MessageBoxResult.Yes)
            {
                _masterList.Remove(selected);
                RefreshDataView(txtSearch.Text.Trim());
                lblStatus.Text = $"Data mahasiswa {selected.Nama} ({selected.NRP}) berhasil dihapus.";
            }
        }

        private void TxtSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            RefreshDataView(txtSearch.Text.Trim());
        }

        private void DgMahasiswa_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (dgMahasiswa.SelectedItem is Mahasiswa selected)
            {
                txtNRP.Text = selected.NRP;
                txtNama.Text = selected.Nama;
                txtEmail.Text = selected.Email;
                dpTanggalLahir.SelectedDate = selected.TanggalLahir;

                foreach (ComboBoxItem item in cmbProdi.Items)
                {
                    if (item.Content?.ToString() == selected.Prodi)
                    {
                        cmbProdi.SelectedItem = item;
                        break;
                    }
                }

                if (selected.JenisKelamin == "Perempuan")
                {
                    rbPerempuan.IsChecked = true;
                }
                else
                {
                    rbLaki.IsChecked = true;
                }

                foreach (ComboBoxItem item in cmbAngkatan.Items)
                {
                    if (item.Content?.ToString() == selected.Angkatan.ToString())
                    {
                        cmbAngkatan.SelectedItem = item;
                        break;
                    }
                }

                foreach (ComboBoxItem item in cmbJalur.Items)
                {
                    if (item.Content?.ToString() == selected.JalurMasuk)
                    {
                        cmbJalur.SelectedItem = item;
                        break;
                    }
                }

                lblStatus.Text = $"Memilih: {selected.Nama} | NRP: {selected.NRP} | {selected.Prodi} ({selected.Angkatan})";
            }
        }

        private static void ShowWarning(string message, string title)
        {
            MessageBox.Show(message, title, MessageBoxButton.OK, MessageBoxImage.Warning);
        }
    }
}
