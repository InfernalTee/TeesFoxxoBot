using System;
using System.Linq;
using System.Configuration;
using System.Collections.Generic;
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
        /// If set, pauses (and waits for ENTER key) before shutting down for any reason.
        /// </summary>
        private static readonly bool INTERACTIVE = Convert.ToBoolean(ConfigurationManager.AppSettings["INTERACTIVE"]);

        /// <summary>
        /// The bot should not respond to messages sent before it was switched on.
        /// 
        /// This constant establishes a deadline by which messages should be sent through
        /// the API to the bot to enforce ignoring previous messages.
        /// </summary>
        private const int HANDLER_THRESHOLD_SECS = 3;

        /// <summary>
        /// Amount of time to wait between each chat for each call to /admin to be obeyed.
        /// </summary>
        private const int ADMIN_COMMAND_TIMEOUT_SECS = 5;

        /// <summary>
        /// The Telegram.Bot API bot client.
        /// </summary>
        private static TelegramBotClient Bot;

        /// <summary>
        /// The bot's user object (returned by the API).
        /// </summary>
        private static User BotUser;

        /// <summary>
        /// For each chat, stores the last time the /admin command was called.
        /// </summary>
        private static IDictionary<long, DateTime> LastAdminCommandTimePerChat = new Dictionary<long, DateTime>();

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
                if (INTERACTIVE) {
                    Console.WriteLine("(Aborted. Press ENTER to quit...)");
                    Console.ReadLine();
                }
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
        /// Controls how the bot will handle messages that it receieves (DMs, chat messages in
        /// chats that it is a member of, etc.). Hooks into TelegramBotClient.OnMessage event handler.
        /// </summary>
        private static async void BotOnMessageReceived(object sender, MessageEventArgs messageEventArgs)
        {
            // Parse metadata based on where the message was sent.
            var message = messageEventArgs.Message;

            // Abort if the message is empty or isn't text.
            if (message?.Type != MessageType.Text) return;

            // Abort if the message was sent before we were switched on.
            if (message.Date.AddSeconds(HANDLER_THRESHOLD_SECS) < DateTime.UtcNow) return;

            // Abort if the message is not a command (starts with /)
            var command = message.Text.Split(' ').First();
            if (string.IsNullOrEmpty(command) || !command.StartsWith('/')) return;

            switch (command)
            {
                case "/admin":
                    // If messageSendTime is less than 5 seconds after /admin was called last time, abort.
                    if (LastAdminCommandTimePerChat.ContainsKey(message.Chat.Id)) {
                        if (
                            LastAdminCommandTimePerChat[message.Chat.Id]
                                .AddSeconds(ADMIN_COMMAND_TIMEOUT_SECS)
                            <= DateTime.UtcNow
                        ) return;
                    }

                    // Otherwise, log the current time as the last time /admin was called for this chat.
                    LastAdminCommandTimePerChat[message.Chat.Id] = DateTime.UtcNow;

                    // Who are the admins in the channel the message was sent from?
                    var adminsInChannel = await GetAdminsOfChannel(message.Chat.Id);

                    Console.WriteLine($"/admin called in {message.Chat.Title} ({message.Chat.Id}) at {DateTime.Now.ToString()}");
                    await Bot.SendTextMessageAsync(
                        message.Chat.Id, (
                            $"Paging all admins: " +
                            adminsInChannel.Select(user => $"@{user.Username}")
                        ));
                    break;
                default:
                    // Do nothing.
                    break;
            }
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

