using AmSecrets;
using AmTools;
using System.ComponentModel.DataAnnotations;

public class MailRequest
{
    [Required]
    [Display(Name = "Название проекта")]
    public string? ProjectName { get; set; }

    [Required]
    [EmailAddress]
    [Display(Name = "Email администратора")]
    public string? AdminEmail { get; set; }

    [Required]
    [Display(Name = "Тема сообщения")]
    public string? FormSubject { get; set; }

    [Required]
    [Display(Name = "Имя отправителя")]
    public string? Name { get; set; }

    [Phone]
    [Display(Name = "Телефон отправителя")]
    public string? Phone { get; set; }

    [EmailAddress]
    [Display(Name = "Email отправителя")]
    public string? Email { get; set; }

    [Required]
    [Display(Name = "Сообщение")]
    public string? Message { get; set; }
    public async Task<Result> SendMail()
    {
        try
        {
            var senderName = ProjectName ?? "WebApi2";
            var senderEmail = Secrets.GmailAppSender;
            var appPassword = Secrets.GmailAppPassword;
            var recievers = new List<string>() { AdminEmail ?? "andrey@mizerov.net", "andrey@mizerov.com" };
            var subject = FormSubject ?? "WebApi2";
            var body = $"""
            <PRE>
                {Name}
                {Email}
                {Phone}
                {Message ?? "---"}
            </PRE>
            """;
            // Отправка письма
            new GmailSmtpSender(
                senderName, senderEmail, appPassword
            )
            .Send(recievers, subject, body);

            try
            {
                await TelegaBotSender.SendHtmlMessage(
                    Secrets.SiteUltreazoomTelebotToken,
                    Secrets.MyTelegramChatId,
                    body
                );
            }
            catch (Exception ex)
            {
                Console.WriteLine("Ошибка при отправке сообщения в Telegram: " + ex.Message);
            }

            return new() { status = "success", message = "Письмо ушло!" };
        }
        catch (Exception ex)
        {
            return new() { status = "error", message = "Ошибка при отправке письма. " + ex.Message };
        }
    }
}

public class Result
{
    public string? status { get; set; }
    public string? message { get; set; }
}