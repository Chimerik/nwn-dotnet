using System;
using System.Linq;
using Anvil.API;

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
  }
}
