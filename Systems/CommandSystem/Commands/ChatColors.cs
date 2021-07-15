using System.Collections.Generic;
using Anvil.API;
using Anvil.Services;

namespace NWN.Systems
{
  class ChatColors
  {
    private PlayerSystem.Player player;
    private ChatChannel channel;
    private byte[] colorArray = new byte[3];
    public ChatColors(PlayerSystem.Player player)
    {
      this.player = player;

      player.menu.Clear();

      player.menu.titleLines = new List<string>() {
        "Quel canal de chat souhaiteriez-vous modifier ?",
      };

      player.menu.choices.Add(("Joueur en canal Parler.", () => HandleRedValueSelection( ChatChannel.PlayerTalk)));
      player.menu.choices.Add(("DM en canal Parler.", () => HandleRedValueSelection(ChatChannel.DmTalk)));
      player.menu.choices.Add(("Joueur en canal Murmurer.", () => HandleRedValueSelection(ChatChannel.PlayerWhisper)));
      player.menu.choices.Add(("DM en canal Murmurer.", () => HandleRedValueSelection(ChatChannel.DmWhisper)));
      player.menu.choices.Add(("Joueur en canal Groupe.", () => HandleRedValueSelection(ChatChannel.PlayerParty)));
      player.menu.choices.Add(("Joueur en canal MP.", () => HandleRedValueSelection(ChatChannel.PlayerTell)));
      player.menu.choices.Add(("DM en canal Crier.", () => HandleRedValueSelection(ChatChannel.DmShout)));
      player.menu.choices.Add(("Les emotes.", () => HandleRedValueSelection((ChatChannel)100)));
      player.menu.choices.Add(("Le correctif.", () => HandleRedValueSelection((ChatChannel)101)));
      
      player.menu.choices.Add(("Retour.", () => CommandSystem.DrawCommandList(player)));
      player.menu.choices.Add(("Quitter.", () => player.menu.Close()));

      player.menu.Draw();
    }
    private async void HandleRedValueSelection(ChatChannel channel)
    {
      this.channel = channel;

      player.menu.Clear();

      player.menu.titleLines = new List<string>() {
        "Quelle valeur de rouge (R) souhaitez-vous utiliser pour ce canal ?",
        "La valeur doit être comprise entre 0 et 255",
        "Utilisez une appli comme rgb color picker sur google",
        "Prononcez simplement la valeur à l'oral.",
      };

      if(player.chatColors.ContainsKey(channel))
        player.menu.choices.Add(("Réinitialiser la couleur de ce canal", () => HandleResetChannelColor()));
      
      player.menu.Draw();

      bool awaitedValue = await player.WaitForPlayerInputByte();

      if (awaitedValue)
      {
        colorArray[0] = byte.Parse(player.oid.LoginCreature.GetObjectVariable<LocalVariableString>("_PLAYER_INPUT"));
        player.oid.LoginCreature.GetObjectVariable<LocalVariableString>("_PLAYER_INPUT").Delete();
        HandleGreenValueSelection();
      }
    }
    private async void HandleGreenValueSelection()
    {
      player.menu.Clear();

      player.menu.titleLines = new List<string>() {
        "Quelle valeur de vert (G) souhaitez-vous utiliser pour ce canal ?",
        "La valeur doit être comprise entre 0 et 255",
        "Utilisez une appli comme rgb color picker sur google",
        "Prononcez simplement la valeur à l'oral.",
      };

      player.menu.Draw();

      bool awaitedValue = await player.WaitForPlayerInputByte();

      if (awaitedValue)
      {
        colorArray[1] = byte.Parse(player.oid.LoginCreature.GetObjectVariable<LocalVariableString>("_PLAYER_INPUT"));
        player.oid.LoginCreature.GetObjectVariable<LocalVariableString>("_PLAYER_INPUT").Delete();
        HandleBlueValueSelection();
      }
    }
    private async void HandleBlueValueSelection()
    {
      player.menu.Clear();

      player.menu.titleLines = new List<string>() {
        "Quelle valeur de bleu (B) souhaitez-vous utiliser pour ce canal ?",
        "La valeur doit être comprise entre 0 et 255",
        "Utilisez une appli comme rgb color picker sur google",
        "Prononcez simplement la valeur à l'oral.",
      };

      player.menu.Draw();

      bool awaitedValue = await player.WaitForPlayerInputInt();

      if (awaitedValue)
      {
        colorArray[2] = byte.Parse(player.oid.LoginCreature.GetObjectVariable<LocalVariableString>("_PLAYER_INPUT"));
        player.oid.LoginCreature.GetObjectVariable<LocalVariableString>("_PLAYER_INPUT").Delete();

        player.menu.Close();

        if (player.chatColors.ContainsKey(channel))
          player.chatColors[channel] = new Color(colorArray[0], colorArray[1], colorArray[2], 255);
        else
          player.chatColors.Add(channel, new Color(colorArray[0], colorArray[1], colorArray[2], 255));

        player.oid.SendServerMessage("La nouvelle couleur a bien été enregistrée pour le canal sélectionné !");
      }
    }
    private void HandleResetChannelColor()
    {
      player.chatColors.Remove(channel);
      player.menu.Close();
      player.oid.SendServerMessage("La couleur de ce canal a bien été réinitialisée.", ColorConstants.Rose);
    }
  }
}
