namespace AmTools;

public class TelegaBotSender
{
    /// <summary>
    /// Отправляет HTML-сообщение одному получателю от имени Telegram-бота.
    /// </summary>
    /// <param name="botToken">Токен вашего бота</param>
    /// <param name="myChatId">ID вашего чата (получателя)</param>
    /// <param name="htmlMessage">HTML-сообщение для отправки</param>
    public static async Task<string> SendHtmlMessage(
        string botToken,
        string myChatId,
        string htmlMessage)
    {
        using var httpClient = new HttpClient();
        var apiUrl = $"https://api.telegram.org/bot{botToken}/sendMessage";

        var formData = new Dictionary<string, string>
        {
            { "chat_id", myChatId },
            { "text", htmlMessage },
            { "parse_mode", "HTML" }
        };

        using var content = new FormUrlEncodedContent(formData);
        var response = await httpClient.PostAsync(apiUrl, content);

        // Если запрос прошел успешно, возвращаем ответ сервера
        if (response.IsSuccessStatusCode)
            return await response.Content.ReadAsStringAsync();

        // Иначе возвращаем описание ошибки
        return $"Ошибка: {await response.Content.ReadAsStringAsync()}";
    }
}
