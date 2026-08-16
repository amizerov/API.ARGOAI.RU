namespace AmTools;

using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;

public class GmailSmtpSender
{
    private readonly string _senderName;      // Имя кто отправляет
    private readonly string _senderEmail;     // Gmail-адрес отправителя
    private readonly string _appPassword;     // сгенерированный «Пароль приложения»

    public GmailSmtpSender(string senderName, string senderEmail, string appPassword)
    {
        _senderName = senderName;
        _senderEmail = senderEmail;
        _appPassword = appPassword;
    }

    public void Send(List<string> toEmailList, string subject, string bodyHtml)
    {
        // 1. Формируем сообщение
        var message = new MimeMessage();
        message.From.Add(new MailboxAddress(_senderName, _senderEmail));
        foreach (var email in toEmailList)
            message.To.Add(MailboxAddress.Parse(email));
        message.Subject = subject;

        // 2. Настраиваем тело письма
        var builder = new BodyBuilder
        {
            HtmlBody = bodyHtml,
            TextBody = StripHtml(bodyHtml)  // опционально: текстовая часть
        };
        message.Body = builder.ToMessageBody();

        // 3. Отправляем через SMTP
        using var client = new SmtpClient();
        client.Connect("smtp.gmail.com", 587, SecureSocketOptions.StartTls);
        client.Authenticate(_senderEmail, _appPassword);
        client.Send(message);
        client.Disconnect(true);
    }

    private string StripHtml(string html)
    {
        // Простейшая очистка — можно заменить на более надёжную логику
        return System.Text.RegularExpressions.Regex.Replace(html, "<.*?>", String.Empty);
    }
}
