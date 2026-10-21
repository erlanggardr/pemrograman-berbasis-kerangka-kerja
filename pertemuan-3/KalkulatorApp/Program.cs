namespace KalkulatorApp
{
    internal static class Program
    {
        /// <summary>
        ///  Titik masuk utama (main entry point) untuk aplikasi Kalkulator WinForms.
        /// </summary>
        [STAThread]
        static void Main()
        {
            ApplicationConfiguration.Initialize();
            Application.Run(new MainForm());
        }
    }
}
