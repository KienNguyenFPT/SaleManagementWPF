using System.Dynamic;
using System.Windows;
using System.Windows.Media;
using MessageBox = System.Windows.MessageBox;
using MessageBoxButton = System.Windows.MessageBoxButton;
using MessageBoxIcon = System.Windows.MessageBoxImage;

namespace SalesWPFApp
{
    public partial class WindowLogin : Window
    {
        private readonly IMemberRepository _memberRepository;
        public WindowLogin(IMemberRepository memberRepository)
        {
            InitializeComponent();
            _memberRepository = memberRepository;
        }

        MemberRepository memberRespository = new MemberRepository();
        dynamic account;

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            account = _memberRepository.GetAccountDefault();
            if (account != null && txtId.Text.Equals(account.loginId) && txtPw.Password.Equals(account.loginPassword))
            {
                account.Role = "Admin";
                account.Name = "Admin";
                memberRespository.setUser(account);
                WindowMain mainWindow = new WindowMain(this);
                mainWindow.Show();
                this.Hide();
            }
            else
            {
                MessageBox.Show("Login Failed! ID/Password wasn't correct!!!", "ERROR", MessageBoxButton.OK, MessageBoxIcon.Error);
            }
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            try
            {
                account = new ExpandoObject();
                Member mem = memberRespository.loginMember(txtId.Text, txtPw.Password);
                if (mem != null)
                {
                    account.Id = mem.Id;
                    account.Name = mem.Name;
                    account.Password = mem.Password;
                    account.Avt = mem.Avt;
                    account.Birthday = mem.Birthday;
                    account.Email = mem.Email;
                    account.Phone = mem.Phone;
                    account.City = mem.City;
                    account.Country = mem.Country;
                    account.Hobby = mem.Hobby;
                    account.Role = "Member";
                    memberRespository.setUser(account);
                    WindowMain mainWindow = new WindowMain(this);
                    mainWindow.Show();
                    this.Hide();
                }
                else
                {
                    MessageBox.Show("Login Failed! ID/Password wasn't correct!!!", "ERROR", MessageBoxButton.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "ERROR", MessageBoxButton.OK, MessageBoxIcon.Error);
            }
        }

        private void txtPwPlaceholder_GotFocus(object sender, RoutedEventArgs e)
        {
            txtPwPlaceholder.Visibility = Visibility.Hidden;
            txtPw.Visibility = Visibility.Visible;
            txtPw.Focus();
        }

        private void txtPw_LostFocus(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtPw.Password))
            {
                txtPw.Visibility = Visibility.Hidden;
                txtPwPlaceholder.Visibility = Visibility.Visible;
            }
        }

        private void txtPw_PasswordChanged(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrEmpty(txtPw.Password))
            {
                txtPwPlaceholder.Visibility = Visibility.Hidden;
            }
            else
            {
                txtPwPlaceholder.Visibility = Visibility.Visible;
            }
        }

        private void TxtId_GotFocus(object sender, RoutedEventArgs e)
        {
            if (txtId.Text == "Enter your account...")
            {
                txtId.Text = "";
                txtId.Foreground = new SolidColorBrush(Colors.Black);
            }
        }

        private void TxtId_LostFocus(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtId.Text))
            {
                txtId.Text = "Enter your account...";
                txtId.Foreground = new SolidColorBrush(Colors.Gray);
            }
        }
    }
}
