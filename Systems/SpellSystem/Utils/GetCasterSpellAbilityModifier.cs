using Anvil.API;

namespace NWN.Systems
{
  public static partial class SpellUtils
  {
    public static int GetCasterSpellAbilityModifier(NwCreature caster)
    {
      return caster.GetAbilityModifier(Ability.Intelligence) > caster.GetAbilityModifier(Ability.Charisma)
            ? caster.GetAbilityModifier(Ability.Intelligence) : caster.GetAbilityModifier(Ability.Charisma);

      // TODO : S'occuper des autres types de modificateurs qui amèneront plus de complexité
    }
  }
}
