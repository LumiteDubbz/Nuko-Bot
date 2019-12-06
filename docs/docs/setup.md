# Setup

This setup tutorial is intended for server ("guild") owners to follow. If you want to learn how to setup the bot for self-hosting, please check out [this]("https://youtu.be/-i73vLhh6Gk") video link.

Once you've added the bot to your server (using [this]("https://discordapp.com/oauth2/authorize?client_id=636923604277395456&scope=bot&permissions=8") link), you'll need to set up some things for the bot to work properly.

**Arguments in `<angle brackets>` are required, those in `[square brackets]` are optional.**

Add the muted role: `>SetMutedRole <@mutedRole>`. This role should be above all other user roles, have the send messages permission disabled in both the rule permissions and channel/category overwrites.

Add moderator roles: `>AddModRole <@moderatorRole> <permissionLevel>`. The valid permission levels are 1, 2 and 3. 1 is for Moderators (access to Mute, Kick, etc.), 2 is for Administrators (access to Ban, SetWelcomeMessage, etc.) and 3 provides access to all commands. Users with the Discord "Administrator" role permission are automatically given access to level 2 commands.

Add the channel to log all moderation actions to: `>SetModLog <#modLogChannel>`. Any actions done by moderators, administrators and owners (such as chat `>Clear`s, `>Kick`s, etc.) will be recorded in this channel.

`>Mute`s are indefinite or until a moderator manually `>Unmute`s the user. To mute a person for a set amount of time, use `>CustomMute <@userToMute> <hours> [reason]` and Nuko will automatically unmute them after their time is up.