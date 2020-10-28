using Microsoft.Extensions.Options;
using MimeKit;
using RaspSecure.Models.Mail;
using MailKit.Net.Smtp;
using System.Threading.Tasks;

namespace RaspSecure.Services
{
    public class MailService
    {
		private readonly MailOptions _mailOptions;

		public MailService(IOptions<MailOptions> mailOptions)
		{
			_mailOptions = mailOptions.Value;
		}
		public async Task Send(string address, string url, string token)
		{
			var message = new MimeMessage();
			message.To.Add(new MailboxAddress("", address));
			var bodyBuilder = new BodyBuilder();
			message.Subject = _mailOptions.MailHeaderName+": сброс пароля";
			bodyBuilder.HtmlBody = string.Format(@"<p>Для сброса пароля перейдите по <a href=""http://{0}/reset/{1}"">ссылке</a></p>", url, token);
			message.Body = bodyBuilder.ToMessageBody();
			await Send(message);
		}
		private async Task Send(MimeMessage message)
		{
			message.From.Add(new MailboxAddress("", _mailOptions.SmtpAddress));
			using (var emailClient = new SmtpClient())
			{
				await emailClient.ConnectAsync(_mailOptions.SmtpServer, _mailOptions.SmtpPort, true);
				emailClient.AuthenticationMechanisms.Remove("XOAUTH2");
				await emailClient.AuthenticateAsync(_mailOptions.SmtpUsername, _mailOptions.SmtpPassword);
				await emailClient.SendAsync(message);
				await emailClient.DisconnectAsync(true);
			}
		}
	}
}
