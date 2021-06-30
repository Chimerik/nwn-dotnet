using NWN.Core;
using NWN.API;
using System.Collections.Generic;

namespace NWN.Systems
{
  class QuickBar
  {
    PlayerSystem.Player player;
    public QuickBar(PlayerSystem.Player player)
    {
      this.player = player;
      DrawQuickbarWelcomePage();
    }
    private void DrawQuickbarWelcomePage()
    {
      player.menu.Clear();

      player.menu.titleLines.Add("Bienvenue dans le système de gestion des barres de raccourcis.");
      player.menu.titleLines.Add("Que souhaitez-vous faire ?");

      player.menu.choices.Add(("Consulter mes barres de raccourcis.", () => DrawQuickBarList()));
      player.menu.choices.Add(("Enregistrer ma barre actuelle.", () => AskQuickBarName()));
      player.menu.choices.Add(("Retour.", () => CommandSystem.DrawCommandList(player)));
      player.menu.choices.Add(("Quitter.", () => player.menu.Close()));

      player.menu.Draw();
    }
    private async void AskQuickBarName()
    {
      player.menu.Clear();

      player.menu.titleLines = new List<string>() {
        "Quel nom souhaitez-vous donner à cette barre de raccourcis ?"
        };

      player.menu.Draw();

      bool awaitedValue = await player.WaitForPlayerInputString();

      if (awaitedValue)
      {
        SaveQuickBar(player.oid.LoginCreature.GetLocalVariable<string>("_PLAYER_INPUT").Value);
        player.oid.LoginCreature.GetLocalVariable<string>("_PLAYER_INPUT").Delete();
      }
    }
    private void SaveQuickBar(string quickBarName)
    {
      var quickbarQuery = NWScript.SqlPrepareQueryCampaign(Config.database, $"INSERT INTO playerQuickbar (characterId, quickbarName, serializedQuickbar) VALUES (@characterId, @quickbarName, @serializedQuickbar)" +
              $"ON CONFLICT (characterId, quickbarName) DO UPDATE SET serializedQuickbar = @serializedQuickbar where characterId = @characterId and quickbarName = @quickbarName");
      NWScript.SqlBindInt(quickbarQuery, "@characterId", player.characterId);
      NWScript.SqlBindString(quickbarQuery, "@quickbarName", quickBarName);
      NWScript.SqlBindString(quickbarQuery, "@serializedQuickbar", player.oid.ControlledCreature.SerializeQuickbar().ToBase64EncodedString());
      NWScript.SqlStep(quickbarQuery);

      player.oid.SendServerMessage($"Votre barre de raccourcis {quickBarName.ColorString(ColorConstants.White)} a bien été enregistrée !", new Color(32, 255, 32));

      DrawQuickbarWelcomePage();
    }
    private void DrawQuickBarList()
    {
      player.menu.Clear();

      player.menu.titleLines = new List<string>() {
        "Voici la liste de vos barres de raccourcis.",
        "Laquelle souhaitez-vous consulter ?"
        };

      var query = NWScript.SqlPrepareQueryCampaign(Config.database, $"SELECT quickbarName, serializedQuickbar from playerQuickbar where characterId = @characterId");
      NWScript.SqlBindInt(query, "@characterId", player.characterId);

      while (NWScript.SqlStep(query) > 0)
      {
        string quickbarName = NWScript.SqlGetString(query, 0);
        string serializedQuickbar = NWScript.SqlGetString(query, 1);

        player.menu.choices.Add((
          quickbarName,
          () => HandleSelectedQuickbar(quickbarName, serializedQuickbar)
        ));
      }

      player.menu.choices.Add(("Retour.", () => DrawQuickbarWelcomePage()));
      player.menu.choices.Add(("Quitter.", () => player.menu.Close()));

      player.menu.Draw();
    }
    private void HandleSelectedQuickbar(string quickbarName, string serializedQuickbar)
    {
      player.menu.Clear();

      player.menu.titleLines = new List<string>() {
        $"Vous avez sélectionné la barre {quickbarName.ColorString(new Color(32, 255, 32))}",
        "Que souhaitez-vous faire ?"
        };

      player.menu.choices.Add(("Utiliser celle-ci.", () => LoadQuickbar(quickbarName, serializedQuickbar)));
      player.menu.choices.Add(("La remplacer par mon actuelle.", () => SaveQuickBar(quickbarName)));
      player.menu.choices.Add(("La supprimer.", () => DeleteQuickbar(quickbarName)));
      player.menu.choices.Add(("Retour.", () => DrawQuickBarList()));
      player.menu.choices.Add(("Quitter.", () => player.menu.Close()));

      player.menu.Draw();
    }
    private void DeleteQuickbar(string quickbarName)
    {
      SqLiteUtils.DeletionQuery("playerQuickbar",
         new Dictionary<string, string>() { { "characterId", player.characterId.ToString() }, { "quickbarName", quickbarName } });

      player.oid.SendServerMessage($"Votre barre de raccourcis {quickbarName.ColorString(ColorConstants.White)} a bien été supprimée.", new Color(32, 255, 32));
      DrawQuickBarList();
    }
    private void LoadQuickbar(string quickbarName, string serializedQuickbar)
    {
      player.oid.ControlledCreature.DeserializeQuickbar(serializedQuickbar.ToByteArray());

      player.oid.SendServerMessage($"Votre barre de raccourcis {quickbarName.ColorString(ColorConstants.White)} a bien été chargée.", new Color(32, 255, 32));
      player.menu.Close();
    }
  }
}
