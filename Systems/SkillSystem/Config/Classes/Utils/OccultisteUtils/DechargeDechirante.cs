using Anvil.API;

namespace NWN.Systems
{
  public static partial class OccultisteUtils
  {
    public static int DechargeDechirante(NwCreature caster, int spellLevel, NwClass casterClass)
    {
      int bonusDamage = 0;

      if (caster is not null & casterClass.Id == CustomClass.Occultiste && spellLevel < 1 && caster.KnowsFeat((Feat)CustomSkill.DechargeDechirante))
      {
        bonusDamage = caster.GetAbilityModifier(Ability.Charisma) > 1 ? caster.GetAbilityModifier(Ability.Charisma) : 1;
        LogUtils.LogMessage($"Décharge Déchirante : +{bonusDamage} dégâts", LogUtils.LogType.Combat);
      }

      return bonusDamage;
    }
  }
}
