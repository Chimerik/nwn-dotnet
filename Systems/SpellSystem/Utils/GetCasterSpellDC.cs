using System;
using System.Linq;
using Anvil.API;
using NWN.Native.API;
using Ability = Anvil.API.Ability;
using ClassType = Anvil.API.ClassType;

namespace NWN.Systems
{
  public static partial class SpellUtils
  {
    public static int GetCasterSpellDC(NwGameObject oCaster, NwSpell spell, Ability ability)
    {
      if(oCaster is NwCreature caster)
        return SpellConfig.BaseSpellDC + caster.GetAbilityModifier(ability) + NativeUtils.GetCreatureProficiencyBonus(caster)
          + (oCaster.ActiveEffects.Any(e => e.Tag == EffectSystem.SorcellerieInneeEffectTag) && spell.GetSpellLevelForClass(ClassType.Sorcerer) < 15).ToInt();
      else
        return (int)Math.Floor((double)(spell.InnateSpellLevel * 2) - 1) 
          + (oCaster.ActiveEffects.Any(e => e.Tag == EffectSystem.SorcellerieInneeEffectTag) && spell.GetSpellLevelForClass(ClassType.Sorcerer) < 15).ToInt();
    }
    public static int GetCasterSpellDC(NwCreature caster, Ability ability)
    {
      return SpellConfig.BaseSpellDC + caster.GetAbilityModifier(ability) + NativeUtils.GetCreatureProficiencyBonus(caster);
    }
    /*public static int GetCasterSpellDC(CNWSCreature caster, Native.API.Ability ability)
    {
      byte mod = caster.m_pStats.GetAbilityMod((byte)ability);
      int bonus = mod > 122 ? mod - 255 : mod;

      return SpellConfig.BaseSpellDC + bonus + NativeUtils.GetCreatureProficiencyBonus(caster)
        + caster.m_appliedEffects.Any(e => e.m_sCustomTag.CompareNoCase(EffectSystem.SorcellerieInneeEffectExoTag).ToBool()).ToInt();
    }*/
  }
}
