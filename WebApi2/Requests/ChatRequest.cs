using AmSecrets;
using AmTools;

public class ChatRequest
{
    public string? Message { get; set; }
    public string? UserId { get; set; }
    public string? Site { get; set; }
    public string? IP { get; set; }
    public async Task<Result> GetAiAnswer()
    {
        var msg = Message ?? "Hawaii";

        await TelegaBotSender.SendHtmlMessage(
                Secrets.SiteUltreazoomTelebotToken,
                Secrets.MyTelegramChatId,
                msg
        );

        Result res = new()
        {
            message = await AIRobot.Assistant.GetAnswer(msg),
            status = "ok"
        };

        await TelegaBotSender.SendHtmlMessage(
                Secrets.SiteUltreazoomTelebotToken,
                Secrets.MyTelegramChatId,
                res.message ?? "error"
        );

        return res;
    }
}
