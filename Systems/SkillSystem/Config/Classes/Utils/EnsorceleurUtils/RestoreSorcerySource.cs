using Anvil.API;

namespace NWN.Systems
{
  public static partial class EnsoUtils
  {
    public static async void RestoreSorcerySource(NwCreature creature, byte nbSource = 0)
    {
      byte? level = creature.GetClassInfo(ClassType.Sorcerer)?.Level;

      if (!level.HasValue)
        return;

      nbSource = nbSource > 0 ? nbSource : level.Value;

      await NwTask.NextFrame();
      creature.SetFeatRemainingUses((Feat)CustomSkill.EnsoPrudence, nbSource);
      creature.SetFeatRemainingUses((Feat)CustomSkill.EnsoAllonge, nbSource);
      creature.SetFeatRemainingUses((Feat)CustomSkill.EnsoExtension, nbSource);
      creature.SetFeatRemainingUses((Feat)CustomSkill.EnsoAmplification, nbSource);
      creature.SetFeatRemainingUses((Feat)CustomSkill.EnsoGemellite, nbSource);
      creature.SetFeatRemainingUses((Feat)CustomSkill.EnsoIntensification, nbSource);
      creature.SetFeatRemainingUses((Feat)CustomSkill.EnsoAcceleration, nbSource);
      creature.SetFeatRemainingUses((Feat)CustomSkill.EnsoGuidage, nbSource);
      creature.SetFeatRemainingUses((Feat)CustomSkill.EnsoSubtilite, nbSource);
      creature.SetFeatRemainingUses((Feat)CustomSkill.EnsoTransmutation, nbSource);
      creature.SetFeatRemainingUses((Feat)CustomSkill.SorcellerieIncarnee, nbSource);
    }
  }
}
