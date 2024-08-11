using Anvil.API;

namespace NWN.Systems
{
  public static partial class EnsoUtils
  {
    public static int GetSorcerySource(NwCreature creature)
    {
      if(creature.KnowsFeat((Feat)CustomSkill.EnsoPrudence))
        return creature.GetFeatRemainingUses((Feat)CustomSkill.EnsoPrudence);

      if (creature.KnowsFeat((Feat)CustomSkill.EnsoAllonge))
        return creature.GetFeatRemainingUses((Feat)CustomSkill.EnsoAllonge);

      if (creature.KnowsFeat((Feat)CustomSkill.EnsoExtension))
        return creature.GetFeatRemainingUses((Feat)CustomSkill.EnsoExtension);

      if (creature.KnowsFeat((Feat)CustomSkill.EnsoAmplification))
        return creature.GetFeatRemainingUses((Feat)CustomSkill.EnsoAmplification);

      if (creature.KnowsFeat((Feat)CustomSkill.EnsoGemellite))
        return creature.GetFeatRemainingUses((Feat)CustomSkill.EnsoGemellite);

      if (creature.KnowsFeat((Feat)CustomSkill.EnsoIntensification))
        return creature.GetFeatRemainingUses((Feat)CustomSkill.EnsoIntensification);

      if (creature.KnowsFeat((Feat)CustomSkill.EnsoAcceleration))
        return creature.GetFeatRemainingUses((Feat)CustomSkill.EnsoAcceleration);

      if (creature.KnowsFeat((Feat)CustomSkill.EnsoGuidage))
        return creature.GetFeatRemainingUses((Feat)CustomSkill.EnsoGuidage);

      if (creature.KnowsFeat((Feat)CustomSkill.EnsoPrudence))
        return creature.GetFeatRemainingUses((Feat)CustomSkill.EnsoPrudence);

      if (creature.KnowsFeat((Feat)CustomSkill.EnsoSubtilite))
        return creature.GetFeatRemainingUses((Feat)CustomSkill.EnsoSubtilite);

      if (creature.KnowsFeat((Feat)CustomSkill.EnsoTransmutation))
        return creature.GetFeatRemainingUses((Feat)CustomSkill.EnsoTransmutation);

      return 0;
    }
  }
}
