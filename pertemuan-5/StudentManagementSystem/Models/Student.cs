namespace StudentManagementSystem.Models
{
    public class Student
    {
        public string NRP { get; set; } = string.Empty;
        public string Nama { get; set; } = string.Empty;
        public string Prodi { get; set; } = string.Empty;
        public int Angkatan { get; set; } = 2026;
        public double IPK { get; set; } = 0.0;
        public string Status { get; set; } = "Aktif";
        public string Email { get; set; } = string.Empty;
        public string Telepon { get; set; } = string.Empty;

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
            NRP = nrp;
            Nama = nama;
            Prodi = prodi;
            Angkatan = angkatan;
            IPK = ipk;
            Status = status;
            Email = email;
            Telepon = telepon;
        }

        public Student Clone()
        {
            return new Student(NRP, Nama, Prodi, Angkatan, IPK, Status, Email, Telepon);
        }
    }
}
