using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;

namespace NWN.Systems
{
  public static partial class Bot
  {
    public const char prefix = '!';
    public static DiscordSocketClient _client;

    // Keep the CommandService and DI container around for use with commands.
    // These two types require you install the Discord.Net.Commands package.
    public static CommandService commandService;
    private static IServiceProvider _services;

    public static IEnumerable<CommandInfo> GetCommands()
    {
      return commandService.Commands;
    }

    public static async Task MainAsync()
    {
      _client = new DiscordSocketClient();
      await _client.DownloadUsersAsync(new List<IGuild> { { _client.GetGuild(680072044364562528) } });

      commandService = new CommandService();

      _client.Log += Log;
      commandService.Log += Log;

      // Setup your DI container.
      _services = ConfigureServices();

      var cfg = new DiscordSocketConfig();
      cfg.GatewayIntents |= GatewayIntents.GuildMembers;

      // Centralize the logic for commands into a separate method.
      await InitCommands();

      var token = Environment.GetEnvironmentVariable("BOT");

      await _client.LoginAsync(TokenType.Bot, token);
      await _client.StartAsync();

      _client.UserJoined += UpdateUserList;
      _client.UserLeft += UpdateUserList;

      await Task.Delay(TimeSpan.FromSeconds(5));

      if(Config.env == Config.Env.Prod)
        await (_client.GetChannel(786218144296468481) as IMessageChannel).SendMessageAsync($"Module en ligne !");

      // Block this task until the program is closed.
      await Task.Delay(Timeout.Infinite);
    }
    private static Task Log(LogMessage message)
    {
      switch (message.Severity)
      {
        case LogSeverity.Critical:
        case LogSeverity.Error:
          Console.ForegroundColor = ConsoleColor.Red;
          break;
        case LogSeverity.Warning:
          Console.ForegroundColor = ConsoleColor.Yellow;
          break;
        case LogSeverity.Info:
          Console.ForegroundColor = ConsoleColor.White;
          break;
        case LogSeverity.Verbose:
        case LogSeverity.Debug:
          Console.ForegroundColor = ConsoleColor.DarkGray;
          break;
      }
      Console.WriteLine($"{DateTime.Now,-19} [{message.Severity,8}] {message.Source}: {message.Message} {message.Exception}");
      Console.ResetColor();

      // If you get an error saying 'CompletedTask' doesn't exist,
      // your project is targeting .NET 4.5.2 or lower. You'll need
      // to adjust your project's target framework to 4.6 or higher
      // (instructions for this are easily Googled).
      // If you *need* to run on .NET 4.5 for compat/other reasons,
      // the alternative is to 'return Task.Delay(0);' instead.
      return Task.CompletedTask;
    }
    // If any services require the client, or the CommandService, or something else you keep on hand,
    // pass them as parameters into this method as needed.
    // If this method is getting pretty long, you can seperate it out into another file using partials.
    private static IServiceProvider ConfigureServices()
    {
      var map = new ServiceCollection()
          // Repeat this for all the service classes
          // and other dependencies that your commands might need.
          .AddSingleton(new CommandService());

      // When all your required services are in the collection, build the container.
      // Tip: There's an overload taking in a 'validateScopes' bool to make sure
      // you haven't made any mistakes in your dependency graph.
      return map.BuildServiceProvider();
    }
    private static async Task InitCommands()
    {
      // Either search the program and add all Module classes that can be found.
      // Module classes MUST be marked 'public' or they will be ignored.
      // You also need to pass your 'IServiceProvider' instance now,
      // so make sure that's done before you get here.
      //      await commandService.AddModulesAsync(Assembly.GetEntryAssembly(), _services);
      // Or add Modules manually if you prefer to be a little more explicit:
      await commandService.AddModuleAsync<InfoModule>(_services);
      // Note that the first one is 'Modules' (plural) and the second is 'Module' (singular).

      // Subscribe a handler to see if a message invokes a command.
      _client.MessageReceived += HandleCommandAsync;
    }
    private static async Task HandleCommandAsync(SocketMessage arg)
    {
      // Bail out if it's a System Message.
      var msg = arg as SocketUserMessage;
      if (msg == null) return;

      // We don't want the bot to respond to itself or other bots.
      if (msg.Author.Id == _client.CurrentUser.Id || msg.Author.IsBot) return;

      // Create a number to track where the prefix ends and the command begins
      int pos = 0;
      // Replace the '!' with whatever character
      // you want to prefix your commands with.
      // Uncomment the second half if you also want
      // commands to be invoked by mentioning the bot instead.
      if (msg.HasCharPrefix(prefix, ref pos) /* || msg.HasMentionPrefix(_client.CurrentUser, ref pos) */)
      {
        // Create a Command Context.
        var context = new SocketCommandContext(_client, msg);

        // Execute the command. (result does not indicate a return value, 
        // rather an object stating if the command executed successfully).
        var result = await commandService.ExecuteAsync(context, pos, _services);

        // Uncomment the following lines if you want the bot
        // to send a message if it failed.
        // This does not catch errors from commandService with 'RunMode.Async',
        // subscribe a handler for 'commandService.CommandExecuted' to see those.
        if (!result.IsSuccess)
        {
          if (result.Error == CommandError.UnknownCommand)
          {
            await msg.Channel.SendMessageAsync("La commande indiquée n'existe pas.\nVeuillez taper \"!help\" pour obtenir la liste des commandes disponibles.");
          }
          else
          {
            await msg.Channel.SendMessageAsync(result.ErrorReason);
          }
        }
      }
    }

    private static async Task UpdateUserList(SocketGuildUser data)
    {
      await _client.DownloadUsersAsync(new List<IGuild> { { _client.GetGuild(680072044364562528) } });
    }
  }
}
