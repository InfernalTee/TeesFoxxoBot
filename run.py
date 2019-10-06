from teesfoxxobot.logger import log
from teesfoxxobot.config import API_KEY
from teesfoxxobot.handlers import on_command_help
from telegram.ext import (
    Updater, InlineQueryHandler, MessageHandler, CommandHandler, Filters
)


def start():
    """Connects to Telegram and starts the bot."""

    bot = Updater(token=API_KEY)
    dp = bot.dispatcher

    # TODO register handlers here
    dp.add_handler(CommandHandler("help", on_command_help))

    log.info("Bot ready, dood! Connected as {username} (with ID {id}).".format(
        username=bot.bot.username,
        id=bot.bot.id
    ))
    bot.start_polling()
    bot.idle()


if __name__ == '__main__':
    start()
