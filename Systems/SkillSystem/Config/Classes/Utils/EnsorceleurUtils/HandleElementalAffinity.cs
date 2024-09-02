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
          LogUtils.LogMessage($"Lignée Draconique - Affinité élémentaire acide : +{bonusDamage} dégâts au jet", LogUtils.LogType.Combat);
          return creature.KnowsFeat((Feat)CustomSkill.EnsoDracoAffiniteAcide) ? bonusDamage : 0;

        case DamageType.Cold:
          LogUtils.LogMessage($"Lignée Draconique - Affinité élémentaire froid : +{bonusDamage} dégâts au jet", LogUtils.LogType.Combat);
          return creature.KnowsFeat((Feat)CustomSkill.EnsoDracoAffiniteFroid) ? bonusDamage : 0;

        case DamageType.Fire:
          LogUtils.LogMessage($"Lignée Draconique - Affinité élémentaire feu : +{bonusDamage} dégâts au jet", LogUtils.LogType.Combat); 
          return creature.KnowsFeat((Feat)CustomSkill.EnsoDracoAffiniteFeu) ? bonusDamage : 0;

        case DamageType.Electrical:
          LogUtils.LogMessage($"Lignée Draconique - Affinité élémentaire électrique : +{bonusDamage} dégâts au jet", LogUtils.LogType.Combat); 
          return creature.KnowsFeat((Feat)CustomSkill.EnsoDracoAffiniteElec) ? bonusDamage : 0;

        case DamageType.Custom1:
          LogUtils.LogMessage($"Lignée Draconique - Affinité élémentaire poison : +{bonusDamage} dégâts au jet", LogUtils.LogType.Combat);
          return creature.KnowsFeat((Feat)CustomSkill.EnsoDracoAffinitePoison) ? bonusDamage : 0;

          default: return 0;
      }     
    }
  }
}
