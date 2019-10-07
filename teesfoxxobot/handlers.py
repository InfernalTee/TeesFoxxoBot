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


def on_admin(bot, update):
    """
    Tells the bot how to respond to the /admin command.
    """

    log.info("Handling on_admin")
    username, user_id, message_text = get_meta_from_update(update)
    admins = update.effective_chat.get_administrators()
    admin_usernames = [
        admin.user.name for admin in admins
    ]
    update.message.reply_text(f"Yo! Admins of this channel are: {', '.join(admin_usernames)}")
