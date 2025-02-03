using System.Collections.Generic;
using System.Linq;
using Anvil.API;
using static NWN.Systems.PlayerSystem.Player;

namespace NWN.Systems
{
  public static partial class SkillSystem
  {
    public static bool OnLearnAmbiMaster(PlayerSystem.Player player, int customSkillId)
    {
      if(!player.oid.LoginCreature.KnowsFeat(Feat.Ambidexterity))
        player.oid.LoginCreature.AddFeat(Feat.Ambidexterity);

      player.oid.OnPlayerEquipItem -= ItemSystem.OnEquipApplyAmbiMaster;
      player.oid.OnPlayerUnequipItem -= ItemSystem.OnUnEquipRemoveAmbiMaster;
      player.oid.OnPlayerEquipItem += ItemSystem.OnEquipApplyAmbiMaster;
      player.oid.OnPlayerUnequipItem += ItemSystem.OnUnEquipRemoveAmbiMaster;

      if (ItemUtils.IsMeleeWeapon(player.oid.LoginCreature.GetItemInSlot(InventorySlot.RightHand)?.BaseItem) 
        && ItemUtils.IsMeleeWeapon(player.oid.LoginCreature.GetItemInSlot(InventorySlot.LeftHand)?.BaseItem)
        && !player.oid.LoginCreature.ActiveEffects.Any(e => e.Tag == EffectSystem.AmbiMasterEffectTag))
        player.oid.LoginCreature.ApplyEffect(EffectDuration.Permanent, EffectSystem.ambiMaster);

      List<NuiComboEntry> abilities = new();

      if (player.oid.LoginCreature.GetRawAbilityScore(Ability.Strength) < 20)
        abilities.Add(new("Force", (int)Ability.Strength));

      if (player.oid.LoginCreature.GetRawAbilityScore(Ability.Dexterity) < 20)
        abilities.Add(new("Dextérité", (int)Ability.Dexterity));

      if (abilities.Count > 0)
      {
        if (!player.windows.TryGetValue("abilityBonusChoice", out var value)) player.windows.Add("abilityBonusChoice", new AbilityBonusChoiceWindow(player, abilities));
        else ((AbilityBonusChoiceWindow)value).CreateWindow(abilities);
      }

      return true;
    }
  }
}
