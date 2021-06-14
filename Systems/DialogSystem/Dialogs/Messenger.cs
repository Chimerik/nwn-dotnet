using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using NWN.API;
using NWN.Core;
using static NWN.Systems.PlayerSystem;

namespace NWN.Systems
{
  class Messenger
  {
    Player player;
    public Messenger(Player player)
    {
      this.player = player;
      this.DrawWelcomePage();
    }
    private void DrawWelcomePage()
    {
      player.menu.Clear();
      player.menu.titleLines = new List<string> {
        "Salutations. Je suis à votre service pour l'envoi et la réception de message écrits.",
        "Je puis également vous assister dans la création d'ouvrages.",
        "Que souhaitez-vous faire ?"
      };

      player.menu.choices.Add(("Lire mes messages.", () => DrawInbox()));
      player.menu.choices.Add(("Quitter.", () => player.menu.Close()));
      player.menu.Draw();
    }
    private void DrawInbox()
    {
      player.menu.Clear();
      player.menu.titleLines = new List<string> {
        "Voici la liste des messages reçus.",
        "Que souhaitez-vous faire ?"
      };

      var query = NWScript.SqlPrepareQueryCampaign(Config.database, $"SELECT rowid, senderName, title, message, sentDate, read from messenger where characterId = @characterId order by sentDate desc");
      NWScript.SqlBindInt(query, "@characterId", player.characterId);

      //Thread.CurrentThread.CurrentCulture = new CultureInfo("fr-FR");

      while (NWScript.SqlStep(query) > 0)
      {
        int messageId = NWScript.SqlGetInt(query, 0);
        string senderName = NWScript.SqlGetString(query, 1);
        string title = NWScript.SqlGetString(query, 2);
        string message = NWScript.SqlGetString(query, 3);
        string sentDate = NWScript.SqlGetString(query, 4);
        int read = NWScript.SqlGetInt(query, 5);

        string displayString = "";
        if (read == 1)
          displayString += "Lu | ";
        displayString += $"{senderName} | {title} | {sentDate}";

        player.menu.choices.Add((displayString, () => DrawMessage(messageId, senderName, title, sentDate, message)));
      }

      player.menu.choices.Add(("Retour.", () => DrawWelcomePage()));
      player.menu.choices.Add(("Quitter.", () => player.menu.Close()));
      player.menu.Draw();
    }
    private void DrawMessage(int messageId, string senderName, string title, string sentDate, string message)
    {
      player.menu.Clear();
      player.menu.titleLines = new List<string> {
        $"{senderName} - {sentDate}",
        $"{title}",
        "Que souhaitez-vous faire ?"
      };

      player.menu.choices.Add(("Lire.", () => DisplayMessage(title, message, messageId)));
      player.menu.choices.Add(("Le récupérer.", () => PrintMessage(title, message, messageId)));
      player.menu.choices.Add(("Supprimer.", () => RemoveMessage(messageId)));
      player.menu.choices.Add(("Retour.", () => DrawInbox()));
      player.menu.choices.Add(("Quitter.", () => player.menu.Close()));
      player.menu.Draw();
    }
    private async void DisplayMessage(string title, string message, int messageId)
    {
      var query = NWScript.SqlPrepareQueryCampaign(Config.database, $"UPDATE messenger SET read = 1 where rowid = @messageId");
      NWScript.SqlBindInt(query, "@messageId", messageId);
      NWScript.SqlStep(query);

      string originalDesc = player.oid.LoginCreature.Description;
      string tempDescription = title.ColorString(ColorConstants.Orange) + "\n\n" + message;
      player.oid.ControlledCreature.Description = tempDescription;
      await player.oid.ControlledCreature.ClearActionQueue();
      await player.oid.ActionExamine(player.oid.ControlledCreature);

      Task waitForDescriptionRewrite = NwTask.Run(async () =>
      {
        await NwTask.Delay(TimeSpan.FromSeconds(0.2));
        player.oid.ControlledCreature.Description = originalDesc;
      });
    }
    private async void PrintMessage(string title, string message, int messageId)
    {
      var query = NWScript.SqlPrepareQueryCampaign(Config.database, $"UPDATE messenger SET read = 1 where rowid = @messageId");
      NWScript.SqlBindInt(query, "@messageId", messageId);
      NWScript.SqlStep(query);

      NwItem letter = await NwItem.Create("skillbookgeneriq", player.oid.ControlledCreature, 1, "letter");
      letter.Name = title;
      letter.Description = message;
    }
    private void RemoveMessage(int messageId)
    {
      var deletionQuery = NWScript.SqlPrepareQueryCampaign(Config.database, $"DELETE from messenger where rowid = @rowid");
      NWScript.SqlBindInt(deletionQuery, "@rowid", messageId);
      NWScript.SqlStep(deletionQuery);

      player.oid.SendServerMessage("Message supprimé.");
      DrawInbox();
    }
  }
}
