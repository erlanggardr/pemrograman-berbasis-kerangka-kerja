using StudentManagementSystem.ViewModels;

namespace StudentManagementSystem.Models
{
    public class ProdiStat : ViewModelBase
    {
        public string ProdiName { get; set; } = string.Empty;
        public string Icon { get; set; } = "🏛️";
        public int TotalStudents { get; set; }
        public double AverageIPK { get; set; }
        public string AverageIPKFormat => AverageIPK.ToString("F2", System.Globalization.CultureInfo.InvariantCulture);
        public int ActiveCount { get; set; }
        public int CutiCount { get; set; }
        public int LulusCount { get; set; }
        public int CumlaudeCount { get; set; }
        public double PercentageOfTotal { get; set; }
        public string PercentageFormat => $"{PercentageOfTotal:F1}%";
        public string TopStudentName { get; set; } = "-";
        public double TopStudentIPK { get; set; }
        public string TopStudentIPKFormat => TopStudentIPK > 0 ? TopStudentIPK.ToString("F2", System.Globalization.CultureInfo.InvariantCulture) : "-";
    }

    public class PredikatStat : ViewModelBase
    {
        public string PredikatName { get; set; } = string.Empty;
        public string Kriteria { get; set; } = string.Empty;
        public int Count { get; set; }
        public double Percentage { get; set; }
        public string PercentageFormat => $"{Percentage:F1}%";
        public string ColorHex { get; set; } = "#2563EB";
    }

    public class StatusStat : ViewModelBase
    {
        public string StatusName { get; set; } = string.Empty;
        public int Count { get; set; }
        public double Percentage { get; set; }
        public string PercentageFormat => $"{Percentage:F1}%";
        public string BadgeBackground { get; set; } = "#ECFDF5";
        public string BadgeForeground { get; set; } = "#059669";
    }
}
