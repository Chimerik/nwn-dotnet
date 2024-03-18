using Anvil.API;

namespace NWN.Systems
{
  public static partial class MonkUtils
  {
    public static async void RestoreKi(NwCreature creature, bool harmony = false)
    {
      byte? level = creature.GetClassInfo(NwClass.FromClassType(ClassType.Monk))?.Level;

      if (!level.HasValue)
        return;

      byte featUse = level.Value < 20 ? level.Value : (byte)20;

      if (harmony)
      {
        byte remainingUses = creature.GetFeatRemainingUses(NwFeat.FromFeatId(CustomSkill.MonkPatience));
        byte restoredUses = (byte)(featUse / 2);

        if (remainingUses + restoredUses < featUse)
          featUse = (byte)(remainingUses + restoredUses);
      }

      await NwTask.NextFrame();
      creature.SetFeatRemainingUses(NwFeat.FromFeatId(CustomSkill.MonkPatience), featUse);
      creature.SetFeatRemainingUses(NwFeat.FromFeatId(CustomSkill.MonkDelugeDeCoups), featUse);
      creature.SetFeatRemainingUses(NwFeat.FromFeatId(CustomSkill.MonkStunStrike), featUse);
      creature.SetFeatRemainingUses(NwFeat.FromFeatId(CustomSkill.MonkDesertion), featUse);
      creature.SetFeatRemainingUses(NwFeat.FromFeatId(CustomSkill.MonkExplosionKi), featUse);
      creature.SetFeatRemainingUses(NwFeat.FromFeatId(CustomSkill.MonkPaumeVibratoire), featUse);
      creature.SetFeatRemainingUses(NwFeat.FromFeatId(CustomSkill.MonkDarkVision), featUse);
      creature.SetFeatRemainingUses(NwFeat.FromFeatId(CustomSkill.MonkTenebres), featUse);
      creature.SetFeatRemainingUses(NwFeat.FromFeatId(CustomSkill.MonkPassageSansTrace), featUse);
      creature.SetFeatRemainingUses(NwFeat.FromFeatId(CustomSkill.MonkSilence), featUse);
      creature.SetFeatRemainingUses(NwFeat.FromFeatId(CustomSkill.MonkFrappeDombre), featUse);
    }
  }
}
