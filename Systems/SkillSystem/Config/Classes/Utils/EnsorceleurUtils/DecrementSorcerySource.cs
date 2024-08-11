using Anvil.API;

namespace NWN.Systems
{
  public static partial class EnsoUtils
  {
    public static void DecrementSorcerySource(NwCreature creature, byte nbSource = 1)
    {
      creature.DecrementRemainingFeatUses((Feat)CustomSkill.EnsoPrudence, nbSource);
      creature.DecrementRemainingFeatUses((Feat)CustomSkill.EnsoAllonge, nbSource);
      creature.DecrementRemainingFeatUses((Feat)CustomSkill.EnsoExtension, nbSource);
      creature.DecrementRemainingFeatUses((Feat)CustomSkill.EnsoAmplification, nbSource);
      creature.DecrementRemainingFeatUses((Feat)CustomSkill.EnsoGemellite, nbSource);
      creature.DecrementRemainingFeatUses((Feat)CustomSkill.EnsoIntensification, nbSource);
      creature.DecrementRemainingFeatUses((Feat)CustomSkill.EnsoAcceleration, nbSource);
      creature.DecrementRemainingFeatUses((Feat)CustomSkill.EnsoGuidage, nbSource);
      creature.DecrementRemainingFeatUses((Feat)CustomSkill.EnsoSubtilite, nbSource);
      creature.DecrementRemainingFeatUses((Feat)CustomSkill.EnsoTransmutation, nbSource);
      creature.DecrementRemainingFeatUses((Feat)CustomSkill.SorcellerieIncarnee, nbSource);

      if (creature.GetFeatRemainingUses((Feat)CustomSkill.SorcellerieIncarnee) < 2)
        creature.SetFeatRemainingUses((Feat)CustomSkill.SorcellerieIncarnee, 0);

      if (creature.GetFeatRemainingUses((Feat)CustomSkill.EnsoAmplification) < 2)
        creature.SetFeatRemainingUses((Feat)CustomSkill.EnsoAmplification, 0);

      if (creature.GetFeatRemainingUses((Feat)CustomSkill.EnsoIntensification) < 3)
        creature.SetFeatRemainingUses((Feat)CustomSkill.EnsoIntensification, 0);

      if (creature.GetFeatRemainingUses((Feat)CustomSkill.EnsoAcceleration) < 3)
        creature.SetFeatRemainingUses((Feat)CustomSkill.EnsoAcceleration, 0);
    }
  }
}
