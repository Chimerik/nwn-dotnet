using Anvil.API;
using NWN.Native.API;
using Ability = Anvil.API.Ability;

namespace NWN.Systems
{
  public static partial class SpellUtils
  {
    public static int GetCasterSpellDC(NwCreature caster, Ability ability)
    {
      return SpellConfig.BaseSpellDC + caster.GetAbilityModifier(ability) + NativeUtils.GetCreatureProficiencyBonus(caster);
    }
    public static int GetCasterSpellDC(CNWSCreature caster, Ability ability)
    {
      byte modifier = caster.m_pStats.GetAbilityMod((byte)ability);
      return SpellConfig.BaseSpellDC + modifier > 122 ? modifier - 255 : modifier + NativeUtils.GetCreatureProficiencyBonus(caster);
    }
  }
}
