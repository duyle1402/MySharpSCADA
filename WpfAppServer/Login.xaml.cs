using System;
using System.Windows;

namespace WpfAppServer
{
    /// <summary>
    /// Interaction logic for Login.xaml
    /// </summary>
    public partial class Login : Window
    {
        MemberIdentity identity;
        public static bool IsOpen = false;

        public Login()
        {
            IsOpen = true;
            InitializeComponent();
        }

        private void button1_Click(object sender, RoutedEventArgs e)
        {
            string user = txtUser.Text;
            if (string.IsNullOrEmpty(user) || user.Length > 10)
            {
                txterr.Text = "Username is incorrect! The number of characters should be between 0-10";
                return;
            }

            identity = new MemberIdentity(user);
            int rs = identity.Authenticate(txtPassword.Password);
            switch (rs)
            {
                case 0:
                    App.Principal = new MemberPrincipal(identity);
                    this.Close();
                    return;
                case -1:
                    txterr.Text = "The password is incorrect! Default user: op Default password: 1";
                    break;
                case -2:
                    txterr.Text = "Username is incorrect! Default user: op Default password: 1";
                    break;
                case -3:
                    txterr.Text = "Username is incorrect! Default user: op Default password: 1";
                    break;
                default:
                    txterr.Text = "Database connection failed! Please check";
                    break;
            }
        }

        private void button2_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
            App.Current.Shutdown();
        }

        private void button3_Click(object sender, RoutedEventArgs e)
        {

        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            //DateTime start, end; string team;
            //WindowHelper.GetWorkInfo(DateTime.Now, out start, out end, out team);
            //var tag1 = App.Server["P1_DIE"];
            //txtUser.Text = tag1.ToString();
            //txtPassword.Text = App.Server["P2_DIE"].ToString();
        }

        private void Image_PreviewMouseDown_1(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            this.Close();
            App.Current.Shutdown();
        }

        private void Image_PreviewMouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            //WindowState = WindowState.Minimized;
        }

        private void DockPanel_PreviewMouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonDown(e);
            this.DragMove();
        }
    }
}
