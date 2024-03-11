using Anvil.API;

namespace NWN.Systems
{
  public static partial class MonkUtils
  {
    public static async void RestoreKi(NwCreature creature)
    {
      byte? level = creature.GetClassInfo(NwClass.FromClassType(ClassType.Monk))?.Level;

      if (!level.HasValue)
        return;

      byte featUse = level.Value < 20 ? level.Value : (byte)20;

      await NwTask.NextFrame();
      creature.SetFeatRemainingUses(NwFeat.FromFeatId(CustomSkill.MonkPatience), featUse);
      creature.SetFeatRemainingUses(NwFeat.FromFeatId(CustomSkill.MonkDelugeDeCoups), featUse);
      creature.SetFeatRemainingUses(NwFeat.FromFeatId(CustomSkill.MonkStunStrike), featUse);
      creature.SetFeatRemainingUses(NwFeat.FromFeatId(CustomSkill.MonkDesertion), featUse);
    }
  }
}
