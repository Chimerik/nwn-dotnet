using Anvil.API;

namespace NWN.Systems
{
  public static partial class SpellUtils
  {
    public static int GetCasterSpellDC(NwCreature caster, NwSpell spell)
    {
      return SpellConfig.BaseSpellDC + GetCasterSpellAbilityModifier(caster, spell) + NativeUtils.GetCreatureProficiencyBonus(caster);
    }
  }
}
