using System;
using Anvil.API;
using NWN.Native.API;
using Ability = Anvil.API.Ability;

namespace NWN.Systems
{
  public static partial class SpellUtils
  {
    public static int GetCasterSpellDC(NwGameObject oCaster, NwSpell spell, Ability ability)
    {
      if(oCaster is NwCreature caster)
        return SpellConfig.BaseSpellDC + caster.GetAbilityModifier(ability) + NativeUtils.GetCreatureProficiencyBonus(caster);
      else
        return (int)Math.Floor((double)(spell.InnateSpellLevel * 2) - 1);
    }
    public static int GetCasterSpellDC(NwCreature caster, Ability ability)
    {
      return SpellConfig.BaseSpellDC + caster.GetAbilityModifier(ability) + NativeUtils.GetCreatureProficiencyBonus(caster);
    }
  }
}
