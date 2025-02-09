using System.Linq;
using NWN.Native.API;

namespace NWN.Systems
{
  public static partial class NativeUtils
  {
    public static int GetIncapacitatedACMalus(CNWSCreature creature)
    {
      if(creature.m_appliedEffects.Any(e => e.m_sCustomTag.ToString() == EffectSystem.HebetementEffectTag
      || In((EffectTrueType)e.m_nType, EffectTrueType.Petrify)
      || (EffectTrueType)e.m_nType == EffectTrueType.SetState && Utils.In(e.GetInteger(0), EffectState.Etourdi, EffectState.Paralyse, EffectState.Endormi, EffectState.Petrifie))
        )
      {
        int dexBonus = GetAbilityModifier(creature, Anvil.API.Ability.Dexterity);
        var armor = creature.m_pInventory.GetItemInSlot((uint)EquipmentSlot.Chest);

        if (armor is not null)
        {
          switch (armor.m_nArmorValue)
          {
            case 3:
            case 4:
            case 5: if (dexBonus > 2) dexBonus = 2; break;
            case 6:
            case 7:
            case 8: dexBonus = 0; break;
          }
        }

        if (dexBonus > 0)
        {
          LogUtils.LogMessage($"Incapable d'agir : -{dexBonus} CA", LogUtils.LogType.Combat);
          return dexBonus;
        }
      }

      return 0;
    }
  }
}
