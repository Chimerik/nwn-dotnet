using System.Linq;
using Anvil.API;
using NWN.Native.API;
using Feat = NWN.Native.API.Feat;

namespace NWN.Systems
{
  public static partial class NativeUtils
  {
    public static int GetDefensiveDuellistBonus(CNWSCreature target, int rangedAttack)
    {
      if(!rangedAttack.ToBool() && target.m_pStats.HasFeat((ushort)Feat.PrestigeDefensiveAwareness1).ToBool())
      {
        var reaction = target.m_appliedEffects.FirstOrDefault(e => e.m_sCustomTag.ToString() == EffectSystem.ReactionEffectTag);

        if (reaction is not null)
        {
          CNWSItem weapon = target?.m_pInventory.GetItemInSlot((uint)EquipmentSlot.RightHand);

          if (weapon is null || !weapon.m_ScriptVars.GetInt(ItemConfig.isFinesseWeaponCExoVariable).ToBool())
            weapon = target?.m_pInventory.GetItemInSlot((uint)EquipmentSlot.LeftHand);
          if (weapon is null || !weapon.m_ScriptVars.GetInt(ItemConfig.isFinesseWeaponCExoVariable).ToBool())
            return 0;

          target.RemoveEffect(reaction);

          return GetCreatureWeaponProficiencyBonus(target, weapon);
        }
      }

      return 0;
    }
  }
}
