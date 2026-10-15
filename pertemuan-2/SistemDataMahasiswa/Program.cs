using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace SistemDataMahasiswa
{
    internal class Program
    {
        private static readonly List<Mahasiswa> listMahasiswa = new List<Mahasiswa>();

        static void Main(string[] args)
        {
            InisialisasiDataAwal();

            bool berjalan = true;
            while (berjalan)
            {
                AmanClearConsole();
                TampilkanHeader();
                Console.WriteLine("Silakan pilih menu operasi:");
                Console.WriteLine(" [1] Tampilkan Semua Data Mahasiswa");
                Console.WriteLine(" [2] Tambah Data Mahasiswa Baru");
                Console.WriteLine(" [3] Cari Data Mahasiswa (NRP / Nama)");
                Console.WriteLine(" [4] Ubah / Update Data Mahasiswa");
                Console.WriteLine(" [5] Hapus Data Mahasiswa");
                Console.WriteLine(" [6] Tampilkan Statistik & Ringkasan Data");
                Console.WriteLine(" [0] Keluar");
                Console.WriteLine("================================================================================");
                Console.Write("Masukkan pilihan Anda [0-6]: ");

                string inputPilihan = Console.ReadLine()?.Trim() ?? "";
                Console.WriteLine();

                switch (inputPilihan)
                {
                    case "1":
                        TampilkanSemuaMahasiswa();
                        break;
                    case "2":
                        TambahMahasiswa();
                        break;
                    case "3":
                        CariMahasiswa();
                        break;
                    case "4":
                        UbahMahasiswa();
                        break;
                    case "5":
                        HapusMahasiswa();
                        break;
                    case "6":
                        TampilkanStatistik();
                        break;
                    case "0":
                        berjalan = false;
                        Console.WriteLine("Terima kasih telah menggunakan Sistem Data Mahasiswa. Sampai jumpa!");
                        continue;
                    default:
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("Pilihan tidak valid! Masukkan angka antara 0 sampai 6.");
                        Console.ResetColor();
                        break;
                }

                TungguKonfirmasiPengguna();
            }
        }

        private static void AmanClearConsole()
        {
            try
            {
                if (!Console.IsOutputRedirected)
                {
                    Console.Clear();
                }
            }
            catch
            {
                // Abaikan jika tidak mendukung clear (misal mode pipe / redirect)
            }
        }

        private static void TungguKonfirmasiPengguna()
        {
            Console.WriteLine("\nTekan tombol sembarang atau ENTER untuk kembali ke menu utama...");
            try
            {
                if (!Console.IsInputRedirected)
                {
                    Console.ReadKey();
                }
                else
                {
                    Console.ReadLine();
                }
            }
            catch
            {
                Console.ReadLine();
            }
        }

        private static void TampilkanHeader()
        {
            Console.WriteLine("================================================================================");
            Console.WriteLine("         SISTEM INFORMASI DATA MAHASISWA - C# CONSOLE APPLICATION               ");
            Console.WriteLine("              Praktikum Pemrograman Berbasis Kerangka Kerja                     ");
            Console.WriteLine("================================================================================");
        }

        private static void InisialisasiDataAwal()
        {
            listMahasiswa.Add(new Mahasiswa("5025241001", "Ahmad Fauzi", "Teknik Informatika", 2024, 3.85));
            listMahasiswa.Add(new Mahasiswa("5025241002", "Budi Santoso", "Teknik Informatika", 2024, 3.42));
            listMahasiswa.Add(new Mahasiswa("5025231015", "Citra Kirana", "Sistem Informasi", 2023, 3.91));
            listMahasiswa.Add(new Mahasiswa("5025241088", "Dina Lestari", "Teknik Komputer", 2024, 3.20));
        }

        private static void TampilkanSemuaMahasiswa()
        {
            Console.WriteLine("--- DAFTAR MAHASISWA TERDAFTAR ---");
            if (listMahasiswa.Count == 0)
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("Belum ada data mahasiswa yang tersimpan.");
                Console.ResetColor();
                return;
            }

            CetakTabelMahasiswa(listMahasiswa);
            Console.WriteLine($"Total data: {listMahasiswa.Count} mahasiswa.");
        }

        private static void TambahMahasiswa()
        {
            Console.WriteLine("--- TAMBAH MAHASISWA BARU ---");

            string nrp;
            while (true)
            {
                Console.Write("Masukkan NRP / NIM (contoh: 5025241099): ");
                nrp = Console.ReadLine()?.Trim() ?? "";
                if (string.IsNullOrEmpty(nrp))
                {
                    Console.WriteLine("NRP tidak boleh kosong!");
                    continue;
                }

                if (listMahasiswa.Any(m => m.NRP.Equals(nrp, StringComparison.OrdinalIgnoreCase)))
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine($"Mahasiswa dengan NRP {nrp} sudah ada dalam sistem! Gunakan NRP lain.");
                    Console.ResetColor();
                    continue;
                }
                break;
            }

            string nama;
            while (true)
            {
                Console.Write("Masukkan Nama Lengkap: ");
                nama = Console.ReadLine()?.Trim() ?? "";
                if (!string.IsNullOrEmpty(nama)) break;
                Console.WriteLine("Nama tidak boleh kosong!");
            }

            string prodi;
            while (true)
            {
                Console.Write("Masukkan Program Studi: ");
                prodi = Console.ReadLine()?.Trim() ?? "";
                if (!string.IsNullOrEmpty(prodi)) break;
                Console.WriteLine("Program Studi tidak boleh kosong!");
            }

            int angkatan;
            while (true)
            {
                Console.Write("Masukkan Tahun Angkatan (2018 - 2026): ");
                if (int.TryParse(Console.ReadLine(), out angkatan) && angkatan >= 2018 && angkatan <= 2026)
                {
                    break;
                }
                Console.WriteLine("Tahun Angkatan tidak valid! Masukkan angka antara 2018 sampai 2026.");
            }

            double ipk;
            while (true)
            {
                Console.Write("Masukkan IPK (0.00 - 4.00): ");
                string inputIpk = (Console.ReadLine() ?? "").Replace(',', '.').Trim();
                if (double.TryParse(inputIpk, NumberStyles.Any, CultureInfo.InvariantCulture, out ipk) && ipk >= 0.0 && ipk <= 4.0)
                {
                    break;
                }
                Console.WriteLine("Nilai IPK tidak valid! Harus berupa angka desimal antara 0.00 hingga 4.00.");
            }

            Mahasiswa mhsBaru = new Mahasiswa(nrp, nama, prodi, angkatan, ipk);
            listMahasiswa.Add(mhsBaru);

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"\n[SUKSES] Data mahasiswa {nama} (NRP: {nrp}) berhasil ditambahkan ke sistem!");
            Console.ResetColor();
        }

        private static void CariMahasiswa()
        {
            Console.WriteLine("--- CARI DATA MAHASISWA ---");
            Console.Write("Masukkan kata kunci pencarian (NRP atau Nama): ");
            string keyword = Console.ReadLine()?.Trim() ?? "";

            if (string.IsNullOrEmpty(keyword))
            {
                Console.WriteLine("Kata kunci pencarian tidak boleh kosong!");
                return;
            }

            var hasil = listMahasiswa
                .Where(m => m.NRP.Contains(keyword, StringComparison.OrdinalIgnoreCase) ||
                            m.Nama.Contains(keyword, StringComparison.OrdinalIgnoreCase))
                .ToList();

            if (hasil.Count == 0)
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine($"Tidak ditemukan mahasiswa dengan kata kunci '{keyword}'.");
                Console.ResetColor();
                return;
            }

            Console.WriteLine($"\nDitemukan {hasil.Count} data yang cocok:");
            CetakTabelMahasiswa(hasil);
        }

        private static void UbahMahasiswa()
        {
            Console.WriteLine("--- UBAH / UPDATE DATA MAHASISWA ---");
            Console.Write("Masukkan NRP mahasiswa yang ingin diubah: ");
            string nrp = Console.ReadLine()?.Trim() ?? "";

            var mhs = listMahasiswa.FirstOrDefault(m => m.NRP.Equals(nrp, StringComparison.OrdinalIgnoreCase));
            if (mhs == null)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"Mahasiswa dengan NRP '{nrp}' tidak ditemukan!");
                Console.ResetColor();
                return;
            }

            Console.WriteLine($"\nData saat ini: {mhs.NRP} - {mhs.Nama} | {mhs.Prodi} | Angkatan: {mhs.Angkatan} | IPK: {mhs.IPK:F2}");
            Console.WriteLine("Tekan [ENTER] jika tidak ingin mengubah nilai atribut tersebut.\n");

            Console.Write($"Nama Baru [{mhs.Nama}]: ");
            string namaBaru = Console.ReadLine()?.Trim() ?? "";
            if (!string.IsNullOrEmpty(namaBaru)) mhs.Nama = namaBaru;

            Console.Write($"Prodi Baru [{mhs.Prodi}]: ");
            string prodiBaru = Console.ReadLine()?.Trim() ?? "";
            if (!string.IsNullOrEmpty(prodiBaru)) mhs.Prodi = prodiBaru;

            Console.Write($"Angkatan Baru [{mhs.Angkatan}]: ");
            string angkatanStr = Console.ReadLine()?.Trim() ?? "";
            if (!string.IsNullOrEmpty(angkatanStr) && int.TryParse(angkatanStr, out int angkatanBaru) && angkatanBaru >= 2018 && angkatanBaru <= 2026)
            {
                mhs.Angkatan = angkatanBaru;
            }

            Console.Write($"IPK Baru [{mhs.IPK:F2}]: ");
            string ipkStr = (Console.ReadLine() ?? "").Trim().Replace(',', '.');
            if (!string.IsNullOrEmpty(ipkStr) && double.TryParse(ipkStr, NumberStyles.Any, CultureInfo.InvariantCulture, out double ipkBaru) && ipkBaru >= 0.0 && ipkBaru <= 4.0)
            {
                mhs.IPK = ipkBaru;
            }

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"\n[SUKSES] Data mahasiswa dengan NRP {mhs.NRP} berhasil diperbarui!");
            Console.ResetColor();
        }

        private static void HapusMahasiswa()
        {
            Console.WriteLine("--- HAPUS DATA MAHASISWA ---");
            Console.Write("Masukkan NRP mahasiswa yang ingin dihapus: ");
            string nrp = Console.ReadLine()?.Trim() ?? "";

            var mhs = listMahasiswa.FirstOrDefault(m => m.NRP.Equals(nrp, StringComparison.OrdinalIgnoreCase));
            if (mhs == null)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"Mahasiswa dengan NRP '{nrp}' tidak ditemukan!");
                Console.ResetColor();
                return;
            }

            Console.Write($"Apakah Anda yakin ingin menghapus data {mhs.Nama} ({mhs.NRP})? [y/N]: ");
            string konfirmasi = Console.ReadLine()?.Trim().ToLower() ?? "";

            if (konfirmasi == "y" || konfirmasi == "ya")
            {
                listMahasiswa.Remove(mhs);
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine($"[SUKSES] Data mahasiswa dengan NRP {nrp} berhasil dihapus dari sistem.");
                Console.ResetColor();
            }
            else
            {
                Console.WriteLine("Penghapusan dibatalkan.");
            }
        }

        private static void TampilkanStatistik()
        {
            Console.WriteLine("--- STATISTIK & RINGKASAN DATA MAHASISWA ---");
            if (listMahasiswa.Count == 0)
            {
                Console.WriteLine("Belum ada data untuk dihitung statistiknya.");
                return;
            }

            int total = listMahasiswa.Count;
            double rataIpk = listMahasiswa.Average(m => m.IPK);
            var mhsTertinggi = listMahasiswa.OrderByDescending(m => m.IPK).First();
            var mhsTerendah = listMahasiswa.OrderBy(m => m.IPK).First();

            Console.WriteLine($"Total Mahasiswa      : {total} orang");
            Console.WriteLine($"Rata-rata IPK        : {rataIpk:F2}");
            Console.WriteLine($"IPK Tertinggi        : {mhsTertinggi.IPK:F2} ({mhsTertinggi.Nama} - {mhsTertinggi.NRP})");
            Console.WriteLine($"IPK Terendah         : {mhsTerendah.IPK:F2} ({mhsTerendah.Nama} - {mhsTerendah.NRP})");
            Console.WriteLine();

            Console.WriteLine("Distribusi Berdasarkan Program Studi:");
            var perProdi = listMahasiswa.GroupBy(m => m.Prodi);
            foreach (var group in perProdi)
            {
                Console.WriteLine($" - {group.Key,-22} : {group.Count()} mahasiswa");
            }
        }

        private static void CetakTabelMahasiswa(List<Mahasiswa> list)
        {
            string garis = "+------------+----------------------+--------------------+----------+------+-----------------------------+";
            Console.WriteLine(garis);
            Console.WriteLine("| NRP        | Nama                 | Program Studi      | Angkatan | IPK  | Predikat Kelulusan          |");
            Console.WriteLine(garis);

            foreach (var m in list)
            {
                Console.WriteLine($"| {m.NRP,-10} | {m.Nama,-20} | {m.Prodi,-18} | {m.Angkatan,-8} | {m.IPK,4:F2} | {m.GetKategoriKelulusan(),-27} |");
            }

            Console.WriteLine(garis);
        }
    }
}
