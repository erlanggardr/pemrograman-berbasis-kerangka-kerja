namespace HelloWorld
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("==================================================");
            Console.WriteLine("   PRAKTIKUM PEMROGRAMAN BERBASIS KERANGKA KERJA");
            Console.WriteLine("             LATIHAN 1: PENGENALAN .NET");
            Console.WriteLine("==================================================");
            Console.WriteLine("Hello, World!");
            Console.WriteLine("Selamat datang di pemrograman C# dengan .NET SDK!");
            Console.WriteLine($"Versi .NET Runtime : {Environment.Version}");
            Console.WriteLine($"Sistem Operasi     : {Environment.OSVersion}");
            Console.WriteLine($"Waktu Eksekusi     : {DateTime.Now:dd MMMM yyyy HH:mm:ss}");
            Console.WriteLine("==================================================");
        }
    }
}
