using System;
using System.Linq;
using System.Configuration;
using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Args;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace FoxxoBot
{
    class Program
    {
        /// <summary>
        /// The API key used to authenticate the bot against the Telegram API.
        /// </summary>
        private static readonly string API_KEY = ConfigurationManager.AppSettings["API_KEY"];

        /// <summary>
        /// The Telegram.Bot API bot client.
        /// </summary>
        private static TelegramBotClient Bot;

        /// <summary>
        /// The bot's user object (returned by the API).
        /// </summary>
        private static User BotUser;

        /// <summary>
        /// The entrypoint to the program.
        /// </summary>
        public static void Main(string[] args)
        {
            // Define how the app is quit (Ctrl+C)
            var exitEvent = new ManualResetEvent(false);
            Console.CancelKeyPress += (sender, eventArgs) => {
                eventArgs.Cancel = true;
                exitEvent.Set();
            };

            // Quit immediately if the API key isn't filled in
            // or we can't connect to Telegram for some reason
            if (!ApiKeyIsValid() || !ConnectBotToTelegram()) {
                Console.WriteLine("(Aborted. Press ENTER to quit...)");
                Console.ReadLine();
                return;
            }

            Console.WriteLine($"Connected as {BotUser.Username} ({BotUser.Id}).");

            // Hook up event handlers
            Bot.OnMessage += BotOnMessageReceived;
            Bot.StartReceiving(Array.Empty<UpdateType>());

            Console.WriteLine("Bot ready! Press <Ctrl+C> or close this window to shut it down.");

            // When Ctrl+C is pressed...
            exitEvent.WaitOne();
            Console.WriteLine("Exit signal received! Shutting down!");
            Bot.StopReceiving();
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

        /// <summary>
        /// Returns true if the API key is filled in.
        /// If not, logs the issue to the console and returns false.
        /// </summary>
        private static bool ApiKeyIsValid() {
            if (string.IsNullOrWhiteSpace(API_KEY)) {
                Console.Write("ERROR: Couldn't find a value for the API key. ");
                Console.WriteLine("Please fill in the value in App.config and restart the bot. ");
                return false;
            }

            return true;
        }

        /// <summary>
        /// Attempts to connect to the Telegram API and populate Bot and BotUser.
        /// Returns true if successful.
        /// Logs the exception and returns false if unsuccessful.
        /// </summary>
        private static bool ConnectBotToTelegram() {
            try {
                Bot = new TelegramBotClient(API_KEY);
                BotUser = Task.Run(async () => await Bot.GetMeAsync()).Result;
            } catch (Exception e) {
                Console.WriteLine($"ERROR! Issue connecting to Telegram API: {e}. Aborting.");
                return false;
            }
            
            return true;
        }
    }
}

