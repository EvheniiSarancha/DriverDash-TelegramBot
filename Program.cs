using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using Telegram.Bot;
using Telegram.Bot.Args;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;
using HtmlAgilityPack;

class Program
{
    private static TelegramBotClient botClient;
    private const string BotToken = "6108221048:AAHLZ-4UNP_SxdvwSaqR-r_u-mqgEEJKfU0";

    static void Main()
    {
        botClient = new TelegramBotClient(BotToken);
        botClient.OnMessage += Bot_OnMessage;
        botClient.StartReceiving();

        Console.WriteLine("Telegram bot is running...");
        Console.ReadLine();

        botClient.StopReceiving();
    }

    private static void Bot_OnMessage(object sender, MessageEventArgs e)
    {
        if (e.Message.Type == Telegram.Bot.Types.Enums.MessageType.Text)
        {
            if (e.Message.Text == "/start")
            {
                SendMenuMessage(e.Message);
            }
            else if (e.Message.Text == "Latest NVIDIA Driver")
            {
                SendLatestDriverMessage(e.Message.Chat.Id);
            }
            else if (e.Message.Text == "Supported GPUs")
            {
                SendSupportedGPUsMessage(e.Message.Chat.Id);
            }
        }
    }

    private static void SendMenuMessage(Message message)
    {
        ReplyKeyboardMarkup replyKeyboardMarkup = new ReplyKeyboardMarkup();
        replyKeyboardMarkup.Keyboard = new KeyboardButton[][]
        {
            new KeyboardButton[] { new KeyboardButton("Check NVIDIA Driver version") },
            new KeyboardButton[] { new KeyboardButton("Check supported GPUs") }
        };
        replyKeyboardMarkup.OneTimeKeyboard = true;
        replyKeyboardMarkup.ResizeKeyboard = true;

        botClient.SendTextMessageAsync(message.Chat.Id, "Please select an option:", replyMarkup: replyKeyboardMarkup);
    }

    private static void SendLatestDriverMessage(long chatId)
    {
        string version = GetLatestDriverVersion();
        string size = GetDriverSize();
        string message = $"Latest NVIDIA Driver Version: {version}\nSize: {size}";

        botClient.SendTextMessageAsync(chatId, message);
    }

    private static void SendSupportedGPUsMessage(long chatId)
    {
        string supportedGPUs = GetSupportedGPUs();
        string message = $"Supported GPUs:\n\n{supportedGPUs}";

        botClient.SendTextMessageAsync(chatId, message);
    }

    public static string GetLatestDriverVersion()
    {
        string url = "https://www.techpowerup.com/download/nvidia-geforce-graphics-drivers/";

        HtmlWeb web = new HtmlWeb();
        HtmlDocument document = web.Load(url);

        HtmlNode driverNode = document.DocumentNode.SelectSingleNode("//a[@class='s--drivers__driver s--drivers__driver--nvidia']");
        string versionText = driverNode.InnerHtml;

        string versionNumber = ExtractVersionNumber(versionText);

        return versionNumber;
    }

    public static string GetDriverSize()
    {
        string url = "https://www.techpowerup.com/download/nvidia-geforce-graphics-drivers/";

        HtmlWeb web = new HtmlWeb();
        HtmlDocument document = web.Load(url);

        HtmlNode fileSizeNode = document.DocumentNode.SelectSingleNode("//div[@class='filesize']");
        string fileSizeText = fileSizeNode.InnerText.Trim();

        return fileSizeText;
    }

    public static string ExtractVersionNumber(string versionText)
    {
        int startIndex = versionText.IndexOf("NVIDIA GeForce") + "NVIDIA GeForce".Length;
        int endIndex = versionText.IndexOf(" WHQL");

        if (startIndex >= 0 && endIndex > startIndex)
        {
            return versionText.Substring(startIndex, endIndex - startIndex).Trim();
        }

        return string.Empty;
    }

    public static string GetSupportedGPUs()
    {
        string url = "https://www.techpowerup.com/download/nvidia-geforce-graphics-drivers/";

        HtmlWeb web = new HtmlWeb();
        HtmlDocument document = web.Load(url);

        HtmlNode gpuListNode = document.DocumentNode.SelectSingleNode("//div[@class='desc p']//ul");
        var gpuNodes = gpuListNode.SelectNodes("li");

        StringBuilder sb = new StringBuilder();
        foreach (var gpuNode in gpuNodes)
        {
            string gpuName = gpuNode.InnerText.Trim();
            sb.AppendLine(gpuName);
        }

        return sb.ToString();
    }
}
