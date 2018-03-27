using Osrs.Net.Mail;

namespace TestingApp
{
    class Program
    {
        static void Main(string[] args)
        {
            SmtpClientConfig config = new SmtpClientConfig(587, "smtp.gmail.com", "oncordev@gmail.com", "JweIAail247!");
            config.HtmlFormat = false;

            SmtpClient client = new SmtpClient(config);
            client.Send("michael.corsello@corselloresearch.com", "welcome to oncor", "say hello from http://oncordev.oncor.com/api/verify/11212121-5515");
            client.Send("oncordev@gmail.com", "michael.corsello@corselloresearch.com", "welcome to oncor", "say hello from http://oncordev.oncor.com/api/verify/11212121-5515");
        }
    }
}
