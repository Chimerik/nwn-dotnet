using Anvil.API;
using System.Collections.Generic;

namespace NWN.Systems
{
  class DMVisualEffects
  {
    // DEPRECATED : a remplacer par gestion via NUI

    PlayerSystem.Player player;
    string vfxName;
    int vfxId;
    public DMVisualEffects(PlayerSystem.Player player)
    {
      this.player = player;
      //DrawVFXWelcomePage();
    }
    /*private void DrawVFXWelcomePage()
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
        vfxName = player.oid.LoginCreature.GetObjectVariable<LocalVariableString>("_PLAYER_INPUT").Value;
        player.oid.LoginCreature.GetObjectVariable<LocalVariableString>("_PLAYER_INPUT").Delete();
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
        vfxId = int.Parse(player.oid.LoginCreature.GetObjectVariable<LocalVariableString>("_PLAYER_INPUT").Value);
        player.oid.LoginCreature.GetObjectVariable<LocalVariableString>("_PLAYER_INPUT").Delete();
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
        vfxId = int.Parse(player.oid.LoginCreature.GetObjectVariable<LocalVariableString>("_PLAYER_INPUT").Value);
        player.oid.LoginCreature.GetObjectVariable<LocalVariableString>("_PLAYER_INPUT").Delete();

        SqLiteUtils.InsertQuery("dmVFXDuration",
          new List<string[]>() { new string[] { "playerName", player.oid.PlayerName }, new string[] { "vfxDuration", vfxName } },
          new List<string>() { "playerName"},
          new List<string[]>() { new string[] { "vfxDuration" } },
          new List<string>() { "playerName" });

        player.oid.SendServerMessage($"La durée de vos effets visuels sera désormais de {vfxId.ToString().ColorString(ColorConstants.White)} secondes !", new Color(32, 255, 32));
      }
    }
    private void SaveVFX()
    {
      SqLiteUtils.InsertQuery("dmVFX",
          new List<string[]>() { new string[] { "playerName", player.oid.PlayerName }, new string[] { "vfxName", vfxName }, new string[] { "vfxId", vfxId.ToString() } },
          new List<string>() { "playerName", "vfxName" },
          new List<string[]>() { new string[] { "vfxId" } },
          new List<string>() { "playerName", "vfxName" });

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

      var result = SqLiteUtils.SelectQuery("dmVFX",
        new List<string>() { { "vfxName" }, { "playerName" } },
        new List<string[]>() { new string[] { "rowid", player.oid.PlayerName } });

      foreach (var vfx in result.Results)
      {
        string vfxListName = vfx.GetString(0);
        int vfxListId = vfx.GetInt(1);

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
      SqLiteUtils.DeletionQuery("dmVFX",
            new Dictionary<string, string>() { { "playerName", player.oid.PlayerName }, { "vfxName", deletedVFXName } });

      player.oid.SendServerMessage($"Votre effet visuel {deletedVFXName.ColorString(ColorConstants.White)} a bien été supprimé.", new Color(32, 255, 32));

      DrawVFXList();
    }*/
  }
}
