using System.Threading.Tasks;
using Discord.Commands;

namespace BotSystem
{
    public static partial class BotCommand
    {
        public static async Task ExecuteHelpCommandAsync(SocketCommandContext context)
        {
            var msg = "";

            foreach (var command in Bot.GetCommands())
            {
                var line = $"**{Bot.prefix}{command.Name}**";

                foreach (var param in command.Parameters)
                {
                    line += $" *<{param.Name}: {param.Type.Name}>*";
                }

                line += $" - {command.Summary}\n";

                // Limit de 2000 chars par message sur discord
                if (msg.Length + line.Length > 2000)
                {
                    await context.Channel.SendMessageAsync(msg);
                    msg = "";
                }

                msg += line;
            }

            if (msg != "")
            {
                await context.Channel.SendMessageAsync(msg);
            }
        }
    }
}
