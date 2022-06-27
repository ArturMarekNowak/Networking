using System.Net;
using System.Net.Mail;

namespace SMTP
{
    class SMTP
    {
        public static void Main()
        {
            // Based on: https://stackoverflow.com/a/25215834/15270760
            using (MailMessage mail = new MailMessage())
            {
                mail.From = new MailAddress("foo@gmail.com");
                mail.To.Add("bar@foobar.com");
                mail.Subject = "Hello World";
                mail.Body = "Hello";
                
                var directory = new DirectoryInfo("Attachments");
                
                foreach (var file in directory.GetFiles())
                {
                    mail.Attachments.Add(new Attachment(file.FullName));
                }
                
                mail.Priority = MailPriority.High;

                using (SmtpClient smtp = new SmtpClient("smtp.gmail.com", 587))
                {
                    smtp.Credentials = new NetworkCredential("foo@gmail.com", "application password");
                    smtp.EnableSsl = true;
                    smtp.Send(mail);
                }
            }
        }
    }
}