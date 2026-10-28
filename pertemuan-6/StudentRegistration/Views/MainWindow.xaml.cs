using System.Windows;
using StudentRegistration.ViewModels;

namespace StudentRegistration.Views
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            DataContext = new MainViewModel();
        }
    }
}
