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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace bebebe
{
    /// <summary>
    /// Логика взаимодействия для LoginView.xaml
    /// </summary>
    public partial class LoginView : Window
    {
        public LoginView()
        {
            InitializeComponent();
        }

        private void SPButtonClicked(object sender, RoutedEventArgs e)
        {
            PasswordTextBox.Text = Password.Password;
            Password.Visibility = Visibility.Collapsed;
            PasswordTextBox.Visibility = Visibility.Visible;
            SeePasswordButton.Visibility = Visibility.Collapsed;
            HidePasswordButton.Visibility = Visibility.Visible;
        }
        private void HPButtonClicked(object sender, RoutedEventArgs e)
        {
            Password.Password = PasswordTextBox.Text;
            Password.Visibility = Visibility.Visible;
            PasswordTextBox.Visibility = Visibility.Collapsed;
            SeePasswordButton.Visibility = Visibility.Visible;
            HidePasswordButton.Visibility = Visibility.Collapsed;
        }
    }
}
