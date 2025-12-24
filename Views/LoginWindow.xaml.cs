using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace prakt15_TRPO.View
{
    /// <summary>
    /// Логика взаимодействия для LoginWindow.xaml
    /// </summary>
    public partial class LoginWindow : Window
    {
        public LoginWindow()
        {
            InitializeComponent();
            PinTextBox.Focus();
        }

        private void ManagerLoginBtn_Click(object sender, RoutedEventArgs e)
        {
            if (PinTextBox.Password == "1234")
            {
                var mainWindow = new MainWindow(true);
                mainWindow.Show();
                this.Close();
            }
            else
            {
                StatusText.Text = "Неверный пин-код!";
                PinTextBox.Focus();
                PinTextBox.SelectAll();
            }
        }

        private void VisitorLoginBtn_Click(object sender, RoutedEventArgs e)
        {
            var mainWindow = new MainWindow(false);
            mainWindow.Show();
            this.Close();
        }
    }
}
