using Anvil.API;

namespace NWN.Systems
{
  public static partial class EnsoUtils
  {
    public static int HandleElementalAffinity(NwCreature creature, DamageType damage)
    {
      if(creature is null)
        return 0;

      int bonusDamage = creature.GetAbilityModifier(Ability.Charisma) < 1 ? 1 : creature.GetAbilityModifier(Ability.Charisma);

      switch (damage)
      {
        case DamageType.Acid:

          if(creature.KnowsFeat((Feat)CustomSkill.EnsoDracoAffiniteAcide))
          {
            LogUtils.LogMessage($"Lignée Draconique - Affinité élémentaire acide : +{bonusDamage} dégâts au jet", LogUtils.LogType.Combat);
            return bonusDamage;
          }
          
          break;

        case DamageType.Cold:
          
          if(creature.KnowsFeat((Feat)CustomSkill.EnsoDracoAffiniteFroid))
          {
            LogUtils.LogMessage($"Lignée Draconique - Affinité élémentaire froid : +{bonusDamage} dégâts au jet", LogUtils.LogType.Combat);
            return bonusDamage;
          }

          break;

        case DamageType.Fire:

          if(creature.KnowsFeat((Feat)CustomSkill.EnsoDracoAffiniteFeu))
          {
            LogUtils.LogMessage($"Lignée Draconique - Affinité élémentaire feu : +{bonusDamage} dégâts au jet", LogUtils.LogType.Combat);
            return bonusDamage;
          }

          break;

        case DamageType.Electrical:

          if(creature.KnowsFeat((Feat)CustomSkill.EnsoDracoAffiniteElec))
          {
            LogUtils.LogMessage($"Lignée Draconique - Affinité élémentaire électrique : +{bonusDamage} dégâts au jet", LogUtils.LogType.Combat);
            return bonusDamage;
          }
          
          break;

        case DamageType.Custom1:
          if(creature.KnowsFeat((Feat)CustomSkill.EnsoDracoAffinitePoison))
          {
            LogUtils.LogMessage($"Lignée Draconique - Affinité élémentaire poison : +{bonusDamage} dégâts au jet", LogUtils.LogType.Combat);
            return bonusDamage;
          }

          break;
      }

      return 0;
    }
  }
}
