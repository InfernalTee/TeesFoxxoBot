using System;
using System.Linq;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Args;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace FoxxoBot
{
    class Program
    {
        private const string API_KEY = "";
        private static readonly TelegramBotClient Bot = new TelegramBotClient(API_KEY);

        public static void Main(string[] args)
        {
            Bot.OnMessage += BotOnMessageReceived;
            var botClient = new TelegramBotClient(API_KEY);
            var me = botClient.GetMeAsync().Result;
            Console.Title = me.Username;
            Bot.StartReceiving(Array.Empty<UpdateType>());
            Console.WriteLine($"Start listening for @{me.Username}");
            Console.ReadLine();
            Bot.StopReceiving();
            Console.ReadKey();

        }

        /// <summary>
        /// Given a ChatId, returns all admins in the chat.
        /// Returns 
        /// </summary>
        private static async Task<User[]> GetAdminsOfChannel(ChatId chatId)
        {
            return (await Bot?.GetChatAdministratorsAsync(chatId) ?? new ChatMember[0])?
                .Select(chatMember => chatMember.User)
                .ToArray();
        }

        /// <summary>
        /// Controls how the bot will handle messages that it receieves (DMs, chat messages in
        /// chats that it is a member of, etc.). Hooks into TelegramBotClient.OnMessage event handler.
        /// </summary>
        private static async void BotOnMessageReceived(object sender, MessageEventArgs messageEventArgs)
        {
            // Parse metadata based on where the message was sent.
            var message = messageEventArgs.Message;

            // Abort if the message is empty or isn't text.
            if (message?.Type != MessageType.Text) return;

            // Who sent the message?
            var pinger = message.From;

            // When was the message sent (in UTC time)?
            var messageSendTime = message.Date;

            // TODO: If messageSendTime is before we were turned on, abort.

            switch (message.Text.Split(' ').First())
            {
                case "/admin":
                    // TODO: If messageSendTime is less than 5 seconds after /admin was called
                    // TODO: last time, abort.

                    // Who are the admins in the channel the message was sent from?
                    var adminsInChannel = await GetAdminsOfChannel(message.Chat.Id);

                    Console.WriteLine($"/admin called in {message.Chat.Title} ({message.Chat.Id})");
                    await Bot.SendTextMessageAsync(message.Chat.Id, $"Admins @{adminsInChannel}, your service was requested by a {pinger}!");
                    break;
                default:
                    // Do nothing.
                    break;
            }
        }
    }
}

