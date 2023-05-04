using System;
using Anvil.API;
using Anvil.API.Events;

namespace NWN.Systems
{
  public partial class SpellSystem
  {
    private static void Enchantement(OnSpellCast onSpellCast, PlayerSystem.Player player)
    {
      if (player.craftJob != null)
      {
        player.oid.SendServerMessage("Veuillez annuler votre travail artisanal en cours avant d'en commencer un nouveau.", ColorConstants.Red);
        return;
      }

      if (onSpellCast.TargetObject is not NwItem targetItem || targetItem == null || targetItem.Possessor != player.oid.ControlledCreature)
      {
        player.oid.SendServerMessage("Cible invalide.", ColorConstants.Red);
        return;
      }

      int skillPoints = 0;

      try
      {
        skillPoints += targetItem.BaseItem.ItemType switch
        {
          BaseItemType.SmallShield or BaseItemType.LargeShield or BaseItemType.TowerShield => player.learnableSkills[CustomSkill.CalligrapheBlindeur].totalPoints,
          BaseItemType.Armor or BaseItemType.Helmet or BaseItemType.Cloak or BaseItemType.Boots or BaseItemType.Gloves or BaseItemType.Bracer or BaseItemType.Belt => player.learnableSkills[CustomSkill.CalligrapheArmurier].totalPoints,
          BaseItemType.Amulet or BaseItemType.Ring => player.learnableSkills[CustomSkill.CalligrapheCiseleur].totalPoints,
          _ => player.learnableSkills[CustomSkill.CalligrapheCoutelier].totalPoints,
        };
      }
      catch(Exception)
      {
        skillPoints = 0;
      }

      if(skillPoints < 1) 
      {
        player.oid.SendServerMessage($"Il est nécessaire de connaître les bases de la calligraphie sur {targetItem.BaseItem.ItemType} avant de pouvoir commencer ce travail !", ColorConstants.Red);
        return;
      }

      if (targetItem.GetObjectVariable<LocalVariableInt>("_AVAILABLE_ENCHANTEMENT_SLOT").HasNothing)
      {
        player.oid.SendServerMessage($"{targetItem.Name.ColorString(ColorConstants.White)} n'a plus d'emplacement d'inscription disponible.", ColorConstants.Red);
        return;
      }

      double cost = Math.Pow(2, skillPoints) * 100;
      int availableInflux = 0;

      foreach (NwItem item in player.oid.LoginCreature.Inventory.Items)
        if (item.Tag == "dose_influx_pur")
          availableInflux += item.StackSize;

      if(availableInflux < cost)
      {
        player.oid.SendServerMessage($"Il vous manque {(int)cost - availableInflux} dose(s) d'influx pur pour pouvoir commencer ce travail !", ColorConstants.Red);
        return;
      }

      foreach (NwItem item in player.oid.LoginCreature.Inventory.Items)
      {
        if (item.Tag == "dose_influx_pur")
        {
          if (item.StackSize > cost)
          {
            item.StackSize -= (int)cost;
            break;
          }
          else if (item.StackSize == cost)
            item.Destroy();
          else if (item.StackSize < cost)
          {
            item.Destroy();
            cost -= item.StackSize;
          }
        }
      }

      player.craftJob = new PlayerSystem.CraftJob(player, targetItem, onSpellCast.Spell, PlayerSystem.JobType.Enchantement);

      /*if (!player.windows.ContainsKey("enchantementSelection")) player.windows.Add("enchantementSelection", new PlayerSystem.Player.EnchantementSelectionWindow(player, onSpellCast.Spell, targetItem));
      else ((PlayerSystem.Player.EnchantementSelectionWindow)player.windows["enchantementSelection"]).CreateWindow(onSpellCast.Spell, targetItem);*/
    }
  }
}
