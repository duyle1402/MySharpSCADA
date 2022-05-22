using System;
using System.Collections.Generic;
using System.Diagnostics;
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
using System.Net.Mail;
namespace WpfAppServer.AS_View
{
    /// <summary>
    /// Interaction logic for ASCompany.xaml
    /// </summary>
    public partial class ASCompany : Window
    {
        public ASCompany()
        {
            InitializeComponent();
            this.SetWindowState();

        }
        private void Window_Closed(object sender, EventArgs e)
        {
            this.SaveWindowState();
        }
        private void Hyperlink_RequestNavigate(object sender, RequestNavigateEventArgs e)
        {
            // for .NET Core you need to add UseShellExecute = true
            // see /*https://docs.microsoft.com/dotnet/api/system.diagnostics.processstartinfo.useshellexecute#property-value*/
            Process.Start(new ProcessStartInfo(e.Uri.AbsoluteUri));
            e.Handled = true;
        }
        private void Hyperlink_RequestNavigate1(object sender, RequestNavigateEventArgs e)
        {
            // for .NET Core you need to add UseShellExecute = true
            // see /*https://docs.microsoft.com/dotnet/api/system.diagnostics.processstartinfo.useshellexecute#property-value*/
            Process.Start(new ProcessStartInfo(e.Uri.AbsoluteUri));
            e.Handled = true;
        }

        private void btnSubmit_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                MailMessage mail = new MailMessage();
                SmtpClient SmtpServer = new SmtpClient("smtp.gmail.com");
                mail.From = new MailAddress(inputTextEmail.Text);
                mail.To.Add("prochief006@gmail.com");
                mail.Subject = inputTextName.Text;
                mail.Body = inputMailBody.Text;

                // đính kèm 
                //System.Net.Mail.Attachment attachment;
                //attachment = new System.Net.Mail.Attachment("your attachment file");
                //mail.Attachments.Add(attachment);

                SmtpServer.Port = 587;
                SmtpServer.Credentials = new System.Net.NetworkCredential(inputTextEmail.Text, "password");
                SmtpServer.EnableSsl = true;

                SmtpServer.Send(mail);
                MessageBox.Show("Mail Send");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Tôi không cần bạn nhập mật khẩu vì lí do bảo mật email của bạn, liên hệ số điện thoại chúng tôi nếu cần thêm thông tin nhé");
            }
        }
    }
}
