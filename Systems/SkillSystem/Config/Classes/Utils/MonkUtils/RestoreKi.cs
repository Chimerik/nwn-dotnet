using Anvil.API;

namespace NWN.Systems
{
  public static partial class MonkUtils
  {
    public static async void RestoreKi(NwCreature creature, bool harmony = false)
    {
      byte? level = creature.GetClassInfo((ClassType)CustomClass.Monk)?.Level;

      if (!level.HasValue)
        return;

      byte featUse = level.Value < 20 ? level.Value : (byte)20;

      if (harmony)
      {
        byte remainingUses = creature.GetFeatRemainingUses((Feat)CustomSkill.MonkPatience);
        byte restoredUses = (byte)(featUse / 2);

        if (remainingUses + restoredUses < featUse)
          featUse = (byte)(remainingUses + restoredUses);
      }

      await NwTask.NextFrame();
      creature.SetFeatRemainingUses((Feat)CustomSkill.MonkPatience, featUse);
      creature.SetFeatRemainingUses((Feat)CustomSkill.MonkDelugeDeCoups, featUse);
      creature.SetFeatRemainingUses((Feat)CustomSkill.MonkStunStrike, featUse);
      creature.SetFeatRemainingUses((Feat)CustomSkill.MonkDesertion, featUse);
      creature.SetFeatRemainingUses((Feat)CustomSkill.MonkExplosionKi, featUse);
      creature.SetFeatRemainingUses((Feat)CustomSkill.MonkPaumeVibratoire, featUse);
      creature.SetFeatRemainingUses((Feat)CustomSkill.MonkDarkVision, featUse);
      creature.SetFeatRemainingUses((Feat)CustomSkill.MonkTenebres, featUse);
      creature.SetFeatRemainingUses((Feat)CustomSkill.MonkPassageSansTrace, featUse);
      creature.SetFeatRemainingUses((Feat)CustomSkill.MonkSilence, featUse);
      creature.SetFeatRemainingUses((Feat)CustomSkill.MonkFrappeDombre, featUse);
    }
  }
}
