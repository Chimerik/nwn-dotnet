using Anvil.API;

namespace NWN.Systems
{
  public static partial class SpellUtils
  {
    public static int HandleIncantationPuissante(NwCreature caster, NwSpell spell)
    {
      int bonusDamage = 0;

      if (caster is not null 
        && ((caster.KnowsFeat((Feat)CustomSkill.ClercIncantationPuissante) && spell.GetSpellLevelByClass(ClassType.Cleric) == 0)
        || (caster.KnowsFeat((Feat)CustomSkill.DruideIncantationPuissante) && spell.GetSpellLevelByClass(ClassType.Druid) == 0)))
      {
        bonusDamage = caster.GetAbilityModifier(Ability.Wisdom) > 0 ? caster.GetAbilityModifier(Ability.Wisdom) : 1;
        LogUtils.LogMessage($"Incantation Puissante : ajout de {bonusDamage} dégâts au jet", LogUtils.LogType.Combat);
      }

      return bonusDamage;
    }
  }
}
