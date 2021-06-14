using NWN.API;
using NWN.Core;
using System.Collections.Generic;
using System.Numerics;

namespace NWN.Systems
{
  class Wind
  {
    PlayerSystem.Player player;
    Vector3 direction = new Vector3(1, 0, 0);
    float magnitude = 0;
    float yaw = 0;
    float pitch = 0;
    public Wind(PlayerSystem.Player player)
    {
      this.player = player;
      DrawMainWindPage();
    }
    private void DrawMainWindPage()
    {
      player.menu.Clear();

      player.menu.titleLines = new List<string>() {
        "Bienvenue dans le système de gestion du vent.",
        "Que souhaitez-vous faire ?"
        };

      player.menu.choices.Add(("Modifier la direction", () => AskDirectionVector()));
      player.menu.choices.Add(("Modifier la magnitude", () => AskMagnitude()));
      //player.menu.choices.Add(("Modifier l'embardée", () => AskYaw()));
      //player.menu.choices.Add(("Modifier le pitch", () => AskPitch()));
      player.menu.choices.Add(("Retour", () => CommandSystem.DrawDMCommandList(player)));
      player.menu.choices.Add(("Quitter", () => player.menu.Close()));
      player.menu.Draw();

      player.oid.SendServerMessage($"Paramétrage du vent de {player.oid.ControlledCreature.Area.Name.ColorString(ColorConstants.White)}" +
        $" Direction : {direction}" +
        $" Magnitude :  {magnitude.ToString().ColorString(ColorConstants.White)}" +
        $" Embardée : {yaw.ToString().ColorString(ColorConstants.White)}" +
        $" Pitch : {pitch.ToString().ColorString(ColorConstants.White)}", ColorConstants.Pink);
    }
    private async void AskDirectionVector()
    {
      player.menu.Clear();

      player.menu.titleLines = new List<string>() {
        "Veuillez entrer le vecteur souhaité."
        };

      player.menu.Draw();

      bool awaitedValue = await player.WaitForPlayerInputString();

      if (awaitedValue)
      {
        string[] vector = player.oid.LoginCreature.GetLocalVariable<string>("_PLAYER_INPUT").Value.Split(",");
        
        if(vector.Length < 3)
        {
          DrawMainWindPage();
          return;
        }
        float x = 1;
        if (float.TryParse(vector[0], out float value))
          x = value;
        float y = 0;
        if (float.TryParse(vector[1], out value))
          y = value;
        float z = 0;
        if (float.TryParse(vector[2], out value))
          z = value;

        direction = new Vector3(x, y, z);
        player.oid.LoginCreature.GetLocalVariable<string>("_PLAYER_INPUT").Delete();
        player.oid.SendServerMessage($"Vous avez choisi {direction} de direction", ColorConstants.Green);
        NWScript.SetAreaWind(player.oid.ControlledCreature.Area, direction, magnitude, yaw, pitch);
        DrawMainWindPage();
      }
    }
    private async void AskMagnitude()
    {
      player.menu.Clear();

      player.menu.titleLines = new List<string>() {
        "Veuillez entrer la magnitude souhaitée."
        };

      player.menu.Draw();

      bool awaitedValue = await player.WaitForPlayerInputInt();

      if (awaitedValue)
      {
        if(float.TryParse(player.oid.LoginCreature.GetLocalVariable<string>("_PLAYER_INPUT").Value, out float value))
          magnitude = float.Parse(player.oid.LoginCreature.GetLocalVariable<string>("_PLAYER_INPUT").Value);
        
        player.oid.LoginCreature.GetLocalVariable<string>("_PLAYER_INPUT").Delete();
        player.oid.SendServerMessage($"Vous avez choisi {magnitude.ToString().ColorString(ColorConstants.White)} de magnitude", ColorConstants.Green);
        NWScript.SetAreaWind(player.oid.ControlledCreature.Area, direction, magnitude, yaw, pitch);
        DrawMainWindPage();
      }
    }
    private async void AskYaw()
    {
      player.menu.Clear();

      player.menu.titleLines = new List<string>() {
        "Veuillez entrer l'embardée souhaitée."
        };

      player.menu.Draw();

      bool awaitedValue = await player.WaitForPlayerInputInt();

      if (awaitedValue)
      {
        yaw = float.Parse(player.oid.LoginCreature.GetLocalVariable<string>("_PLAYER_INPUT").Value);
        player.oid.LoginCreature.GetLocalVariable<string>("_PLAYER_INPUT").Delete();
        player.oid.SendServerMessage($"Vous avez choisi {yaw.ToString().ColorString(ColorConstants.White)} d'embardée", ColorConstants.Green);
        NWScript.SetAreaWind(player.oid.ControlledCreature.Area, direction, magnitude, yaw, pitch);
        DrawMainWindPage();
      }
    }
    private async void AskPitch()
    {
      player.menu.Clear();

      player.menu.titleLines = new List<string>() {
        "Veuillez entrer le pitch souhaitée."
        };

      player.menu.Draw();

      bool awaitedValue = await player.WaitForPlayerInputInt();

      if (awaitedValue)
      {
        pitch = float.Parse(player.oid.LoginCreature.GetLocalVariable<string>("_PLAYER_INPUT").Value);
        player.oid.LoginCreature.GetLocalVariable<string>("_PLAYER_INPUT").Delete();
        player.oid.SendServerMessage($"Vous avez choisi {pitch.ToString().ColorString(ColorConstants.White)} de pitch", ColorConstants.Green);
        NWScript.SetAreaWind(player.oid.ControlledCreature.Area, direction, magnitude, yaw, pitch);
        DrawMainWindPage();
      }
    }
  }
}
