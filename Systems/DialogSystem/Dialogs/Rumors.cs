using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Anvil.API;
using NWN.Core;
using static NWN.Systems.PlayerSystem;

namespace NWN.Systems
{
  class Rumors
  {
    Player player;
    string rumorTitle;
    public Rumors(Player player)
    {
      this.player = player;
      this.DrawWelcomePage();
    }
    private void DrawWelcomePage()
    {
      player.menu.Clear();
      player.menu.titleLines = new List<string> {
        "Bonjour, ami ! Prend place et cette petite chopine.",
        "A moins que tu n'aies quelque goût pour les dernières histoires du cru ?",
      };

      player.menu.choices.Add(("Rumeurs majeures", () => DrawDMRumorsList()));
      player.menu.choices.Add(("Potins et on-dits", () => DrawPCRumorsList()));
      player.menu.choices.Add(("Raconter un potin", () => GetRumorTitle()));
      player.menu.choices.Add(("Supprimer l'un de mes potins", () => DrawMyRumorsList()));

      player.menu.choices.Add(("Quitter", () => player.menu.Close()));
      player.menu.Draw();
    }
    private async void GetRumorTitle()
    {
      rumorTitle = "";

      player.menu.Clear();
      player.menu.titleLines.Add($"Veuillez indiquer le titre de votre rumeur à l'oral.");
      player.menu.Draw();

      bool awaitedValue = await player.WaitForPlayerInputInt();

      if (awaitedValue)
      {
        rumorTitle = player.oid.LoginCreature.GetObjectVariable<LocalVariableString>("_PLAYER_INPUT").Value;
        GetRumorContent();
        player.oid.LoginCreature.GetObjectVariable<LocalVariableString>("_PLAYER_INPUT").Delete();
      }
    }
    private async void GetRumorContent()
    {
      player.menu.Clear();
      player.menu.titleLines.Add($"Veuillez indiquer le contenu de votre rumeur à l'oral.");
      player.menu.Draw();

      bool awaitedValue = await player.WaitForPlayerInputInt();

      if (awaitedValue)
      {
        SaveRumorToDatabase(player.oid.LoginCreature.GetObjectVariable<LocalVariableString>("_PLAYER_INPUT").Value);
        player.oid.LoginCreature.GetObjectVariable<LocalVariableString>("_PLAYER_INPUT").Delete();
      }
    }
    private void SaveRumorToDatabase(string rumorContent)
    {
      SqLiteUtils.InsertQuery("rumors",
          new List<string[]>() {
            new string[] { "accountId", player.accountId.ToString() },
            new string[] { "title", rumorTitle },
            new string[] { "content", rumorContent } },
          new List<string>() { "accountId", "title" },
          new List<string[]>() { new string[] { "content" } });

      player.oid.SendServerMessage($"Héhé {rumorTitle.ColorString(ColorConstants.White)}, c'est pas tombé dans l'oreille d'un sourd !", ColorConstants.Pink);
      player.menu.Close();

      if(!player.oid.IsDM)
        (Bot._client.GetChannel(680072044364562532) as Discord.IMessageChannel).SendMessageAsync($"{Bot._client.GetGuild(680072044364562528).EveryoneRole.Mention} Création de la rumeur {rumorTitle} par {player.oid.LoginCreature.Name} à valider.");
    }
    private void DrawMyRumorsList()
    {
      player.menu.Clear();
      player.menu.titleLines.Add($"Quelle rumeur souhaitez-vous supprimer ?");

      var result = SqLiteUtils.SelectQuery("rumors",
        new List<string>() { { "title" }, { "rowid" } },
        new List<string[]>() { new string[] { "accountId", player.accountId.ToString() } });

      foreach (var rumor in result.Results)
      {
        int rumorId = rumor.GetInt(1);
        rumorTitle = rumor.GetString(0);
        player.menu.choices.Add((rumorTitle, () => HandleDeleteRumor(rumorId)));
      }

      player.menu.choices.Add(("Retour", () => DrawWelcomePage()));
      player.menu.Draw();
    }
    private void HandleDeleteRumor(int rumorId)
    {
      SqLiteUtils.DeletionQuery("rumors",
         new Dictionary<string, string>() { { "rowid", rumorId.ToString() } });

      player.oid.SendServerMessage($"Votre rumeur {rumorTitle.ColorString(ColorConstants.White)} a bien été supprimé", ColorConstants.Pink);

      player.menu.Close();
    }
    private void DrawDMRumorsList()
    {
      player.menu.Clear();
      player.menu.titleLines.Add("Quelle rumeur majeure souhaitez-vous entendre ?");

      var query =  NwModule.Instance.PrepareCampaignSQLQuery(Config.database, "SELECT title, content from rumors r " +
        "LEFT JOIN PlayerAccounts pa on r.accountId = pa.ROWID " +
        "where pa.rank in ('admin', 'staff')");

      foreach (var result in query.Results)
      {
        string rumorContent = result.GetString(1);
        rumorTitle = result.GetString(0);
        player.menu.choices.Add((rumorTitle, () => HandleRumorSelected(rumorContent)));
      }

      player.menu.choices.Add(("Retour", () => DrawWelcomePage()));
      player.menu.Draw();
    }
    private void DrawPCRumorsList()
    {
      player.menu.Clear();
      player.menu.titleLines.Add("Quel potin souhaitez-vous entendre ?");

      var query = NwModule.Instance.PrepareCampaignSQLQuery(Config.database, "SELECT title, content from rumors r " +
        "LEFT JOIN PlayerAccounts pa on r.accountId = pa.ROWID " +
        "where pa.rank not in ('admin', 'staff')");

      foreach (var result in query.Results)
      {
        string rumorContent = result.GetString(1);
        rumorTitle = result.GetString(0);
        player.menu.choices.Add((rumorTitle, () => HandleRumorSelected(rumorContent)));
      }

      player.menu.choices.Add(("Retour", () => DrawWelcomePage()));
      player.menu.Draw();
    }
    private async void HandleRumorSelected(string rumorContent)
    {
      string originalDesc = player.oid.ControlledCreature.Description;
      string tempDescription = rumorTitle.ColorString(ColorConstants.Orange) + "\n\n" + rumorContent;
      player.oid.ControlledCreature.Description = tempDescription;
      await player.oid.ControlledCreature.ClearActionQueue();
      await player.oid.ActionExamine(player.oid.ControlledCreature);

      Task waitForDescriptionRewrite = NwTask.Run(async () =>
      {
        await NwTask.Delay(TimeSpan.FromSeconds(0.2));
        player.oid.ControlledCreature.Description = originalDesc;
      });

      //player.menu.Close();
    }
  }
}
