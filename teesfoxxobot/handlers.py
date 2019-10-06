from teesfoxxobot.logger import log
from teesfoxxobot.helpers import get_meta_from_update
from telegram import ParseMode, ChatAction


def on_command_help(bot, update):
    """
    Provides the user with information about the bot.
    """

    HELP_COMMAND_TEXT = (
        "Fill in!! Your help message here!!" 
    )

    username, user_id, message_text = get_meta_from_update(update)

    bot.send_chat_action(user_id, ChatAction.TYPING)
    bot.sendMessage(
        chat_id=user_id, parse_mode=ParseMode.MARKDOWN,
        text=HELP_COMMAND_TEXT
    )
