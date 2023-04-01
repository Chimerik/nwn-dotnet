using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Anvil.API;

using Discord;
using Discord.WebSocket;

namespace NWN.Systems
{
  public static partial class Bot
  {
    public static DiscordSocketClient _client;
    public static SocketForumChannel forum;
    public static SocketGuild discordServer;
    public static IMessageChannel staffGeneralChannel;
    public static IMessageChannel playerGeneralChannel;
    public static IMessageChannel logChannel;
    public static SocketUser chimDiscordUser;
    public static SocketUser bigbyDiscordUser;

    public static OverwritePermissions requestForumPermissions = new();
    public static SocketCategoryChannel forumCategory;
    public static SocketForumChannel logForum;
    public static SocketForumChannel chatLogForum;

    public static async Task MainAsync()
    {
      _client = new DiscordSocketClient();      
      _client.Log += Log;

      var config = new DiscordSocketConfig() { GatewayIntents = GatewayIntents.GuildMembers | GatewayIntents.AllUnprivileged };
      _client = new DiscordSocketClient(config);      

      await _client.LoginAsync(TokenType.Bot, Environment.GetEnvironmentVariable("BOT"));
      await _client.StartAsync();

      _client.SlashCommandExecuted += SlashCommandHandler;
      _client.Ready += OnDiscordConnected;
      _client.GuildMembersDownloaded += OnUsersDownloaded;
      _client.UserJoined += UpdateUserList;
      _client.UserLeft += UpdateUserListOnLeave;
      _client.Disconnected += OnDiscordDisconnected;

      requestForumPermissions = requestForumPermissions.Modify(PermValue.Deny, PermValue.Deny, PermValue.Allow, PermValue.Allow, PermValue.Allow, PermValue.Deny, PermValue.Deny, PermValue.Allow, PermValue.Allow, PermValue.Allow, PermValue.Deny
        , PermValue.Allow);

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

      return Task.CompletedTask;
    }
    private static async Task UpdateUserList(SocketGuildUser data)
    {
      await _client.DownloadUsersAsync(new List<IGuild> { { discordServer } });

      if (data.IsBot)
        return;

      await data.AddRoleAsync(1026507902074245216);
      await playerGeneralChannel.SendMessageAsync($"Bonjour {data.Mention} et bienvenue sur le Discord des Larmes des Erylies.\n\n Pour commencer, n'hésite pas à consulter notre livre du joueur : \n https://docs.google.com/document/d/1ammPGnH-sVjNHnJHCMAm_khbe8mBqTPFeCfqCvJt7ig/edit?usp=sharing");

      if (!forumCategory.Channels.Any(c => c.Name == data.Username.ToLower().Replace(" ", "-")))
      {
        var chan = await discordServer.CreateForumChannelAsync(data.Username, f => { f.CategoryId = forumCategory.Id; } );
        await chan.AddPermissionOverwriteAsync(data, requestForumPermissions);
        await chan.CreatePostAsync("Suivi personnalisé", ThreadArchiveDuration.OneWeek, null, $"Bonjour {data.Mention} !\n\n" +
          $"Bienvenue sur ton forum personnalisé de suivi de joueur. Ce forum et les sujets qu'il contient ne sont visibles que pour toi et le staff.\n\n" +
          $"Cela permettra donc de nous faire parvenir tes demandes RP et d'en suivre l'avancement, ainsi que de tenir au courant le staff des derniers développements de ton personnage !");
      }
    }
    private static async Task UpdateUserListOnLeave(SocketGuild guild, SocketUser user)
    {
      await _client.DownloadUsersAsync(new List<IGuild> { { discordServer } });
    }
    private static async Task OnDiscordConnected()
    {
      await _client.DownloadUsersAsync(new List<IGuild> { { _client.GetGuild(680072044364562528) } });
    }
    private static Task OnDiscordDisconnected(Exception e)
    {
      ModuleSystem.Log.Info($"WARNING - Discord Disconnected - Error :\n\n{e.Message}\n{e.StackTrace}");
      HandleDiscordReconnect();
      return Task.CompletedTask;
    }

    private static async void HandleDiscordReconnect()
    {
      ModuleSystem.Log.Info("Trying to reconnect");

      await NwTask.Delay(TimeSpan.FromSeconds(10));

      if(_client.ConnectionState != ConnectionState.Connected)
      {
        try
        {
          ModuleSystem.Log.Info("Discord still disconnected firing login async");
          await _client.LoginAsync(TokenType.Bot, Environment.GetEnvironmentVariable("BOT"));
          ModuleSystem.Log.Info("Discord still disconnected, firing start async");
          await _client.StartAsync();
          ModuleSystem.Log.Info("Discord restarted");
        }
        catch(Exception e)
        {
          ModuleSystem.Log.Info(e.Message + "\n" + e.StackTrace);
        }
      }

    }
    private static async Task OnUsersDownloaded(SocketGuild server)
    {
      if (discordServer != null)
        return;

      discordServer = server;
      forum = discordServer.GetForumChannel(1031269379947638884);
      staffGeneralChannel = _client.GetChannel(680072044364562532) as IMessageChannel;
      playerGeneralChannel = _client.GetChannel(1026545572099924088) as IMessageChannel;
      logChannel = _client.GetChannel(703964971549196339) as IMessageChannel;
      forumCategory = discordServer.GetCategoryChannel(1031290085154500739);
      logForum = discordServer.GetForumChannel(1084562366794043453);
      chatLogForum = discordServer.GetForumChannel(1091614035335729213);

      chimDiscordUser = _client.GetUser(232218662080086017);
      bigbyDiscordUser = _client.GetUser(225961076448034817);

      try
      {
        var activeThreads = await logForum.GetActiveThreadsAsync();
        var publicArchivedThreads = await logForum.GetPublicArchivedThreadsAsync();
        //var privateArchivedThreads = await logForum.GetPrivateArchivedThreadsAsync();
        
        foreach (var logType in (LogUtils.LogType[])Enum.GetValues(typeof(LogUtils.LogType)))
        {
          string logName;

          if (logType == LogUtils.LogType.Console)
            logName = "Chim Test Server Logs";
          else
            logName = logType.ToString();

          var post = activeThreads.FirstOrDefault(t => t.Name == logName);
          post ??= publicArchivedThreads.FirstOrDefault(t => t.Name == logName);
          //post ??= privateArchivedThreads.FirstOrDefault(t => t.Name == logName);
          post ??= await logForum.CreatePostAsync(logName, ThreadArchiveDuration.OneWeek, null, "Système de logs du module");

          LogUtils.logChannelDictionary.Add(logType, post);
          LogUtils.logPile.TryAdd(logType, new Queue<string>());
        }
      }
      catch(Exception e) 
      {
        ModuleSystem.Log.Info($"{e.Message}\n{e.StackTrace}");
      }

      LogUtils.LogMessage("Module en ligne !", LogUtils.LogType.ModuleAdministration);

      /*try
      {
        var guildCommand = new SlashCommandBuilder()
        .WithName("refresh_creature_stats")
        .WithDescription("Met à jour les stats des créatures")
        .WithDefaultMemberPermissions(GuildPermission.MentionEveryone);
        .AddOption("titre", ApplicationCommandOptionType.String, "Titre", isRequired: true)
        .AddOption("contenu", ApplicationCommandOptionType.String, "Demande", isRequired: true);

        var slash = guildCommand.Build();
        await discordServer.CreateApplicationCommandAsync(slash);
      }
      catch (Exception exception)
      {
        Utils.LogMessageToDMs(exception.Message + exception.StackTrace);
      }*/

      //CreateAllSlashCommand();
    }
    private static void CreateAllSlashCommand()
    {
      CreateSlashCommand("info_developpement", "Liste des développements et améliorations en cours", GuildPermission.SendMessages);
      CreateSlashCommand("info_backlog", "Liste de futurs projets à prioriser", GuildPermission.SendMessages);
      CreateSlashCommand("info_alignement", "Informations sur les alignements", GuildPermission.SendMessages);
      CreateSlashCommand("info_animations", "Informations sur la gestion des animations", GuildPermission.SendMessages);
      CreateSlashCommand("info_bonus_investissement", "Informations sur le concept de bonus d'investissement", GuildPermission.SendMessages);
      CreateSlashCommand("info_mort", "La gestion de la mort sur le module", GuildPermission.SendMessages);
      CreateSlashCommand("info_jets_de_dés", "La gestion des jets de dés sur le module", GuildPermission.SendMessages);
      CreateSlashCommand("info_evil", "Le Role Play des personnages d'alignement mauvais", GuildPermission.SendMessages);
      CreateSlashCommand("info_soins", "La gestion des blessures et des soins sur le module", GuildPermission.SendMessages);
      CreateSlashCommand("info_savoir", "La gestion du savoir et des connaissances des personnages sur le module", GuildPermission.SendMessages);
      CreateSlashCommand("info_raison_d_etre", "Notre raison d'être, ce qui nous motive et ce que nous aimerions que le module soit", GuildPermission.SendMessages);
      CreateSlashCommand("info_magie", "Informations sur la magie", GuildPermission.SendMessages);
      CreateSlashCommand("info_multicompte", "Règles concernant le multi-compte", GuildPermission.SendMessages);
      CreateSlashCommand("info_pvp", "Règles concernant le PvP", GuildPermission.SendMessages);
      CreateSlashCommand("info_sorts_rp", "Règles concernant les sorts RP", GuildPermission.SendMessages);
      CreateSlashCommand("info_univers", "Pourquoi cet univers ?", GuildPermission.SendMessages);
      CreateSlashCommand("info_livre_joueur", "Tout sur l'univers et le contexte RP du module", GuildPermission.SendMessages);
      CreateSlashCommand("rumeur_supprimer", "Supprimer une rumeur", GuildPermission.MentionEveryone, "rumeur", "Identifiant de la rumeur", ApplicationCommandOptionType.Integer);
      CreateSlashCommand("rumeur_liste", "Afficher la liste des rumeurs en cours", GuildPermission.MentionEveryone);
      CreateSlashCommand("rumeur_lire", "Lire une rumeur", GuildPermission.MentionEveryone, "rumeur", "Identifiant de la rumeur", ApplicationCommandOptionType.Integer);
      CreateSlashCommand("reboot", "Reboot le module", GuildPermission.Administrator);
      CreateSlashCommand("refill", "Refill ressources", GuildPermission.Administrator);
      CreateSlashCommand("register", "Lier son compte Discord et Never", GuildPermission.SendMessages, "public_key", "Votre clef publique Never", ApplicationCommandOptionType.String);
      CreateSlashCommand("quiestla", "Combien sommes nous actuellement en jeu ?", GuildPermission.SendMessages);
      CreateSlashCommand("joueurs_liste", "Obtenir la liste des joueurs connectés", GuildPermission.MentionEveryone);
    }
    private static async void CreateSlashCommand(string name, string description, GuildPermission permission, string optionName = "", string optionDescription = "", ApplicationCommandOptionType optionType = ApplicationCommandOptionType.Integer)
    {
      try
      {
        var guildCommand = new SlashCommandBuilder()
        .WithName(name)
        .WithDescription(description)
        .WithDefaultMemberPermissions(permission);

        if (!string.IsNullOrEmpty(optionName))
          guildCommand = guildCommand.AddOption(optionName, optionType, optionDescription, isRequired: true);

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
      LogUtils.LogMessage($"Discord - commande {command.CommandName} utilisée par - {command.User.Username}", LogUtils.LogType.ModuleAdministration);

      switch (command.CommandName)
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
          await command.RespondAsync("Informations générales concernant les jets de dés sur le module :\nhttps://docs.google.com/document/d/1IBaRsSZIE3Zg2GJ0SSBNY6-dR2xoTmYYmEhlA95WEfs/edit?usp=sharing");
          break;
        case "info_evil":
          await command.RespondAsync("Informations générales concernant le Role Play des personnages d'alignement mauvais :\nhttps://docs.google.com/document/d/1eCqJ2G27RFEwVBOrnei4KDEnbGsmvzghqtqz3w6vPbQ/edit?usp=sharing");
          break;
        case "info_soins":
          await command.RespondAsync("Informations générales concernant les blessures et les soins :\nhttps://docs.google.com/document/d/12yT3ZvLtGXVteW29JTJxpQ6GRpQ3ArWUeCvXAbbCM4o/edit?usp=sharing");
          break;
        case "info_savoir":
          await command.RespondAsync("Informations générales concernant le savoir et les connaissances d'un personnage :\nhttps://docs.google.com/document/d/1sJtd0XZmBpeTa9s5lVRfaMw3QdfLM8md2GJJHvDdM6Y/edit?usp=sharing");
          break;
        case "info_raison_d_etre":
          await command.RespondAsync("Notre raison d'être, ce qui nous motive et ce que nous aimerions que le module soit :\nhttps://docs.google.com/document/d/1kRRVt_dYdYR-6JPw_-CLbk7eYYCsL-Ots_7xqg5lCRo/edit?usp=sharing");
          break;
        case "info_magie":
          await command.RespondAsync("Informations générales concernant la magie :\nhttps://docs.google.com/document/d/1Wsn-9TPOoYzSUgJVNb_y9yf4cdA9ODUdG6zewdO0LjE/edit?usp=sharing");
          break;
        case "info_multicompte":
          await command.RespondAsync("Règles concernant le multi-compte :\nhttps://docs.google.com/document/d/17d2ooZJoPkC-oMFzpyAIjpr5CbSM1lLB3yyzoxRzHR8/edit?usp=sharing");
          break;
        case "info_pvp":
          await command.RespondAsync("Règles concernant le PvP :\nhttps://docs.google.com/document/d/1z-dQok7wEMdcZsk24C19VITYMYOpypHqHQQutBJeK68/edit?usp=sharing");
          break;
        case "info_sorts_rp":
          await command.RespondAsync("Règles concernant les sorts RP :\nhttps://docs.google.com/document/d/1MOG5Kw2Yyaa0TW7mEdGdFvc3vFY1t08G_bJNDRqa70E/edit?usp=sharing");
          break;
        case "info_univers":
          await command.RespondAsync("Informations concernant les choix de l'univers du module :\nhttps://docs.google.com/document/d/1mwKEVr-7MLUeWynivYuJeHbcnCvymfv9zHM23J-nPMI/edit?usp=sharing");
          break;
        case "info_livre_joueur":
          await command.RespondAsync("Présentation de l'univers et du contexte RP du module :\nhttps://docs.google.com/document/d/1ammPGnH-sVjNHnJHCMAm_khbe8mBqTPFeCfqCvJt7ig/edit?usp=sharing");
          break;
        case "rumeur_supprimer": // droit staff et admin
          await BotSystem.ExecuteDeleteRumorCommand(command);
          break;
        case "rumeur_liste": // droit staff et admin
          await BotSystem.ExecuteGetRumorsListCommand(command);
          break;
        case "rumeur_lire": // droit staff et admin,
          await BotSystem.ExecuteGetRumorCommand(command);
          break;
        case "reboot": // droit admin
          await BotSystem.ExecuteRebootCommand(command);
          break;
        case "refill": // droit admin
          await BotSystem.ExecuteRefillCommand(command);
          break;
        case "register": // droit général
          await BotSystem.ExecuteRegisterDiscordId(command);
          break;
        case "quiestla": // droit général
          await BotSystem.ExecuteGetConnectedPlayersCommand(command);
          break;
        case "joueurs_liste": // droit admin & staff
          await BotSystem.ExecuteGetConnectedPlayersCommand(command, true);
          break;
        case "refresh_creature_stats": // droit admin & staff
          Config.creatureStats.Clear();
          ModuleSystem.InitializeCreatureStats();
          break;
        case "annonce": // droit staff
          await BotSystem.ExecuteBroadcastAnnouncementCommand(command);
          break;
        case "staff_demande": // droit général
          await BotSystem.ExecutePlayerRequestCommand(command);
          break;
      }
    }
  }
}
