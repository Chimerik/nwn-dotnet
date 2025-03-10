﻿using System.Linq;
using NWN.Native.API;

namespace NWN.Systems
{
  public static partial class NativeUtils
  {
    public static Anvil.API.Ability HandleCoupAuBut(CNWSCreature creature, CNWSItem attackWeapon, Anvil.API.Ability attackStat, string effectTag)
    {
      if (attackWeapon is not null && GetCreatureWeaponProficiencyBonus(creature, attackWeapon) > 0)
      {
        var coupAuBut = creature.m_appliedEffects.FirstOrDefault(e => e.m_sCustomTag.ToString() == effectTag);
        
        if (coupAuBut is not null)
          attackStat = (Anvil.API.Ability)coupAuBut.m_nCasterLevel;

        EffectUtils.RemoveTaggedNativeEffect(creature, effectTag);
      }
       

      return attackStat;
    }
  }
}
