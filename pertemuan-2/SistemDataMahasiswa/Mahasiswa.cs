namespace SistemDataMahasiswa
{
    public class Mahasiswa
    {
        public string NRP { get; set; } = string.Empty;
        public string Nama { get; set; } = string.Empty;
        public string Prodi { get; set; } = string.Empty;
        public int Angkatan { get; set; }
        public double IPK { get; set; }

        public Mahasiswa() { }

        public Mahasiswa(string nrp, string nama, string prodi, int angkatan, double ipk)
        {
            NRP = nrp;
            Nama = nama;
            Prodi = prodi;
            Angkatan = angkatan;
            IPK = ipk;
        }

        public string GetKategoriKelulusan()
        {
            if (IPK >= 3.51) return "Dengan Pujian (Cumlaude)";
            if (IPK >= 3.00) return "Sangat Memuaskan";
            if (IPK >= 2.50) return "Memuaskan";
            return "Cukup";
        }
    }
}
