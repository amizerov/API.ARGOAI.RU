using System.Net.Http.Headers;
using System.Text.Json.Serialization;
using System.Text.Json;
using System.Text;
using AmSecrets;
using System.Reflection;

namespace AIRobot
{
    public class Assistant
    {
        static string[] _models = new[] { "gpt-4o-mini", "gpt-4o", "gpt-4.1", "gpt-4.1-mini" };
        static string _model = "gpt-4o-mini";
        static int _maxTokens = 800;
        static double _temperature = 0.99;

        const string HistoryFile = "chat_history.json";
        const string SysPromptFile = "SysPrompt.txt";

        static string _apiKey = Secrets.OpenAI_ApiKey;
        const string OpenAiApiUrl = "https://api.openai.com/v1/chat/completions";

        public static async Task<string> GetAnswer(string userMessage)
        {
            var commandResponse = ProcessCommands(userMessage);
            if (commandResponse.Length > 0) return commandResponse;

            // Добавление системного промпта перед отправкой запроса
            var systemPrompt = new Message
            {
                Role = "system",
                Content = LoadSysPrompt()
            };

            List<Message> conversationHistory = LoadHistory();
            conversationHistory.Add(new(){ Role = "user", Content = userMessage });

            var messagesToSend = new List<Message> { systemPrompt };
            messagesToSend.AddRange(conversationHistory);

            var request = new ChatRequest
            {
                Model = _model,
                Messages = messagesToSend,
                MaxTokens = _maxTokens,
                Temperature = _temperature
            };

            var answer = await MakeRequest(request);

            conversationHistory.Add(new Message { Role = "assistant", Content = answer });
            SaveHistory(conversationHistory);

            return answer;
        }

        static async Task<string> MakeRequest(ChatRequest request, CancellationToken cancellationToken = default)
        {
            HttpClient httpClient = new();

            var options = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };
            var json = JsonSerializer.Serialize(request, options);
            using var content = new StringContent(json, Encoding.UTF8, "application/json");

            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _apiKey);
            using var response = await httpClient.PostAsync(OpenAiApiUrl, content, cancellationToken);

            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync(cancellationToken);
                throw new Exception($"OpenAI API error ({response.StatusCode}): {error}");
            }

            var jsonResponse = await response.Content.ReadAsStringAsync(cancellationToken);
            var result = JsonSerializer.Deserialize<ChatResponse>(jsonResponse, options);
            var answer = result?.Choices?.FirstOrDefault()?.Message?.Content?.Trim() ?? "Ответ не получен.";

            return answer;
        }

        static List<Message> GetDefaultHistory()
        {
            return new List<Message>
            {
                new Message
                {
                    Role = "system",
                    Content = File.ReadAllText("SysPrompt.txt")
                }
            };
        }

        static void SaveHistory(List<Message> conversationHistory)
        {
            var options = new JsonSerializerOptions { WriteIndented = true };
            File.WriteAllText(HistoryFile, JsonSerializer.Serialize(conversationHistory, options));
        }

        static List<Message> LoadHistory()
        {
            try
            {
                if (File.Exists(HistoryFile))
                {
                    var json = File.ReadAllText(HistoryFile);
                    return JsonSerializer.Deserialize<List<Message>>(json) ?? GetDefaultHistory();
                }
            }
            catch
            {
                File.WriteAllText("log.txt", "Error loading history");
            }
            return GetDefaultHistory();
        }

        static string LoadSysPrompt()
        {
            try
            {
                if (File.Exists(SysPromptFile))
                {
                    var p = File.ReadAllText(SysPromptFile, Encoding.UTF8);
                    return p;
                }
            }
            catch
            {
                File.WriteAllText("log.txt", "Error loading prompt");
            }
            return "Системный промпт не найден.";
        }

        static string ProcessCommands(string userMessage)
        {
            // сохраняем оригинал, чтобы не потерять регистр
            var raw = userMessage;

            // работаем с обрезанной и приведённой к нижнему регистру копией
            var cmd = raw.TrimStart().ToLowerInvariant();

            if (cmd == "vvv" || cmd == "ver" || cmd == "вер")
                return $"Версия: {Assembly.GetExecutingAssembly().GetName().Version}";

            if (cmd.StartsWith("setmodel "))
            {
                var model = userMessage.Split(' ')[1];
                if (_models.Contains(model))
                {
                    _model = model;
                    return $"Модель изменена на {_model}";
                }
                else
                {
                    return $"Недопустимая модель: {model}";
                }
            }
            if (cmd == "getmodel")
            {
                return $"Текущая модель: {_model}";
            }
            if (cmd == "gettemp")
            {
                return $"Температура: {_temperature}";
            }
            if (cmd.StartsWith("settemp "))
            {
                var temp = cmd.Split(' ')[1];
                if (double.TryParse(temp, out double temperature) && temperature > 0 && temperature < 1)
                {
                    _temperature = temperature;
                    return $"Температура изменена на {_temperature}";
                }
                else
                {
                    return $"Недопустимая температура: {temp}";
                }
            }
            if (cmd == "getmaxtokens")
            {
                return $"Максимальное количество токенов: {_maxTokens}";
            }
            if (cmd.StartsWith("setmaxtokens "))
            {
                var maxTokens = cmd.Split(' ')[1];
                if (int.TryParse(maxTokens, out int maxTokensValue) && maxTokensValue > 0 && maxTokensValue < 5000)
                {
                    _maxTokens = maxTokensValue;
                    return $"Максимальное количество токенов изменено на {_maxTokens}";
                }
                else
                {
                    return $"Недопустимое значение: {maxTokens}";
                }
            }
            if(cmd == "getmodels")
            {
                return $"Доступные модели: {string.Join(", ", _models)}";
            }
            if(cmd == "/getperson")
            {
                var person = LoadSysPrompt();
                return person;
            }
            if (cmd.StartsWith("/setperson:"))
            {
                try { 
                    var person = userMessage.Substring(12);
                    File.WriteAllText(SysPromptFile, person, Encoding.UTF8);
                    return "Системный промпт обновлён.";
                }
                catch (Exception ex)
                {
                    return $"Ошибка при обновлении системного промпта: {ex.Message}";
                }
            }
            return "";
        }
    }

    public class ChatRequest
    {
        [JsonPropertyName("model")]
        public string? Model { get; set; }

        [JsonPropertyName("messages")]
        public List<Message>? Messages { get; set; }

        [JsonPropertyName("max_tokens")]
        public int MaxTokens { get; set; }

        [JsonPropertyName("temperature")]
        public double Temperature { get; set; }
    }

    public class ChatResponse
    {
        public List<Choice>? Choices { get; set; }
    }

    public class Choice
    {
        public Message? Message { get; set; }
    }

    public class Message
    {
        public string? Role { get; set; }
        public string? Content { get; set; }
    }
}
