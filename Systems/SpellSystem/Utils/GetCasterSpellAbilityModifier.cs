using Anvil.API;

namespace NWN.Systems
{
  public static partial class SpellUtils
  {
    public static int GetCasterSpellAbilityModifier(NwCreature caster, NwSpell spell)
    {

      return spell.Id switch
      {
        CustomSpell.SearingSmite => caster.GetAbilityModifier(Ability.Charisma),
        CustomSpell.FaerieFire => caster.GetAbilityModifier(Ability.Wisdom) > caster.GetAbilityModifier(Ability.Charisma)
                    ? caster.GetAbilityModifier(Ability.Wisdom) : caster.GetAbilityModifier(Ability.Charisma),
        _ => caster.GetAbilityModifier(Ability.Intelligence) > caster.GetAbilityModifier(Ability.Charisma)
                    ? caster.GetAbilityModifier(Ability.Intelligence) : caster.GetAbilityModifier(Ability.Charisma),
      };

      // TODO : S'occuper des autres types de modificateurs qui amèneront plus de complexité
    }
  }
}
