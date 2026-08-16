namespace AmSecrets;

public class Secrets
{
    static public string ChatTitle
    {
        get
        {
            return "Андрей прогер...";
        }
    }
    static public string PathToChatFace
    {
        get
        {
            string path = "D:\\Argo\\argoai.ru\\chat-popup-widget\\src\\assets\\face1.jpg";
            if (File.Exists(path))
            {
                return path;
            }
            else
                throw new Exception("File with Chat Face is not found");
        }
    }

    static public string SqlConnectionString
    {
        get
        {
            string cs = "";
            string path = "D:\\Projects\\Common\\Secrets\\AIChat_SqlConnectionString.txt";
            if (File.Exists(path))
            {
                cs = File.ReadAllText(path);
            }
            else
                throw new Exception("File with Sql Connection is not found");

            return cs;
        }
    }
    static public string OpenAI_ApiKey
    {
        get
        {
            string cs = "";
            string path = "D:\\Projects\\Common\\Secrets\\AIChat_OpenAI_ApiKey.txt";
            if (File.Exists(path))
            {
                cs = File.ReadAllText(path);
            }
            else
                throw new Exception("FileAIChat_OpenAI_ApiKey is not found");
            return cs;
        }
    }
    static public string GmailAppPassword
    {
        get
        {
            string cs = "";
            string path = "D:\\Projects\\Common\\Secrets\\site.ultrazoom.ru.gmail.password.txt";
            if (File.Exists(path))
            {
                cs = File.ReadAllText(path);
            }
            else
                throw new Exception("File site.ultrazoom.ru.gmail.password is not found");
            return cs;
        }
    }
    static public string GmailAppSender
    {
        get
        {
            string cs = "";
            string path = "D:\\Projects\\Common\\Secrets\\site.ultrazoom.ru.gmail.sender.txt";
            if (File.Exists(path))
            {
                cs = File.ReadAllText(path);
            }
            else
                throw new Exception("File site.ultrazoom.ru.gmail.sender is not found");
            return cs;
        }
    }
    static public string SiteUltreazoomTelebotToken
    {
        get
        {
            string cs = "";
            string path = "D:\\Projects\\Common\\Secrets\\site.ultrazoom.ru.telebot.token.txt";
            if (File.Exists(path))
            {
                cs = File.ReadAllText(path);
            }
            else
                throw new Exception("File site.ultrazoom.ru.telegram.token is not found");
            return cs;
        }
    }
    static public string MyTelegramChatId
    {
        get
        {
            string cs = "";
            string path = "D:\\Projects\\Common\\Secrets\\site.ultrazoom.ru.telegram.chatid.txt";
            if (File.Exists(path))
            {
                cs = File.ReadAllText(path);
            }
            else
                throw new Exception("File site.ultrazoom.ru.telegram.chatid is not found");
            return cs;
        }
    }
}
