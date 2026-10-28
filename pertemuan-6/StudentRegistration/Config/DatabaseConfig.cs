namespace StudentRegistration.Config
{
    public static class DatabaseConfig
    {
        // Default Connection String menggunakan SQL Server LocalDB (bawaan Visual Studio / .NET)
        // Mendukung Windows Authentication dan sertifikat lokal tepercaya.
        public static string ServerName { get; set; } = @"(localdb)\mssqllocaldb";
        public static string DatabaseName { get; set; } = "StudentRegistrationDB";

        public static string ConnectionString =>
            $"Server={ServerName};Database={DatabaseName};Integrated Security=True;TrustServerCertificate=True;Connect Timeout=5;";

        public static string MasterConnectionString =>
            $"Server={ServerName};Database=master;Integrated Security=True;TrustServerCertificate=True;Connect Timeout=5;";
    }
}
