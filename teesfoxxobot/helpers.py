def get_meta_from_update(update) -> tuple:
    """Returns username, user_id (str), and the message text from an update."""

    query_username = (
        update.effective_user.username if update.effective_user
        else ''
    )
    query_user_id = (
        str(update.effective_user.id) if update.effective_user
        else ''
    )
    query_text = (
        update.inline_query.query if update.inline_query
        else update.message.text
    )

    return query_username, query_user_id, query_text
