# Tee's Foxxobot (for Telegram)!
When pinged with `/admin`, it will alert all administrators of the chat it was pinged in.
To use it, add `@FoxxyFluffBot` to your chat on Telegram.

#### Made by these wonderful people
<img src="tee.jpg" width="100" align="left"/><br />
<p align="left">
    <b>Tee</b><br />
    - <a href="https://t.me/InfernalTee">Telegram</a> 
    - <a href="https://github.com/InfernalTee">GitHub</a>
</p>

<img src="jinhai.jpg" width="100" align="left"/><br />
<p align="left">
    <b>Jinhai</b><br />
    - <a href="https://t.me/Jinhai">Telegram</a> 
    - <a href="https://github.com/OzuYatamutsu">GitHub</a>
</p>

<img src="kourii.jpg" width="100" align="left"/><br />
<p align="left">
    <b>Kourii</b><br />
    - <a href="https://t.me/KouriiRaiko">Telegram</a> 
    - <a href="https://github.com/KouriiRaiko">GitHub</a>
</p>


### To install
Foxxobot is a .NET Core 3 app, written in C#. It requires the .NET Core 3 SDK (or newer).
Fill in the value of `API_KEY` in `App.config` with the bot's API token before running.

You will also need a channel to send the alerts to, which your administrators should subscribe to.
Fill in the value of `CHANNEL_ID` in `App.config` with the ID of your channel.

#### From the CLI
Run `dotnet restore` from the project root to resolve all dependencies.

#### From Visual Studio Code or Visual Studio
Open the folder as a solution as normal.
