using System.Linq;
using Anvil.API;
using NWN.Native.API;

namespace NWN.Systems
{
  public static partial class NativeUtils
  {
    public static Anvil.API.Ability HandleCoupAuBut(CNWSCreature creature, CNWSItem attackWeapon, Anvil.API.Ability attackStat, CExoString effectTag)
    {
      if (attackWeapon is not null && GetCreatureWeaponProficiencyBonus(creature, attackWeapon) > 0)
      {
        var coupAuBut = creature.m_appliedEffects.FirstOrDefault(e => e.m_sCustomTag.CompareNoCase(effectTag).ToBool());
        if (coupAuBut is not null)
          attackStat = (Anvil.API.Ability)coupAuBut.GetInteger(3);

        EffectUtils.RemoveTaggedEffect(creature, effectTag);
      }
       

      return attackStat;
    }
  }
}
