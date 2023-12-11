using System.Collections.Generic;
using System.Linq;
using Anvil.API;
using static NWN.Systems.PlayerSystem.Player;

namespace NWN.Systems
{
  public static partial class SkillSystem
  {
    public static bool OnLearnLameDoutretombe(PlayerSystem.Player player, int customSkillId)
    {
      if (!player.oid.LoginCreature.KnowsFeat(NwFeat.FromFeatId(CustomSkill.LameDoutretombe)))
        player.oid.LoginCreature.AddFeat(NwFeat.FromFeatId(CustomSkill.LameDoutretombe));

      List<Ability> abilities = new();

      if (player.oid.LoginCreature.GetRawAbilityScore(Ability.Strength) < 20)
        abilities.Add(Ability.Strength);

      if (player.oid.LoginCreature.GetRawAbilityScore(Ability.Dexterity) < 20)
        abilities.Add(Ability.Dexterity);

      if (abilities.Count > 0)
      {
        if (!player.windows.TryGetValue("abilityBonusChoice", out var value)) player.windows.Add("abilityBonusChoice", new AbilityBonusChoiceWindow(player, abilities));
        else ((AbilityBonusChoiceWindow)value).CreateWindow(abilities);
      }

      player.oid.LoginCreature.OnItemEquip -= ItemSystem.OnEquipApplyLameDoutretombe;
      player.oid.LoginCreature.OnItemUnequip -= ItemSystem.OnUnEquipRemoveLameDoutretombe;
      player.oid.LoginCreature.OnItemEquip += ItemSystem.OnEquipApplyLameDoutretombe;
      player.oid.LoginCreature.OnItemUnequip += ItemSystem.OnUnEquipRemoveLameDoutretombe;

      if (player.oid.LoginCreature.GetItemInSlot(InventorySlot.RightHand)?.BaseItem.ItemType == BaseItemType.TwoBladedSword
        && !player.oid.LoginCreature.ActiveEffects.Any(e => e.Tag == EffectSystem.LameDoutretombeEffectTag))
      {
        player.oid.LoginCreature.ApplyEffect(EffectDuration.Permanent, EffectSystem.lameDoutretombe);
        player.oid.LoginCreature.GetItemInSlot(InventorySlot.RightHand).GetObjectVariable<LocalVariableInt>(ItemConfig.IsFinesseWeaponVariable).Value = 1;
      }
        

      return true;
    }
  }
}
