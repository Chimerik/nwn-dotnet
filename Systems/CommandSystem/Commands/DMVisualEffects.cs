﻿using NWN.Core;
using NWN.API;
using NWN.API.Constants;
using System.Collections.Generic;

namespace NWN.Systems
{
  class DMVisualEffects
  {
    PlayerSystem.Player player;
    string vfxName;
    int vfxId;
    public DMVisualEffects(PlayerSystem.Player player)
    {
      this.player = player;
      DrawVFXWelcomePage();
    }
    private void DrawVFXWelcomePage()
    {
      player.menu.Clear();

      player.menu.titleLines.Add("Bienvenue dans le système de gestion des effets visuels.");
      player.menu.titleLines.Add("Que souhaitez-vous faire ?");

      player.menu.choices.Add(("Consulter mes effets visuels.", () => DrawVFXList()));
      player.menu.choices.Add(("Enregistrer un VFX.", () => AskVFXName()));
      player.menu.choices.Add(("Paramétrer la durée de mes effets visuels.", () => AskVFXDuration()));
      player.menu.choices.Add(("Retour.", () => CommandSystem.DrawDMCommandList(player)));
      player.menu.choices.Add(("Quitter.", () => player.menu.Close()));

      player.menu.Draw();
    }
    private async void AskVFXName()
    {
      player.menu.Clear();

      player.menu.titleLines = new List<string>() {
        "Quel nom souhaitez-vous donner à cet effet visuel ?"
        };

      player.menu.Draw();

      bool awaitedValue = await player.WaitForPlayerInputString();

      if (awaitedValue)
      {
        vfxName = player.oid.LoginCreature.GetLocalVariable<string>("_PLAYER_INPUT").Value;
        player.oid.LoginCreature.GetLocalVariable<string>("_PLAYER_INPUT").Delete();
        AskVFXId();
      }
    }
    private async void AskVFXId()
    {
      player.menu.Clear();

      player.menu.titleLines = new List<string>() {
        "Quel est l'identifiant de l'effet visuel concerné ?"
        };

      player.menu.Draw();

      bool awaitedValue = await player.WaitForPlayerInputInt();

      if (awaitedValue)
      {
        vfxId = int.Parse(player.oid.LoginCreature.GetLocalVariable<string>("_PLAYER_INPUT").Value);
        player.oid.LoginCreature.GetLocalVariable<string>("_PLAYER_INPUT").Delete();
        SaveVFX();
      }
    }
    private async void AskVFXDuration()
    {
      player.menu.Clear();

      player.menu.titleLines = new List<string>() {
        "Combien de temps souhaitez-vous que vos effets visuels soient appliqués ?",
        "(Le temps indiqué doit être en secondes)"
        };

      player.menu.Draw();

      bool awaitedValue = await player.WaitForPlayerInputInt();

      if (awaitedValue)
      {
        vfxId = int.Parse(player.oid.LoginCreature.GetLocalVariable<string>("_PLAYER_INPUT").Value);
        player.oid.LoginCreature.GetLocalVariable<string>("_PLAYER_INPUT").Delete();

        var query = NWScript.SqlPrepareQueryCampaign(Config.database, $"INSERT INTO dmVFXDuration (playerName, vfxDuration) VALUES (@playerName, @vfxDuration)" +
              $"ON CONFLICT (playerName) DO UPDATE SET vfxDuration = @vfxDuration where playerName = @playerName and vfxName = @vfxName");
        NWScript.SqlBindString(query, "@playerName", player.oid.PlayerName);
        NWScript.SqlBindInt(query, "@vfxDuration", vfxId);
        NWScript.SqlStep(query);

        player.oid.SendServerMessage($"La durée de vos effets visuels sera désormais de {vfxId.ToString().ColorString(ColorConstants.White)} secondes !", new Color(32, 255, 32));
      }
    }
    private void SaveVFX()
    {
      var query = NWScript.SqlPrepareQueryCampaign(Config.database, $"INSERT INTO dmVFX (playerName, vfxName, vfxId) VALUES (@playerName, @vfxName, @vfxId)" +
              $"ON CONFLICT (playerName, vfxName) DO UPDATE SET vfxId = @vfxId where playerName = @playerName and vfxName = @vfxName");
      NWScript.SqlBindString(query, "@playerName", player.oid.PlayerName);
      NWScript.SqlBindString(query, "@vfxName", vfxName);
      NWScript.SqlBindInt(query, "@vfxId", vfxId);
      NWScript.SqlStep(query);

      player.oid.SendServerMessage($"Votre effet visuel {vfxName.ColorString(ColorConstants.White)} a bien été enregistré !", new Color(32, 255, 32));

      DrawVFXWelcomePage();
    }
    private void DrawVFXList()
    {
      player.menu.Clear();

      player.menu.titleLines = new List<string>() {
        "Voici la liste de vos effets visuels.",
        "Lequel souhaitez-vous supprimer ?"
        };

      var query = NWScript.SqlPrepareQueryCampaign(Config.database, $"SELECT vfxName, vfxId from dmVFX where playerName = @playerName");
      NWScript.SqlBindString(query, "@playerName", player.oid.PlayerName);

      while (NWScript.SqlStep(query) > 0)
      {
        string vfxListName = NWScript.SqlGetString(query, 0);
        int vfxListId = NWScript.SqlGetInt(query, 1);

        player.menu.choices.Add((
          $"{vfxListName} - {vfxListId}",
          () => DeleteVFX(vfxListName)
        ));
      }

      player.menu.choices.Add(("Retour.", () => DrawVFXWelcomePage()));
      player.menu.choices.Add(("Quitter.", () => player.menu.Close()));

      player.menu.Draw();
    }
    private void DeleteVFX(string deletedVFXName)
    {
      var query = NWScript.SqlPrepareQueryCampaign(Config.database, "DELETE FROM dmVFX WHERE playerName = @playerName AND vfxName = @vfxName");
      NWScript.SqlBindString(query, "@playerName", player.oid.PlayerName);
      NWScript.SqlBindString(query, "@vfxName", deletedVFXName);
      NWScript.SqlStep(query);

      player.oid.SendServerMessage($"Votre effet visuel {deletedVFXName.ColorString(ColorConstants.White)} a bien été supprimé.", new Color(32, 255, 32));
      DrawVFXList();
    }
  }
}
