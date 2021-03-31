using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using NWN.API;
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
    private void GetRumorTitle()
    {
      rumorTitle = "";

      player.menu.Clear();
      player.menu.titleLines.Add($"Veuillez indiquer le titre de votre rumeur à l'oral.");
      player.menu.Draw();

      Task playerInput = NwTask.Run(async () =>
      {
        player.oid.GetLocalVariable<int>("_PLAYER_INPUT_STRING").Value = 1;
        player.setString = "";
        await NwTask.WaitUntil(() => player.setString != "");
        rumorTitle = player.setString;
        GetRumorContent();
        player.setString = "";
      });
    }
    private void GetRumorContent()
    {
      player.menu.Clear();
      player.menu.titleLines.Add($"Veuillez indiquer le contenu de votre rumeur à l'oral.");
      player.menu.Draw();

      Task playerInput = NwTask.Run(async () =>
      {
        player.oid.GetLocalVariable<int>("_PLAYER_INPUT_STRING").Value = 1;
        player.setString = "";
        await NwTask.WaitUntil(() => player.setString != "");
        SaveRumorToDatabase(player.setString);
        player.setString = "";
      });
    }
    private void SaveRumorToDatabase(string rumorContent)
    {
      var query = NWScript.SqlPrepareQueryCampaign(Config.database, $"INSERT INTO rumors (accountId, title, content) VALUES (@accountId, @title, @content)" +
          $"ON CONFLICT (accountId, title) DO UPDATE SET content = @content;");
      NWScript.SqlBindInt(query, "@accountId", player.accountId);
      NWScript.SqlBindString(query, "@title", rumorTitle);
      NWScript.SqlBindString(query, "@content", rumorContent);
      NWScript.SqlStep(query);

      player.oid.SendServerMessage($"Héhé {rumorTitle.ColorString(Color.WHITE)}, c'est pas tombé dans l'oreille d'un sourd !", Color.PINK);
      player.menu.Close();

      if(!player.oid.IsDM)
        (Bot._client.GetChannel(680072044364562532) as Discord.IMessageChannel).SendMessageAsync($"{Bot._client.GetGuild(680072044364562528).EveryoneRole.Mention} Création de la rumeur {rumorTitle} par {player.oid.Name} à valider.");
    }
    private void DrawMyRumorsList()
    {
      player.menu.Clear();
      player.menu.titleLines.Add($"Quelle rumeur souhaitez-vous supprimer ?");
      
      var query = NWScript.SqlPrepareQueryCampaign(Config.database, $"SELECT title, rowid from rumors where accountId = @accountId");
      NWScript.SqlBindInt(query, "@accountId", player.accountId);

      while (NWScript.SqlStep(query) > 0)
      {
        int rumorId = NWScript.SqlGetInt(query, 1);
        rumorTitle = NWScript.SqlGetString(query, 0);
        player.menu.choices.Add((rumorTitle, () => HandleDeleteRumor(rumorId)));
      }

      player.menu.choices.Add(("Retour", () => DrawWelcomePage()));
      player.menu.Draw();
    }
    private void HandleDeleteRumor(int rumorId)
    {
      var deletionQuery = NWScript.SqlPrepareQueryCampaign(Config.database, $"DELETE FROM rumors where rowid = @rowid");
      NWScript.SqlBindInt(deletionQuery, "@rowid", rumorId);
      NWScript.SqlStep(deletionQuery);

      player.oid.SendServerMessage($"Votre rumeur {rumorTitle.ColorString(Color.WHITE)} a bien été supprimé", Color.PINK);
      player.menu.Close();
    }
    private void DrawDMRumorsList()
    {
      player.menu.Clear();
      player.menu.titleLines.Add("Quelle rumeur majeure souhaitez-vous entendre ?");
      
      var query = NWScript.SqlPrepareQueryCampaign(Config.database, "SELECT title, content from rumors r " +
        "LEFT JOIN PlayerAccounts pa on r.accountId = pa.ROWID " +
        "where pa.rank in ('admin', 'staff')");

      while (NWScript.SqlStep(query) > 0)
      {
        string rumorContent = NWScript.SqlGetString(query, 1);
        rumorTitle = NWScript.SqlGetString(query, 0);
        player.menu.choices.Add((rumorTitle, () => HandleRumorSelected(rumorContent)));
      }

      player.menu.choices.Add(("Retour", () => DrawWelcomePage()));
      player.menu.Draw();
    }
    private void DrawPCRumorsList()
    {
      player.menu.Clear();
      player.menu.titleLines.Add("Quel potin souhaitez-vous entendre ?");

      var query = NWScript.SqlPrepareQueryCampaign(Config.database, "SELECT title, content from rumors r " +
        "LEFT JOIN PlayerAccounts pa on r.accountId = pa.ROWID " +
        "where pa.rank not in ('admin', 'staff')");

      while (NWScript.SqlStep(query) > 0)
      {
        string rumorContent = NWScript.SqlGetString(query, 1);
        rumorTitle = NWScript.SqlGetString(query, 0);
        player.menu.choices.Add((rumorTitle, () => HandleRumorSelected(rumorContent)));
      }

      player.menu.choices.Add(("Retour", () => DrawWelcomePage()));
      player.menu.Draw();
    }
    private void HandleRumorSelected(string rumorContent)
    {
      string originalDesc = player.oid.Description;
      string tempDescription = rumorTitle.ColorString(Color.ORANGE) + "\n\n" + rumorContent;
      player.oid.Description = tempDescription;
      player.oid.ClearActionQueue();
      player.oid.ActionExamine(player.oid);

      Task waitForDescriptionRewrite = NwTask.Run(async () =>
      {
        await NwTask.Delay(TimeSpan.FromSeconds(0.2));
        player.oid.Description = originalDesc;
      });

      //player.menu.Close();
    }
  }
}
