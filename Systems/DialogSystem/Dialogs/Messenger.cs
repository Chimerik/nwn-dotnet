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

      var query = SqLiteUtils.SelectQuery("messenger",
        new List<string>() { { "rowid" }, { "senderName" }, { "title" }, { "message" }, { "sentDate" }, { "read" } },
        new List<string[]>() { new string[] { "characterId", player.characterId.ToString() } },
        " sentDate desc");

      //Thread.CurrentThread.CurrentCulture = new CultureInfo("fr-FR");
      foreach (var result in query.Results)
      {
        int messageId = result.GetInt(0);
        string senderName = result.GetString(1);
        string title = result.GetString(2);
        string message = result.GetString(3);
        string sentDate = result.GetString(4);
        int read = result.GetInt(5);

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
      SqLiteUtils.UpdateQuery("messenger",
        new List<string[]>() { new string[] { "read", "1" } },
        new List<string[]>() { new string[] { "rowid", messageId.ToString() } });

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
      SqLiteUtils.UpdateQuery("messenger",
        new List<string[]>() { new string[] { "read", "1" } },
        new List<string[]>() { new string[] { "rowid", messageId.ToString() } });

      NwItem letter = await NwItem.Create("skillbookgeneriq", player.oid.ControlledCreature, 1, "letter");
      letter.Name = title;
      letter.Description = message;
    }
    private void RemoveMessage(int messageId)
    {
      SqLiteUtils.DeletionQuery("messenger",
          new Dictionary<string, string>() { { "rowid", messageId.ToString() } });

      player.oid.SendServerMessage("Message supprimé.");

      DrawInbox();
    }
  }
}
