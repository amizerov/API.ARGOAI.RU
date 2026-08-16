using AmSecrets;
using AmTools;

public class ChatRequest
{
    public string? Message { get; set; }
    public string? UserId { get; set; }
    public string? Site { get; set; }
    public string? IP { get; set; }
    public async Task<Result> GetAiAnswer(CancellationToken cancellationToken = default)
    {
        var msg = Message ?? "Hawaii";
        var sessionKey = GetSessionKey();

        await TelegaBotSender.SendHtmlMessage(
                Secrets.SiteUltreazoomTelebotToken,
                Secrets.MyTelegramChatId,
                msg
        );

        Result res = new()
        {
            message = await AIRobot.Assistant.GetAnswer(msg, sessionKey, cancellationToken),
            status = "ok"
        };

        await TelegaBotSender.SendHtmlMessage(
                Secrets.SiteUltreazoomTelebotToken,
                Secrets.MyTelegramChatId,
                res.message ?? "error"
        );

        return res;
    }

    string GetSessionKey()
    {
        var userKey = FirstNotEmpty(UserId, IP);
        var siteKey = FirstNotEmpty(Site);

        if (userKey is null && siteKey is null)
            return ArgoDb.ChatHistoryRepository.DefaultSessionKey;

        var rawKey = $"{siteKey ?? "default"}:{userKey ?? "default"}";

        if (rawKey.Length <= 128)
            return rawKey;

        var hash = System.Security.Cryptography.SHA256.HashData(
            System.Text.Encoding.UTF8.GetBytes(rawKey));
        return Convert.ToHexString(hash).ToLowerInvariant();
    }

    static string? FirstNotEmpty(params string?[] values) =>
        values.FirstOrDefault(x => !string.IsNullOrWhiteSpace(x))?.Trim();
}
