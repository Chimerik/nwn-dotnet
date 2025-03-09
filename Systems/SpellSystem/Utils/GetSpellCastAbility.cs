using System.Linq;
using Anvil.API;

namespace NWN.Systems
{
  public static partial class SpellUtils
  {
    public static Ability GetSpellCastAbility(NwGameObject oCaster, NwClass casterClass, NwFeat feat)
    {
      Ability castAbility = casterClass.SpellCastingAbility;

      if (feat is not null && oCaster is NwCreature caster)
        castAbility = (Ability)new int[3] { caster.GetAbilityModifier(Ability.Intelligence), caster.GetAbilityModifier(Ability.Wisdom), caster.GetAbilityModifier(Ability.Charisma) }.OrderDescending().First();

      return castAbility;
    }
  }
}
