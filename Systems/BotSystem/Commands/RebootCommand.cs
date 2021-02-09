using Discord.Commands;
using System;
using System.Threading.Tasks;
using NWN.API;
using NWN.Services;
using NWNX.API;
using Microsoft.Data.Sqlite;

namespace NWN.Systems
{
    public static partial class BotSystem
    {
        public static async Task ExecuteRebootCommand(SocketCommandContext context)
        {
            if (DiscordUtils.GetPlayerStaffRankFromDiscord(context.User.Id) != "admin")
            {
                await context.Channel.SendMessageAsync("Noooon, vous n'êtes pas la maaaaaître ! Le maaaaître est bien plus poli, d'habitude !");
                return;
            }

            Administration.PlayerPassword = "REBOOT";

            foreach (NwPlayer player in NwModule.Instance.Players)
            {
                player.FloatingTextString("Attention - Le serveur va redémarrer dans 30 secondes.", false);
                // TODO : ajouter Journal entry
                /*PlayerListEntry.Value.playerJournal.rebootCountDown = DateTime.Now.AddSeconds(30);

            JournalEntry journalEntry = new JournalEntry();
            journalEntry.sName = $"REBOOT SERVEUR - {Utils.StripTimeSpanMilliseconds((TimeSpan)(PlayerListEntry.Value.playerJournal.rebootCountDown - DateTime.Now))}";
            journalEntry.sText = "Attention, le serveur reboot bientôt. Accrochez bien vos ceintures.\n" +
              "Non pas que vous ayez grand chose à faire, votre personnage est automatiquement sauvegardé et le module sera de retour dans moins d'une minute.";
            journalEntry.sTag = "reboot";
            journalEntry.nPriority = 1;
            journalEntry.nQuestCompleted = 0;
            journalEntry.nQuestDisplayed = 1;
            PlayerPlugin.AddCustomJournalEntry(PlayerListEntry.Key, journalEntry);

            PlayerListEntry.Value.rebootUpdate();*/
            }

            Task Reboot = NwTask.Run(async () =>
            {
                await NwTask.Delay(TimeSpan.FromSeconds(30));

                using (var connection = new SqliteConnection($"{Config.db_path}"))
                {
                    connection.Open();

                    var command = connection.CreateCommand();
                    command.CommandText =
                    @"UPDATE moduleInfo SET year = @year, month = @month, day = @day, hour = @hour, minute = @minute, 
                    second = @second
                    ";
                    command.Parameters.AddWithValue("year", NwDateTime.Now.Year);
                    command.Parameters.AddWithValue("month", NwDateTime.Now.Month);
                    command.Parameters.AddWithValue("day", NwDateTime.Now.DayInMonth);
                    command.Parameters.AddWithValue("hour", NwDateTime.Now.Hour);
                    command.Parameters.AddWithValue("minute", NwDateTime.Now.Minute);
                    command.Parameters.AddWithValue("second", NwDateTime.Now.Second); 
                    command.ExecuteNonQuery();
                }

                Administration.ShutdownServer();
                return 20;
            });
        }
    }
}
