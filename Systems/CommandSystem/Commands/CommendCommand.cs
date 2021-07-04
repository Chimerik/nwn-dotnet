using System.Collections.Generic;
using NWN.API;
using NWN.API.Constants;
using NWN.API.Events;
using NWN.Core;

namespace NWN.Systems
{
  class CommendCommand
  {
    PlayerSystem.Player player;
    public CommendCommand(PlayerSystem.Player player)
    {
      this.player = player;

      player.menu.Clear();
      player.menu.titleLines.Add("Veuillez sélectionner le joueur que vous souhaitez recommander.");
      player.menu.choices.Add(("Retour.", () => CommandSystem.DrawCommandList(player)));
      player.menu.choices.Add(("Quitter.", () => player.menu.Close()));

      PlayerSystem.cursorTargetService.EnterTargetMode(player.oid, OnTargetSelected, ObjectTypes.Creature, MouseCursor.Magic);

      player.menu.Draw();
    }
    private void OnTargetSelected(ModuleEvents.OnPlayerTarget selection)
    {
      if (selection.IsCancelled || selection.TargetObject.IsPlayerControlled(out NwPlayer oPC) || oPC == null || !PlayerSystem.Players.TryGetValue(oPC.LoginCreature, out PlayerSystem.Player commendTarget))
        return;

      if(commendTarget.bonusRolePlay < 4)
      {
        commendTarget.oid.SendServerMessage("Un joueur vient de vous recommander pour une augmentation de bonus roleplay !", ColorConstants.Rose);
        
        if(commendTarget.bonusRolePlay == 1)
        {
          commendTarget.bonusRolePlay = 2;
          commendTarget.oid.SendServerMessage("Votre bonus roleplay est désormais de 2", new Color(32, 255, 32));

          SqLiteUtils.UpdateQuery("PlayerAccounts",
          new List<string[]>() { new string[] { "bonusRolePlay", commendTarget.bonusRolePlay.ToString() } },
          new List<string[]>() { new string[] { "rowid", commendTarget.accountId.ToString() } });
        }

        Utils.LogMessageToDMs($"{selection.Player.LoginCreature.Name} vient de recommander {oPC.LoginCreature.Name} pour une augmentation de bonus roleplay.");
      }

      commendTarget.oid.SendServerMessage($"Vous venez de recommander {oPC.LoginCreature.Name.ColorString(ColorConstants.White)} pour une augmentation de bonus roleplay !", ColorConstants.Rose);
      CommandSystem.DrawCommandList(player);
    }
  }
}
