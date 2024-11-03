using Anvil.API;

namespace NWN.Systems
{
  public static partial class SpellUtils
  {
    public static int HandleEvocateurSuperieur(NwCreature caster, NwSpell spell)
    {
      int bonusDamage = 0;

      if (caster is not null && caster.KnowsFeat((Feat)CustomSkill.EvocateurSuperieur) && spell.SpellSchool == SpellSchool.Evocation)
      {
        bonusDamage += caster.GetAbilityModifier(Ability.Intelligence);
        LogUtils.LogMessage($"Evocation supérieure : +{bonusDamage} dégâts", LogUtils.LogType.Combat);
      }

      return bonusDamage;
    }
  }
}
