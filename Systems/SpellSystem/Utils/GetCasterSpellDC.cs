using Anvil.API;

namespace NWN.Systems
{
  public static partial class SpellUtils
  {
    public static int GetCasterSpellDC(NwCreature caster)
    {
      return SpellConfig.BaseSpellDC + GetCasterSpellAbilityModifier(caster) + NativeUtils.GetCreatureProficiencyBonus(caster);
    }
  }
}
