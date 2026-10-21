namespace StudentRegistrationApp.Models
{
    public class Mahasiswa
    {
        public string NRP { get; set; } = string.Empty;
        public string Nama { get; set; } = string.Empty;
        public string Prodi { get; set; } = string.Empty;
        public string JenisKelamin { get; set; } = string.Empty;
        public DateTime TanggalLahir { get; set; }
        public int Angkatan { get; set; }
        public string JalurMasuk { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public DateTime TanggalRegistrasi { get; set; } = DateTime.Now;

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
