using System;
using System.Threading.Tasks;
using Telegram.Bot.Framework;
using Telegram.Bot.Framework.Abstractions;
using Telegram.Bot.Types;

public class SimpleBot : BotBase
{
    public SimpleBot() : base("YOUR_BOT_TOKEN")
    {
        // Здесь вы можете добавить обработчики команд и других событий бота
    }

    public override Task HandleUpdateAsync(ITelegramBotClient botClient, Update update)
    {
        if (update.Type == UpdateType.Message)
        {
            var message = update.Message;

            // Обработка входящего текстового сообщения
            if (message.Type == MessageType.Text)
            {
                string response = "Привет, я простой телеграм-бот!";

                // Отправка ответного сообщения
                return botClient.SendTextMessageAsync(message.Chat.Id, response);
            }
        }

        return Task.CompletedTask;
    }
}

class Program
{
    static async Task Main()
    {
        var bot = new SimpleBot();
        await bot.RunAsync();
    }
}