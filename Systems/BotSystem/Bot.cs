using System;
using System.Collections.Generic;
using System.Linq;
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
    public static SocketGuild discordServer;
    public static IMessageChannel staffGeneralChannel;
    public static IMessageChannel playerGeneralChannel;
    public static IMessageChannel logChannel;
    public static SocketUser chimDiscordUser;
    public static SocketUser bigbyDiscordUser;

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

      commandService = new CommandService();
      
      _client.Log += Log;
      commandService.Log += Log;

      // Setup your DI container.
      _services = ConfigureServices();

      var config = new DiscordSocketConfig()
      {
        // Other config options can be presented here.
        GatewayIntents = GatewayIntents.All
      };

      _client = new DiscordSocketClient(config);      
      
      // Centralize the logic for commands into a separate method.
      await InitCommands();

      await _client.LoginAsync(TokenType.Bot, Environment.GetEnvironmentVariable("BOT"));
      await _client.StartAsync();

      //_client.Ready += CreateSlashGlobalCommands;
      _client.SlashCommandExecuted += SlashCommandHandler;
      _client.Ready += OnDiscordConnected;
      _client.GuildMembersDownloaded += OnUsersDownloaded;
      _client.UserJoined += UpdateUserList;
      _client.UserLeft += UpdateUserListOnLeave;
      _client.Disconnected += OnDiscordDisconnected;

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
      await _client.DownloadUsersAsync(new List<IGuild> { { discordServer } });
      await data.SendMessageAsync($"Bonjour {data.DisplayName} et bienvenue sur le Discord des Larmes des Erylies.\n\n Pour commencer, n'hésite pas à consulter notre livre du joueur : \n https://docs.google.com/document/d/1ammPGnH-sVjNHnJHCMAm_khbe8mBqTPFeCfqCvJt7ig/edit?usp=sharing");
    }
    private static async Task UpdateUserListOnLeave(SocketGuild guild, SocketUser user)
    {
      await _client.DownloadUsersAsync(new List<IGuild> { { discordServer } });
    }
    private static async Task OnDiscordConnected()
    {
      await _client.DownloadUsersAsync(new List<IGuild> { { _client.GetGuild(680072044364562528) } });
    }
    private static async Task OnDiscordDisconnected(Exception e)
    {
      ModuleSystem.Log.Info($"WARNING - Discord Disconnected - Error :\n\n{e.Message}\n{e.StackTrace}");
    }
    private static async void HandleDiscordReconnect()
    {
      ModuleSystem.Log.Info("Trying to reconnect");

      await _client.LoginAsync(TokenType.Bot, Environment.GetEnvironmentVariable("BOT"));
      await _client.StartAsync();

      //if (_client.ConnectionState != ConnectionState.Connected)


    }
    private static async Task OnUsersDownloaded(SocketGuild server)
    {
      discordServer = server;
      staffGeneralChannel = _client.GetChannel(680072044364562532) as IMessageChannel;
      playerGeneralChannel = _client.GetChannel(786218144296468481) as IMessageChannel;
      logChannel = _client.GetChannel(703964971549196339) as IMessageChannel;

      chimDiscordUser = _client.GetUser(232218662080086017);
      bigbyDiscordUser = _client.GetUser(225961076448034817);

      Utils.LogMessageToDMs("Module en ligne !");

      try
      {
        var guildCommand = new SlashCommandBuilder()
        .WithName("animations")
        .WithDescription("Informations générales sur les animations sur le module");
        /*var guildCommand = new SlashCommandBuilder()
        .WithName("test")
        .WithDescription("test argument")
        .AddOption("arg", ApplicationCommandOptionType.Integer, "le test de notre argument", isRequired: true)
        .WithDefaultMemberPermissions(GuildPermission.Administrator);*/

        var slash = guildCommand.Build();
        await discordServer.CreateApplicationCommandAsync(slash);
      }
      catch (Exception exception)
      {
        Utils.LogMessageToDMs(exception.Message + exception.StackTrace);
      }
    }
    private static async Task SlashCommandHandler(SocketSlashCommand command)
    {
      switch(command.CommandName)
      {
        default:
          await command.RespondAsync("Commande non reconnue, veuillez vérifier votre saisie.", ephemeral: true);
          break;
        case "info_developpement":
          await command.RespondAsync("Voici la liste des prochains développements prévus sur le module :\nhttps://docs.google.com/document/d/1F0LMG8QjVld4dT1HpKf1ALe3WfVVlKZbawKmQMAHASM/edit?usp=sharing");
          break;
        case "info_backlog":
          await command.RespondAsync("La liste des futurs projets à prioriser :\nhttps://docs.google.com/document/d/1pudZjfhxLwIeHzHZlLHozH2qiquvIi-sXrhfRRLGpuU/edit?usp=sharing");
          break;
        case "info_alignement":
          await command.RespondAsync("Informations générales concernant les alignements sur le module :\nhttps://docs.google.com/document/d/159t2E4TsLkKgrN_ix2IhfdDQWUJm-C-5-57ePSyx09o/edit?usp=sharing");
          break;
        case "info_animations":
          await command.RespondAsync("Informations générales concernant les animations sur le module :\nhttps://docs.google.com/document/d/1Dzo6fCjf-JNvTR6pATbk-6lLGxCYpzG-Vk8O4hCXaZg/edit?usp=sharing");
          break;
        case "info_bonus_investissement":
          await command.RespondAsync("Informations générales concernant le bonus d'investissement :\nhttps://docs.google.com/document/d/1pobNknqQ0QG-_pBQxBz0X5wAOkdiqC13eCUGyVr5CQE/edit?usp=sharing");
          break;
        case "info_mort":
          await command.RespondAsync("Informations générales concernant la mort sur le module :\nhttps://docs.google.com/document/d/1kzOeWtOgnIvk4Jw6vmtD_xVWIU2GUmfvdZ4h8Z2BDAY/edit?usp=sharing");
          break;
        case "info_jets_de_dés":
          await command.RespondAsync("Informations générales concernant la mort sur le module :\nhttps://docs.google.com/document/d/1IBaRsSZIE3Zg2GJ0SSBNY6-dR2xoTmYYmEhlA95WEfs/edit?usp=sharing");
          break;
        case "info_evil":
          await command.RespondAsync("Informations générales concernant le Role Play des personnages d'alignement mauvais :\nhttps://docs.google.com/document/d/1eCqJ2G27RFEwVBOrnei4KDEnbGsmvzghqtqz3w6vPbQ/edit?usp=sharing");
          break;
        case "info_soins":
          await command.RespondAsync("Informations générales concernant les blessures et les soins :\nhttps://docs.google.com/document/d/12yT3ZvLtGXVteW29JTJxpQ6GRpQ3ArWUeCvXAbbCM4o/edit?usp=sharing");
          break;
        case "test":
          await command.RespondAsync($"Test d'argument, vous avez envoyé : {command.Data.Options.First().Value}", ephemeral: true);
          break;
        case "mail_supprimer":
          await BotSystem.ExecuteDeleteMailCommand(command);
          break;
        case "rumeur_pj_supprimer": // droit général
          await BotSystem.ExecuteDeleteMyRumorCommand(command);
          break;
        case "rumeur_supprimer": // droit staff et admin
          await BotSystem.ExecuteDeleteRumorCommand(command);
          break;
      }
    }
  }
}
