using Anvil.API;

namespace NWN.Systems
{
  public static partial class SpellUtils
  {
    public static int HandleIncantationPuissante(NwCreature caster, int damage, NwSpell spell)
    {
      if ((caster.KnowsFeat((Feat)CustomSkill.ClercIncantationPuissante) && spell.GetSpellLevelForClass(ClassType.Cleric) == 0)
        || (caster.KnowsFeat((Feat)CustomSkill.DruideIncantationPuissante) && spell.GetSpellLevelForClass(ClassType.Druid) == 0))
      {
        int wisMod = caster.GetAbilityModifier(Ability.Wisdom) > 0 ? caster.GetAbilityModifier(Ability.Wisdom) : 1;
        LogUtils.LogMessage($"Incantation Puissante : ajout de {wisMod} dégâts au jet", LogUtils.LogType.Combat);
        damage += wisMod;
      }

      return damage;
    }
  }
}
