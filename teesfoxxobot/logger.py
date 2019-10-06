from logging import getLogger, StreamHandler, FileHandler, DEBUG
from sys import stdout


log = getLogger(__name__)
log.setLevel(DEBUG)
log.addHandler(StreamHandler(stdout))
log.addHandler(FileHandler('foxxo_bot.log'))
